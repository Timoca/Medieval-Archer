using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private Player player;
    private BowController bowController;
    private GameOverUI gameOverUI;
    private Animator animator;
    private void Start()
    {
        player = GetComponent<Player>();

        player.currentHealth = player.maxHealth;

        bowController = FindAnyObjectByType<BowController>();
        animator = GetComponentInChildren<Animator>();

        // Zoek de GameOverUI in de scene en zet deze uit.
        gameOverUI = FindAnyObjectByType<GameOverUI>();
        gameOverUI.gameObject.SetActive(false);
    }

    public void TakeDamage(int damage, string monster)
    {
        if (player.isDead) return; // Voorkom verdere schade als de speler al dood is

        player.currentHealth -= damage;

        // Speel de 'Hit' animatie
        animator.SetTrigger("isHit");
        PlayHitSound(monster);

        if (player.currentHealth <= 0)
        {
            player.currentHealth = 0;
            StartCoroutine(DieCoroutine());
        }
    }

    private void PlayHitSound(string monster)
    {
        // Kijk welk monster de speler heeft geraakt en speel het juiste geluid af
        switch (monster)
        {
            case "Goblin":
                AudioManager.Instance.PlaySFX(player.hitClipGoblin);
                break;
            case "HobGoblin":
                AudioManager.Instance.PlaySFX(player.hitClipHobGoblin);
                break;
            case "Golem":
                AudioManager.Instance.PlaySFX(player.hitClipGolem);
                break;
            default:
                break;
        }
    }

    private IEnumerator DieCoroutine()
    {
        player.isDead = true;

        // Speel de doodanimatie af
        if (animator != null)
        {
            animator.ResetTrigger("isHit");
            animator.SetBool("isDead", true);
        }

        // Schakel de bewegings- en schietscripts uit
        gameObject.GetComponent<PlayerMovement>().enabled = false;
        gameObject.GetComponent<PlayerShooting>().enabled = false;
        bowController.enabled = false;

        // Wacht 2 seconden voordat de GameOverUI getoond wordt
        yield return new WaitForSeconds(2f);


        // Laat de GameOverUI zien.
        if (gameOverUI != null)
        {
            gameOverUI.gameObject.SetActive(true);
        }

        // Speel de GameOver muziek
        AudioManager.Instance.PlayGameOverSound();
    }
}
