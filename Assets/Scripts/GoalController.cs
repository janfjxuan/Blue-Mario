using UnityEngine;

public class GoalController : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject goal;
    private SpriteRenderer goalSprite;
    private Animator goalAnimator;
    private AudioSource goalAudio;
    private bool triggered = false;

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
        if (triggered || !other.gameObject.CompareTag("Player")) return;
        triggered = true;

        var player = other.GetComponent<PlayerController>();
        if (player != null) player.BeginLevelEnd();

        goalAnimator.SetTrigger("goalScore");
    }

    public void GoalScore()
    {
        goalSprite.enabled = false;
        goalAudio.Play();
        gameManager.LevelComplete();
    }

    public void ResetGoal()
    {
        triggered = false;
        goalSprite.enabled = true;
        goalAnimator.Play("goal-idle");
    }
}
