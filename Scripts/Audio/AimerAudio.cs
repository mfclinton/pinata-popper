using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AimerAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] shootSounds;
    
    // References
    private Aimer aimer;
    private AudioSource audioSource;
    
    private void Awake()
    {
        aimer = FindObjectOfType<Aimer>();
        audioSource = GetComponent<AudioSource>();
    }
    
    private void OnEnable()
    {
        aimer.OnShootEvent += PlayShootSound;
    }
    
    private void OnDisable()
    {
        aimer.OnShootEvent -= PlayShootSound;
    }
    
    private void PlayShootSound(AShootable shot)
    {
        AudioClip shootSound = shootSounds[Random.Range(0, shootSounds.Length)];
        audioSource.PlayOneShot(shootSound);
    }
}