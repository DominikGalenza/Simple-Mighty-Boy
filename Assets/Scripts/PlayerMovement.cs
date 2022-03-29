using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Vector2 deathKick = new Vector2(20f, 20f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    [SerializeField] float playerSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    Vector2 moveInput;
    Rigidbody2D myRigidbody;

    float gravityScaleAtStart;
    bool isAlive = true;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    void Update()
    {
        if(!isAlive) { return; }
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void OnFire(InputValue value)
    {
        if(!isAlive) { return; }
        Instantiate(bullet, gun.position, transform.rotation);
    }
    
    void OnMove(InputValue value)
    {
        if(!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if(!isAlive) { return; }
        if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }

        if(value.isPressed)
        {
            myRigidbody.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    void ClimbLadder()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) 
        { 
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
            return; 
        }
        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0f;
        bool playerHasVerticalMovement = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalMovement);
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * playerSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool playerHasHorizontalMovement = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalMovement);
    }

    void FlipSprite()
    {
        bool playerHasHorizontalMovement = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if(playerHasHorizontalMovement)
        {
            transform.localScale = new Vector2 (Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    void Die()
    {
        if(myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidbody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
}
