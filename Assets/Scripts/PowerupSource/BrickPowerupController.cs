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
    private AudioSource brickAudio;
    public AudioClip brickBreak;
    void Start()
    {
        if (powerup != null)
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
        brickAudio = GetComponent<AudioSource>();
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
                PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
                if (powerup != null && !powerup.hasSpawned)
                {
                    // // enable sprite
                    // this.GetComponent<SpriteRenderer>().enabled = true;
                    // bounce
                    brickAudio.Play();
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
                else if (player != null && player.IsSuper) // breakable and super mario 
                {
                    BreakBrick();
                }
                else // breakable and small mario
                {
                    brickAudio.Play();
                    brickAnimator.SetTrigger("bounce");
                }
            }
        }
    }

    private void BreakBrick()
    {
        brickAudio.PlayOneShot(brickBreak);
        brickAnimator.SetTrigger("break");
        GetComponent<Collider2D>().enabled = false;
        SpawnDebris();
    }

    public GameObject debrisPrefab;
    public float debrisForce = 3f;
    private void SpawnDebris()
    {
        Vector2[] directions =
        {
            new Vector2(-1f, 2f),   // upper left
            new Vector2(1f, 2f),    // upper right
            new Vector2(-0.7f, 1f), // lower left
            new Vector2(0.7f, 1f)   // lower right
        };

        foreach (Vector2 dir in directions)
        {
            GameObject piece = Instantiate(debrisPrefab, transform.position, Quaternion.identity);
            piece.GetComponent<Rigidbody2D>().linearVelocity = dir * debrisForce;
            Destroy(piece, 2f);
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
        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
        brickAnimator.Play("brick-idle");
        brickAnimator.Rebind();
        brickAnimator.Update(0f);
        if (powerup != null)
        {
            powerupGameObject.transform.position = powerupStartPosition;
            powerup.ResetPowerup();
        }
    }
}