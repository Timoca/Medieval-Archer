using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighScoresUI : MonoBehaviour
{
    [SerializeField] TMP_Text highScoresText;

    private void Start()
    {
        GetHighScores();
    }

    public void GetHighScores()
    {
        ScoreManager scoreManager = ScoreManager.Instance;
        List<HighScore> scores = scoreManager.GetHighScores();
        foreach (HighScore score in scores)
        {
            highScoresText.text += $"{score.score}         {score.dateTime}\n";
        }
    }
    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayUIClickSound()
    {
        AudioManager.Instance.PlayUIClick();
    }
}
