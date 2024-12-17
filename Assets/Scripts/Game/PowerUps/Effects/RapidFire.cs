using UnityEngine;

public class RapidFire : PowerUp
{
    [SerializeField] private float cooldownMultplier = 2;
    protected override void ApplyEffect(Player player)
    {
        player.shootCooldown /= cooldownMultplier;
    }

    protected override void RemoveEffect(Player player)
    {
        player.shootCooldown *= cooldownMultplier;
    }
}
