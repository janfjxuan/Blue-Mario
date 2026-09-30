using UnityEngine;

public class Goal : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject goal;
    public SpriteRenderer goalSprite;
    public Animator goalAnimator;

    void Start()
    {
        goalSprite.enabled = true;
        goalAnimator.Play("goal-idle");
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
        gameManager.LevelComplete();
    }

    public void ResetGoal()
    {
        goalSprite.enabled = true;
        goalAnimator.Play("goal-idle");
    }
}
