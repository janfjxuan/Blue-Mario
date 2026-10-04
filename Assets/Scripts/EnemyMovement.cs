using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyMovement : MonoBehaviour
{
    public UnityEvent stomped;
    public GameObject goomba;
    public float speed;
    private int moveRight = -1;
    private bool alive = true;
    // private float maxOffset = 5.0f;
    // private float enemyPatroltime = 2.0f;
    // private Vector2 velocity;
    private Rigidbody2D enemyBody;
    private Animator enemyAnimator;
    private Collider2D enemyCollider;
    public Vector3 startPosition;

    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();
        // get the starting position
        startPosition = transform.position;
        // ComputeVelocity();
    }
    // void ComputeVelocity()
    // {
    //     velocity = new Vector2(moveRight * maxOffset / enemyPatroltime, 0);
    // }
    // void Movegoomba()
    // {
    //     enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    // }

    void FixedUpdate()
    {
        if (!alive) return;
        enemyBody.linearVelocity = new Vector2(moveRight * speed, enemyBody.linearVelocity.y);
        // if (Mathf.Abs(enemyBody.position.x - startPosition.x) < maxOffset)
        // {// move goomba
        //     Movegoomba();
        // }
        // else
        // {
        //     // change direction
        //     moveRight *= -1;
        //     ComputeVelocity();
        //     Movegoomba();
        // }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint2D contact in col.contacts)
        {
            if (contact.normal.x * moveRight < -0.5f)
            {
                moveRight *= -1;
                break;
            }
        }
    }

    public void Stomp()
    {
        if (!alive) return;
        alive = false;
        enemyCollider.enabled = false;
        enemyAnimator.SetTrigger("killGoomba");
        stomped.Invoke();
    }

    public void KillGoomba()
    {
        goomba.SetActive(false);
    }

    public void GameRestart()
    {
        goomba.SetActive(true);
        alive = true;
        enemyCollider.enabled = true;
        enemyAnimator.Rebind();
        enemyAnimator.Update(0f);
        transform.localPosition = startPosition;
        moveRight = -1;
        // ComputeVelocity();
    }
}