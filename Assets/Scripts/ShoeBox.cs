using System.Collections;
using UnityEngine;

public class ShoeBox : MonoBehaviour
{
    public GameObject questionBox;
    public Animator questionBoxAnimator;

    public GameObject ceiling;
    public GameObject shoes;
    public int hitCount = 1;
    [System.NonSerialized]
    public int currentHitCount;
    void Start()
    {
        shoes.SetActive(false);
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
                if (shoes.activeSelf == false)
                {
                    shoes.SetActive(true);
                }
                Animator coinAnimator = shoes.GetComponent<Animator>();
                coinAnimator.SetTrigger("popUp");
                AudioSource coinAudio = shoes.GetComponent<AudioSource>();
                if (coinAudio != null)
                {
                    coinAudio.Play();
                }
                currentHitCount--;
                if (currentHitCount == 0)
                {
                    questionBoxAnimator.SetBool("isEmpty", true);
                    questionBoxAnimator.Play("question-box-empty");
                    ceiling.SetActive(true);
                    StartCoroutine(DisableShoe());
                }
            }
        }
    }
    private IEnumerator DisableShoe()
    {
        yield return new WaitForSeconds(1.2f);
        shoes.SetActive(false);
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
