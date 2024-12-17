using UnityEngine;

public class SpeedBoost : PowerUp
{
    public float speedMultiplier = 2f;

    protected override void ApplyEffect(Player player)
    {
        player.speed *= speedMultiplier;
        Debug.Log("Speed Boost Applied!");
    }

    protected override void RemoveEffect(Player player)
    {
        player.speed /= speedMultiplier;
        Debug.Log("Speed Boost Removed!");
    }
}
