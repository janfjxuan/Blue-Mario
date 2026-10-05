using System.Collections;
using UnityEngine;

public class MagicMushroomBox : MonoBehaviour
{
    public GameObject questionBox;
    public Animator questionBoxAnimator;

    public GameObject ceiling;
    public GameObject magicMushroom;

    
    public int hitCount = 1;
    [System.NonSerialized]
    public int currentHitCount;
    void Start()
    {
        magicMushroom.SetActive(false);
        ceiling.SetActive(false);
        currentHitCount = hitCount;
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ContactPoint2D contact = other.GetContact(0);
            if (contact.normal.y > 0.5f && currentHitCount > 0)
            {
                if (magicMushroom.activeSelf == false)
                {
                    magicMushroom.SetActive(true);
                }
                Animator magicMushroomAnimator = magicMushroom.GetComponent<Animator>();
                magicMushroomAnimator.SetTrigger("popUp");

                AudioSource magicMushroomAudio = magicMushroom.GetComponent<AudioSource>();
                if (magicMushroomAudio != null)
                {
                    magicMushroomAudio.Play();
                }
                currentHitCount--;
                if (currentHitCount == 0)
                {
                    questionBoxAnimator.SetBool("isEmpty", true);
                    questionBoxAnimator.Play("question-box-empty");
                    ceiling.SetActive(true);
                }
            }
        }
    }
    public void ResetQuestionBox()
    {
        currentHitCount = hitCount;
        questionBoxAnimator.SetBool("isEmpty", false);
        questionBoxAnimator.Rebind();
        questionBoxAnimator.Update(0f);
        ceiling.SetActive(false);
    }
}
