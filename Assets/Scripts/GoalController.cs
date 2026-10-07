using UnityEngine;

public class GoalController : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject goal;
    private SpriteRenderer goalSprite;
    private Animator goalAnimator;
    private AudioSource goalAudio;

    void Start()
    {
        goalSprite = GetComponent<SpriteRenderer>();
        goalSprite.enabled = true;
        goalAnimator = GetComponent<Animator>();
        goalAnimator.Play("goal-idle");
        goalAudio = GetComponent<AudioSource>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            goalAnimator.SetTrigger("goalScore");
        }
    }

    public void GoalScore()
    {
        goalSprite.enabled = false;
        goalAudio.Play();
        gameManager.LevelComplete();
    }

    public void ResetGoal()
    {
        goalSprite.enabled = true;
        goalAnimator.Play("goal-idle");
    }
}
