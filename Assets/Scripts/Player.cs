using System;
using UnityEngine;

public class Player : MonoBehaviour, IObserver, ISubscriber
{
    [SerializeField] Transform groundCheckPivot;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundCheckDistance;
    [SerializeField] float jumpForce;
    [SerializeField] float movementSpeed;
    [SerializeField] float delayTime = 1;

# if UNITY_EDITOR
    public float LandingDealyTime => delayTime;
#endif

    public Animator Animator;
    Rigidbody _rigidBody;
    public GenericStateMachine<ECharacterState> StateMachine;
    public bool JumpRequested { get; private set; } = false;
    public bool MoveRequest { get; private set; } = false;
    public bool IsGrounded { get; private set; }
    public Vector2 MoveDirection { get; private set; }
    public bool AttackRequested { get; private set; }
    private bool _canMove = true;
    private void Awake()
    {
        Publisher.Subscribe(this, typeof(PauseMessage));
        Animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody>();

        StateMachine = new GenericStateMachine<ECharacterState>();
        
        StateMachine.RegisterState(ECharacterState.Idle, new IdleCharacterState(this));
        StateMachine.RegisterState(ECharacterState.Walking, new WalkingCharacterState(this));
        StateMachine.RegisterState(ECharacterState.Jumping, new JumpingCharacterState(this));
        StateMachine.RegisterState(ECharacterState.Falling, new FallingCharacterState(this));
        StateMachine.RegisterState(ECharacterState.Landing, new LandingCharacterState(this, delayTime));
        StateMachine.RegisterState(ECharacterState.Attacking, new AttackCharacterState(this));

        SetState(ECharacterState.Idle);
    }

    private void OnEnable()
    {
        GameManager.Instance.Attach(this);
    }
    private void OnDisable()
    {
        GameManager.Instance.Detach(this);
    }

    public void GroundCheck()
    {
        IsGrounded = Physics.Raycast(new Ray(groundCheckPivot.position, Vector3.down), groundCheckDistance, groundMask);
    }

    public void SetState(ECharacterState newState)
    {
        StateMachine.SetState(newState);
    }

    void Update()
    {
        if (!_canMove)
            return;

        GroundCheck();

        StateMachine.OnUpdate();
    }

    private void FixedUpdate()
    {
        if (!_canMove)
            return;

        StateMachine.OnFixedUpdate();
    }

    internal void JumpRequest()
    {
        JumpRequested = true;
    }

    public void JumpResponse()
    {
        JumpRequested = false; 
    }

    internal void MoveDirectionRequest(Vector2 direction)
    {
        MoveRequest = direction != Vector2.zero;
        MoveDirection = direction;
    }

    internal void JumpImpulse()
    {
        _rigidBody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
    }

    internal void MoveHorizontal()
    {
        Vector3 horizontalMovement = 
            movementSpeed * Time.fixedDeltaTime * new Vector3(MoveDirection.x, 0, MoveDirection.y).normalized;

        _rigidBody.linearVelocity =
            new Vector3(horizontalMovement.x, _rigidBody.linearVelocity.y, horizontalMovement.z);
    }

    internal void AttackRequest()
    {
        AttackRequested = true;
    }

    public void AttackResponse()
    {
        AttackRequested = false; 
    }

    public void ObserverUpdate(ISubject subject)
    {
        var gameManager = subject as GameManager;
        if (gameManager != null)
        {
            _canMove = !gameManager.IsPaused;
        }
    }

    public void OnPublish(IPublisherMessage message)
    {
    }

    public void OnDisableSubscriber()
    {
    }
}
