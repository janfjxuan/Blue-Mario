using System.Collections;
using UnityEngine;

// 100% from claude
public class PipeController : MonoBehaviour
{
    [Header("Entry (sideways)")]
    public bool canEnter = true;
    public bool enterMovingRight = true;     // direction Mario walks to go in
    public float slideDistance = 1f;         // how far he slides into the pipe
    public float slideDuration = 0.8f;
    public AudioSource pipeAudio;

    [Header("Destination")]
    public Transform destination;            // marker where Mario's FEET end up
    public bool exitUpward = false;          // true: destination is the top of a vertical pipe, he rises out
    public float exitDuration = 0.8f;

    [Header("Events")]
    public SimpleGameEvent onTeleport;       // raised at the moment of teleport (camera/music use this later)

    [Header("Draw order")]
    public SpriteRenderer entryPipeSprite;   // this pipe's own sprite (found automatically if left empty)
    public SpriteRenderer exitPipeSprite;    // the surface vertical pipe's sprite (only needed when Exit Upward is on)

    [Header("Camera")]
    public CameraArea destinationArea;           // the underground room; leave EMPTY when the destination is the main level
    public CameraController cameraController;    // optional, found automatically if empty

    void OnTriggerStay2D(Collider2D other)
    {
        if (!canEnter || destination == null) return;
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<PlayerController>();
        if (player == null || player.InPipe || !player.alive || player.IsTransforming) return;
        if (!player.IsGrounded) return;
        if (!player.IsMoving || player.FacingRight != enterMovingRight) return;

        player.StartCoroutine(Transition(player));
    }

    private IEnumerator Transition(PlayerController player)
    {
        player.InPipe = true;
        player.SetInputLockedForPipe(true);
        pipeAudio.Play();

        if (entryPipeSprite == null) entryPipeSprite = GetComponentInChildren<SpriteRenderer>();
        if (entryPipeSprite != null) player.SetSortingBehind(entryPipeSprite);

        Rigidbody2D body = player.marioBody;
        Collider2D col = player.marioCollider;
        float height = col.bounds.size.y;            // measure before disabling the collider

        col.enabled = false;
        body.gravityScale = 0f;
        body.linearVelocity = Vector2.zero;

        // 1. slide into the pipe
        Vector2 dir = enterMovingRight ? Vector2.right : Vector2.left;
        yield return player.StartCoroutine(Slide(body, body.position, body.position + dir * slideDistance, slideDuration));

        // 2. teleport
        player.ClearGroundContacts();
        Vector2 target = destination.position;
        if (onTeleport != null) onTeleport.Raise();

        if (exitUpward)
        {
            // 3. rise out of the vertical pipe
            if (exitPipeSprite != null) player.SetSortingBehind(exitPipeSprite);
            Vector2 start = target - Vector2.up * height;
            MovePlayerInstantly(player, body, start);
            SnapCamera(target.x);
            yield return player.StartCoroutine(Slide(body, start, target, exitDuration));
        }
        else
        {
            player.RestoreSorting();
            MovePlayerInstantly(player, body, target);
            SnapCamera(target.x);
            yield return new WaitForSeconds(0.1f);   // a beat so the camera can settle
        }

        // 4. restore control
        player.RestoreSorting();
        col.enabled = true;
        body.gravityScale = player.DefaultGravity;
        player.SetInputLockedForPipe(false);
        player.InPipe = false;
    }

    private IEnumerator Slide(Rigidbody2D body, Vector2 from, Vector2 to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.fixedDeltaTime;
            body.MovePosition(Vector2.Lerp(from, to, t / duration));
            yield return new WaitForFixedUpdate();
        }
        body.position = to;
    }

    private void MovePlayerInstantly(PlayerController player, Rigidbody2D body, Vector2 pos)
    {
        body.position = pos;
        // also move the transform, so the camera doesn't read a stale position for one frame
        player.transform.position = new Vector3(pos.x, pos.y, player.transform.position.z);
    }

    private void SnapCamera(float playerX)
    {
        if (cameraController == null) cameraController = FindFirstObjectByType<CameraController>();
        if (cameraController == null) return;

        if (destinationArea != null) cameraController.SetArea(destinationArea, playerX);
        else cameraController.SetDefaultArea(playerX);
    }
}