using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : MonoBehaviour
{
    #region Singleton
    private static PlayerController instance;
    public static PlayerController Instance { get { return instance; } }

    private void Initialize()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Terminate()
    {
        if (this == Instance)
        {
            instance = null;
        }
    }
    #endregion

    [Header("Allow moves")]
    [SerializeField] private bool allowMoving = true;
    [SerializeField] private bool allowJumping = true;
    [SerializeField] private bool allowDashing = false;
    [SerializeField] private bool allowWallHang = false;
    [SerializeField] private bool allowWallHops = false;
    [SerializeField] private bool allowWallClimb = false;
    [SerializeField] private bool lookAtMouse = false;

    public bool IsInEvent;

    [HideInInspector] public bool AllowMoving { get { return allowMoving; } set { allowMoving = value; } }
    [HideInInspector] public bool AllowJumping { get { return allowJumping; } set { allowJumping = value; } }
    [HideInInspector] public bool AllowDashing { get { return allowDashing; } set { allowDashing = value; } }
    [HideInInspector] public bool AllowWallHang { get { return allowWallHang; } set { allowWallHang = value; } }
    [HideInInspector] public bool AllowWallHops { get { return allowWallHops; } set { allowWallHops = value; } }
    [HideInInspector] public bool AllowWallClimb { get { return allowWallClimb; } set { allowWallClimb = value; } }
    [HideInInspector] public int AmountOfJumps { get { return amountOfJumps; } set { amountOfJumps = Mathf.Clamp(value, 1, 10000); } }
    [HideInInspector] public bool LookAtMouse { get { return lookAtMouse; } set { lookAtMouse = value; } }

    [Header("Movement")]
    [SerializeField] private float acceleration = 70f; // The movement speed acceleration of the player
    private float accelerationRuntime;
    [SerializeField] private float maxSpeed = 12f; // The maximum speed of the player
    [SerializeField] private float groundLinDrag = 20f; // The friction applied when not moving <= decceleration

    [Header("Buffer & Timer")]
    [SerializeField] private float jumpBufferTime = .1f; // The time window that allows the player to perform an action before it is allowed
    private float jumpBufferTimer = 1000f;
    [SerializeField] private float coyoteTimeTime = .1f; // The time window in which the player can jump after walking over an edge
    private float coyoteTimeTimer = 1000f;
    [SerializeField] private float dashBufferTime = .1f;
    private float dashBufferTimer = 1000f;
    [SerializeField] private float wallHopBufferTime = .1f; //The time window that allows the player to perform a wall jump after leaving the wall
    private float wallHopBufferTimer = 1000f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight; // The jump height of the object in units(metres)
    [SerializeField] private float airLinDrag = 2.5f; // The air resistance while jumping
    [SerializeField] private float fullJumpFallMultiplier = 8f; // Gravity applied when doing a full jump
    [SerializeField] private float halfJumpFallMultiplier = 5f; // Gravity applied when doing half jump
    [SerializeField] private int amountOfJumps = 1; // The amount of additional jumps the player can make
    [SerializeField] private float m_AirborneSteer = 35f;
    [SerializeField] private VisualEffect m_JumpParticles;
    [SerializeField] private VisualEffect m_LandingParticles;
    private int jumpsCounted;
    private Vector2 lastJumpPos;

    [Header("Dash")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashCooldown = 1.5f;
    private float dashCDTimer = 1000f;
    private bool isDashing;

    [Header("Wall Movement")]
    [SerializeField] private float wallHopHeight = 1.5f;
    [SerializeField] private float onWallGravityMultiplier;
    [SerializeField] private float wallClimbAcceleration = 50f;
    [SerializeField] private float wallClimbMaxSpeed = 6f;

    [Header("References")]
    public Rigidbody2D RigidBody;
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private CollisionCheck cc;
    [SerializeField] private ScriptableInt m_PlayerHealth;
    [SerializeField] private Animator m_Animator;
    public Health Health;
    private Camera mainCam;
    private Mouse mouse;

    private bool facingRight;
    private bool jumpHeld;
    private bool wallHangHeld;

    private Vector2 moveVal;
    private Vector3 mousePos;
    private float previousYVelocity = 0;

    private float horizontalDir => moveVal.x;
    private float verticalDir => moveVal.y;
    private bool changingDir => (RigidBody.velocity.x > 0f && horizontalDir < 0f)
                                 || (RigidBody.velocity.x < 0f && horizontalDir > 0f);
    private bool canMove => allowMoving && horizontalDir != 0f;
    public bool canJump
    {
        get
        {
            if (allowJumping)
            {
                if (amountOfJumps > 1)
                    return jumpBufferTimer < jumpBufferTime && (coyoteTimeTimer < coyoteTimeTime || jumpsCounted < amountOfJumps);
                else
                    return jumpBufferTimer < jumpBufferTime && coyoteTimeTimer < coyoteTimeTime;
            }
            else
                return false;
        }
    }
    private bool canDash => allowDashing && dashBufferTimer < dashBufferTime && !isDashing && dashCDTimer >= dashCooldown;
    private bool canWallHop => allowWallHops && wallHopBufferTimer < wallHopBufferTime;
    private bool canWallHang => allowWallHang && wallHangHeld && (cc.m_IsOnLeftWall || cc.m_IsOnRightWall);

    private void Awake()
    {
        Initialize();

    }

    private void InitializeInput()
    {
        InputActionMap map = GameManager.Instance.PlayerInput.currentActionMap;

        map.FindAction("Move").performed += OnMove;
        map.FindAction("Move").canceled += OnMove;

        map.FindAction("Jump").performed += OnJump;
        map.FindAction("Jump").canceled += OnJump;

        map.FindAction("Dash").performed += OnDash;
        map.FindAction("Dash").canceled += OnDash;

        map.FindAction("WallHang").performed += OnWallHang;
        map.FindAction("WallHang").canceled += OnWallHang;
    }
    private void RemoveBindings()
    {
        InputActionMap map = GameManager.Instance.PlayerInput.currentActionMap;

        map.FindAction("Move").performed -= OnMove;
        map.FindAction("Move").canceled -= OnMove;

        map.FindAction("Jump").performed -= OnJump;
        map.FindAction("Jump").canceled -= OnJump;

        map.FindAction("Dash").performed -= OnDash;
        map.FindAction("Dash").canceled -= OnDash;

        map.FindAction("WallHang").performed -= OnWallHang;
        map.FindAction("WallHang").canceled -= OnWallHang;
    }

    private void Start()
    {
        InitializeInput();
        mouse = Mouse.current;
        mainCam = Camera.main;

        #region Add Death event
        if (Health == null)
            Health = GetComponent<Health>();

        Health.E_TriggerDeath += PlayerDied;
        #endregion
    }
    private void Update()
    {

        Debug.Log(canWallHop);

        mousePos = mainCam.ScreenToWorldPoint(mouse.position.ReadValue());

        #region Player looking rotation
        if (lookAtMouse)
        {
            if (canWallHang)
            {
                if (cc.m_IsOnLeftWall)
                    m_SpriteRenderer.flipX = false;
                else if (cc.m_IsOnRightWall)
                    m_SpriteRenderer.flipX = true;
            }
            else
            {
                if (mousePos.x < transform.position.x)
                {
                    facingRight = false;
                    m_SpriteRenderer.flipX = true;
                }
                else if (mousePos.x > transform.position.x)
                {
                    facingRight = true;
                    m_SpriteRenderer.flipX = false;
                }
            }
        }
        else
        {
            if (canWallHang)
            {
                if (cc.m_IsOnLeftWall)
                    m_SpriteRenderer.flipX = false;
                else if (cc.m_IsOnRightWall)
                    m_SpriteRenderer.flipX = true;
            }
            else
            {
                if (horizontalDir < 0)
                {
                    facingRight = false;
                    m_SpriteRenderer.flipX = true;
                }
                else if (horizontalDir > 0)
                {
                    facingRight = true;
                    m_SpriteRenderer.flipX = false;
                }
            }
        }
        #endregion

        #region Animation

        // Jump & Fall Animation
        if (!canWallHang)
        {
            if (RigidBody.velocity.y > 7)
                m_Animator.SetTrigger("Jumping");
            else if (RigidBody.velocity.y < -7)
                m_Animator.SetTrigger("Falling");
        }

        // WallHang & Climb Animation
        if (canWallHang)
        {
            if (verticalDir == 0)
                m_Animator.SetTrigger("WallHang");

            if (allowWallClimb && verticalDir != 0)
                m_Animator.SetTrigger("WallClimb");
        }
        // Run and Idle Animation
        if (cc.m_IsGrounded && !canWallHang)
        {
            if (Mathf.Abs(RigidBody.velocity.x) > 7)
                m_Animator.SetTrigger("Running");
            else
                m_Animator.SetTrigger("Idle");
        }


        #endregion

        if (IsInEvent) return; // !!!DONT RUN CODE BELOW IF IS IN EVENT!!!

        if (cc.m_IsGrounded)
        {
            if (previousYVelocity < -20)
            {
                CameraManager.Instance.Shake();

                m_LandingParticles.SetFloat("MoveDir", horizontalDir);
                m_LandingParticles.Play();
            }

            ApplyGroundLinearDrag();
            jumpsCounted = 0; //reset jumps counter
            coyoteTimeTimer = 0; //reset coyote time counter
            isDashing = false;
            accelerationRuntime = acceleration;
        }
        else
        {
            accelerationRuntime = m_AirborneSteer;

            ApplyAirLinearDrag();

            if (canWallHang)
            {
                isDashing = false;

                //if (allowWallClimb && verticalDir != 0)
                ApplyWallHangGravity();
            }
            else
                ApplyFallGravity();
        }
        if (allowWallHops && canWallHang)
        {
            jumpsCounted = amountOfJumps - 1;

            if (((cc.m_IsOnLeftWall && horizontalDir < 0) || (cc.m_IsOnRightWall && horizontalDir > 0)) && jumpBufferTimer < jumpBufferTime)
            {
                wallHopBufferTimer = 0;
            }
        }


        #region Increase Buffer & Timer
        coyoteTimeTimer += Time.deltaTime;
        jumpBufferTimer += Time.deltaTime;
        dashBufferTimer += Time.deltaTime;
        dashCDTimer += Time.deltaTime;
        wallHopBufferTimer += Time.deltaTime;
        #endregion

        previousYVelocity = RigidBody.velocity.y;
    }

    private void FixedUpdate()
    {
        if (IsInEvent) return; // !!!DONT RUN CODE BELOW IF IS IN EVENT!!!

        if (canDash)
            Dash(mousePos.x, mousePos.y);

        // has performed no dash
        if (!isDashing)
        {
            if (canMove)
                Move(horizontalDir);

            if (canJump)
            {
                // Normal jump
                if (!canWallHang)
                    Jump(jumpHeight, Vector2.up);
            }
            // is on wall and climbs it
            if (canWallHang && allowWallClimb)
                Climb();

            // Is on wall and jumps against it (climb with jumps)
            if (canWallHang && canWallHop)
                Jump(wallHopHeight, new Vector2(-horizontalDir / 2f, 1f));
        }

        // has performed a dash and has an additional jump ready
        else if (isDashing && canJump)
        {
            if (canJump)
                Jump(jumpHeight, Vector2.up);
        }
    }

    private void PlayerDied()
    {
        Debug.LogWarning("Player Dies");
        UIManager.Instance.OpenDeathPanel();
        //this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

    #region Input

    public void OnMove(CallbackContext ctx)
    {
        if (allowMoving)
            moveVal = ctx.ReadValue<Vector2>();
        else
            moveVal = Vector2.zero;
    }
    public void OnJump(CallbackContext ctx)
    {
        if (ctx.performed)
        {
            jumpBufferTimer = 0; //reset the jump buffer
            jumpHeld = true;
        }
        if (ctx.canceled)
            jumpHeld = false;
    }
    public void OnDash(CallbackContext ctx)
    {
        dashBufferTimer = 0;
    }
    public void OnWallHang(CallbackContext ctx)
    {
        if (ctx.performed)
            wallHangHeld = true;
        if (ctx.canceled)
            wallHangHeld = false;
    }
    #endregion

    private void Move(float _xDir)
    {
        RigidBody.AddForce(new Vector2(_xDir, 0) * accelerationRuntime);
        if (Mathf.Abs(RigidBody.velocity.x) > maxSpeed)
            RigidBody.velocity = new Vector2(Mathf.Sign(RigidBody.velocity.x) * maxSpeed, RigidBody.velocity.y); //Clamp velocity when max speed is reached!
    }

    /// <summary>
    /// Makes the player jump with a specific force to reach an exact amount of units in vertical space
    /// </summary>
    public void Jump(float _jumpHeight, Vector2 _dir)
    {
        // Play sound
        SoundManager.Instance.PlaySound(SoundManager.Instance.JumpSound);

        // Play particles
        m_JumpParticles.SetFloat("MoveDir", -horizontalDir);
        m_JumpParticles.Play();

        if (coyoteTimeTimer > coyoteTimeTime && jumpsCounted < 1)
        {
            jumpsCounted = amountOfJumps;
        }

        lastJumpPos = transform.position;
        coyoteTimeTimer = coyoteTimeTime;
        jumpBufferTimer = jumpBufferTime;
        wallHopBufferTimer = wallHopBufferTime;
        jumpsCounted++;

        ApplyAirLinearDrag();

        RigidBody.gravityScale = fullJumpFallMultiplier;

        RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0f); //set y velocity to 0
        float jumpForce;

        cc.StartCoroutine(cc.DisableWallRay());

        jumpForce = Mathf.Sqrt(_jumpHeight * -2f * (Physics.gravity.y * RigidBody.gravityScale));
        RigidBody.AddForce(_dir * jumpForce, ForceMode2D.Impulse);
    }

    private void Dash(float _x, float _y, bool directionBased = false)
    {
        isDashing = true;
        dashCDTimer = 0;

        RigidBody.velocity = Vector2.zero;
        RigidBody.gravityScale = 0f;
        RigidBody.drag = 0f;

        Vector2 dir;

        // Based on the moving direction of the player
        if (directionBased)
        {
            if (_x != 0f || _y != 0f)
                dir = new Vector2(_x, _y);

            // Based on the direction the player faces
            else
            {
                if (facingRight)
                    dir = new Vector2(1, 0);
                else
                    dir = new Vector2(-1, 0);
            }
        }
        // Based on position of x&y relative to the player
        else
            dir = new Vector2(_x - transform.position.x, _y - transform.position.y);

        dir.Normalize();

        RigidBody.AddForce(dir * dashForce, ForceMode2D.Impulse);
    }
    private void Climb()
    {
        RigidBody.AddForce(new Vector2(0, verticalDir) * wallClimbAcceleration);
        if (Mathf.Abs(RigidBody.velocity.y) > wallClimbMaxSpeed)
            RigidBody.velocity = new Vector2(RigidBody.velocity.x, Mathf.Sign(RigidBody.velocity.y) * wallClimbMaxSpeed);
    }

    #region Drag&Gravity
    /// <summary>
    /// Applies the ground friction based on wether the player is moving or giving no horizontal inputs
    /// </summary>
    private void ApplyGroundLinearDrag()
    {
        if (Mathf.Abs(horizontalDir) < .4f || changingDir)
        {
            RigidBody.drag = groundLinDrag;
        }
        else
        {
            RigidBody.drag = 0f;
        }
    }
    /// <summary>
    /// Applies the air resistance when the player is jumping
    /// </summary>
    private void ApplyAirLinearDrag()
    {
        RigidBody.drag = airLinDrag;
    }
    /// <summary>
    /// Applies the fall gravity based on the players jump height and input
    /// </summary>
    private void ApplyFallGravity()
    {
        if (RigidBody.velocity.y < 0f || transform.position.y - lastJumpPos.y > jumpHeight)
        {
            RigidBody.gravityScale = fullJumpFallMultiplier;
        }
        else if (RigidBody.velocity.y > 0f && !jumpHeld)
        {
            RigidBody.gravityScale = halfJumpFallMultiplier;
        }
        else
        {
            RigidBody.gravityScale = 1f;
        }
    }
    private void ApplyWallHangGravity()
    {
        RigidBody.gravityScale = onWallGravityMultiplier;
        RigidBody.velocity = new Vector2(0, 0f); //set y velocity to 0
    }
    #endregion

    private void OnDestroy()
    {
        MapManager.Instance.RemoveBindings();
        RemoveBindings();
        Terminate();
    }
}
