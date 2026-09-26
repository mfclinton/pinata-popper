using UnityEngine;
using UnityEngine.Playables;

public class SequenceManager : MonoBehaviour
{
    [Header("Main Menu Sequences")]
    [SerializeField] private PlayableDirector mainMenuEnterSequence;
    [SerializeField] private PlayableDirector mainMenuExitSequence;
    
    [Header("Game Sequences")]
    [SerializeField] private PlayableDirector gameEnterSequence;
    [SerializeField] private PlayableDirector gameExitSequence;
    
    [Header("Settings Sequences")]
    [SerializeField] private PlayableDirector settingsEnterSequence;
    [SerializeField] private PlayableDirector settingsExitSequence;
    
    [Header("Game Over Sequences")]
    [SerializeField] private PlayableDirector gameOverEnterSequence;
    [SerializeField] private PlayableDirector gameOverExitSequence;
    
    [Header("Transition Sequences")]
    [SerializeField] private PlayableDirector mainMenuToGameSequence;
    [SerializeField] private PlayableDirector gameToMainMenuSequence;
    
    public void PlayMainMenuEnterSequence(bool isPlaying)
    {
        if (isPlaying)
        {
            mainMenuEnterSequence.Play();
            mainMenuExitSequence.Stop();
        }
        else
        {
            mainMenuEnterSequence.Stop();
            mainMenuExitSequence.Play();
        }
    }
    
    public void PlayGameEnterSequence(bool isPlaying)
    {
        if (isPlaying)
        {
            gameEnterSequence.Play();
            gameExitSequence.Stop();
        }
        else
        {
            gameEnterSequence.Stop();
            gameExitSequence.Play();
        }
    }
    
    public void PlaySettingsEnterSequence(bool isPlaying)
    {
        if (isPlaying)
        {
            settingsEnterSequence.Play();
            settingsExitSequence.Stop();
        }
        else
        {
            settingsEnterSequence.Stop();
            settingsExitSequence.Play();
        }
    }
    
    public void PlayGameOverEnterSequence(bool isPlaying)
    {
        if (isPlaying)
        {
            gameOverEnterSequence.Play();
            gameOverExitSequence.Stop();
        }
        else
        {
            gameOverEnterSequence.Stop();
            gameOverExitSequence.Play();
        }
    }
    
    // TODO: Implement these
    public void PlayMainMenuToGameSequence()
    {
        mainMenuToGameSequence.Play();
    }
    
    public void PlayGameToMainMenuSequence()
    {
        gameToMainMenuSequence.Play();
    }
}
