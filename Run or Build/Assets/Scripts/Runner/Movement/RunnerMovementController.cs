using UnityEngine;

public class RunnerMovementController : IRunnerController
{
    private Vector2 _axis;
    private Rigidbody2D _rb;
    private bool _isGrounded;

    private readonly ConfigRunner _config;
    private readonly DataCharacter _dataCharacter;

    public RunnerMovementController(DataBaseRunner baseRunner)
    {
        _dataCharacter = baseRunner.Character;
        _config = baseRunner.ConfigRunner;
    }

    public void Start()
    {
        _rb = _dataCharacter.Rb;
    }

    public void Update(float deltaTime)
    {
        HandleGrounding();
        AccelerationAxis(deltaTime);
    }
    
    public void FixedUpdate(float deltaTime)
    {
        Gravity(deltaTime);
        HandleMove(deltaTime);
    }

    #region Update

    private void HandleGrounding()
    {
        if (CanJump)
        {
        }
        else
        {
        }
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
    
    #endregion
   
    #region FixedUpdate
    
    private void Gravity(float deltaTime)
    {
        var velocity = _rb.velocity + Vector2.up * (_config.FallMultiplier * Physics.gravity.y * deltaTime);
        _rb.velocity = Vector2.ClampMagnitude(velocity, _config.MaxVelocityMagnitude);
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
    
    public void Move(Vector2 input)
    {
        _axis = input;
    }

    public void Jump(float jumpForce)
    {
        var idealVel = new Vector2(_rb.velocity.x, jumpForce);
        if (CanJump) 
            _rb.velocity = Vector2.MoveTowards(_rb.velocity, idealVel, 100);
    }

    public bool CanJump { get; set; }
    public bool IsDash { get; set; }
}