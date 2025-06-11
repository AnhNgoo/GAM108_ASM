using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private AudioClip portalSound;
    [SerializeField] private float transitionDelay = 0.5f; // Độ trễ để chạy animation

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                StartCoroutine(ChangeScene());
            }
        }
    }

    private IEnumerator ChangeScene()
    {
        if (portalSound != null)
        {
            AudioSource.PlayClipAtPoint(portalSound, transform.position);
        }
        yield return new WaitForSeconds(transitionDelay);
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(0); // Quay lại Level1 nếu hết scene
        }
    }
}