using UnityEngine;

public class Border : MonoBehaviour
{
    [SerializeField] private Transform _runner;
    [SerializeField] private float _forcePunch;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out GamblerController _)) return;

        var rb = other.GetComponent<Rigidbody2D>();
        var velocity = rb.velocity;
        if (velocity == Vector2.zero)
            other.transform.position = _runner.transform.position;
        rb.AddForce(-velocity * _forcePunch, ForceMode2D.Impulse);
    }
}