using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PlayerInput))]

public class RunnerController : MonoBehaviour
{
    [SerializeField] private ConfigRunner _config;
    [SerializeField] private LayerMask _groundLayer;

    private IRunnerController[] _controllers;

    private Rigidbody2D _rb;
    private BoxCollider2D _runnerCollider;
    private PlayerControls _input;

    private Vector2 _axis;
    private Vector2 _size;
    private float _timeLeftGrounded = -10;
    private bool _isGround;
    private bool _canJump;
    private bool _coyoteUsable;
    private bool CanCoyote => _coyoteUsable && _timeLeftGrounded + _config.CoyoteTime > Time.time;

    private readonly RaycastHit2D[] _ground = new RaycastHit2D[1];
    private readonly RaycastHit2D[] _leftWall = new RaycastHit2D[1];
    private readonly RaycastHit2D[] _rightWall = new RaycastHit2D[1];

    private void Awake()
    {
        TryGetComponent(out _rb);
        TryGetComponent(out _runnerCollider);
        _size = _runnerCollider.size;

        _controllers = new IRunnerController[1];

        Setup();
    }

    private void Setup()
    {
        _input = new PlayerControls();
        _input.Enable();

        _input.RunnerMap.Jump.canceled += context => _canJump = false;
        
        _input.RunnerMap.Dash.started += context => AssignmentBool(true);
        _input.RunnerMap.Dash.canceled += context => AssignmentBool(false);

        void AssignmentBool(bool bl)
        {
            for (var i = 0; i < _controllers.Length; i++)
                _controllers[i].IsDash = bl;
        }
    }

    private void Start()
    {
        InitDataControllers();
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
            ConfigRunner = _config
        };

        _controllers[0] = new RunnerMovementController(dataBaseRunner);

        for (var i = 0; i < _controllers.Length; i++)
            _controllers[i].Start();
    }

    #region Updates
    
    private void Update()
    {
        var position = transform.position;
        
        var rightWall = BoxCast(position, Vector2.right, _rightWall);
        var leftWall = BoxCast(position, Vector2.left, _leftWall);

        var grounded = BoxCast(position, Vector2.down, _ground);
        _isGround = grounded > 0;
        
        var pushingRightWall = rightWall > 0 && _axis.x > 0 && !_isGround;
        var pushingLeftWall = leftWall > 0 && _axis.x < 0 && !_isGround;

        if (pushingRightWall || pushingLeftWall)
            _isGround = _canJump = true;

        if (_isGround)
        {
            _timeLeftGrounded = Time.time;
            _coyoteUsable = true;
            _canJump = true;
        }

        for (var i = 0; i < _controllers.Length; i++)
        {
            _controllers[i].Update(Time.deltaTime);
            if ((_isGround || CanCoyote) && _canJump)
                _controllers[i].CanJump = true;
            else
                _controllers[i].CanJump = false;
        }
    }

    private int BoxCast(Vector3 position, Vector2 direction, RaycastHit2D[] result)
    {
        return Physics2D.BoxCastNonAlloc(position, _size, 0, direction, result, _config.Offset, _groundLayer);
    } 

    private void FixedUpdate()
    {
        for (var i = 0; i < _controllers.Length; i++)
            _controllers[i].FixedUpdate(Time.deltaTime);
    }

    #endregion

    #region Input Events

    public void OnJump(InputValue value)
    {
        _coyoteUsable = false;
        for (var i = 0; i < _controllers.Length; i++)
        {
            _controllers[i].Jump(_config.JumpForce);
        }
    }
    
    public void OnMove(InputValue value)
    {
        _axis = value.Get<Vector2>();
        for (var i = 0; i < _controllers.Length; i++)
            _controllers[i].Move(_axis);
    }

    #endregion
}