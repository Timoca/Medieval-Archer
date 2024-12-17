using System.Collections;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    [Header("PowerUp Settings")]
    public float duration = 15f; // Duur van de power-up in seconden

    public void Activate(Player player)
    {
        ApplyEffect(player);
        StartCoroutine(PowerUpDuration(player));
    }

    protected abstract void ApplyEffect(Player player);

    protected abstract void RemoveEffect(Player player);

    private IEnumerator PowerUpDuration(Player player)
    {
        yield return new WaitForSeconds(duration);
        RemoveEffect(player);
        Destroy(gameObject); // Verwijder de power-up object na gebruik
    }
}
