using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    public float timer;

    public LayerMask playerLayer;
    public LayerMask playerDashLayer;

    [SerializeField] public MovementState movementState;

    public Walking walking;
    public Jump jump;
    public Dash dash;
    public WallSliding wallSliding;
    public FreeFall freeFall;
    public WallJump wallJump;

    public int jumpsLeft = 2;
    public float WallDrag = 10f;
    public float WallJumpSpeed = 5f;
    public float WallJumpTime = 4f;
    public int LightJumps = 2;
    public int DarkJumps = 1;
    public float DashTime = 5f;

    private float horizontal;
    [SerializeField] private float speed = 6f;
    public float Speed { get => speed; }
    public float DashSpeed = 15f;
    [SerializeField] private float jumpingPower = 28f;
    public float JumpingPower { get => jumpingPower; }

    public LightMovement lightMovement;
    public DarkMovement darkMovement;
    //Static instances of the state scripts
    //Get rid of the public access

    public MovementScript state_; //Contains the current state script

    public Rigidbody2D rb2D; //Temporarily public for the grip script
    public SpriteRenderer sr; //Temporarily public

    public float wallSlidingSpeed = 0.5f;
    
    [Header("Touching Values")]
    [SerializeField] private bool _isFacingRight = true;
    public bool IsFacingRight { get => _isFacingRight; set => _isFacingRight = value; }
    private bool grounded;
    public bool Grounded { get => bottomLeftCheck || bottomCenterCheck || bottomRightCheck; }

    public GameObject _leftCheck;
    public GameObject _rightCheck;
    public GameObject _bottomLeftCheck;
    public GameObject _bottomCenterCheck;
    public GameObject _bottomRightCheck;

    private bool leftCheck;
    private bool rightCheck;
    private bool bottomLeftCheck;
    private bool bottomCenterCheck;
    private bool bottomRightCheck;

    public bool LeftCheck { get => leftCheck; set => leftCheck = value; }
    public bool RightCheck { get => rightCheck; set => rightCheck = value; }
    public bool BottomLeftCheck { set => bottomLeftCheck = value; }
    public bool BottomCenterCheck { set => bottomCenterCheck = value; }
    public bool BottomRightCheck { set => bottomRightCheck = value; }

    public GameObject _groundCheck; //Temporarily public for the grip script
    private LayerMask _groundLayer;

    private bool _element; //Variable that saves the current element of the player
    public bool Element { get => _element; set => _element = value; }

    // Start is called before the first frame update
    void Start()
    {
        playerLayer = LayerMask.GetMask("Player");
        playerDashLayer = LayerMask.GetMask("PlayerDash");
        //Initialize all relevant components
        rb2D = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        _element = true;

        _groundLayer = 6;

        lightMovement = new LightMovement(this);
        darkMovement = new DarkMovement(this);
        state_ = lightMovement;

        walking = new Walking(this);
        jump = new Jump(this);
        dash = new Dash(this);
        wallSliding = new WallSliding(this);
        freeFall = new FreeFall(this);
        wallJump = new WallJump(this);

        movementState = walking;
        movementState.enterState();

    }

    void Update()
    {
        IsGrounded();
        IsSliding();
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0) movementState.timerDone();
        }
    }

    private void FixedUpdate()
    {
        
    }

    public bool IsGrounded() //Temporarily public for the grip script
    {
        if(bottomLeftCheck || bottomCenterCheck || bottomRightCheck && !IsSliding())
        {
            movementState.ground();
            return true;
        }
        return false;
    }
    public bool IsSliding()
    {
        if (leftCheck || rightCheck)
        {
            movementState.wallSlide();
            return true;
        }
        return false;
    }
    public void ChangeElement()
    {
        _element = !_element;
        sr.color = _element ? Color.green : Color.red;
    }
    public void HorizontalMovement(float horizontal) => movementState.horizontal(horizontal);
    public void Jump() => movementState.jump();
    public void Dash() => movementState.dash();

}

