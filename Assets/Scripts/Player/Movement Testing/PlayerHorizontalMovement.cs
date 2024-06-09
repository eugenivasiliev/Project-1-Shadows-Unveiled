using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input = UnityEngine.Windows.Input;

public class PlayerHorizontalMovement : MonoBehaviour
{
    private Player _player;
    private Rigidbody2D _rb;
    private PlayerJump _jump;
    private Vector2 _moveDirection;

    public Transform groundCheckPoint;
    public Vector2 groundCheckSize;
    public bool IsFacingRight { get; private set; }
    public float LastOnGroundTime { get; private set; }
    public LayerMask groundLayer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _player = GetComponent<Player>();
        _jump = GetComponent<PlayerJump>();
    }

    /*private void OnEnable()
    {
        InputManager.horizontalMovement += InputDirection;
    }

    private void OnDisable()
    {
        InputManager.horizontalMovement -= InputDirection;
    }*/

    private void Update()
    {
        LastOnGroundTime -= Time.deltaTime;

        if (_moveDirection.x != 0)
            CheckFacingDirection(_moveDirection.x > 0);

        if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
            LastOnGroundTime = 0.1f;
    }
    
    public void InputDirection(float direction)
    {
        _moveDirection.x = direction;
    }

    private void FixedUpdate()
    {
        if (_jump.IsWallJumping)
            Movement(_player.currentDataMovement.wallJumpRunLerp);

        else
            Movement(1);
        
    }

    private void Movement(float lerpAmount)
    {
        float targetSpeed = _moveDirection.x * _player.currentDataMovement.runMaxSpeed;

        targetSpeed = Mathf.Lerp(_rb.velocity.x, targetSpeed, lerpAmount);
        
        // Calculate acceleration
        float accelRate;

        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f)
                ? _player.currentDataMovement.runAccelAmount
                : _player.currentDataMovement.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f)
                ? _player.currentDataMovement.runAccelAmount * _player.currentDataMovement.accelInAir
                : _player.currentDataMovement.runDeccelAmount * _player.currentDataMovement.deccelInAir;
        
        // Apex Acceleration
        if (_jump.IsOnAir() && Mathf.Abs(_rb.velocity.y) < _player.currentDataMovement.jumpHangTimeThreshold)
        {
            accelRate *= _player.currentDataMovement.jumpHangAccelerationMult;
            targetSpeed *= _player.currentDataMovement.jumpHangMaxSpeedMult;
        }
        
        // Momentum
        if (_player.currentDataMovement.doConserveMomentum &&
            Mathf.Abs(_rb.velocity.x) > Mathf.Abs(targetSpeed) &&
            Mathf.Sign(_rb.velocity.x) == Mathf.Sign(targetSpeed) &&
            Mathf.Abs(targetSpeed) > 0.01f &&
            LastOnGroundTime < 0)
        {
            accelRate = 0;
        }
        
        // Current velocity VS needed velocity
        float speedDif = targetSpeed - _rb.velocity.x;

        float movement = speedDif * accelRate;
        
        _rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }

    private void CheckFacingDirection(bool movedRight)
    {
        if (movedRight != IsFacingRight)
            Turn();
    }
}
