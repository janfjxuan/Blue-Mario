using System.Collections;
using UnityEngine;

public class MagicMushroomPowerup : MovingPowerup
{
    // setup this object's type
    // instantiate variables
    protected override void Start()
    {
        base.Start(); // call base class Start()
        type = PowerupType.MagicMushroom;
        horizontalSpeed = 3f;
        powerupCollider.enabled = false;
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        base.OnCollisionEnter2D(col);
        if (moving)
        {
            foreach (ContactPoint2D contact in col.contacts)
            { 
                if (contact.normal.x * direction < -0.5f)
                {
                    direction *= -1;
                    break;
                }
            }
        }
    }

    // interface implementation
    public override void ApplyPowerup(MonoBehaviour i)
    {
        // base.ApplyPowerup(i);
        // try
        MarioStateController mario;
        bool result = i.TryGetComponent<MarioStateController>(out mario);
        if (result)
        {
            mario.SetPowerup(this.powerupType);
        }
    }
}