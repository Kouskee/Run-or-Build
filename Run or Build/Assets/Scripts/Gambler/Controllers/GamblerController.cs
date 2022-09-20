using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class GamblerController : NetworkBehaviour
{
    [SerializeField] private GamblerConfig _config;
    
    private IGamblerController[] _controllers;
    
    private Animator _anim;
    private PlayerInput _playerInput;
    private Rigidbody2D _rb;
    
    private Vector2 _axis;
    

    private void Awake()
    {
        TryGetComponent(out _playerInput);
        TryGetComponent(out _rb);
        TryGetComponent(out _anim);
        _controllers = new IGamblerController[1];
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

        var dataBaseRunner = new DataBaseGambler()
        {
            Character = dataCharacter,
            Config = _config
        };
        
        _controllers[0] = new GamblerMovementController(dataBaseRunner, _anim);

        for (var i = 0; i < _controllers.Length; i++)
            _controllers[i].Start();
    }

    private void Update()
    {
        if (!hasAuthority)
            return;
        for (var i = 0; i < _controllers.Length; i++)
        {
            _controllers[i].Update(Time.deltaTime);
            _controllers[i].Move(_axis);
        }
    }

    private void FixedUpdate()
    {
        if (!hasAuthority)
            return;
        for (var i = 0; i < _controllers.Length; i++)
        {
            _controllers[i].FixedUpdate();
        }
    }
    
    private void OnFlying(InputValue value)
    {
        var axis = value.Get<Vector2>();

        if(_axis.x != axis.x && axis.x != 0)
            transform.eulerAngles = new Vector3(0, axis.x >= 0 ? 0 : 180, 0);

        _axis = axis;
    }
    
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