using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Aimer aimer;
    
    // Event Delegates
    public delegate void OnScoreUpdate(int score, bool forceUpdateUI = false);
    public event OnScoreUpdate OnScoreUpdateEvent;
    public delegate void OnHighScoreUpdate(int score, bool forceUpdateUI = false);
    public event OnHighScoreUpdate OnHighScoreUpdateEvent;
    
    public delegate void OnGameOver();
    public event OnGameOver OnGameOverEvent;

    public event Action OnGameReset;
    
    // Internal Variables
    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public bool GameOver { get; private set; }
    
    // References
    private PinataManager pinataManager;
    private NewgroundsHelper newgroundsHelper;
    
    private void Awake()
    {
        Application.targetFrameRate = 60; // TODO: Put this somewhere better

        pinataManager = FindObjectOfType<PinataManager>();
        newgroundsHelper = FindObjectOfType<NewgroundsHelper>();
        
        Score = 0;
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
        GameOver = false;
    }

    private void OnEnable()
    {
        pinataManager.OnPinataMergeEvent += ProcessMergeScore;
    }
    
    private void OnDisable()
    {
        pinataManager.OnPinataMergeEvent -= ProcessMergeScore;
    }

    private void ProcessMergeScore(Pinata a, Pinata b, Pinata c)
    {
        int pointsEarned = Mathf.RoundToInt(Mathf.Pow(2, c.Tier + 1)); // TODO: Make this a formula
        UpdateScore(pointsEarned);
    }

    public void UpdateScore(int score, bool forceUpdateUI = false)
    {
        Score += score;
        OnScoreUpdateEvent?.Invoke(Score, forceUpdateUI);
    }
    
    public void UpdateHighScore(int score, bool forceUpdateUI = false)
    {
        HighScore = score;
        OnHighScoreUpdateEvent?.Invoke(score, forceUpdateUI);
    }
    
    public void SetNewHighScore(int score)
    {
        newgroundsHelper?.PostScore(score);
        
        if(score <= HighScore)
            return;
        
        PlayerPrefs.SetInt("HighScore", score);
        UpdateHighScore(score);
    }

    public void TriggerGameOver()
    {
        GameOver = true;
        
        ShowAimer(false);
        SetNewHighScore(Score);
        
        OnGameOverEvent?.Invoke();
    }
    
    public void Reset()
    {
        GameOver = false;
        
        Score = 0;
        UpdateScore(Score, true);
        UpdateHighScore(HighScore, true);
        
        pinataManager.Reset();
        ShowAimer(true);
        
        OnGameReset?.Invoke();
    }

    public void ShowAimer(bool isActive)
    {
        // Show Aimer
        aimer.gameObject.SetActive(isActive);
        if(isActive)
            aimer.Reset();
    }
}