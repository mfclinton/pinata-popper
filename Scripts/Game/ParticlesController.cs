using UnityEngine;

public class ParticlesController : MonoBehaviour
{
    private ParticleSystem[] particleSystems;
    
    private void Awake()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }
    
    public void PlayParticles()
    {
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            particleSystem.Play();
        }
    }
    
    public void StopParticles()
    {
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            particleSystem.Stop();
        }
    }
    
    public void ToggleParticles(bool play)
    {
        if (play)
            PlayParticles();
        else
            StopParticles();
    }
}
