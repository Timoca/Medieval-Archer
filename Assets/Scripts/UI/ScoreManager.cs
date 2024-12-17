using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("High Score Settings")]
    public int maxHighScores = 5;

    private List<HighScore> highScores = new List<HighScore>();

    private void Awake()
    {
        // Implementeer de singleton patroon
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadHighScores();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int score)
    {
        string currentDateTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
        HighScore newHighScore = new HighScore(score, currentDateTime);
        highScores.Add(newHighScore);

        // Sorteer de lijst in aflopende volgorde en beperk tot maxHighScores
        highScores = highScores.OrderByDescending(hs => hs.score).Take(maxHighScores).ToList();

        SaveHighScores();
    }

    public List<HighScore> GetHighScores()
    {
        return new List<HighScore>(highScores);
    }

    private void LoadHighScores()
    {
        highScores.Clear();

        for (int i = 1; i <= maxHighScores; i++)
        {
            string scoreKey = $"HighScore_{i}";
            string dateKey = $"HighScore_{i}_Date";

            if (PlayerPrefs.HasKey(scoreKey) && PlayerPrefs.HasKey(dateKey))
            {
                int score = PlayerPrefs.GetInt(scoreKey);
                string dateTime = PlayerPrefs.GetString(dateKey);
                highScores.Add(new HighScore(score, dateTime));
            }
        }
    }


    private void SaveHighScores()
    {
        // Zorg ervoor dat alleen de top maxHighScores worden opgeslagen
        for (int i = 1; i <= maxHighScores; i++)
        {
            if (i <= highScores.Count)
            {
                string scoreKey = $"HighScore_{i}";
                string dateKey = $"HighScore_{i}_Date";

                PlayerPrefs.SetInt(scoreKey, highScores[i - 1].score);
                PlayerPrefs.SetString(dateKey, highScores[i - 1].dateTime);
            }
            else
            {
                // Verwijder overbodige high scores
                string scoreKey = $"HighScore_{i}";
                string dateKey = $"HighScore_{i}_Date";

                PlayerPrefs.DeleteKey(scoreKey);
                PlayerPrefs.DeleteKey(dateKey);
            }
        }

        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class HighScore
{
    public int score;
    public string dateTime;

    public HighScore(int score, string dateTime)
    {
        this.score = score;
        this.dateTime = dateTime;
    }
}
