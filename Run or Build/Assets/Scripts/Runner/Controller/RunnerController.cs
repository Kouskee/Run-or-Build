using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class RunnerController : NetworkBehaviour
{
    [SerializeField] private RunnerConfig _config;
    [SerializeField] private LayerMask _groundLayer;

    private IRunnerController[] _controllers;

    private PlayerInput _playerInput;
    private Rigidbody2D _rb;
    private BoxCollider2D _runnerCollider;

    private Vector2 _axis;
    private Vector2 _size;
    private float _timeLeftGrounded = -10;
    private bool _isGround;
    private bool _coyoteUsable;
    private bool CanCoyote => _coyoteUsable && _timeLeftGrounded + _config.CoyoteTime > Time.time;

    private readonly RaycastHit2D[] _groundHit = new RaycastHit2D[1];
    // private readonly RaycastHit2D[] _leftWall = new RaycastHit2D[1];
    // private readonly RaycastHit2D[] _rightWall = new RaycastHit2D[1];
    
    private PlayerControls _input;

    private void Awake()
    {
        TryGetComponent(out _playerInput);
        TryGetComponent(out _rb);
        TryGetComponent(out _runnerCollider);
        _size = _runnerCollider.bounds.size;

        _controllers = new IRunnerController[1];
    }

    private void Start()
    {
        InitDataControllers();
        _input = new PlayerControls();
        _input.Enable();
        _input.RunnerMap.Jump.canceled += context => _coyoteUsable = false;
    }

    private void InitDataControllers()
    {
        var dataCharacter = new DataCharacter
        {
            Rb = _rb,
            Transform = transform
        };

        var dataBaseRunner = new DataBaseRunner
        {
            Character = dataCharacter,
            Config = _config
        };

        _controllers[0] = new RunnerMovementController(dataBaseRunner);

        for (var i = 0; i < _controllers.Length; i++)
            _controllers[i].Start();
    }

    #region Updates

    private void Update()
    {
        if (!hasAuthority)
            return;

        // var rightWall = BoxCast(position, Vector2.right, _rightWall);
        // var leftWall = BoxCast(position, Vector2.left, _leftWall);
        // var pushingRightWall = rightWall > 0 && _axis.x > 0 && !_isGround;
        // var pushingLeftWall = leftWall > 0 && _axis.x < 0 && !_isGround;
        
        GroundCheck();
        
        for (var i = 0; i < _controllers.Length; i++)
        {
            _controllers[i].Update(Time.deltaTime);
            _controllers[i].Move(_axis);
        }
    }
    
    private void GroundCheck()
    {
        var grounded = BoxCast(transform.position, Vector2.down, _groundHit);
        _isGround = grounded > 0;

        if (!_isGround) return;

        _coyoteUsable = true;
        _timeLeftGrounded = Time.time;
    }

    private int BoxCast(Vector3 position, Vector2 direction, RaycastHit2D[] result)
    {
        var size = new Vector2(_size.x - .1f, 0.001f);
        return Physics2D.BoxCastNonAlloc(position, size, 0, direction, result, _config.Offset, _groundLayer);
    }

    private void FixedUpdate()
    {
        if (!hasAuthority)
            return;
        for (var i = 0; i < _controllers.Length; i++)
            _controllers[i].FixedUpdate(Time.deltaTime);
    }

    #endregion

    #region Input Events

    private void OnJump(InputValue value)
    {
        if (!(_isGround || CanCoyote)) return;
    
        for (var i = 0; i < _controllers.Length; i++)
        {
            _controllers[i].Jump(_config.JumpForce);
        }
    }

    private void OnMove(InputValue value)
    {
        _axis = value.Get<Vector2>();
    }

    #endregion

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        _playerInput.enabled = true;
    }

    public override void OnStopAuthority()
    {
        base.OnStopAuthority();
        _playerInput.enabled = false;
    }
}