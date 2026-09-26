using System;
using UnityEngine;

public class PinataManagerAudio : MonoBehaviour
{
    [Header("Combo Sounds")]
    [SerializeField] AudioSource comboAudioSource;
    [SerializeField] private float maxTimeBetweenComboSounds;
    [SerializeField] private AudioClip[] comboSounds;
    
    [Header("Collision Sounds")]
    [SerializeField] AudioSource[] collisionAudioSources;
    [SerializeField] private AudioClip[] collisionSounds;
    [SerializeField] private float maxCollisionVelocity = 10f;
    
    
    // Internal Variables
    private float lastComboSoundTime;
    private int comboCount;
    
    int collisionAudioSourceIndex = 0;
    
    // References
    PinataManager pinataManager;

    private void Awake()
    {
        pinataManager = FindObjectOfType<PinataManager>();
    }
    
    private void OnEnable()
    {
        pinataManager.OnPinataMergeEvent += ProcessMergeAudio;
        pinataManager.OnPinataCollisionEvent += ProcessCollisionAudio;
    }
    
    private void OnDisable()
    {
        pinataManager.OnPinataMergeEvent -= ProcessMergeAudio;
        pinataManager.OnPinataCollisionEvent -= ProcessCollisionAudio;
    }
    
    private void ProcessMergeAudio(Pinata a, Pinata b, Pinata c)
    {
        if(lastComboSoundTime + maxTimeBetweenComboSounds < Time.time)
            comboCount = 0;
        else
            comboCount = Mathf.Min(comboCount + 1, comboSounds.Length - 1);
        
        AudioClip comboSound = comboSounds[comboCount];
        comboAudioSource.PlayOneShot(comboSound);
        
        lastComboSoundTime = Time.time;
    }

    private void ProcessCollisionAudio(Pinata a, Pinata b, Collision2D collision)
    {
        AudioSource collisionAudioSource = collisionAudioSources[collisionAudioSourceIndex];

        // TODO: Calculate a volume based on the collision velocity
        float collisionVelocity = collision.relativeVelocity.magnitude;
        float normalizedVolume = Mathf.Clamp01(collisionVelocity / maxCollisionVelocity);
        
        AudioClip collisionSound = collisionSounds[UnityEngine.Random.Range(0, collisionSounds.Length)];
        
        collisionAudioSource.volume = normalizedVolume;
        collisionAudioSource.PlayOneShot(collisionSound);
        
        collisionAudioSourceIndex = (collisionAudioSourceIndex + 1) % collisionAudioSources.Length;
    }
}