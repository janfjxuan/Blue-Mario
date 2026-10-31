using UnityEngine;

public class QuestionBoxPowerupController : MonoBehaviour, IPowerupController
{
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            ContactPoint2D contact = other.GetContact(0);
            if (contact.normal.y > 0.5f && !powerup.hasSpawned)
            {
                // spawn the powerup
                if (powerupGameObject.activeSelf == false)
                {
                    powerupGameObject.SetActive(true);
                }
                powerup.SpawnPowerup();
                if (powerupAudio != null)
                {
                    powerupAudio.Play();
                }
                if (powerup.hasSpawned)
                {
                    // show disabled sprite
                    questionBoxAnimator.SetTrigger("spawnPowerup");
                    ceiling.SetActive(true);
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