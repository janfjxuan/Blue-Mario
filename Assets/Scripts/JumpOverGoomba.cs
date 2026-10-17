using UnityEngine;

public class JumpOverGoomba : MonoBehaviour
{
    // public Transform enemyLocation;
    // GameManager gameManager;
    // private bool onGroundState;

    // [System.NonSerialized]
    // public int score = 0; // we don't want this to show up in the inspector

    // [System.NonSerialized]
    // public float timer = 10f;

    // private bool countScoreState = false;
    public Vector3 boxSize;
    public float maxDistance;
    public LayerMask layerMask;

    void OnCollisionEnter2D(Collision2D col)
    {
        // if (col.gameObject.CompareTag("Ground")) onGroundState = true;

    }

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
