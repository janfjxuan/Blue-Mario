using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickPowerupController : MonoBehaviour, PowerupController
{
    public Animator powerupAnimator;
    public BasePowerup powerup; // reference to this brick's powerup
    public bool isBreakable = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!powerup.hasSpawned)
            {
                // enable sprite
                this.GetComponent<SpriteRenderer>().enabled = true;
                // bounce
                this.GetComponent<Animator>().SetTrigger("bounce");
                // spawn powerup
                powerupAnimator.SetTrigger("spawned");

                if (!isBreakable)
                {
                    // show disabled sprite if it's not breakable type of brick
                    this.GetComponent<Animator>().SetTrigger("spawned");
                }
            }
            else if (isBreakable)
            {
                this.GetComponent<Animator>().SetTrigger("bounce");
            }
        }
    }

    // the brick bouncing event is not due to spring but due to animation
    // there's no need to set this
    // just implemented to conform to interface
    public void Disable()
    {

    }



}