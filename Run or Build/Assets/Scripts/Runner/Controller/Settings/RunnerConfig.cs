using UnityEngine;

[CreateAssetMenu(fileName = "RunnerConfig", menuName = "Characters/RunnerConfig")]
public class RunnerConfig : ScriptableObject
{
    [Header("Ground")] 
    [SerializeField, Range(-1, 1)] private float _offset;

    [Header("Jump / Gravity")] 
    [SerializeField, Range(1, 50)] private int _jumpForce;
    [SerializeField, Range(0, 5)] private int _fallMultiplier;
    [SerializeField, Range(1, 5)] private float _mass;
    [SerializeField, Range(-10, 0)] private float _gravityScale;
    [SerializeField, Range(0.1f, 1f)] private float _coyoteTime;
    
    [Header("Move")] 
    [SerializeField, Range(1, 20)] private float _speed;
    [SerializeField, Range(1, 20)] private float _speedAir;
    [SerializeField, Range(1, 10)] private int _acceleration;
    
    public float Offset => _offset;

    public float JumpForce => _jumpForce;
    public float FallMultiplier => _fallMultiplier;
    public float Mass => _mass;
    public float GravityScale => _gravityScale;
    public float CoyoteTime => _coyoteTime;
    
    public float Speed => _speed;
    public float SpeedAir => _speedAir;
    public float Acceleration => _acceleration;
}
