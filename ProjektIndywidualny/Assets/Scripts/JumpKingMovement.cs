using UnityEngine;

public class JumpKingMovement : MonoBehaviour
{
    public float baseJumpForce = 10f;
    public float maxJumpForce = 20f;
    public float jumpChargeTime = 1f;
    public float moveSpeed = 5f;
    public float maxFallSpeed = 10f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isJumping;
    private bool isGrounded;
    private float jumpChargeTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true; //Prewencja obrotu postaci dookoła siebie
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

        // Poruszanie sie lewo/prawo
        float moveInput = Input.GetAxis("Horizontal");

        // Fizyka poruszania
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Odwrócenie postaci twarza do kierunku poruszania
        if (moveInput > 0f)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput < 0f)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void StartJumpCharge()
    {
        // Definicja dla ładowania skoku
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
        // Matematyka dla skoku rozwinięta o naładowanie
        float jumpForce = Mathf.Lerp(baseJumpForce, maxJumpForce, jumpChargeTimer / jumpChargeTime);
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        isGrounded = false;
        isJumping = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Sprawdzanie czy postać jest na ziemii
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }

        // Jeszcze w toku, naprawa zatrzymywania się na blokach, nie jest to tragedia, ale wciąż :)
        foreach (ContactPoint2D contactPoint in collision.contacts)
        {
            if (Mathf.Abs(contactPoint.normal.y) < 0.5f)
            {
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -maxFallSpeed, float.MaxValue));
                break;
            }
        }
    }
}
