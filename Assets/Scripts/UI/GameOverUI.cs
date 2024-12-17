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

    private GameUI gameUI;
    private BowController bowController;
    private Player player;
    private bool scoreAdded = false;

    void Start()
    {
        player = FindAnyObjectByType<Player>();
        gameUI = FindAnyObjectByType<GameUI>();
        bowController = FindAnyObjectByType<BowController>();
    }


    void Update()
    {
        if (player.isDead && !scoreAdded)
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
        player.GetComponent<PlayerMovement>().enabled = true;
        player.GetComponent<PlayerShooting>().enabled = true;
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
