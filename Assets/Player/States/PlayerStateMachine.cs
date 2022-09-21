// https://www.notion.so/Movement-and-Controls-51ed8d38a18a45ccadc3f377561e35d6

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class PlayerStateMachine : MonoBehaviour
{
    private Controller2D _controller2D;
    private Collider2D _collider2D;
    private Animator _animator;
    private PlayerActionControls _playerActionControls;
    private SpriteRenderer _spriteRenderer;
    private Transform _transform;
    
    [SerializeField]
    private LayerMask _enemyLayer;
    
    [SerializeField]
    private float _accelerationAirborne = 10f;
    [SerializeField]
    private float _accelerationGrounded = 10f;
    [SerializeField]
    private float _jumpHeight = 2f;
    [SerializeField]
    private float _maxFootSpeed = 2f; // v_x
    [SerializeField]
    private float _horizontalDistancetoJumpApex = .5f; // x_h
    [SerializeField]
    private float _maxFallSpeed = 20f;
    [SerializeField]
    private float _wallSlideSpeed = -.005f;
    [SerializeField]
    private float _wallStickTime = .25f;
    [SerializeField]
    private float _dashTime = .25f;
    [SerializeField]
    private float _dashDistance = .25f;
    [SerializeField]
    private float _rollTime = .5f;
    [SerializeField]
    private float _rollDistance = .01f;
    [SerializeField]
    private float _attackRange = 0.5f;
    [SerializeField]
    private float _attackCooldownLength = 1f;

    public readonly Vector2[] WallJumps = { new Vector2(0.03f, 10f), new Vector2(0.035f, 13.5f), new Vector2(0.04f, 17f) };

    [SerializeField]
    private Transform _attackPoint;


    // Animation Strings
    private string[] _animationStates = new string[] { "Idle", "Walk", "Run", "Sprint", "Jump", "Jump Apex", "Fall", "Land", "Dash", "Roll", "Teleport 1", "Teleport 2", "Wall Grab", "Wall Slide Right", "Wall Slide Left", "Block", "Attack 1", "Attack 2", "Attack 3", "Attack 4", "Airborne Attack", "Roll Attack", "Hit", "Death" };

    private Dictionary<string, int> _animationStatesDict = new Dictionary<string, int>();

   
    // Jump height and force variables
    // To Do: Change jumpForce to depend on horizontal movement and jump height;
    private bool _isJumpPressed;
    private bool _isMovementPressed;
    private bool _isRollDashPressed;
    private bool _isAttackPressed;
    private bool _isDashFinished;
    private bool _isRollFinished;
    private bool _isJumpEnabled;
    private bool _requireJumpPressed;
    private bool _requireRollDashPressed;
    private bool _canWallJump;
    private bool _canDash;
    private bool _lastDirection;
    private bool _checkAttackCollisions;
    private bool _attackIsFinished;
   
    private Vector3 _velocity;
    private Vector3 _currentMovement;

    private float _moveInputX;
    private float _targetDirection;
    private float _initialJumpVelocity;
    private float _timeToWallUnstick;
    private float _attackCooldown;
    private float _rollSpeed;
    private float _rollFrameTime;
    private float _dashSpeed;
    private float _gravity;
    private float _timeScale;
    private float _deltaTime;
    
    private string _debugCurrentState;

    private int _currentAnimationStateHash;
    private int _numConsecutiveAttacks = 0;
    


    // State Variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    // Getters and Setters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public Controller2D Controller2D { get { return _controller2D; } }
    public Transform Transform { get { return _transform; } set { _transform = value; } }
    public Animator Animator { get { return _animator; } }
    public SpriteRenderer SpriteRenderer { get { return _spriteRenderer; } }

    // Attacking Related
    public Transform AttackPoint { get { return _attackPoint; } set { _attackPoint = value; } }
    public float AttackRange { get { return _attackRange; } }
    public float AttackCooldownLength { get { return _attackCooldownLength; } }
    public float AttackCooldown { get { return _attackCooldown; } set { _attackCooldown = value; } }
    public LayerMask EnemyLayer { get { return _enemyLayer; } }
    public int NumConsecutiveAttacks { get { return _numConsecutiveAttacks; } set { _numConsecutiveAttacks = value; } }
    
    
    public float VelocityY { get { return _velocity.y; } set { _velocity.y = value; } }
    public float VelocityX { get { return _velocity.x; } set { _velocity.x = value; } }
    public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
    public float CurrentMovementX { get { return _currentMovement.x; } set { _currentMovement.x = value; } }

    // Jump Calculations
    public float MaxFootSpeed { get { return _maxFootSpeed; } }
    public float InitialJumpVelocity { get { return _initialJumpVelocity; } }
    public float Gravity { get { return _gravity; } }

    // Movement 
    public float MaxFallSpeed { get { return _maxFallSpeed; } }
    public float AccelerationGrounded { get { return _accelerationGrounded; } }
    public float AccelerationAirborne { get { return _accelerationAirborne; } }

    // Other Movement Variables
    public float TargetDirection { get { return _targetDirection; } set { _targetDirection = value; } }
    public float MoveInputX { get { return _moveInputX; } set { _moveInputX = value; } }

    public float TimeScale { get { return _timeScale; } set { _timeScale = value; } }
    public float DeltaTime { get { return _deltaTime; } }

    // Wall Slide
    public float WallSlideSpeed { get { return _wallSlideSpeed; } }
    public float TimeToWallUnstick { get { return _timeToWallUnstick; } set { _timeToWallUnstick = value;} }
    public float WallStickTime { get { return _wallStickTime; } }
    
    // Abilities
    public float DashTime { get { return _dashTime; } }
    public float RollTime { get { return _rollTime; } }
    public float DashSpeed { get { return _dashSpeed; } }
    public float RollSpeed { get { return _rollSpeed; } }
    public float RollFrameTime { get { return _rollFrameTime; } }

    // Booleans
    public bool IsMovementPressed { get { return _isMovementPressed; } }
    public bool IsRollDashPressed { get { return _isRollDashPressed; } }
    public bool IsRollFinished { get { return _isRollFinished; } set { _isRollFinished = value; } }
    public bool IsDashFinished { get { return _isDashFinished; } set { _isDashFinished = value; } }
    public bool IsJumpPressed { get { return _isJumpPressed; } }
    public bool IsAttackPressed { get { return _isAttackPressed; } }
    public bool RequireJumpPressed { get { return _requireJumpPressed; } set { _requireJumpPressed = value; } }
    public bool RequireRollDashPressed { get { return _requireRollDashPressed; } set { _requireRollDashPressed = value; } }
    public bool CanWallJump { get { return _canWallJump; } set { _canWallJump = value; } }
    public bool CanDash { get { return _canDash; } set { _canDash = value; } }
    public bool LastDirection { get { return _lastDirection; } set { _lastDirection = value; } }
    public bool CheckAttackCollisions { get { return _checkAttackCollisions; } set { _checkAttackCollisions = value; } }
    public bool AttackIsFinished { get { return _attackIsFinished; }  set { _attackIsFinished = value; } }

    public string DebugCurrentState { get { return _debugCurrentState; } set { _debugCurrentState = value; } }
    
    public Dictionary<string, int> AnimationStatesDict { get { return _animationStatesDict; } }


    void Awake()
    {
        // set initial references
        _playerActionControls = new PlayerActionControls();
        _controller2D = GetComponent<Controller2D>();
        _collider2D = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _transform = GetComponent<Transform>();

        // setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        // initialize input action callbacks
        _playerActionControls.Gameplay.Jump.performed += context => OnJump(context);
        _playerActionControls.Gameplay.Jump.canceled += context => OnJump(context);
        _playerActionControls.Gameplay.Move.performed += context => OnMove(context);
        _playerActionControls.Gameplay.Move.canceled += context => OnMove(context);
        _playerActionControls.Gameplay.RollDash.performed += context => OnRollDash(context);
        _playerActionControls.Gameplay.RollDash.canceled += context => OnRollDash(context);
        _playerActionControls.Gameplay.Attack.performed += context => OnAttack(context);
        _playerActionControls.Gameplay.Attack.canceled += context => OnAttack(context);

    
        _initialJumpVelocity = (2f * _jumpHeight * _maxFootSpeed) / _horizontalDistancetoJumpApex;
        _gravity = (-2f * _jumpHeight * _maxFootSpeed * _maxFootSpeed) / (_horizontalDistancetoJumpApex * _horizontalDistancetoJumpApex);
        _timeScale = 1;

        _dashSpeed = _dashDistance / _dashTime;
        _rollSpeed = _rollDistance / _rollTime;

        _attackCooldown = 0;

        // there are 20 animation frames for the roll
        _rollFrameTime = 20 / (_rollTime * 60);

        for (int i = 0; i <_animationStates.Length; i++)
        {
            int hash = Animator.StringToHash("Base Layer." + _animationStates[i]);
            _animationStatesDict.Add(_animationStates[i], hash);
        }
    }

    void Start()
    {
       /* for (int i = 0; i < _animationStates.Length; i++)
        {
            Debug.Log(_animationStatesDict[_animationStates[i]] + _animationStates[i]);
        }*/
    }

    // Update() is called once per frame
    void Update()
    {
        _deltaTime = Time.deltaTime * _timeScale;
        _currentState.UpdateStatesLogic();
        _currentState.UpdateStatesPhysics();
        _controller2D.Move(_currentMovement);

        
        // Debug.Log(_currentState);
    }

    
    void OnEnable()
    {
        _playerActionControls.Enable();
    }

    void OnDisable()
    {
        _playerActionControls.Disable();
    }

    public void ChangeAnimationState(string newAnimationState)
    {
        int newAnimationStateHash = _animationStatesDict[newAnimationState];

        if (newAnimationStateHash == _currentAnimationStateHash) return;

        if (_animator != null)
        {
            _animator.Play(newAnimationStateHash);
        }
        _currentAnimationStateHash = newAnimationStateHash;

    }


    public IEnumerator WaitCoroutine(float waitTime, System.Action<bool> callback)
    {
        //yield on a new YieldInstruction that waits for x seconds.
        yield return new WaitForSeconds(waitTime);

        //After we have waited x seconds set _finished to true
        bool isFinished = true;

        if (callback != null) callback(isFinished);
    }


    public float CalculateHorizontalMovement ()
    {
        

        float result = 0;


        return result;
    }
    
    public void AnimationEventHandler(int id)
    {
        switch (id) {
            case 0:
                _checkAttackCollisions = true;
                break;
            case 1:
                _attackIsFinished = true;
                break;
            default:
                Debug.Log("Invalid Animation Event ID");
                break;
        }
    }

    // Input Response Methods
    #region
  
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isJumpPressed = true;

        } else if (context.canceled)
        {
            _isJumpPressed = false;
            _requireJumpPressed = false;
        }

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            _isMovementPressed = true;
            _moveInputX = _playerActionControls.Gameplay.Move.ReadValue<float>();

        } else if (context.canceled) {
            _isMovementPressed = false;
            _moveInputX = _playerActionControls.Gameplay.Move.ReadValue<float>();
        }
    }

    public void OnRollDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isRollDashPressed = true;
        } else if (context.canceled) {
            _isRollDashPressed = false;
            _requireRollDashPressed = false;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isAttackPressed = true;
        }
        else if (context.canceled)
        {
            _isAttackPressed = false;
        }
    }

    #endregion // Input Response Callbacks

    public void OnDrawGizmosSelected()
    {

        

        Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);

    }
}