public class MovementState
{
    public PlayerMovement pMovement;

    public virtual void enterState() { }
    public virtual void exitState() { }
    public virtual void horizontal(float horizontal) { }
    public virtual void ground() { }
    public virtual void jump() { }
    public virtual void dash() { }
    public virtual void wallSlide() { }
    public virtual void timerDone() { }
}

public class Walking : MovementState
{
    public Walking(PlayerMovement pMovement)
    {
        this.pMovement = pMovement;
    }
    public override void horizontal(float horizontal)
    {
        pMovement.rb2D.position += horizontal * pMovement.Speed * Time.deltaTime * Vector2.right;
        if (horizontal != 0) pMovement.sr.flipX = (horizontal < 0);
    }
    public override void enterState()
    {
        base.enterState();
        pMovement.jumpsLeft = pMovement.Element ? pMovement.LightJumps : pMovement.DarkJumps;
    }
    public override void jump()
    {
        if (pMovement.jumpsLeft > 0)
        {
            pMovement.movementState = pMovement.jump;
            pMovement.movementState.enterState();
        }
    }
    public override void dash()
    {
        pMovement.movementState = pMovement.dash;
        pMovement.movementState.enterState();
    }
}

public class FreeFall : MovementState
{
    public FreeFall(PlayerMovement pMovement)
    {
        this.pMovement = pMovement;
    }
    public override void ground()
    {
        pMovement.movementState = pMovement.walking;
        pMovement.movementState.enterState();
    }
    public override void horizontal(float horizontal)
    {
        pMovement.rb2D.position += horizontal * pMovement.Speed * Time.deltaTime * Vector2.right;
        if (horizontal != 0) pMovement.sr.flipX = (horizontal < 0);
    }
    public override void jump()
    {
        if (pMovement.jumpsLeft > 0)
        {
            pMovement.movementState = pMovement.jump;
            pMovement.movementState.enterState();
        }
    }
    public override void dash()
    {
        pMovement.movementState = pMovement.dash;
        pMovement.movementState.enterState();
    }
    public override void wallSlide()
    {
        pMovement.movementState = pMovement.wallSliding;
        pMovement.movementState.enterState();
    }
}

public class WallSliding : MovementState
{
    public WallSliding(PlayerMovement pMovement)
    {
        this.pMovement = pMovement;
    }
    public override void ground()
    {
        pMovement.rb2D.drag = 1f;
        pMovement.movementState = pMovement.walking;
        pMovement.movementState.enterState();
    }
    public override void horizontal(float horizontal)
    {
        if (horizontal < 0 && pMovement._rightCheck)
        {
            pMovement.rb2D.drag = 1f;
            pMovement.movementState = pMovement.freeFall;
            pMovement.movementState.enterState();
        }
        else if (horizontal > 0 && pMovement._leftCheck)
        {
            pMovement.rb2D.drag = 1f;
            pMovement.movementState = pMovement.freeFall;
            pMovement.movementState.enterState();
        }
    }
    public override void enterState()
    {
        Debug.Log("Sliding");
        base.enterState();
        pMovement.jumpsLeft = pMovement.Element ? pMovement.LightJumps : pMovement.DarkJumps;
        pMovement.rb2D.drag = pMovement.WallDrag;
    }
    public override void dash()
    {
        pMovement.rb2D.drag = 1f;
        pMovement.movementState = pMovement.dash;
        pMovement.movementState.enterState();
    }

    public override void jump()
    {
        pMovement.rb2D.drag = 1f;
        pMovement.movementState= pMovement.wallJump;
        pMovement.movementState.enterState();
    }
}

