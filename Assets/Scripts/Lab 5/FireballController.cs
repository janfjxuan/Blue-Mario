using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float scaleSpeed = 3.0f;
    private Rigidbody2D fireBallBody;
    private Collider2D fireBallCollider;
    private Animator fireBallAnimator;

    public float horizontalSpeed = 8f;
    public float bounceHeight = 1f;

    void Awake()
    {
        fireBallBody = GetComponent<Rigidbody2D>();
        fireBallCollider = GetComponent<Collider2D>();
        fireBallAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        StartCoroutine(ScaleAndDestroyCoroutine());
        GetComponent<AudioSource>().Play();
    }

    void FixedUpdate()
    {
        // keep the horizontal speed constant, preserving the direction the action launched it in
        Vector2 v = fireBallBody.linearVelocity;
        if (Mathf.Abs(v.x) > 0.01f)
        {
            v.x = Mathf.Sign(v.x) * horizontalSpeed;
            fireBallBody.linearVelocity = v;
        }
    }

    private IEnumerator ScaleAndDestroyCoroutine()
    {
        // Wait for 3 seconds
        yield return new WaitForSecondsRealtime(3);
        // Gradually scale down the GameObject
        while (transform.localScale.x > 0.01f)
        {
            transform.localScale -= Vector3.one * scaleSpeed * Time.deltaTime;
            yield return null;
        }

        // Ensure the GameObject is completely scaled down
        transform.localScale = Vector3.zero;

        // Destroy the GameObject
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player") return;

        if (col.gameObject.tag == "Enemy")
        {
            if (col.gameObject.TryGetComponent<EnemyMovement>(out var enemy))
                enemy.Yeet(col.transform.position.x > transform.position.x);
            Explode();
        }

        if (Mathf.Abs(col.GetContact(0).normal.x) > 0.5f)
        {
            Explode();
        }
        else if (Mathf.Abs(col.GetContact(0).normal.y) > 0.5f)
        {
            float g = Mathf.Abs(Physics2D.gravity.y) * fireBallBody.gravityScale;
            float ySpeed = Mathf.Sqrt(2f * g * bounceHeight);
            fireBallBody.linearVelocity = new Vector2(fireBallBody.linearVelocity.x, ySpeed);
        }
    }

    void Explode()
    {
        fireBallBody.linearVelocity = new Vector2(0f, 0f);
        fireBallCollider.enabled = false;
        fireBallAnimator.SetTrigger("explode");
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}