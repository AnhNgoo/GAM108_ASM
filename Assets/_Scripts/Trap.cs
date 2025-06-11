using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private AudioClip trapSound; // Âm thanh khi chạm bẫy (tùy chọn)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Die();
                if (trapSound != null)
                {
                    AudioSource.PlayClipAtPoint(trapSound, transform.position);
                }
            }
        }
    }
}