public class WallJump : MovementState
{
    public WallJump(PlayerMovement pMovement)
    {
        this.pMovement = pMovement;
    }
    float time = 0f;
    override public void enterState()
    {
        base.enterState();
        pMovement.timer = pMovement.WallJumpTime;
        if (pMovement.RightCheck)
            pMovement.rb2D.velocity += pMovement.WallJumpSpeed * Vector2.left;
        if (pMovement.LeftCheck)
            pMovement.rb2D.velocity += pMovement.WallJumpSpeed * Vector2.right;
        pMovement.jumpsLeft--;
        pMovement.rb2D.AddForce(Vector2.up * pMovement.JumpingPower);
    }

    public override void timerDone()
    {
        //pMovement.gameObject.layer = pMovement.playerLayer;
        pMovement.movementState = pMovement.freeFall;
        pMovement.movementState.enterState();
    }
}

public class Dash : MovementState
{
    public Dash(PlayerMovement pMovement)
    {
        this.pMovement = pMovement;
    }
    float time = 0f;
    override public void enterState()
    {
        Debug.Log("Dashing");
        base.enterState();
        pMovement.timer = pMovement.DashTime;
        //pMovement.gameObject.layer = pMovement.playerDashLayer;
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal >= 0) pMovement.rb2D.velocity = pMovement.DashSpeed * Vector2.right;
        else pMovement.rb2D.velocity = pMovement.DashSpeed * Vector2.left;
    }
    public override void timerDone()
    {
        //pMovement.gameObject.layer = pMovement.playerLayer;
        pMovement.movementState = pMovement.freeFall;
        pMovement.movementState.enterState();
    }

}

public class Jump : MovementState
{
    public Jump(PlayerMovement pMovement)
    {
        this.pMovement = pMovement;
    }
    override public void enterState()
    {
        base.enterState();
        pMovement.jumpsLeft--;
        pMovement.rb2D.AddForce(Vector2.up * pMovement.JumpingPower);
        pMovement.movementState = pMovement.freeFall;
        pMovement.movementState.enterState();
    }
}

public class MovementScript
{
    //State class for the movement with all its virtual functions

    public virtual void enterState() { }
    public virtual void exitState() { }
    public virtual void jump() { }
    public virtual void dash() { }
    public virtual void flip() { }
    public virtual void horizontalMovement(float horizontal) { }
}

public class LightMovement : MovementScript
{
    private PlayerMovement player;
    private float horizontal;

    private int jumpCount = 0;

    public LightMovement(PlayerMovement player)
    {
        this.player = player;
    }

    public override void enterState()
    {
        player.sr.color = Color.green;
        //Temporary to see the state change
    }

    public override void jump()
    {
        if (player.IsGrounded()) jumpCount = 2;
        if (jumpCount == 0) return;
        player.rb2D.AddForce(new Vector2(0, player.JumpingPower));
        jumpCount--;
    }

    public override void flip()
    {
        player.sr.flipX = horizontal <= 0;
    }

    public override void exitState()
    {
        player.Element = !player.Element;
        player.state_ = player.darkMovement;
        player.state_.enterState();
    }

    public override void horizontalMovement(float horizontal)
    {
        player.rb2D.position += new Vector2(horizontal * player.Speed * Time.deltaTime, 0);
        if (horizontal != 0) flip();
    }

}

public class DarkMovement : MovementScript
{
    private PlayerMovement player;
    private float horizontal;

    public DarkMovement(PlayerMovement player)
    {
        this.player = player;
    }

    public override void enterState()
    {
        player.sr.color = Color.red;
        //Temporary to see the state change
    }

    public override void jump()
    {
        if (!player.IsGrounded()) return;
        player.rb2D.AddForce(new Vector2(0, player.JumpingPower));
    }

    public override void flip()
    {
        player.sr.flipX = horizontal <= 0;
    }

    public override void exitState()
    {
        player.Element = !player.Element;
        player.state_ = player.lightMovement;
        player.state_.enterState();
    }

    public override void horizontalMovement(float horizontal)
    {
        player.rb2D.velocity += new Vector2(horizontal * player.Speed * Time.deltaTime, 0);
        //Different movement control to compare with the original
        if (horizontal != 0) flip();
    }

}
