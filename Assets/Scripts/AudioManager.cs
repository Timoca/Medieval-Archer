using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    // Singleton Instance
    public static AudioManager Instance { get; private set; }

    [Header("Audio Settings")]
    public AudioSource musicSource; // Referentie naar de AudioSource component
    public AudioSource sfxSource;    // Referentie naar een aparte AudioSource voor SFX
    public float fadeDuration = 1.0f; // Duur van de fade in/out in seconden

    public List<SceneMusic> sceneMusicList;
    private Dictionary<string, AudioClip> sceneMusicDict;

    [Header("Audio Clips")]
    public AudioClip buttonClickClip;
    public AudioClip gameOverClip;

    private void Awake()
    {
        // Implementatie van Singleton patroon
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialiseer de Dictionary
            sceneMusicDict = new Dictionary<string, AudioClip>();
            foreach (var sceneMusic in sceneMusicList)
            {
                if (!sceneMusicDict.ContainsKey(sceneMusic.sceneName))
                {
                    sceneMusicDict.Add(sceneMusic.sceneName, sceneMusic.musicClip);
                }
                else
                {
                    Debug.LogWarning($"Duplicate scene name detected: {sceneMusic.sceneName}");
                }
            }

            // Abonneer op Scene Loaded Event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Start muziek voor de eerste scene
        string currentScene = SceneManager.GetActiveScene().name;
        if (sceneMusicDict.ContainsKey(currentScene))
        {
            StartCoroutine(FadeInMusic(sceneMusicDict[currentScene]));
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        if (sceneMusicDict.ContainsKey(sceneName))
        {
            // Controleer of de muziek al aan het spelen is
            if (musicSource.clip == sceneMusicDict[sceneName])
            {
                return;
            }
            else
            {
                StartCoroutine(ChangeMusic(sceneMusicDict[sceneName]));
            }
        }
    }

    // Coroutine voor het veranderen van muziek met fade in/out
    private IEnumerator ChangeMusic(AudioClip newClip)
    {
        if (musicSource.isPlaying)
        {
            yield return StartCoroutine(FadeOutMusic());
        }

        musicSource.clip = newClip;
        yield return StartCoroutine(FadeInMusic(newClip));
    }

    // Coroutine voor Fade In
    private IEnumerator FadeInMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.volume = 0f;
        musicSource.Play();

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        musicSource.volume = 1f;
    }

    // Coroutine voor Fade Out
    private IEnumerator FadeOutMusic()
    {
        float startVolume = musicSource.volume;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop();
    }

    // Methode om SFX af te spelen
    public void PlaySFX(AudioClip sfxClip)
    {
        if (sfxClip == null)
        {
            Debug.LogWarning("AudioManager: SFX Clip is null.");
            return;
        }

        // Varieer de pitch licht tussen 0.10 en 1.10
        float randomPitch = 1f + Random.Range(-0.1f, 0.1f);
        sfxSource.pitch = randomPitch;

        // Speel het geluid af
        sfxSource.PlayOneShot(sfxClip);
    }

    public void PlayUIClick()
    {
        // Varieer de pitch licht tussen 0.10 en 1.10
        float randomPitch = 1f + Random.Range(-0.1f, 0.1f);
        sfxSource.pitch = randomPitch;

        // Speel het geluid af
        sfxSource.PlayOneShot(buttonClickClip);
    }

    // Methode om het Game Over geluid af te spelen en muziek te verlagen
    public void PlayGameOverSound()
    {
        StartCoroutine(PlayGameOverSoundCoroutine());
    }

    private IEnumerator PlayGameOverSoundCoroutine()
    {
        // demp de muziek
        musicSource.volume = 0.2f;

        // Speel het Game Over geluid af
        PlaySFX(gameOverClip);

        // Wacht tot het geluid is afgelopen
        yield return new WaitForSeconds(gameOverClip.length);

        // Hervat de muziek
        musicSource.volume = 1f;
    }


    private void OnDestroy()
    {
        // Unsubscribe van Scene Loaded Event
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}

// Mapping van Scene Namen naar Audio Clips
[System.Serializable]
public class SceneMusic
{
    public string sceneName;
    public AudioClip musicClip;
}