using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel; // Reference to UI panel
    [SerializeField] private TextMeshProUGUI gameOverText; // Reference to UI text

	private void Start()
	{
        gameOverPanel.SetActive(false);
    }

	void OnEnable()
    {
        GameOverManager.OnGameOver += ShowGameOverScreen; //  Subscribe to event
    }

    void OnDisable()
    {
        GameOverManager.OnGameOver -= ShowGameOverScreen; //  Unsubscribe
    }

    void ShowGameOverScreen(object sender, GameOverManager.GameOverEventArgs e)
    {
        gameOverPanel.SetActive(true); //  Show the "You Won" UI
        gameOverText.text = e.PlayerWon ? "You Won!" : "You Lost!";
        //Time.timeScale = 0f; //  Stop the game
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //  Restart scene
    }
}
