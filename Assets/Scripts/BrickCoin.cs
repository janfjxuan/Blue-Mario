using System.Collections;
using UnityEngine;

public class BrickCoin : MonoBehaviour
{
    public GameObject brick;
    // public Animator brickAnimator; // for future breaking animation
    public GameObject coin;
    public int hitCount = 1; // brick only 1 coin
    [System.NonSerialized]
    public int currentHitCount;
    void Start()
    {
        coin.SetActive(false);
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
