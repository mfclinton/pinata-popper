using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [Header("World References")]
    [SerializeField] private Aimer aimer;
    
    public bool SettingsVisible { get; private set; }
    
    // Internal References
    private ViewManager viewManager;
    private GameManager gameManager;
    private UIReferences uiReferences;
    private SequenceManager sequenceManager;
    
    // Events
    public event Action OnSettingsToggled;

    #region Unity Callbacks
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        viewManager = FindObjectOfType<ViewManager>();
        uiReferences = FindObjectOfType<UIReferences>();
        sequenceManager = FindObjectOfType<SequenceManager>();
        
        InitializeUI();
    }

    private void Start()
    {
        ShowMainMenuUI(true);
    }
    
    #endregion

    #region Initialization Methods
    private void InitializeUI()
    {
        InitializeMainMenuUI();
        InitializeGameOverUI();
        InitializeSettingsUI();
        
        // Events
        gameManager.OnGameOverEvent += OnGameOver;
    }

    private void InitializeMainMenuUI()
    {
        Button startGameButton = uiReferences.StartGameButton;
        startGameButton.clicked += OnStartGameButtonTriggered;
    }

    private void InitializeGameOverUI()
    {
        Button gameOverRestartButton = uiReferences.GameOverRestartButton;
        Button gameOverHomeButton = uiReferences.GameOverHomeButton;

        gameOverRestartButton.clicked += OnRestartButtonTriggered;
        gameOverHomeButton.clicked += OnHomeButtonTriggered;
    }
    
    private void InitializeSettingsUI()
    {
        Button settingsButton = uiReferences.SettingsButton;
        Button settingsRestartButton = uiReferences.SettingsRestartButton;
        Button settingsHomeButton = uiReferences.SettingsHomeButton;
        
        settingsButton.clicked += OnSettingsButtonTriggered;
        settingsRestartButton.clicked += OnRestartButtonTriggered;
        settingsHomeButton.clicked += OnHomeButtonTriggered;
        
        // Player Input Events
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.OnSettings += OnSettingsButtonTriggered;
    }
    
    #endregion

    #region UI Button Callbacks

    private void OnStartGameButtonTriggered()
    {
        // SwitchToGameUI();
        sequenceManager.PlayMainMenuToGameSequence();
        gameManager.Reset();
    }
    
    private void OnRestartButtonTriggered()
    {
        ShowGameOverUI(false);
        gameManager.Reset();
    }
    
    private void OnHomeButtonTriggered()
    {
        // SwitchToMainMenuUI();
        sequenceManager.PlayGameToMainMenuSequence();
        
        // Reset Game
        Debug.Log("Resetting Game");
        gameManager.SetNewHighScore(gameManager.Score);
        gameManager.Reset();
        gameManager.ShowAimer(false);
        
        SettingsVisible = false;
    }
    
    private void OnSettingsButtonTriggered()
    {
        ShowSettingsUI(!SettingsVisible);
    }

    #endregion

    #region UI Switching Methods
    private void SwitchToMainMenuUI()
    {
        ShowMainMenuUI(true);
    }
    
    private void SwitchToGameUI()
    {
        ShowGameUI(true);
    }

    private void OnGameOver()
    {
        ShowGameOverUI(true);
    }
    #endregion

    #region UI State Methods
    
    private void ShowMainMenuUI(bool isActive)
    {
        sequenceManager.PlayMainMenuEnterSequence(isActive);
        ShowSettingsButtons(!isActive); // TODO
    }
    
    private void ShowGameUI(bool isActive)
    {
        sequenceManager.PlayGameEnterSequence(isActive);
        ShowSettingsButtons(isActive); // TODO
    }
    
    private void ShowGameOverUI(bool isActive)
    {
        sequenceManager.PlayGameOverEnterSequence(isActive);
    }
    
    private void ShowSettingsUI(bool isActive)
    {
        SettingsVisible = isActive;
        sequenceManager.PlaySettingsEnterSequence(isActive);
        
        OnSettingsToggled?.Invoke();
    }
    
    #endregion

    #region UI State Helpers
    
    public void ShowSettingsButtons(bool isActive)
    {
        VisualElement settingsButtonsParent = uiReferences.SettingsPanelButtonParent;
        settingsButtonsParent.style.visibility = isActive ? Visibility.Visible : Visibility.Hidden;
    }

    #endregion
}
