using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform player; // Mario's Transform
    public Transform endLimit; // GameObject that indicates end of map
    private float offset; // initial x-offset between camera and Mario
    private float startX; // smallest x-coordinate of the Camera
    private float endX; // largest x-coordinate of the camera
    private float camY;

    private float initialStartX, initialEndX, initialY;
    private Vector3 initialPosition;
    private float viewportHalfWidth;

    void Start()
    {
        // get coordinate of the bottomleft of the viewport
        // z doesn't matter since the camera is orthographic
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)); // the z-component is the distance of the resulting plane from the camera 
        viewportHalfWidth = Mathf.Abs(bottomLeft.x - transform.position.x);
        offset = 6.0f;
        startX = transform.position.x;
        endX = endLimit.transform.position.x - viewportHalfWidth;
        camY = transform.position.y;

        initialStartX = startX; initialEndX = endX; initialY = camY;
        initialPosition = transform.position;
    }

    void Update()
    {
        float desiredX = Mathf.Clamp(player.position.x + offset, startX, endX);
        transform.position = new Vector3(desiredX, camY, transform.position.z);
    }

    public void ResetCamera()
    {
        startX = initialStartX; endX = initialEndX; camY = initialY;
        transform.position = initialPosition;
    }

    // snap to another area (underground room)
    public void SetArea(CameraArea area, float playerX)
    {
        startX = area.cameraStart.position.x;
        endX = area.endLimit.position.x - viewportHalfWidth;
        camY = area.cameraStart.position.y;
        SnapToPlayerX(playerX);
    }

    // snap back to the main level's area (the one captured in Start)
    public void SetDefaultArea(float playerX)
    {
        startX = initialStartX; endX = initialEndX; camY = initialY;
        SnapToPlayerX(playerX);
    }

    private void SnapToPlayerX(float playerX)
    {
        float x = Mathf.Clamp(playerX + offset, startX, endX);
        transform.position = new Vector3(x, camY, transform.position.z);
    }
}
