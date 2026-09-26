using System.Collections;
using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    public GameObject questionBox;
    public Animator questionBoxAnimator;

    public GameObject ceiling;
    public GameObject coin;
    public int hitCount;
    [System.NonSerialized]
    public int currentHitCount;
    void Start()
    {
        coin.SetActive(false);
        ceiling.SetActive(false);
        currentHitCount = hitCount;
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ContactPoint2D contact = other.GetContact(0);
            if (contact.normal.y > 0.5f)
            {
                if (currentHitCount > 0)
                {
                    if (coin.activeSelf == false)
                    {
                        coin.SetActive(true);
                    }
                    Animator coinAnimator = coin.GetComponent<Animator>();
                    coinAnimator.SetTrigger("popUp");
                    AudioSource coinAudio = coin.GetComponent<AudioSource>();
                    if (coinAudio != null)
                    {
                        coinAudio.Play();
                    }
                }
                currentHitCount--;
            }
            if (currentHitCount == 0)
            {
                questionBoxAnimator.SetBool("isEmpty", true);
                questionBoxAnimator.Play("question-box-empty");
                ceiling.SetActive(true);
                StartCoroutine(DisableCoin());
            }
        }
    }
    private IEnumerator DisableCoin()
    {
        yield return new WaitForSeconds(1.2f);
        coin.SetActive(false);
    }
}

