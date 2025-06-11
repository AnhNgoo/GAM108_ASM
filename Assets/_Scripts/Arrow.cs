using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f; // Thời gian tồn tại của mũi tên

    void Start()
    {
        // Tự hủy sau 2 giây
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu va chạm với enemy, hủy cả enemy và mũi tên
        if (collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject); // Hủy enemy
            Destroy(gameObject); // Hủy mũi tên
        }
    }
}