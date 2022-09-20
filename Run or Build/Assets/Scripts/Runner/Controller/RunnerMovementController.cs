using UnityEngine;

public class RunnerMovementController : IRunnerController
{
    private Vector2 _axis;
    private Rigidbody2D _rb;
    private bool _isGrounded;

    private readonly RunnerConfig _config;
    private readonly DataCharacter _dataCharacter;

    public RunnerMovementController(DataBaseRunner baseRunner)
    {
        _dataCharacter = baseRunner.Character;
        _config = baseRunner.Config;
    }

    public void Start()
    {
        _rb = _dataCharacter.Rb;
    }

    public void Update(float deltaTime)
    {
        AccelerationAxis(deltaTime);
    }
    
    public void FixedUpdate(float deltaTime)
    {
        Gravity(deltaTime);
        HandleMove(deltaTime);
    }

    #region Update

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
    
    #endregion
   
    #region FixedUpdate
    
    private void Gravity(float deltaTime)
    {
        var velocity = _rb.velocity + Vector2.up * (_config.FallMultiplier * _config.GravityScale * _config.Mass * deltaTime);

        var clampVelocity = Vector3.ClampMagnitude(velocity, 30);
        _rb.velocity = clampVelocity;
    }
    
    private void HandleMove(float deltaTime)
    {
        var speed = _config.Speed;
        if (!CanJump) speed = _config.SpeedAir;

        var velocity = _rb.velocity;
        var idealVel = new Vector2(_axis.x * speed, velocity.y);
        _rb.velocity = Vector2.MoveTowards(velocity, idealVel, 100 * deltaTime);
    }
    
    #endregion

    public void Jump(float jumpForce)
    {
        var velocity = _rb.velocity;
        var idealVel = new Vector2(velocity.x, jumpForce);
        _rb.velocity = Vector2.MoveTowards(velocity, idealVel, 100);
        //_rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void Move(Vector2 axis)
    {
        _axis = axis;
    }

    public bool CanJump { get; set; }
}