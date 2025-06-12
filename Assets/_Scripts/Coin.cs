using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 1; // Giá trị của đồng xu
    [SerializeField] private AudioClip collectSound; // Âm thanh khi nhặt

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.CollectCoin(coinValue);
                if (collectSound != null && AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(collectSound);
                }
                Destroy(gameObject); // Hủy đồng xu
            }
        }
    }

    public int GetValue()
    {
        return coinValue;
    }
}