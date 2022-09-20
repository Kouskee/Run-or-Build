using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class Border : MonoBehaviour
{
    [SerializeField] private Transform _runner;
    [SerializeField] private Color _borderColor;
    [SerializeField] private float _forcePunch;
    
    private Vector2 _size;
    private BoxCollider2D _box;

    private void Start()
    {
        TryGetComponent(out _box);
    }

    public void AcceptCollider()
    {
        _size = _box.size;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _borderColor;
        Gizmos.DrawCube(transform.position, _size);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        var rb = other.GetComponent<Rigidbody2D>();
        var velocity = rb.velocity;
        if (velocity == Vector2.zero)
            other.transform.position = _runner.transform.position;
        rb.AddForce(-velocity * _forcePunch, ForceMode2D.Impulse);
    }
}

[CustomEditor(typeof(Border))]
public class ChangeGizmos : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var border = (Border)target;
        if(GUILayout.Button("AcceptCollider"))
            border.AcceptCollider();
    }
}
