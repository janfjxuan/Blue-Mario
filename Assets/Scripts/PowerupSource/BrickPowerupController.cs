using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickPowerupController : MonoBehaviour, IPowerupController
{
    public BasePowerup powerup; // reference to this brick's powerup
    private Vector2 powerupStartPosition;
    public bool isBreakable = false;
    private Animator brickAnimator;
    private AudioSource brickAudio;
    public AudioClip brickBreak;
    public GameObject debrisPrefab;
    public float debrisForce = 3f;
    void Start()
    {
        if (powerup != null)
        {
            powerup.SetActive();
            powerupStartPosition = powerup.gameObject.transform.position;
        }
        else
        {
            isBreakable = true; // no coin means breakable
        }
        brickAnimator = GetComponent<Animator>();
        brickAudio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        if (other.GetContact(0).normal.y <= 0.5f) return; // not hit from below

        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        bool isSuper = player != null && player.IsSuper;
        bool hasCoin = powerup != null && !powerup.hasSpawned;

        if (isBreakable && isSuper)
        {
            BreakBrick();
            return;
        }

        if (!isBreakable && !hasCoin) return;

        BounceBrick();

        if (hasCoin)
        {
            powerup.SpawnPowerup();
            AudioSource coinAudio = powerup.GetComponent<AudioSource>();
            if (coinAudio != null) coinAudio.Play();

            if (!isBreakable) brickAnimator.SetTrigger("empty");
        }
    }
    private void BounceBrick()
    {
        brickAudio.Play();
        brickAnimator.SetTrigger("bounce");
    }

    private void BreakBrick()
    {
        brickAudio.PlayOneShot(brickBreak);
        brickAnimator.SetTrigger("break");
        GetComponent<Collider2D>().enabled = false;
        SpawnDebris();
    }
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
            powerup.gameObject.transform.position = powerupStartPosition;
            powerup.ResetPowerup();
        }
    }
}