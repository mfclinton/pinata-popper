using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private bool enableFadeIn = true;
    [SerializeField] private bool enableFadeOut = true;
    [SerializeField] private float fadeInTime = 2f;
    [SerializeField] private float fadeOutTime = 5f;
    [SerializeField] private Vector2 timeBetweenSongs = new Vector2(1f, 2f); 

    private AudioSource audioSource;
    private GameManager gameManager;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        StartCoroutine(PlayRandomClipWithDelay());
    }
    
    private void OnEnable()
    {
        gameManager.OnGameOverEvent += HandleGameOver;
        gameManager.OnGameReset += OnGameReset;
    }
    
    private void OnDisable()
    {
        gameManager.OnGameOverEvent -= HandleGameOver;
        gameManager.OnGameReset -= OnGameReset;
    }
    
    private void OnGameReset()
    {
        StartCoroutine(PlayRandomClipWithDelay());
    }

    private IEnumerator PlayRandomClipWithDelay()
    {
        PlayRandomClip();
        yield return new WaitForSeconds(Random.Range(timeBetweenSongs.x, timeBetweenSongs.y));
    }
    
    private void PlayRandomClip()
    {
        int randomClipIndex = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[randomClipIndex];
        StartCoroutine(HandleAudioPlay());
    }

    private IEnumerator HandleAudioPlay()
    {
        audioSource.Play();

        if (enableFadeIn)
            yield return Fade(true, fadeInTime);
        else
            audioSource.volume = 1f;
    
        float remainingTime = audioSource.clip.length - audioSource.time;
        yield return new WaitForSeconds(remainingTime - (enableFadeOut ? fadeOutTime : 0f));

        if (enableFadeOut)
            yield return Fade(false, fadeOutTime);

        yield return new WaitForSeconds(Random.Range(timeBetweenSongs.x, timeBetweenSongs.y));
        PlayRandomClip();
    }

    private IEnumerator Fade(bool fadeIn, float duration)
    {
        float targetVolume = fadeIn ? 1f : 0f;
        float speed = 1f / duration;
        
        float initialVolume = fadeIn ? 0f : 1f;
        audioSource.volume = initialVolume;

        while (!Mathf.Approximately(audioSource.volume, targetVolume))
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, speed * Time.deltaTime);
            yield return null;
        }

        if (!fadeIn) audioSource.Stop();
    }
    
    public void HandleGameOver()
    {
        StopAllCoroutines();
        if (enableFadeOut)
            StartCoroutine(Fade(false, fadeOutTime));
        else
            audioSource.Stop();
    }
}
