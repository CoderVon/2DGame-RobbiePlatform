using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    private BoxCollider2D collider;

    [Header("移动参数")]
    public float speed = 8f;
    public float crouchSpeedDivisor = 3f;

    [Header("跳跃参数")]
    public float jumpForce = 8f;
    public float jumpHoldForce = 1.9f;
    public float jumpHoldDuration = 0.1f;//按跳跃键时间
    public float crouchJumpBoost = 2.5f;//超级跳加成

    float jumpTime;


    [Header("状态")]
    public bool isCrouch;
    public bool isOnGround;
    public bool isJump;

    [Header("环境检测")]
   // public float footOffset = 0.4f;//脚的距离的位置
    //public float headClearance = 0.5f;//头顶检测距离
    //public float groundDistance-0.2f;//地面检测距离
    public LayerMask groundLayer;

    float xVelocity;

    //按键设置
    bool jumpPressed;
    bool jumpHeld;
    bool crouchHeld;


    //碰撞体的尺寸
    Vector2 colliderStandSize;//尺寸
    Vector2 colliderStandOffset;//位置
    Vector2 colliderCrouchSize;
    Vector2 colliderCrouchOffset;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();

        //获取初始碰撞体尺寸
        colliderStandSize = collider.size;
        colliderStandOffset = collider.offset;
        //给下蹲碰撞体尺寸赋值
        colliderCrouchSize = new Vector2(collider.size.x, collider.size.y / 2f);
        colliderCrouchOffset = new Vector2(collider.offset.x, collider.offset.y / 2f);
    }
    private void Update()
    {
        jumpPressed = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");
        crouchHeld = Input.GetButton("Crouch");
    }
    private void FixedUpdate()
    {
        PhysicsCheck();
        GroundMovement();
        MidAirMovement();
    }

    void PhysicsCheck()
    {
        if (collider.IsTouchingLayers(groundLayer))
            isOnGround = true;
        else
            isOnGround = false;
    }
    //地面移动
    void GroundMovement()
    {
        if (crouchHeld && !isCrouch && isOnGround)
            Crouch();
        else if (!crouchHeld && isCrouch)
            StandUp();
        else if (!isOnGround && isCrouch)
            StandUp();
        
        xVelocity = Input.GetAxisRaw("Horizontal");

        if (isCrouch)
            xVelocity /= crouchSpeedDivisor;

        rigidbody.velocity = new Vector2(xVelocity * speed, rigidbody.velocity.y);

        if (xVelocity < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (xVelocity > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    //跳跃相关移动
    void MidAirMovement()
    {
        if (jumpPressed && isOnGround && !isJump)
        {
            //蹲着跳
            if (isCrouch && isOnGround)
            {
                StandUp();
                rigidbody.AddForce(new Vector2(0f, crouchJumpBoost), ForceMode2D.Impulse);
            }

            isOnGround = false;
            isJump = true;


            jumpTime = Time.time + jumpHoldDuration;//当前时间加上判定的跳跃增量时间

            rigidbody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
        else if (isJump)
        {
            if (jumpHeld)//长按跳跃
                rigidbody.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);

            if (jumpTime < Time.time)//当前时间加上判定的跳跃增量时间小于当前时间（过了跳跃增量时间）
                isJump = false;
        }
    }

    void Crouch()
    {
        isCrouch = true;
        collider.size = colliderCrouchSize;
        collider.offset = colliderCrouchOffset;
    }
    void StandUp()
    {
        isCrouch = false;
        collider.size = colliderStandSize;
        collider.offset = colliderStandOffset;
    }
}
