using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI coinText; // Text để hiển thị số coin
    [SerializeField] private TextMeshProUGUI livesText;

    private bool isPaused = false;
    private int playerLives = 3;
    private const int MAX_LIVES = 3;

    void Awake()
    {
        Instance = this;
        // DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (PlayerController.Instance == null)
        {
            Debug.LogError("PlayerController not found in the scene!");
        }

        pauseMenuPanel.SetActive(false);
        losePanel.SetActive(false);
        winPanel.SetActive(false);
        UpdateCoinText();
        UpdateLivesText();
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
        UpdateLivesText();
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
    public void ShowLosePanel()
    {
        // isPaused = true;
        Time.timeScale = 0f;
        losePanel.SetActive(true);
    }

    public void ShowWinPanel()
    {
        // isPaused = true;
        Time.timeScale = 0f;
        winPanel.SetActive(true);
    }

    public void ReloadLevel()
    {
        playerLives = MAX_LIVES;
        losePanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        playerLives = MAX_LIVES;
        Time.timeScale = 1f;
        winPanel.SetActive(false);
        SceneManager.LoadScene(0); // Assumes level 1 is at build index 0
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

    private void UpdateLivesText()
    {
        if (livesText != null)
        {
            livesText.text = $"Lives: {playerLives}/{MAX_LIVES}";
        }
    }

    public void DecreaseLife()
    {
        playerLives--;
        UpdateLivesText();
        if (playerLives <= 0)
        {
            ShowLosePanel();
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }
    
    public void WinGame()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2) // Assuming level 3 is at build index 2
        {
            ShowWinPanel();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}