using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public List<SpawnWave> spawnWaves;            // Lijst van spawn waves
    public float spawnDistance = 5f;            // Afstand buiten het scherm om te spawnen
    public LayerMask wallLayer;                 // LayerMask voor muren

    private Transform playerTransform;
    private Player player;
    private Camera mainCamera;
    private int currentWaveIndex = 0;
    public int currentMonsterCount = 0;
    public float gameTime = 0f;
    private bool isWaveActive = false;
    private float waveEndTime = 0f;

    void Start()
    {
        player = FindFirstObjectByType<Player>();
        playerTransform = player.transform;

        mainCamera = Camera.main;

        // Sorteer de spawnWaves op tijdToActivate
        spawnWaves.Sort((a, b) => a.timeToActivate.CompareTo(b.timeToActivate));

        // Start het spawnproces
        StartCoroutine(SpawnWavesCoroutine());
    }

    private void Update()
    {
        if (!player.isDead)
        {
            gameTime += Time.deltaTime;
        }
        else
        {
            StopCoroutine(SpawnWavesCoroutine());
        }
    }

    private IEnumerator SpawnWavesCoroutine()
    {
        for (int i = 0; i < spawnWaves.Count; i++)
        {
            SpawnWave wave = spawnWaves[i];

            // Wacht tot de tijd voor deze wave is bereikt
            yield return new WaitUntil(() => gameTime >= wave.timeToActivate);

            // Bereken de duur van de wave
            float waveDuration;
            if (i < spawnWaves.Count - 1)
            {
                // Duration is het verschil tussen de volgende wave en de huidige wave
                waveDuration = spawnWaves[i + 1].timeToActivate - wave.timeToActivate;
            }
            else
            {
                // Voor de laatste wave, stel een zeer lange duur in
                waveDuration = Mathf.Infinity;
            }

            // Start de wave
            StartWave(wave, waveDuration);

            // Wacht tot de wave is afgelopen
            if (waveDuration != Mathf.Infinity)
            {
                yield return new WaitForSeconds(waveDuration);
                EndWave();
            }
        }
    }

    private void StartWave(SpawnWave wave, float waveDuration)
    {
        isWaveActive = true;
        waveEndTime = gameTime + waveDuration;
        currentWaveIndex++;

        Debug.Log("Starting wave " + currentWaveIndex);

        // Start de coroutine om monsters te spawnen met interval
        StartCoroutine(SpawnMonstersCoroutine(wave));

        // Start een coroutine om de wave te beëindigen na waveDuration
        if (waveDuration != Mathf.Infinity)
        {
            StartCoroutine(EndWaveAfterDuration(waveDuration));
        }
    }

    private IEnumerator SpawnMonstersCoroutine(SpawnWave wave)
    {
        while (currentMonsterCount < wave.maxMonstersInWave && gameTime < waveEndTime)
        {
            SpawnMonster(wave.monstersToSpawn);
            currentMonsterCount++;
            Debug.Log($"Spawned monster. Current count: {currentMonsterCount}/{wave.maxMonstersInWave}");

            // Wacht voor het volgende spawn interval
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }


    private IEnumerator EndWaveAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        EndWave();
    }

    private void EndWave()
    {
        Debug.Log("Wave ended");
        isWaveActive = false;
    }

    private void SpawnMonster(List<GameObject> monsterPrefabs)
    {
        if (playerTransform == null)
            return;

        Vector3 spawnPosition = GetSpawnPosition();

        // Controleer of de spawnpositie niet op een muur ligt
        if (!IsPositionOnWall(spawnPosition))
        {
            // Kies een willekeurig monster uit de lijst
            GameObject selectedMonster = monsterPrefabs[Random.Range(0, monsterPrefabs.Count)];
            Instantiate(selectedMonster, spawnPosition, Quaternion.identity);
        }
        else
        {
            SpawnMonster(monsterPrefabs); // Probeer opnieuw
        }
    }

    private Vector3 GetSpawnPosition()
    {
        // Kies willekeurig een zijde van het scherm: 0 = Links, 1 = Rechts, 2 = Boven, 3 = Onder
        int side = Random.Range(0, 4);
        Vector3 spawnPos = Vector3.zero;

        // Bepaal de wereldco�rdinaten van de schermgrenzen
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        switch (side)
        {
            case 0: // Links
                spawnPos = new Vector3(bottomLeft.x - spawnDistance, Random.Range(bottomLeft.y, topRight.y), 0);
                break;

            case 1: // Rechts
                spawnPos = new Vector3(topRight.x + spawnDistance, Random.Range(bottomLeft.y, topRight.y), 0);
                break;

            case 2: // Boven
                spawnPos = new Vector3(Random.Range(bottomLeft.x, topRight.x), topRight.y + spawnDistance, 0);
                break;

            case 3: // Onder
                spawnPos = new Vector3(Random.Range(bottomLeft.x, topRight.x), bottomLeft.y - spawnDistance, 0);
                break;
        }

        // Zorg ervoor dat de spawnpositie een bepaalde afstand van de speler blijft
        Vector3 directionFromPlayer = (spawnPos - playerTransform.position).normalized;
        spawnPos = playerTransform.position + directionFromPlayer * (mainCamera.orthographicSize * Mathf.Max(mainCamera.aspect, 1f) + spawnDistance);

        return spawnPos;
    }

    private bool IsPositionOnWall(Vector3 position)
    {
        float checkRadius = 1f; // Radius van de cirkel die we gebruiken om te controleren of er een muur is
        Collider2D hit = Physics2D.OverlapCircle(position, checkRadius, wallLayer);
        return hit != null;
    }

    // Methode om het aantal actieve monsters bij te houden
    public void MonsterDestroyed()
    {
        currentMonsterCount--;

        Debug.Log("iswaveactive " + isWaveActive + " currentwaveindex " + currentWaveIndex + " spawnwavescount " + spawnWaves.Count);
        if (isWaveActive && currentWaveIndex <= spawnWaves.Count)
        {
            Debug.Log("Current wave index: " + currentWaveIndex);
            SpawnWave currentWave = spawnWaves[currentWaveIndex - 1];
            if (currentMonsterCount < currentWave.maxMonstersInWave && gameTime < waveEndTime)
            {
                Debug.Log("Spawning new monster");
                SpawnMonster(currentWave.monstersToSpawn);
                currentMonsterCount++;
            }
        }
    }
}

[System.Serializable]
public class SpawnWave
{
    public float timeToActivate;                 // Tijd in seconden wanneer deze wave activeert
    public List<GameObject> monstersToSpawn;     // Lijst van monster prefabs voor deze wave
    public float spawnInterval;                  // Interval tussen spawns in deze wave
    public int maxMonstersInWave;                // Maximum aantal monsters in deze wave
}