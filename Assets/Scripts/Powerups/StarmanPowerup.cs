using System.Collections;
using UnityEngine;

public class StarmanPowerup : MovingPowerup
{
    // setup this object's type
    // instantiate variables
    public float bounceHeight = 3f;
    protected override void Start()
    {
        base.Start(); // call base class Start()
        type = PowerupType.StarMan;
        horizontalSpeed = 6f;
        powerupCollider.enabled = false;
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        base.OnCollisionEnter2D(col);
        if (moving && col.GetContact(0).normal.y > 0.5f)
        {
            float g = Mathf.Abs(Physics2D.gravity.y) * powerupBody.gravityScale;
            float ySpeed = Mathf.Sqrt(2f * g * bounceHeight);
            powerupBody.linearVelocity = new Vector2(powerupBody.linearVelocity.x, ySpeed);
        }
    }

    // interface implementation
    public override void ApplyPowerup(MonoBehaviour i)
    {
        if (i.TryGetComponent<BuffStateController>(out var buff))
        {
            buff.SetPowerup(PowerupType.StarMan);
        }
    }
}