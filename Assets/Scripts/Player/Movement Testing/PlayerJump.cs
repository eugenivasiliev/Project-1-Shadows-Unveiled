using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PlayerJump : MonoBehaviour
{
    private Rigidbody2D _rb;
    private PlayerHorizontalMovement _movement;
    private Player _player;
    private Vector2 _inputDirection;

    // Jump
    private bool _isJumpCut;
    private bool _isJumpFalling;

    // Wall Jump
    private float _wallJumpStartTime;
    private int _lastWallJumpDir;
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize;

    [Space(5)]
    
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize;

    public bool IsJumping { get; private set; }
    public bool IsWallJumping { get; private set; }
    public bool IsSliding { get; private set; }

    // Timers
    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }
    public float LastPressedJumpTime { get; private set; }

    //Layers
    [SerializeField] private LayerMask _groundLayer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _player = GetComponent<Player>();
        _movement = GetComponent<PlayerHorizontalMovement>();
    }

    private void Start()
    {
        SetGravityScale(_player.currentDataMovement.gravityScale);
    }

    private void OnEnable()
    {
        InputManager.jumped += OnJumpPerform;
        InputManager.jumping += OnJumpingPerform;
    }

    private void OnDisable()
    {
        InputManager.jumped -= OnJumpPerform;
        InputManager.jumping -= OnJumpingPerform;
    }

    private void Update()
    {
        // Timers
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;

        if (!IsJumping)
        {
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping)
            {
                LastOnGroundTime = _player.currentDataMovement.coyoteTime;
            }

            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) &&
                  _movement.IsFacingRight) ||
                 (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) &&
                  !_movement.IsFacingRight)) && !IsWallJumping)
            {
                LastOnWallRightTime = _player.currentDataMovement.coyoteTime;
            }

            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) &&
                !_movement.IsFacingRight) ||
                (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && _movement.IsFacingRight)) && !IsWallJumping)
            {
                LastOnWallLeftTime = _player.currentDataMovement.coyoteTime;
            }
        }
        
        // Checks
        if (IsJumping && _rb.velocity.y < 0)
        {
            IsJumping = false;

            if (!IsWallJumping)
                _isJumpFalling = true;
        }

        if (IsWallJumping && Time.time - _wallJumpStartTime > _player.currentDataMovement.wallJumpTime)
        {
            IsWallJumping = false;
        }

        if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
            _isJumpCut = false;

            if (!IsJumping)
                _isJumpFalling = false;
        }
        
        // Jump
        if (CanJump() && LastPressedJumpTime > 0)
        {
            IsJumping = true;
            IsWallJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;
            Jump();
        }
        
        // Wall Jump
        else if(CanWallJump() && LastPressedJumpTime > 0)
        {
            IsWallJumping = true;
            IsJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;
            _wallJumpStartTime = Time.time;
            _lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

            WallJump(_lastWallJumpDir);
        }
        
        // Slide
        if (CanSlide() &&
            ((LastOnWallLeftTime > 0 && _inputDirection.x < 0) ||
             (LastOnWallRightTime > 0 && _inputDirection.x > 0)))
            IsSliding = true;
        else
            IsSliding = false;
        
        // Gravity Conditions
        
        if (IsSliding)
        {
            SetGravityScale(0);
        }
        else if (_rb.velocity.y < 0 && _inputDirection.y < 0)
        {
            SetGravityScale(_player.currentDataMovement.gravityScale * _player.currentDataMovement.fastFallGravityMult);

            _rb.velocity = new Vector2(_rb.velocity.x,
                Mathf.Max(_rb.velocity.y, -_player.currentDataMovement.maxFastFallSpeed));
        }
        
        else if (_isJumpCut)
        {
            SetGravityScale(_player.currentDataMovement.gravityScale * _player.currentDataMovement.jumpCutGravityMult);

            _rb.velocity = new Vector2(_rb.velocity.x,
                Mathf.Max(_rb.velocity.y, -_player.currentDataMovement.maxFallSpeed));
        }
        
        else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(_rb.velocity.y) <
                 _player.currentDataMovement.jumpHangTimeThreshold)
        {
            SetGravityScale(_player.currentDataMovement.gravityScale * _player.currentDataMovement.jumpHangGravityMult);
        }
        
        else if (_rb.velocity.y < 0)
        {
            SetGravityScale(_player.currentDataMovement.gravityScale * _player.currentDataMovement.fallGravityMult);
        }

        else
        {   
            SetGravityScale(_player.currentDataMovement.gravityScale);
        }
    }

    private void FixedUpdate()
    {
        if (IsSliding)
            Slide();
    }

    private void Jump()
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        
        // Jumping Actions
        float force = _player.currentDataMovement.jumpForce;
        if (_rb.velocity.y < 0)
            force -= _rb.velocity.y;
        
        _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    private void WallJump(int dir)
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;
        
        // Wall Jumping Actions
        Vector2 force = new Vector2(_player.currentDataMovement.wallJumpForce.x,
            _player.currentDataMovement.wallJumpForce.y);

        force.x *= dir;

        if (Mathf.Sign(_rb.velocity.x) != Mathf.Sign(force.x))
            force.x -= _rb.velocity.x;

        if (_rb.velocity.y < 0)
            force.y -= _rb.velocity.y;
        
        _rb.AddForce(force, ForceMode2D.Impulse);
    }
    
    
    private void Slide()
    {
        float speedDif = _player.currentDataMovement.slideSpeed - _rb.velocity.y;
        float movement = speedDif * _player.currentDataMovement.slideAccel;

        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime),
            Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));
        
        _rb.AddForce(movement * Vector2.up);
    }
    
    // Input Calls

    public void OnJumpPerform()
    {
        LastPressedJumpTime = _player.currentDataMovement.jumpInputBufferTime;
    }

    public void OnJumpingPerform()
    {
        if (CanJumpCut() || CanWallJumpCut())
            _isJumpCut = true;
    }
    
    // Checks

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !IsJumping;
    }

    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
            (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
    }

    private bool CanJumpCut()
    {
        return IsJumping && _rb.velocity.y > 0;
    }

    private bool CanWallJumpCut()
    {
        return IsWallJumping && _rb.velocity.y > 0;
    }

    public bool CanSlide()
    {
        if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && LastOnGroundTime <= 0)
            return true;
        else
            return false;
    }

    public void SetGravityScale(float scale)
    {
        _rb.gravityScale = scale;
    }

    public bool IsOnAir()
    {
        return IsJumping || IsWallJumping || _isJumpFalling;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
    }
}