using System;
using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] string monsterName;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;               // Snelheid van het monster

    public float stoppingDistance = 1.5f;      // Afstand waarop het monster stopt met bewegen

    [Header("Combat Settings")]
    public int hitsToKill = 2;                  // Aantal hits om het monster te doden

    public int damage = 10;                     // Schade die het monster doet aan de speler

    [Header("Score Settings")]
    public int score = 10;                     // Punten die je krijgt als je het monster dood.

    [Header("Audio Settings")]
    public AudioClip hitSound;                  // Geluid dat wordt afgespeeld als het monster wordt geraakt

    private bool isFacingRight = true;
    private bool isAttacking = false;
    private bool isAttackingRoutineRunning = false;
    private bool isDead = false;

    private Transform playerTransform;
    private MonsterSpawner spawner;
    private GameUI gameUI;
    private Animator animator;

    void Start()
    {
        // Zoek de speler in de scene. Zorg ervoor dat de speler het tag "Player" heeft.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Monster: Geen speler gevonden met tag 'Player'. Zorg ervoor dat de speler correct is getagd.");
        }

        // Zoek de MonsterSpawner in de scene
        spawner = FindAnyObjectByType<MonsterSpawner>();

        if (spawner == null)
        {
            Debug.LogError("Monster: Geen MonsterSpawner gevonden in de scene.");
        }

        gameUI = FindAnyObjectByType<GameUI>();
        if (gameUI == null)
        {
            Debug.LogError("Monster: geen scoresysteem gevonden");
        }

        // Haal de Animator component op als deze bestaat
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (playerTransform != null && !isDead)
        {
            // Bereken de afstand tot de speler
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer > stoppingDistance)
            {
                // Beweeg het monster naar de speler
                Vector3 direction = (playerTransform.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;

                // Speel de loopanimatie indien beschikbaar
                if (animator != null)
                {
                    animator.SetBool("isWalking", true);
                }
                isAttacking = false;
            }
            else
            {
                // Stop met bewegen en speel de idle animatie
                if (animator != null)
                {
                    animator.SetBool("isWalking", false);
                }
                // Start de Coroutine om de speler aan te vallen
                isAttacking = true;
            }

            float moveDirection = (playerTransform.position.x > transform.position.x) ? 1f : -1f;

            if (!isDead)
            {
                Flip(moveDirection);
            }
        }
        if (!isAttackingRoutineRunning && isAttacking)
        {
            StartCoroutine(AttackRoutine());
            isAttackingRoutineRunning = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ammo"))
        {
            // Speel het hitgeluid af
            if (hitSound != null)
            {
                AudioManager.Instance.PlaySFX(hitSound);
            }

            hitsToKill--;

            if (hitsToKill <= 0)
            {
                Die();
            }
            else
            {
                // Speel de 'Hit' animatie
                animator.SetTrigger("isHit");
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        // Doe schade aan de speler
        GameObject player = playerTransform.gameObject;
        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, monsterName);
            }
        }

        // Wacht 1 seconde voordat het monster de speler weer aanvalt
        yield return new WaitForSeconds(1f);
        isAttackingRoutineRunning = false;
    }

    private void Die()
    {
        //disable de collider
        GetComponent<Collider2D>().enabled = false;
        //Zet de order in layer 1 lager zodat het monster achter de speler komt
        GetComponentInChildren<SpriteRenderer>().sortingOrder -= 1;

        // Speel de doodanimatie af
        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }

        // Stop verdere beweging en flips van het monster
        moveSpeed = 0f;
        isDead = true;

        // Start de Coroutine om te wachten tot de animatie is afgespeeld
        StartCoroutine(DespawnAfterAnimation());
    }

    private IEnumerator DespawnAfterAnimation()
    {
        // Voeg punten toe
        gameUI.AddScore(score);

        // Haal de lengte van de 'Dead' animatie
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;

        // Wacht de animatielengte
        yield return new WaitForSeconds(animationLength + 1f);

        // Vernietig het monster
        Destroy(gameObject);

        if (spawner != null)
        {
            spawner.MonsterDestroyed();
        }
    }

    void Flip(float direction)
    {
        if (direction > 0 && !isFacingRight || direction < 0 && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }
}