using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIReferences : MonoBehaviour
{
    [Header("UI Documents")]
    [SerializeField] private UIDocument mainMenuDocument;
    [SerializeField] private UIDocument gameDocument;
    [SerializeField] private UIDocument gameOverDocument;
    [SerializeField] private UIDocument settingsDocument;
    public UIDocument MainMenuDocument => mainMenuDocument;
    public UIDocument GameDocument => gameDocument;
    public UIDocument GameOverDocument => gameOverDocument;
    public UIDocument SettingsDocument => settingsDocument;
    
    // Main Menu References
    private VisualElement mainMenuTitle;
    private Button startGameButton;
    public VisualElement MainMenuTitle => mainMenuTitle;
    public Button StartGameButton => startGameButton;
    
    // Game Over References
    private VisualElement gameOverPanel;
    private Label gameOverScoreLabel;
    private Label gameOverHighScoreLabel;
    private Button gameOverRestartButton;
    private Button gameOverHomeButton;
    public VisualElement GameOverPanel => gameOverPanel;
    public Label GameOverScoreLabel => gameOverScoreLabel;
    public Label GameOverHighScoreLabel => gameOverHighScoreLabel;
    public Button GameOverRestartButton => gameOverRestartButton;
    public Button GameOverHomeButton => gameOverHomeButton;

    // Game References
    private VisualElement gameScoresContainer;
    private VisualElement gamesCurScoreContainer;
    private VisualElement gamesHighScoreContainer;
    private Label gameCurScoreLabel;
    private Label gameHighScoreLabel;
    public VisualElement GameScoresContainer => gameScoresContainer;
    public VisualElement GameCurScoreContainer => gamesCurScoreContainer;
    public VisualElement GameHighScoreContainer => gamesHighScoreContainer;
    public Label GameCurScoreLabel => gameCurScoreLabel;
    public Label GameHighScoreLabel => gameHighScoreLabel;
    
    // Settings References
    private VisualElement settingsPanel;
    private Button settingsButton;
    private VisualElement settingsPanelButtonParent;
    private Button settingsRestartButton;
    private Button settingsHomeButton;
    public VisualElement SettingsPanel => settingsPanel;
    public Button SettingsButton => settingsButton;
    public VisualElement SettingsPanelButtonParent => settingsPanelButtonParent;
    public Button SettingsRestartButton => settingsRestartButton;
    public Button SettingsHomeButton => settingsHomeButton;
    
    // Slider References
    private Slider musicSlider;
    private Slider sfxSlider;
    public Slider MusicSlider => musicSlider;
    public Slider SFXSlider => sfxSlider;
    
    // References
    public UIReferences Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Initialize();
    }

    private void Initialize()
    {
        mainMenuTitle = mainMenuDocument.rootVisualElement.Q<VisualElement>(UIConstants.MainMenuTitle);
        startGameButton = mainMenuDocument.rootVisualElement.Q<Button>(UIConstants.MainMenuStartGameButtonName);
        
        gameOverPanel = gameOverDocument.rootVisualElement.Q<VisualElement>(UIConstants.GameOverPanel);
        gameOverScoreLabel = gameOverDocument.rootVisualElement.Q<Label>(UIConstants.GameOverScoreLabelName);
        gameOverHighScoreLabel = gameOverDocument.rootVisualElement.Q<Label>(UIConstants.GameOverHighScoreLabelName);
        gameOverRestartButton = gameOverDocument.rootVisualElement.Q<Button>(UIConstants.GameOverRestartButtonName);
        gameOverHomeButton = gameOverDocument.rootVisualElement.Q<Button>(UIConstants.GameOverHomeButtonName);
        
        gameScoresContainer = gameDocument.rootVisualElement.Q<VisualElement>(UIConstants.GameScoresContainer);
        gamesCurScoreContainer = gameDocument.rootVisualElement.Q<VisualElement>(UIConstants.GameCurScoreContainer);
        gamesHighScoreContainer = gameDocument.rootVisualElement.Q<VisualElement>(UIConstants.GameHighScoreContainer);
        gameCurScoreLabel = gameDocument.rootVisualElement.Q<Label>(UIConstants.GameCurScoreValueLabelName);
        gameHighScoreLabel = gameDocument.rootVisualElement.Q<Label>(UIConstants.GameHighScoreScoreValueLabelName);
        
        settingsPanel = settingsDocument.rootVisualElement.Q<VisualElement>(UIConstants.SettingsPanel);
        settingsButton = settingsDocument.rootVisualElement.Q<Button>(UIConstants.GameSettingsButton);
        settingsPanelButtonParent = settingsDocument.rootVisualElement.Q<VisualElement>(UIConstants.SettingsPanelButtonsParent);
        settingsRestartButton = settingsDocument.rootVisualElement.Q<Button>(UIConstants.GameSettingsRestartButton);
        settingsHomeButton = settingsDocument.rootVisualElement.Q<Button>(UIConstants.GameSettingsHomeButton);
        
        musicSlider = settingsDocument.rootVisualElement.Q<Slider>(UIConstants.MusicSlider);
        sfxSlider = settingsDocument.rootVisualElement.Q<Slider>(UIConstants.SFXSlider);
    }
}
