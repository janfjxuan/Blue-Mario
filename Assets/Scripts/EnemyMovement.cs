using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private float maxOffset = 5.0f;
    private float enemyPatroltime = 2.0f;
    public int moveRight = -1;
    private Vector2 velocity;
    private Rigidbody2D enemyBody;
    public Vector3 startPosition;

    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        // get the starting position
        startPosition = transform.position;
        ComputeVelocity();
    }
    void ComputeVelocity()
    {
        velocity = new Vector2(moveRight * maxOffset / enemyPatroltime, 0);
    }
    void Movegoomba()
    {
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(enemyBody.position.x - startPosition.x) < maxOffset)
        {// move goomba
            Movegoomba();
        }
        else
        {
            // change direction
            moveRight *= -1;
            ComputeVelocity();
            Movegoomba();
        }
    }
}