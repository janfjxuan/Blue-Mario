using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickPowerupController : MonoBehaviour, IPowerupController
{
    public Animator powerupAnimator;
    public BasePowerup powerup; // reference to this brick's powerup
    private GameObject powerupGameObject;
    private Vector2 powerupStartPosition;
    public bool isBreakable = false;
    private Animator brickAnimator;
    void Start()
    {
        if(powerup != null)
        {
            powerupGameObject = powerup.gameObject;
            powerupGameObject.SetActive(false);
            powerupStartPosition = powerupGameObject.transform.position;
        } 
        else
        {
            isBreakable = true; // no coin means breakable
        }
        brickAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            ContactPoint2D contact = other.GetContact(0);
            if (contact.normal.y > 0.5f)
            {
                if (powerup != null && !powerup.hasSpawned)
                {
                    // // enable sprite
                    // this.GetComponent<SpriteRenderer>().enabled = true;
                    // bounce
                    brickAnimator.SetTrigger("bounce");
                    // spawn powerup (FOR LOOP this)
                    if (powerupGameObject.activeSelf == false)
                    {
                        powerupGameObject.SetActive(true);
                    }
                    powerupAnimator.SetTrigger("spawn");
                    AudioSource coinAudio = powerupGameObject.GetComponent<AudioSource>();
                    if (coinAudio != null)
                    {
                        coinAudio.Play();
                    }
                }
                if (!isBreakable) // empty block
                {
                    brickAnimator.SetTrigger("empty");
                } 
                else if(isBreakable && false) // breakable and big mario not implemented yet
                {
                    brickAnimator.SetTrigger("break");
                }
                else // breakable and small mario
                {
                    brickAnimator.SetTrigger("bounce");
                }
            }
        }
    }

    // the brick bouncing event is not due to spring but due to animation
    // there's no need to set this
    // just implemented to conform to interface
    public void Disable()
    {

    }

    public void ResetBrick()
    {
        brickAnimator.Play("brick-idle");
        brickAnimator.Rebind();
        brickAnimator.Update(0f);
        if(powerup != null)
        {
            powerupGameObject.transform.position = powerupStartPosition;
            powerup.ResetPowerup();
        } 
    }
}