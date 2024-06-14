using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Movement : MonoBehaviour
{

	private PlayerMovementData _data;
	
	private PlayerAnimations _animations;
	
	public PlayerMovementData LightProfile;
	public PlayerMovementData DarkProfile;

    [SerializeField] private AudioSource collectSound;

	#region Variables
    public Rigidbody2D RB { get; private set; }
    
	public bool IsFacingRight { get; private set; }
	public bool IsJumping { get; private set; }
	public bool IsDoubleJumping { get; private set; }
	public bool IsWallJumping { get; private set; }
	public bool IsDashing { get; private set; }
	public bool IsLightDashing { get; private set; }
	public bool IsSliding { get; private set; }

	public bool CanDashDirection;

	public Vector2 DirectionToDash;
	
	public float LastOnGroundTime { get; private set; }
	public float LastOnWallTime { get; private set; }
	public float LastOnWallRightTime { get; private set; }
	public float LastOnWallLeftTime { get; private set; }
	
	private bool _isJumpCut;
	private bool _isJumpFalling;
	
	private float _wallJumpStartTime;
	private int _lastWallJumpDir;

	private int _dashesLeft;
	private bool _dashRefilling;
	private Vector2 _lastDashDir;
	private bool _isDashAttacking;

	private Vector2 _moveInput;
	public float LastPressedJumpTime { get; private set; }
	public float LastPressedDashTime { get; private set; }
	
	[Header("Checks")] 
	[SerializeField] private Transform _groundCheckPoint;
	
	[SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
	[Space(5)]
	[SerializeField] private Transform _frontWallCheckPoint;
	[SerializeField] private Transform _backWallCheckPoint;
	[SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);

    [Header("Layers & Tags")]
	[SerializeField] private LayerMask _groundLayer;


	[HideInInspector] public Rigidbody2D standingPlatform;

    [SerializeField] private int score = 0;

	#endregion

    private void Awake()
	{
		RB = GetComponent<Rigidbody2D>();
		_animations = GetComponent<PlayerAnimations>();
		_data = LightProfile;
	}

	private void Start()
	{
		SetGravityScale(_data.gravityScale);
		IsFacingRight = true;
	}

	private void OnEnable()
	{
		InputManager.jumped += OnJumpInput;
		InputManager.jumping += OnJumpUpInput;
		InputManager.dashed += OnDashInput;
		InputManager.changeElement += ChangeCurrentProfile;
	}

	private void OnDisable()
	{
		InputManager.jumped -= OnJumpInput;
		InputManager.jumping -= OnJumpUpInput;
		InputManager.dashed -= OnDashInput;
		InputManager.changeElement -= ChangeCurrentProfile;
	}

	private void Update()
	{
        
        //footsteps.enabled = CanJump() && !IsSliding && RB.velocity != Vector2.zero;

        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
		LastOnWallTime -= Time.deltaTime;
		LastOnWallRightTime -= Time.deltaTime;
		LastOnWallLeftTime -= Time.deltaTime;

		LastPressedJumpTime -= Time.deltaTime;
		LastPressedDashTime -= Time.deltaTime;
		#endregion

		#region INPUT HANDLER

		if (!MainUIManager.PauseState)
		{
			_moveInput.x = Input.GetAxisRaw("Horizontal");
			_moveInput.y = Input.GetAxisRaw("Vertical");

			if (_moveInput.x != 0)
				CheckDirectionToFace(_moveInput.x > 0);
		}
		#endregion

		#region COLLISION CHECKS
		if (!IsDashing && !IsJumping)
		{
			if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping)
			{
				LastOnGroundTime = _data.coyoteTime;
				IsDoubleJumping = false;
			}		
			
			if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
					|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)) && !IsWallJumping)
				LastOnWallRightTime = _data.coyoteTime;
			
			if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
				|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)) && !IsWallJumping)
				LastOnWallLeftTime = _data.coyoteTime;
			
			LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
		}
		#endregion

		#region JUMP CHECKS
		if (IsJumping && RB.velocity.y <= 0)
		{
			IsJumping = false;
			
			_isJumpFalling = true;
		}

		if (IsWallJumping && Time.time - _wallJumpStartTime > _data.wallJumpTime)
		{
			IsWallJumping = false;
		}

		if (LastOnGroundTime > 0 && (!IsJumping || !IsDoubleJumping) && !IsWallJumping)
        {
			_isJumpCut = false;
			
			_isJumpFalling = false;
		}

		if (!IsDashing)
		{
			if (CanJump() && LastPressedJumpTime > 0)
			{
				IsJumping = true;
				IsDoubleJumping = false;
				IsWallJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;
				
				_animations.JumpAnimationTrigger();
				
				Jump();
			}

			if (CanDoubleJump() && LastPressedJumpTime > 0)
			{
				IsDoubleJumping = true;
				IsWallJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;
				Jump();
			}

			else if (CanWallJump() && LastPressedJumpTime > 0)
			{
				IsWallJumping = true;
				IsJumping = false;
				IsDoubleJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;
				_wallJumpStartTime = Time.time;
				_lastWallJumpDir = LastOnWallRightTime > 0 ? -1 : 1;
			
				WallJump(_lastWallJumpDir);
			}	
		}
		#endregion

		#region DASH CHECKS

		if (CanDashDirection && LastPressedDashTime > 0)
		{
			Sleep(_data.dashSleepTime);

			IsDashing = true;
			IsJumping = false;
			IsWallJumping = false;
			_isJumpCut = false;

			StartCoroutine(nameof(StartDash), DirectionToDash);
		}

		if (CanDash() && LastPressedDashTime > 0)
		{
			Sleep(_data.dashSleepTime);

			if (IsOnLightElement() == false)
			{
				if (_moveInput.x > 0 || _moveInput.x < 0)
					_lastDashDir = new Vector2(_moveInput.x, 0);

				if (_moveInput.y > 0 || _moveInput.y < 0)
					_lastDashDir = new Vector2(0, _moveInput.y);
				else
					_lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;
			}
			else
			{
				_lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;
			}

			IsDashing = true;
			IsJumping = false;
			IsWallJumping = false;
			_isJumpCut = false;

			StartCoroutine(nameof(StartDash), _lastDashDir);
		}

		#endregion
		

		#region SLIDE CHECKS
		if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
			IsSliding = true;
		else
			IsSliding = false;
		#endregion

		#region GRAVITY

		if (!_isDashAttacking)
		{
			if (IsSliding)
			{
				SetGravityScale(0);
			}
			else if (RB.velocity.y < 0 && _moveInput.y < 0)
			{
				SetGravityScale(_data.gravityScale * _data.fastFallGravityMult);
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -_data.maxFastFallSpeed));
			}
			else if (_isJumpCut)
			{
				SetGravityScale(_data.gravityScale * _data.jumpCutGravityMult);
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -_data.maxFallSpeed));
			}
			else if ((IsJumping || IsDoubleJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < _data.jumpHangTimeThreshold)
			{
				SetGravityScale(_data.gravityScale * _data.jumpHangGravityMult);
			}
			else if (RB.velocity.y < 0)
			{
				SetGravityScale(_data.gravityScale * _data.fallGravityMult);
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -_data.maxFallSpeed));
			}
			else
			{
				SetGravityScale(_data.gravityScale);
			}
		}
		else
		{
			SetGravityScale(0);
		}
		#endregion
    }

    private void FixedUpdate()
	{
		if (!IsDashing)
		{
			if (IsWallJumping)
				Run(_data.wallJumpRunLerp);
			else
				Run(1);
		}
		else if (_isDashAttacking)
		{
			Run(_data.dashEndRunLerp);
		}
		
		if (IsSliding)
			Slide();
    }

    #region INPUT
    public void OnJumpInput()
	{
		LastPressedJumpTime = _data.jumpInputBufferTime;
	}

	public void OnJumpUpInput()
	{
		if (CanJumpCut() || CanWallJumpCut())
			_isJumpCut = true;
	}

	public void OnDashInput()
	{
		LastPressedDashTime = _data.dashInputBufferTime;
	}
    #endregion

    #region GRAVITY
    public void SetGravityScale(float scale)
	{
		RB.gravityScale = scale;
	}

    private void Sleep(float duration)
    {
	    StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
	    Time.timeScale = 0;
	    yield return new WaitForSecondsRealtime(duration);
	    Time.timeScale = 1;
    }
    #endregion
    
    #region RUN
    private void Run(float lerpAmount)
	{
		float targetSpeed = _moveInput.x * _data.runMaxSpeed;
		targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

		#region Calculate AccelRate
		float accelRate;
		
		if (LastOnGroundTime > 0)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _data.runAccelAmount : _data.runDeccelAmount;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _data.runAccelAmount * _data.accelInAir : _data.runDeccelAmount * _data.deccelInAir;
		#endregion

		#region Bonus Jump Apex Accel
		if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < _data.jumpHangTimeThreshold)
		{
			accelRate *= _data.jumpHangAccelerationMult;
			targetSpeed *= _data.jumpHangMaxSpeedMult;
		}
		#endregion

		#region Momentum
		if(_data.doConserveMomentum && 
			Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && 
			Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && 
			Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
		{
			accelRate = 0; 
		}
		#endregion
		
		float speedDif = targetSpeed - RB.velocity.x + (standingPlatform ? standingPlatform.velocity.x : 0.0f);

		float movement = speedDif * accelRate;
		
		RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
	}

	private void Turn()
	{
		//stores scale and flips the player along the x axis, 
		Vector3 scale = transform.localScale; 
		scale.x *= -1;
		transform.localScale = scale;

		IsFacingRight = !IsFacingRight;
	}
    #endregion

    #region JUMP
    private void Jump()
	{
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;

		#region Perform Jump
		float force = _data.jumpForce;
		if (RB.velocity.y < 0)
			force -= RB.velocity.y;

		RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		#endregion
	}

	private void WallJump(int dir)
	{
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;
		LastOnWallRightTime = 0;
		LastOnWallLeftTime = 0;

		#region Perform Wall Jump
		Vector2 force = new Vector2(_data.wallJumpForce.x, _data.wallJumpForce.y);
		force.x *= dir;

		if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
			force.x -= RB.velocity.x;

		if (RB.velocity.y < 0)
			force.y -= RB.velocity.y;
		
		RB.AddForce(force, ForceMode2D.Impulse);
		#endregion
	}
	#endregion

	#region DASH

	private IEnumerator StartDash(Vector2 dir)
	{
		LastOnGroundTime = 0;
		LastPressedDashTime = 0;

		float startTime = Time.time;

		_dashesLeft--;
		_isDashAttacking = true;
		
		_animations.DashAnimationTrigger(1);

		SetGravityScale(0);

		while (Time.time - startTime <= _data.dashAttackTime)
		{
			RB.velocity = dir.normalized * _data.dashSpeed;

			yield return null;
		}

		startTime = Time.time;

		_isDashAttacking = false;
		
		SetGravityScale(_data.gravityScale);
		RB.velocity = _data.dashEndSpeed * dir.normalized;
		
		_animations.DashAnimationTrigger(0);

		while (Time.time - startTime <= _data.dashEndTime)
		{
			yield return null;
		}

		if (IsOnLightElement())
		{
			_lastDashDir = Vector2.zero;
		}

		IsDashing = false;
	}

	private IEnumerator RefillDash(int amount)
	{
		_dashRefilling = true;
		yield return new WaitForSeconds(_data.dashRefillTime);
		_dashRefilling = false;
		_dashesLeft = Mathf.Min(_data.dashAmount, _dashesLeft + amount);
	}

	#endregion

	#region SLIDE
	private void Slide()
	{
		if (RB.velocity.y > 0)
		{
			RB.AddForce(-RB.velocity.y * Vector2.up, ForceMode2D.Impulse);
		}
		
		float speedDif = _data.slideSpeed - RB.velocity.y;	
		float movement = speedDif * _data.slideAccel;
		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

		RB.AddForce(movement * Vector2.up);
	}
    #endregion


    #region CHECKS
    public void CheckDirectionToFace(bool isMovingRight)
	{
		if (isMovingRight != IsFacingRight)
			Turn();
	}

    private bool CanJump()
    {
		return LastOnGroundTime > 0 && !IsJumping;
    }
    
    private bool CanDoubleJump()
    {
	    if (!_data.canDoubleJump) return false;
	    return LastOnGroundTime < 0 && !IsDoubleJumping && !CanWallJump();
	    
    }

	private bool CanWallJump()
    {
		return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
			 (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
	}

	private bool CanJumpCut()
    {
		return (IsJumping || IsDoubleJumping) && RB.velocity.y > 0;
    }

	private bool CanWallJumpCut()
	{
		return IsWallJumping && RB.velocity.y > 0;
	}

	private bool CanDash()
	{
		if (!IsDashing && _dashesLeft < _data.dashAmount && LastOnGroundTime > 0 && !_dashRefilling)
		{
			StartCoroutine(nameof(RefillDash), 1);
		}

		return _dashesLeft > 0;
	}

	public bool CanSlide()
    {
		if (LastOnWallTime > 0 && !IsJumping && !IsDoubleJumping && !IsWallJumping && !IsDashing && LastOnGroundTime <= 0)
			return true;
		
		return false;
	}
    #endregion

    #region ELEMENTS METHODS

    private void ChangeCurrentProfile()
    {
	    if (_data == LightProfile)
		    _data = DarkProfile;
	    else
		    _data = LightProfile;
    }

    public bool IsOnLightElement()
    {
	    return _data.dataType == DataMovementType.LIGHT;
    }

    #endregion

    public void Collect(int _score) { score += _score; collectSound.Play(); }
    public void CollectDashCrystal() { StartCoroutine(nameof(RefillDash), 1); }

    #region TESTING
    private void OnDrawGizmosSelected()
    {
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
		Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
	}
    #endregion
}
