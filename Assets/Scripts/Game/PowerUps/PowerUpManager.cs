using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance { get; private set; }

    [Header("PowerUp Settings")]
    public List<PowerUpPickup> powerUpPickupPrefabs; // Lijst van verschillende PowerUpPickup prefabs
    public Transform spawnAreaTransform;       // Referentie naar het SpawnArea object
    public LayerMask wallLayer;                // LayerMask voor muren
    public float minSpawnDistance = 2f;        // Minimale afstand tussen power-ups

    private BoxCollider2D spawnAreaCollider;
    private List<Vector3> activePowerUpPositions = new List<Vector3>();
    private void Awake()
    {
        // Singleton patroon implementatie
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Abonneer op Scene Loaded Event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Herinitialiseer de spawn area
        if (spawnAreaTransform != null)
        {
            spawnAreaCollider = spawnAreaTransform.GetComponent<BoxCollider2D>();
            if (spawnAreaCollider == null)
            {
                Debug.LogError("PowerUpManager: SpawnArea heeft geen BoxCollider2D component.");
                return;
            }
        }
        else
        {
            Debug.LogError("PowerUpManager: SpawnArea Transform is niet toegewezen.");
            return;
        }

        // Clear actieve power-up posities
        activePowerUpPositions.Clear();

        // Start het spawnen van power-ups
        StartCoroutine(SpawnInitialPowerUps());
    }


    private IEnumerator SpawnInitialPowerUps()
    {
        foreach (PowerUpPickup pickupPrefab in powerUpPickupPrefabs)
        {
            SpawnPowerUpPickup(pickupPrefab);
            yield return new WaitForSeconds(0.5f); // Kleine vertraging tussen spawns
        }
    }

    public void SpawnPowerUpPickup(PowerUpPickup pickupPrefab)
    {
        if (spawnAreaCollider == null)
        {
            Debug.LogError("PowerUpManager: SpawnArea collider is niet ingesteld.");
            return;
        }

        Vector3 spawnPosition;
        int maxAttempts = 20;
        int attempts = 0;

        do
        {
            spawnPosition = GetRandomSpawnPosition();
            attempts++;
            if (attempts > maxAttempts)
            {
                Debug.LogWarning("PowerUpManager: Kan geen geldige spawnpositie vinden voor " + pickupPrefab.name);
                return;
            }
        }
        while (!IsValidSpawnPosition(spawnPosition));

        // Instantiate de PowerUpPickup prefab
        PowerUpPickup pickup = Instantiate(pickupPrefab, spawnPosition, Quaternion.identity);
        activePowerUpPositions.Add(spawnPosition);
        Debug.Log("PowerUpManager: " + pickupPrefab.name + " gespawned op " + spawnPosition);
    }


    private Vector3 GetRandomSpawnPosition()
    {
        // Haal de afmetingen van de collider op
        Bounds bounds = spawnAreaCollider.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector3(x, y, 0f);
    }

    private bool IsValidSpawnPosition(Vector3 position)
    {
        // Controleer op muren
        if (Physics2D.OverlapCircle(position, 1f, wallLayer))
        {
            return false;
        }

        // Controleer op andere power-ups
        foreach (Vector3 pos in activePowerUpPositions)
        {
            if (Vector3.Distance(position, pos) < minSpawnDistance)
            {
                return false;
            }
        }

        return true;
    }

    public void OnPowerUpUsed(PowerUp powerUp)
    {
        StartCoroutine(RespawnPowerUpAfterDuration(powerUp));
    }

    private IEnumerator RespawnPowerUpAfterDuration(PowerUp powerUp)
    {
        yield return new WaitForSeconds(powerUp.duration);
        // Vind de pickup prefab die overeenkomt met deze PowerUp
        PowerUpPickup pickupPrefab = FindPickupPrefab(powerUp);
        if (pickupPrefab != null)
        {
            SpawnPowerUpPickup(pickupPrefab);
        }
        else
        {
            Debug.LogWarning("PowerUpManager: Geen pickup prefab gevonden voor " + powerUp.name);
        }
    }

    private PowerUpPickup FindPickupPrefab(PowerUp powerUp)
    {
        foreach (var pickupPrefab in powerUpPickupPrefabs)
        {
            if (pickupPrefab.powerUp == powerUp)
            {
                return pickupPrefab;
            }
        }
        return null;
    }

    public void RemovePowerUpPosition(Vector3 position)
    {
        activePowerUpPositions.Remove(position);
    }

    public void ApplyPowerUp(PowerUp powerUp, Player player)
    {
        powerUp.Activate(player);
    }
}
