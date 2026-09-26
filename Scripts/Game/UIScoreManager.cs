using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class UIScoreManager : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float scoreTickDurationPerPoint = .05f;
    [SerializeField] private float maxScoreTickDuration = 3f;
    [SerializeField] private AnimationCurve scoreTickCurve;
    
    // Internal References
    private UIReferences uiReferences;
    
    // Internal Variables
    private int currentDisplayedScore = 0;
    private int currentDisplayedHighScore = 0;
    private Coroutine scoreTickerCoroutine;
    private Coroutine highScoreTickerCoroutine;

    private void Awake()
    {
        uiReferences = FindObjectOfType<UIReferences>();
    }
    
    private void Start()
    {
        InitializeUI();
    }
    
    private void InitializeUI()
    {
        // Events
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.OnScoreUpdateEvent += UpdateScoreLabels;
        gameManager.OnHighScoreUpdateEvent += UpdateHighScoreLabels;
    }

    #region UI Update Methods
    
    IEnumerator TickScore(int startScore, int targetScore, float duration, Action<int> setter)
    {
        Debug.Log($"Duration: {duration}");

        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float elapsed = (Time.time - startTime) / duration;
            float curveValue = scoreTickCurve.Evaluate(elapsed);
            
            int newScoreValue = Mathf.RoundToInt(Mathf.Lerp(startScore, targetScore, curveValue));
            setter(newScoreValue);
            
            yield return null;
        }

        setter(targetScore);
    }
    
    private void UpdateScoreLabels(int score, bool forceUpdateUI = false)
    {
        if (scoreTickerCoroutine != null)
            StopCoroutine(scoreTickerCoroutine);

        if (!forceUpdateUI)
        {
            float duration = scoreTickDurationPerPoint * Mathf.Abs(score - currentDisplayedScore);
            duration = Mathf.Clamp(duration, 0, maxScoreTickDuration);
            scoreTickerCoroutine = StartCoroutine(TickScore(currentDisplayedScore, score, duration, SetScoreLabels));
        }
        else
        {
            SetScoreLabels(score);
        }
        
        uiReferences.GameOverScoreLabel.text = score.ToString();
    }
    
    private void UpdateHighScoreLabels(int newHighScore, bool forceUpdateUI = false)
    {
        if (highScoreTickerCoroutine != null)
            StopCoroutine(highScoreTickerCoroutine);

        if (!forceUpdateUI)
        {
            float duration = scoreTickDurationPerPoint * Mathf.Abs(newHighScore - currentDisplayedHighScore);
            duration = Mathf.Clamp(duration, 0, maxScoreTickDuration);
            highScoreTickerCoroutine = StartCoroutine(TickScore(currentDisplayedHighScore, newHighScore, duration, SetHighScoreLabels));
        }
        else
        {
            SetHighScoreLabels(newHighScore);
        }
        
        uiReferences.GameOverHighScoreLabel.text = newHighScore.ToString();
    }
    
    private void SetScoreLabels(int score)
    {
        Label gameCurScoreLabel = uiReferences.GameCurScoreLabel;
        Label gameOverScoreLabel = uiReferences.GameOverScoreLabel;
        
        gameCurScoreLabel.text = $"{score.ToString()}";
        gameOverScoreLabel.text = score.ToString();
        currentDisplayedScore = score;
    }
    
    private void SetHighScoreLabels(int score)
    {
        Label gameHighScoreLabel = uiReferences.GameHighScoreLabel;
        Label gameOverHighScoreLabel = uiReferences.GameOverHighScoreLabel;
        
        gameHighScoreLabel.text = $"{score.ToString()}";
        gameOverHighScoreLabel.text = score.ToString();
        currentDisplayedHighScore = score;
    }
    
    #endregion
}
