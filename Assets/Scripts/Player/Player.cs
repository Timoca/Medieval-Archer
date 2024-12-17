using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    public int maxHealth = 100;
    public float speed = 5f;

    [Header("Shooting Settings")]
    public float arrowSpeed = 10f;
    public float shootCooldown = 0.5f;
    public float shootDelay = 0.1f;

    [Header("Audio Settings")]
    public AudioClip shootClip;
    public AudioClip hitClipGoblin;
    public AudioClip hitClipHobGoblin;
    public AudioClip hitClipGolem;

    [HideInInspector]
    public int currentHealth;
}
