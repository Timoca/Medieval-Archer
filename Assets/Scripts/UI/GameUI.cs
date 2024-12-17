using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text healthText;
    [SerializeField] Slider healthSlider;

    private MonsterSpawner monsterSpawner;
    private Player player;
    private float gameTime;
    [HideInInspector]
    public int score = 0;

    private void Start()
    {
        monsterSpawner = FindFirstObjectByType<MonsterSpawner>();
        player = FindFirstObjectByType<Player>();
    }

    private void Update()
    {
        // We nemen de gameTime van de MonsterSpawner.
        gameTime = monsterSpawner.gameTime;

        // We updaten de timerText met de gameTime.
        int seconds = (int)gameTime % 60;
        int minutes = (int)gameTime / 60;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // We updaten de healthSlider en de healthtext met de huidige health van de speler.
        healthSlider.value = player.currentHealth / (float)player.maxHealth;
        healthText.text = player.currentHealth.ToString();
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
    }
}
