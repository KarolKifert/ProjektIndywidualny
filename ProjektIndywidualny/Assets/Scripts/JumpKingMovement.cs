using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class JumpKingMovement : MonoBehaviour
{
    public float baseJumpForce = 5f;
    public float maxJumpForce = 10f;
    public float jumpChargeTime = 1f;
    public float moveSpeed = 3f;
    public float maxFallSpeed = 10f;
    public float wallBounceForce = 10f;
    public float levelChangeHeight = 4.3f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isJumping;
    private bool isGrounded;
    private float jumpChargeTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;

        StartCoroutine(SetPlayerPosition());

    }

    private IEnumerator SetPlayerPosition()
    {

        if (PlayerPrefs.HasKey("PlayerXPosition"))
        {
            float playerXPosition = PlayerPrefs.GetFloat("PlayerXPosition");
            if (PlayerPrefs.HasKey("PlayerXPosition"))
            {
                float bottomOfLevel = PlayerPrefs.GetFloat("PlayerYPosition");
                transform.position = new Vector2(playerXPosition, bottomOfLevel);

                if (PlayerPrefs.HasKey("PlayerYVelocity"))
                {
                    float playerYVelocity = PlayerPrefs.GetFloat("PlayerYVelocity");
                    rb.velocity = new Vector2(rb.velocity.x, playerYVelocity);
                }
            }
        }
        yield return new WaitForEndOfFrame();
    }

    private void Update()
    {
        // Skakanie 
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            StartJumpCharge();
        }

        if (Input.GetButton("Jump") && isJumping)
        {
            UpdateJumpCharge();
        }

        if (Input.GetButtonUp("Jump") && isJumping)
        {
            Jump();
        }

        // Lewo/prawo
        float moveInput = Input.GetAxis("Horizontal");

        // Fizyka ruchu
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Postać patrzy w kierunku ruchu
        if (moveInput > 0f)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput < 0f)
        {
            spriteRenderer.flipX = true;
        }

        if (transform.position.y > levelChangeHeight)
        {
            ChangeLevel();

        }

        if (transform.position.y < -5f)
        {
            PreviousLevel();
        }
    }

    private void StartJumpCharge()
    {
        isJumping = true;
        jumpChargeTimer = 0f;
    }

    private void UpdateJumpCharge()
    {
        if (jumpChargeTimer < jumpChargeTime)
        {
            jumpChargeTimer += Time.deltaTime;
        }
    }

    private void Jump()
    {
        float jumpForce = Mathf.Lerp(baseJumpForce, maxJumpForce, jumpChargeTimer / jumpChargeTime);
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        isGrounded = false;
        isJumping = false;
    }

    private void ChangeLevel()
    {
        // Zachowanie położenia x i y postaci oraz prędkości na y
        PlayerPrefs.SetFloat("PlayerXPosition", transform.position.x);
        PlayerPrefs.SetFloat("PlayerYVelocity", rb.velocity.y);
        PlayerPrefs.SetFloat("PlayerYPosition", -2.5f);

        string currentSceneName = SceneManager.GetActiveScene().name;
        int currentLevelNumber = int.Parse(currentSceneName.Substring(currentSceneName.Length - 1));
        string nextSceneName = "Level" + (currentLevelNumber + 1);

        SceneManager.LoadScene(nextSceneName);
    }

    private void PreviousLevel()
    {
        PlayerPrefs.SetFloat("PlayerXPosition", transform.position.x);
        PlayerPrefs.SetFloat("PlayerYVelocity", rb.velocity.y);
        PlayerPrefs.SetFloat("PlayerYPosition", 5f);

        string currentSceneName = SceneManager.GetActiveScene().name;
        int currentLevelNumber = int.Parse(currentSceneName.Substring(currentSceneName.Length - 1));
        string nextSceneName = "Level" + (currentLevelNumber - 1);

        SceneManager.LoadScene(nextSceneName);
       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }

        // Odbijanie od ścian
        if (collision.gameObject.CompareTag("Wall"))
        {
            Rigidbody2D playerRigidbody = rb;
            Vector2 relativeVelocity = collision.relativeVelocity;

            // Kierunek odbicia
            Vector2 bounceDirection = Vector2.Reflect(relativeVelocity.normalized, collision.contacts[0].normal);

            // Siła odbicia
            playerRigidbody.AddForce(bounceDirection * wallBounceForce, ForceMode2D.Impulse);
        }
    }
}
