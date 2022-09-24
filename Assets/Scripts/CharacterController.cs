using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class CharacterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float acceleration = 70f; // The movement speed acceleration of the player
    [SerializeField] private float maxSpeed = 12f; // The maximum speed of the player
    [SerializeField] private float groundLinDrag = 20f; // The friction applied when not moving <= decceleration

    [Header("Jump Buffer")]
    [SerializeField] private float jumpBufferTime = .1f; // The time window that allows the player to perform an action before it is allowed
    private float jumpBufferTimer = 1000f;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTimeTime = .1f; // The time window in which the player can jump after walking over an edge
    private float coyoteTimeTimer = 1000f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight; // The jump height of the object in units(metres)
    [SerializeField] private float airLinDrag = 2.5f; // The air resistance while jumping
    [SerializeField] private float fullJumpFallMultiplier = 8f; // Gravity applied when doing a full jump
    [SerializeField] private float halfJumpFallMultiplier = 5f; // Gravity applied when doing half jump
    [SerializeField] private int amountOfJumps = 1; // The amount of additional jumps the player can make
    private int jumpsCounted;
    private Vector2 lastJumpPos;

    public Rigidbody2D RigidBody;
    [SerializeField] private CollisionCheck cc;

    private bool jumpHeld;
    private Vector2 moveVal;
    public float m_HorizontalDir
    {
        get
        {
            return moveVal.x;
        }
    }
    public bool m_ChangingDir
    {
        get
        {
            return (RigidBody.velocity.x > 0f && m_HorizontalDir < 0f)
                   || (RigidBody.velocity.x < 0f && m_HorizontalDir > 0f);
        }
    }
    public bool m_CanMove
    {
        get
        {
            return m_HorizontalDir != 0f;
        }
    }
    public bool m_CanJump
    {
        get
        {
            if (amountOfJumps > 1)
            {
                return jumpBufferTimer < jumpBufferTime
                    && (coyoteTimeTimer < coyoteTimeTime || jumpsCounted < amountOfJumps);
            }
            else
            {
                return jumpBufferTimer < jumpBufferTime
                    && coyoteTimeTimer < coyoteTimeTime;
            }
        }
    }

    private void Update()
    {

        if (cc.m_IsGrounded)
        {
            ApplyGroundLinearDrag();
            jumpsCounted = 0; //reset jumps counter
            coyoteTimeTimer = 0; //reset coyote time counter
        }
        else
        {
            ApplyAirLinearDrag();

            ApplyFallGravity();
        }

        coyoteTimeTimer += Time.deltaTime;
        jumpBufferTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (m_CanMove)
            Move();
        if (m_CanJump)
            Jump(jumpHeight, Vector2.up);
    }

    #region Input

    public void OnMove(CallbackContext ctx)
    {
        Debug.Log("Move");

        moveVal = ctx.ReadValue<Vector2>();
    }
    public void OnJump(CallbackContext ctx)
    {
        Debug.Log("Jump");

        if (ctx.performed)
        {
            jumpBufferTimer = 0; //reset the jump buffer
            jumpHeld = true;
        }
        if (ctx.canceled)
            jumpHeld = false;
    }

    #endregion

    private void Move()
    {
        RigidBody.AddForce(new Vector2(m_HorizontalDir, 0f) * acceleration);
        if (Mathf.Abs(RigidBody.velocity.x) > maxSpeed)
            RigidBody.velocity = new Vector2(Mathf.Sign(RigidBody.velocity.x) * maxSpeed, RigidBody.velocity.y); //Clamp velocity when max speed is reached!
    }

    /// <summary>
    /// Makes the player jump with a specific force to reach an exact amount of units in vertical space
    /// </summary>
    public void Jump(float _jumpHeight, Vector2 _dir)
    {
        if (coyoteTimeTimer > coyoteTimeTime && jumpsCounted < 1)
        {
            jumpsCounted = amountOfJumps;
        }

        lastJumpPos = transform.position;
        coyoteTimeTimer = coyoteTimeTime;
        jumpBufferTimer = jumpBufferTime;
        jumpsCounted++;

        ApplyAirLinearDrag();

        RigidBody.gravityScale = fullJumpFallMultiplier;

        RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0f); //set y velocity to 0
        float jumpForce;

        cc.StartCoroutine(cc.DisableWallRay());

        jumpForce = Mathf.Sqrt(_jumpHeight * -2f * (Physics.gravity.y * RigidBody.gravityScale));
        RigidBody.AddForce(_dir * jumpForce, ForceMode2D.Impulse);
    }

    #region Drag&Gravity
    /// <summary>
    /// Applies the ground friction based on wether the player is moving or giving no horizontal inputs
    /// </summary>
    private void ApplyGroundLinearDrag()
    {
        if (Mathf.Abs(m_HorizontalDir) < .4f || m_ChangingDir)
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
    #endregion
}
