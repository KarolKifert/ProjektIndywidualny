using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed;
    public float jumpForce = 0.0f;


    private Rigidbody2D rb;
    private GatherInput gI;
    private Animator anim;

    public float rayLength;
    public bool grounded;
    public bool preJump = false;
    public LayerMask groundLayer;
    public Transform checkPointLeft;
    public Transform checkPointRight;
    public PhysicsMaterial2D bounceMat, normalMat;


    private int direction = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gI = GetComponent<GatherInput>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Flip();
        SetAnimatorValues();
        PlayerJump();
        CheckStatus();
        PlayerMove();
    }

    private void PlayerMove()
    {
        if (jumpForce == 0.0f && grounded)
        {
            rb.velocity = new Vector2(speed * gI.valueX, rb.velocity.y);
        }

    }
    private void Flip()
    {
        if (gI.valueX * direction < 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
            direction *= -1;
        }
    }
    private void PlayerJump()
    {
        if (gI.jumpInput && grounded)
        {
            jumpForce += 0.5f;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
            rb.sharedMaterial = bounceMat;
            preJump = true;
        }
        else
        {
            preJump = false;
        }
        if (gI.jumpInput && grounded && jumpForce >= 20.0f || gI.jumpInput == false && jumpForce >= 0.1f)
        {
            float tempX = gI.valueX * speed;
            float tempY = jumpForce;
            rb.velocity = new Vector2(tempX, tempY);
            Invoke("ResetJump", 0.025f);
        }
        if (rb.velocity.y <= -1)
        {
            rb.sharedMaterial = normalMat;
        }
    }

    private void ResetJump()
    {

        jumpForce = 0.0f;
    }
    private void CheckStatus()
    {
        RaycastHit2D leftCheckHit = Physics2D.Raycast(checkPointLeft.position, Vector2.down, rayLength, groundLayer);
        RaycastHit2D rightCheckHit = Physics2D.Raycast(checkPointRight.position, Vector2.down, rayLength, groundLayer);

        if (leftCheckHit || rightCheckHit)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        SeeRays(leftCheckHit, rightCheckHit);
    }

    private void SeeRays(RaycastHit2D leftCheckHit, RaycastHit2D rightCheckHit)
    {
        Color color1 = leftCheckHit ? Color.green : Color.red;
        Color color2 = rightCheckHit ? Color.green : Color.red;

        Debug.DrawRay(checkPointLeft.position, Vector2.down * rayLength, color1);
        Debug.DrawRay(checkPointRight.position, Vector2.down * rayLength, color2);
    }

    private void SetAnimatorValues()
    {
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("vSpeed", rb.velocity.y);
        anim.SetBool("grounded", grounded);
        anim.SetBool("preJump", preJump);
    }
}
