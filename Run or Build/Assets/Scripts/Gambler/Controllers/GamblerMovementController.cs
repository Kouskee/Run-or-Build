using UnityEngine;

public class GamblerMovementController : IGamblerController
{
    private Rigidbody2D _rb;
    
    private Vector2 _axis;
    
    private readonly Animator _anim;
    private readonly GamblerConfig _config;
    private readonly DataCharacter _data;
    
    private readonly int _speed = Animator.StringToHash("Speed");
    
    public GamblerMovementController(DataBaseGambler data, Animator anim)
    {
        _config = data.Config;
        _data = data.Character;
        _anim = anim;
    }
    
    public void Start()
    {
        _rb = _data.Rb;
        _rb.drag = _config.Drag;
    }

    public void Update(float deltaTime)
    {
        AccelerationAxis(deltaTime);
    }

    public void FixedUpdate()
    {
        var speed = _config.Speed;
        var idealVel = _axis * speed;
        _rb.AddForce(idealVel, ForceMode2D.Impulse);
    }
    
    private void AccelerationAxis(float deltaTime)
    {
        if (_axis.x < 0)
        {
            if (_rb.velocity.x > 0) _axis.x = 0;
            _axis.x = MoveTowards(-1, 1);
        }
        else if (_axis.x > 0)
        {
            if(_rb.velocity.x < 0) _axis.x = 0;
            _axis.x = MoveTowards(1, 1);
        }
        else {
            _axis.x = MoveTowards(0, 2);
        }
        
        float MoveTowards(float target, float multiplier) =>
            Mathf.MoveTowards(_axis.x, target, _config.Acceleration * multiplier * deltaTime);
    }

    public void Move(Vector2 axis)
    {
        _anim.SetFloat(_speed, axis.magnitude);
        _axis = axis;
    }
}
