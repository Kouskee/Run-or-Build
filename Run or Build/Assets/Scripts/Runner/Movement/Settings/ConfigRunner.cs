using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Runner/Settings")]
public class ConfigRunner : ScriptableObject
{
    [Header("Ground")] 
    [SerializeField, Range(-1, 1)] private float _offset;

    [Header("Jump / Gravity")] 
    [SerializeField, Range(5, 50)] private int _jumpForce;
    [SerializeField, Range(0.1f, 0.3f)] private float _coyoteTime;
    [SerializeField, Range(1, 10)] private int _fallMultiplier;
    [SerializeField, Range(10, 50)] private int _maxVelocityMagnitude;
    
    [Header("Move")] 
    [SerializeField, Range(1, 20)] private float _speed;
    [SerializeField, Range(1, 20)] private float _speedAir;
    [SerializeField, Range(1, 20)] private float _speedDash;
    [SerializeField, Range(1, 10)] private int _acceleration;
    
    public float Offset => _offset;

    public float JumpForce => _jumpForce;
    public float CoyoteTime => _coyoteTime;
    public float FallMultiplier => _fallMultiplier;
    public float MaxVelocityMagnitude => _maxVelocityMagnitude;
    
    public float Speed => _speed;
    public float SpeedAir => _speedAir;
    public float SpeedDash => _speedDash;
    public float Acceleration => _acceleration;
}
