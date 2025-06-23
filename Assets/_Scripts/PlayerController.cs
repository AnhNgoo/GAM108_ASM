using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float rollSpeed = 8f;
    [SerializeField] private float rollDuration = 0.5f;
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject arrowPrefab; // Prefab của mũi tên
    [SerializeField] private float arrowSpeed = 10f; // Tốc độ bay của mũi tên
    [SerializeField] private Transform arrowSpawnPoint; // Vị trí spawn mũi tên
    [SerializeField] private float shootCooldown = 0.5f;

    public static PlayerController Instance { get; set; }

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private bool isGrounded;
    private bool isRolling;
    private bool isClimbing;
    private float defaultGravityScale;
    private float lastShootTime;
    private int coinCount = 0;
    private Vector2 initialPosition;

    private void Awake() {
        // Triển khai Singleton
        Instance = this;
    }

    void Start()
    {
        InitializeComponents();
        initialPosition = transform.position;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused()) return;

        CheckGroundStatus();
        if (!isClimbing)
        {
            HandleMovement();
            HandleJump();
        }
        else
        {
            HandleClimb();
        }
        HandleRoll();
        HandleShoot();
        UpdateAnimations();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        defaultGravityScale = rb.gravityScale;
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
    }

    private void HandleMovement()
    {
        if (isRolling) return;

        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput < -0.01f)
        {
            sr.flipX = true;
        }
        else if (moveInput > 0.01f)
        {
            sr.flipX = false;
        }
    }

    private void HandleClimb()
    {
        float verticalInput = Input.GetAxis("Vertical");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalInput * climbSpeed);

        // Nếu không di chuyển dọc, giữ nhân vật đứng yên trên thang
        if (Mathf.Abs(verticalInput) > 0.01f)
        {
            animator.SetTrigger("Clamber");
        }
        else
        {
            animator.ResetTrigger("Clamber");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
    }

    private void HandleJump()
    {
        if (isRolling) return;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger("Jump");
        }
    }

    private void HandleRoll()
    {
        if (isRolling) return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(Roll());
        }
    }

    private void HandleShoot()
    {
        if (isRolling) return;

        if (Input.GetMouseButtonDown(0) && Time.time >= lastShootTime + shootCooldown)
        {
            animator.SetTrigger("Atk");
            lastShootTime = Time.time;
        }
    }

    private void SpawnArrow()
    {
        // Tạo mũi tên tại vị trí spawn point
        Vector2 spawnPosition = arrowSpawnPoint != null ? arrowSpawnPoint.position : transform.position;
        GameObject arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);

        // Xác định hướng mũi tên dựa trên flipX của sprite
        float direction = sr.flipX ? -1f : 1f;
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        arrowRb.linearVelocity = new Vector2(direction * arrowSpeed, 0f);

        // Xoay sprite mũi tên nếu cần
        SpriteRenderer arrowSr = arrow.GetComponent<SpriteRenderer>();
        if (arrowSr != null && sr.flipX)
        {
            arrowSr.flipX = true;
        }
    }

    private void UpdateAnimations()
    {
        bool isRun = Mathf.Abs(rb.linearVelocity.x) > 0.1f && isGrounded && !isRolling;
        animator.SetBool("IsRun", isRun);
    }

    private IEnumerator Roll()
    {
        isRolling = true;
        float direction = sr.flipX ? -1f : 1f;
        rb.linearVelocity = new Vector2(direction * rollSpeed, rb.linearVelocity.y);
        animator.SetTrigger("Roll");

        yield return new WaitForSeconds(rollDuration);
        isRolling = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = true;
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
        else if (collision.CompareTag("Coin"))
        {
            Coin coin = collision.GetComponent<Coin>();
            if (coin != null)
            {
                CollectCoin(coin.GetValue());
                Destroy(collision.gameObject);
            }
        }
        else if (collision.CompareTag("Goal")) // Assuming a "Goal" tag for level completion
        {
            GameManager.Instance.WinGame();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = false;
            rb.gravityScale = defaultGravityScale;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Die();
        }
    }

    public void CollectCoin(int value)
    {
        coinCount += value;
        Debug.Log($"Coins: {coinCount}");
    }

    public void Die()
    {
        GameManager.Instance.DecreaseLife();
        if (GameManager.Instance.IsPaused()) return;
            // Đặt lại vị trí nhân vật về vị trí ban đầu
        transform.position = initialPosition;
        // Đặt lại vận tốc
        // rb.linearVelocity = Vector2.zero;
        // // Đặt lại trạng thái
        // isRolling = false;
        // isClimbing = false;
        // rb.gravityScale = defaultGravityScale;
        // // Đặt lại trạng thái hoạt hình
        // animator.ResetTrigger("Roll");
        // animator.ResetTrigger("Jump");
        // animator.ResetTrigger("Clamber");
        // animator.SetBool("IsRun", false);
        // Debug.Log("Player died and respawned at initial position!");
    }

    public int GetCoinCount()
    {
        return coinCount;
    }
}