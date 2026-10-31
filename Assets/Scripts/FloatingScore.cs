using UnityEngine;
using TMPro;

public class FloatingScore : MonoBehaviour
{
    public float riseSpeed = 1.5f;
    public float lifetime = 0.8f;

    private TMP_Text text;
    private float timer;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void Show(string value)
    {
        text.text = value;
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        if (timer >= lifetime) Destroy(gameObject);
    }
}