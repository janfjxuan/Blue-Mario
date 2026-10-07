using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMushroomPowerup : BasePowerup
{
    // setup this object's type
    // instantiate variables
    private Collider2D magicMushroomCollider;
    private Animator magicMushroomAnimator;
    private void Awake()
    {
        magicMushroomAnimator = GetComponent<Animator>();
        magicMushroomCollider = GetComponent<Collider2D>();
        gameObject.SetActive(false);
    }
    protected override void Start()
    {
        base.Start(); // call base class Start()
        type = PowerupType.MagicMushroom;
        magicMushroomAnimator.enabled = true;
        magicMushroomCollider.enabled = false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            // TODO: do something when colliding with Player
            // PowerupManager.instance.powerupCollected.Invoke(this);
            // then destroy powerup (optional)
            // DestroyPowerup();
            gameObject.SetActive(false);
        }
        else if (col.gameObject.layer == 10) // else if hitting Pipe, flip travel direction
        {
            if (spawned)
            {
                goRight = !goRight;
                rigidBody.AddForce(Vector2.right * 3 * (goRight ? 1 : -1), ForceMode2D.Impulse);

            }
        }
    }

    // interface implementation
    public override void SpawnPowerup()
    {
        spawned = true;
        magicMushroomAnimator.enabled = false;
        magicMushroomCollider.enabled = true;
        rigidBody.AddForce(Vector2.right * 3, ForceMode2D.Impulse); // move to the right
    }


    // interface implementation
    public override void ApplyPowerup(MonoBehaviour i)
    {
        // TODO: do something with the object
        // PlayerMovement mario;
        // bool result = i.TryGetComponent<PlayerMovement>(out mario);
        // if (result)
        // {
        //     mario.MakeSuperMario();
        // }
    }

    public override void ResetPowerup()
    {
        spawned = false;
        magicMushroomAnimator.enabled = true;
        magicMushroomCollider.enabled = false;
        gameObject.SetActive(false);
    }
}