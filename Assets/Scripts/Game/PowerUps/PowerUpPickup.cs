using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public PowerUp powerUp;
    public AudioClip PowerUpSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                // Instantiate de PowerUp op de positie van de speler en activeer deze
                PowerUp powerUpInstance = Instantiate(powerUp, player.transform.position, Quaternion.identity);
                PowerUpManager.Instance.ApplyPowerUp(powerUpInstance, player);

                // Informeer de PowerUpManager dat deze PowerUp is gebruikt
                PowerUpManager.Instance.OnPowerUpUsed(powerUp);

                // Verwijder de spawnpositie en de pickup object
                PowerUpManager.Instance.RemovePowerUpPosition(transform.position);

                // Speel het pickup geluid af
                if (PowerUpSound != null)
                {
                    AudioManager.Instance.PlaySFX(PowerUpSound);
                }

                // Verwijder het pickup object
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("PowerUpPickup: Player component not found on collided object.");
            }
        }
    }
}
