using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Button restartButton;
    [SerializeField] Button mainMenuButton;

    private PlayerHealth playerHealth;
    private GameUI gameUI;
    private BowController bowController;
    private bool isDead = false;
    private bool scoreAdded = false;

    void Start()
    {
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        gameUI = FindAnyObjectByType<GameUI>();
        bowController = FindAnyObjectByType<BowController>();
    }


    void Update()
    {
        isDead = playerHealth.isDead;
        if (isDead && !scoreAdded)
        {
            ScoreManager.Instance.AddScore(gameUI.score);
            scoreAdded = true; // Zet de vlag op true om dubbele toevoegingen te voorkomen

            // Haal de hoogste highscore op
            List<HighScore> highScores = ScoreManager.Instance.GetHighScores();
            string highScoreDisplay = "0";
            if (highScores.Count > 0)
            {
                HighScore topHighScore = highScores[0];
                highScoreDisplay = topHighScore.score.ToString();
            }
            scoreText.text = "You've died!\n\nHighscore: " + highScoreDisplay + "\nScore: " + gameUI.score;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // Zet de timescale op 1 zodat het spel hervat.
        Time.timeScale = 1;
        playerHealth.GetComponent<PlayerMovement>().enabled = true;
        playerHealth.GetComponent<PlayerShooting>().enabled = true;
        bowController.enabled = true;

        PlayUIClickSound();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        // Zet de timescale op 1 zodat het spel hervat.
        Time.timeScale = 1;

        PlayUIClickSound();
    }

    public void PlayUIClickSound()
    {
        AudioManager.Instance.PlayUIClick();
    }
}
