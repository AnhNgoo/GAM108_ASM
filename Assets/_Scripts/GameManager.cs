using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private TextMeshProUGUI coinText; // Text để hiển thị số coin

    private bool isPaused = false;

    void Awake()
    {
        // Triển khai Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (PlayerController.Instance == null)
        {
            Debug.LogError("PlayerController not found in the scene!");
        }

        // Ẩn pause menu khi bắt đầu
        pauseMenuPanel.SetActive(false);

        // Cập nhật điểm số ban đầu
        UpdateCoinText();
    }

    void Update()
    {
        // Nhấn phím Escape để toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        // Cập nhật điểm số liên tục
        UpdateCoinText();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Tạm dừng game
        pauseMenuPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Tiếp tục game
        pauseMenuPanel.SetActive(false);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f; // Đặt lại time scale trước khi chuyển scene
        SceneManager.LoadScene("MainMenu");
    }

    private void UpdateCoinText()
    {
        if (PlayerController.Instance != null && coinText != null)
        {
            coinText.text = $"Score: {PlayerController.Instance.GetCoinCount()}";
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}