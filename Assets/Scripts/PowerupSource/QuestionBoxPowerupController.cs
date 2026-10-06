using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestionBoxPowerupController : MonoBehaviour, IPowerupController
{
    public Animator powerupAnimator;
    public BasePowerup powerup; // reference to this question box's powerup
    private GameObject powerupGameObject;
    private AudioSource powerupAudio;
    private Animator questionBoxAnimator;
    public GameObject ceiling;
    void Start()
    {
        powerupGameObject = powerup.gameObject;
        powerupAudio = powerupGameObject.GetComponent<AudioSource>();
        ceiling.SetActive(false);
        questionBoxAnimator = GetComponent<Animator>();
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
            if (contact.normal.y > 0.5f && !powerup.hasSpawned)
            {
                // show disabled sprite
                questionBoxAnimator.SetTrigger("spawnPowerup"); // this is the question box's animator
                ceiling.SetActive(true);

                // spawn the powerup
                if (powerupGameObject.activeSelf == false)
                {
                    powerupGameObject.SetActive(true);
                }
                powerupAnimator.SetTrigger("spawn"); // this is the animator belonging to the powerup in that question box
                if (powerupAudio != null)
                {
                    Debug.Log("audio");
                    powerupAudio.Play();
                }
            }
        }
    }

    // used by animator
    public void Disable()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        transform.localPosition = new Vector3(0, 0, 0);
    }

    public void ResetQuestionBox()
    {
        questionBoxAnimator.Play("question-box-blink");
        questionBoxAnimator.Rebind();
        questionBoxAnimator.Update(0f);
        ceiling.SetActive(false);
        powerup.ResetPowerup();
    }
}