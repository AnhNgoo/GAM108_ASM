using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f; // Tốc độ di chuyển của enemy
    [SerializeField] private float moveDistance = 3f; // Khoảng cách di chuyển trái-phải
    [SerializeField] private bool startMovingRight = true; // Hướng di chuyển ban đầu
    
    private Vector2 startPosition;
    private float leftBoundary;
    private float rightBoundary;
    private bool movingRight;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Start()
    {
        // Khởi tạo các component và vị trí
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
        
        // Thiết lập ranh giới di chuyển
        leftBoundary = startPosition.x - moveDistance;
        rightBoundary = startPosition.x + moveDistance;
        
        // Thiết lập hướng di chuyển ban đầu
        movingRight = startMovingRight;
        UpdateSpriteDirection();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        // Di chuyển enemy
        float moveDirection = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);

        // Kiểm tra ranh giới và đổi hướng
        if (transform.position.x >= rightBoundary)
        {
            movingRight = false;
            UpdateSpriteDirection();
        }
        else if (transform.position.x <= leftBoundary)
        {
            movingRight = true;
            UpdateSpriteDirection();
        }
    }

    private void UpdateSpriteDirection()
    {
        // Xoay sprite theo hướng di chuyển
        if (sr != null)
        {
            sr.flipX = !movingRight;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Gây sát thương khi va chạm với player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Die();
            }
        }
    }
}