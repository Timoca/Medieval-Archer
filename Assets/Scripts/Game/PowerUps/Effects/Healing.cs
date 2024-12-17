using UnityEngine;

public class Healing : PowerUp
{
    public int healAmount = 30;
    protected override void ApplyEffect(Player player)
    {
        player.currentHealth += healAmount;
        if (player.currentHealth > player.maxHealth)
        {
            player.currentHealth = player.maxHealth;
        }
    }

    protected override void RemoveEffect(Player player)
    {
        // Healing power-up heeft geen effect na de duur van de power-up
    }
}
