using UnityEngine;

public class JumpOverGoomba : MonoBehaviour
{
    public Vector3 boxSize;
    public float maxDistance;
    public LayerMask layerMask;

    private bool OnGroundCheck()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, maxDistance, layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
