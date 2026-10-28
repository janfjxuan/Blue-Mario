using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EnemyMovement : MonoBehaviour
{
    public UnityEvent stomped;
    public GameObject goomba;
    public float speed;
    private int moveRight = -1;
    private bool alive = true;
    private Rigidbody2D enemyBody;
    private Animator enemyAnimator;
    private Collider2D enemyCollider;
    private AudioSource enemyAudio;
    private Vector3 startPosition;
    public float yeetForce = 3f;


    void Awake()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();
        enemyAudio = GetComponent<AudioSource>();
        // get the starting position
        startPosition = transform.position;
    }
    void FixedUpdate()
    {
        if (!alive) return;
        enemyBody.linearVelocity = new Vector2(moveRight * speed, enemyBody.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint2D contact in col.contacts)
        {
            if (contact.normal.x * moveRight < -0.5f)
            {
                moveRight *= -1;
                break;
            }
        }
    }

    public void Stomp()
    {
        if (!alive) return;
        alive = false;
        enemyCollider.enabled = false;
        enemyBody.linearVelocity = Vector2.zero;
        enemyBody.bodyType = RigidbodyType2D.Kinematic;
        enemyAudio.Play();
        enemyAnimator.SetTrigger("stompGoomba");
        stomped.Invoke();
    }

    public void Yeet(bool launchRight)
    {
        if (!alive) return;
        alive = false;
        enemyCollider.enabled = false;
        enemyAudio.Play();
        enemyAnimator.SetTrigger("yeetGoomba");
        Vector2 direction = new Vector2(launchRight ? 1f : -1f, 2f);
        enemyBody.linearVelocity = direction * yeetForce;
        stomped.Invoke();
    }

    public void KillGoomba()
    {
        goomba.SetActive(false);
    }

    public void GameRestart()
    {
        CancelInvoke();
        goomba.SetActive(true);
        alive = true;
        enemyBody.bodyType = RigidbodyType2D.Dynamic;
        enemyCollider.enabled = true;
        enemyAnimator.Rebind();
        enemyAnimator.Update(0f);
        transform.localPosition = startPosition;
        moveRight = -1;
        enemyBody.linearVelocity = Vector2.zero;
        enemyBody.angularVelocity = 0f;
        transform.rotation = Quaternion.identity;
    }
}