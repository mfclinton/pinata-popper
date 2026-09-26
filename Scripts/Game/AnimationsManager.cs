using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[Serializable]
public class AnimationData
{
    public enum AnimationType
    {
        Pop,
        Slide,
        Rotate
    }

    [Tooltip("Duration of the animation in seconds")]
    public float duration = 2f;

    [Tooltip("Animation curve defining the pacing of the animation")]
    public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [Tooltip("Type of animation to apply")]
    public AnimationType type;
    
    [Header("Slide Animation Fields")]
    [Tooltip("Start position percentage for Slide animation")]
    public Vector2 slideStartPercentage;
    [Tooltip("End position percentage for Slide animation")]
    public Vector2 slideEndPercentage;

    [Header("Rotation Animation Fields")]
    [Tooltip("Start angle for Rotate animation")]
    [Range(-360, 360)]
    public float rotateStartAngle;
    [Tooltip("End angle for Rotate animation")]
    [Range(-360, 360)]
    public float rotateEndAngle;

    public UIElementAnimation CreateAnimation(VisualElement element)
    {
        switch (type)
        {
            case AnimationType.Pop:
                return UIAnimations.Pop(element, Vector2.zero, Vector2.one, duration, curve);
            case AnimationType.Slide:
                // Utilize the slideStartPercentage and slideEndPercentage here
                return UIAnimations.Slide(element, slideStartPercentage.x, slideEndPercentage.x, slideStartPercentage.y, slideEndPercentage.y, duration, curve);
            case AnimationType.Rotate:
                // Utilize the rotateStartAngle and rotateEndAngle here
                return UIAnimations.Rotate(element, rotateStartAngle, rotateEndAngle, duration, curve);
            default:
                return null;
        }
    }
}

public class AnimationsManager : MonoBehaviour
{
    // Enum for type of animation
    
    
    [Header("Title Animation")]
    [Tooltip("The Logo")]
    [SerializeField] private AnimationData titleAnimData;
    
    [Header("Play Button Animation")]
    [Tooltip("The Play Button")]
    [SerializeField] private AnimationData playBtnAnimData;
    
    [Header("Score Panel Animation")]
    [Tooltip("The Game Score/Highscore Panel")]
    [SerializeField] private AnimationData curScoreContainerAnimData;
    [SerializeField] private AnimationData highscoreContainerAnimData;
    
    [Header("Settings Panel Animation")]
    [Tooltip("The Settings Panel")]
    [SerializeField] private AnimationData settingsPanelAnimData;
    
    [Header("Settings Button Animation")]
    [Tooltip("The Settings Cog Button")]
    [SerializeField] private AnimationData settingsBtnAnimData;
    
    [Tooltip("The Settings Parent Container")]
    [SerializeField] private AnimationData settingsBtnParentAnimData;
    
    [Header("Game Over Panel Animation")]
    [Tooltip("The Game Over Panel")]
    [SerializeField] private AnimationData gameOverPanelAnimData;
    
    // Component References
    private UIReferences uiReferences;
    
    // Animation references
    private UIElementAnimation titleAnim, playBtnAnim, curScoreContainerAnim, highScoreContainerAnim, settingsPanelAnim, settingsBtnParentAnim, settingBtnAnim, gameOverPanelAnim;
    
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
        titleAnim = titleAnimData.CreateAnimation(uiReferences.MainMenuTitle);
        playBtnAnim = playBtnAnimData.CreateAnimation(uiReferences.StartGameButton);
        curScoreContainerAnim = curScoreContainerAnimData.CreateAnimation(uiReferences.GameCurScoreContainer);
        highScoreContainerAnim = highscoreContainerAnimData.CreateAnimation(uiReferences.GameHighScoreContainer);
        settingsPanelAnim = settingsPanelAnimData.CreateAnimation(uiReferences.SettingsPanel);
        settingBtnAnim = settingsBtnAnimData.CreateAnimation(uiReferences.SettingsButton);
        settingsBtnParentAnim = settingsBtnParentAnimData.CreateAnimation(uiReferences.SettingsButton.parent);
        gameOverPanelAnim = gameOverPanelAnimData.CreateAnimation(uiReferences.GameOverPanel);
    }

    // Toggle Animations
    public void ToggleTitleAnimation(bool popIn) => PlayAnimation(titleAnim, popIn);
    
    public void ToggleCurScoreContainerAnimation(bool slideIn) => PlayAnimation(curScoreContainerAnim, slideIn);
    
    public void ToggleHighScoreContainerAnimation(bool slideIn) => PlayAnimation(highScoreContainerAnim, slideIn);

    public void TogglePlayButtonAnimation(bool popIn) => PlayAnimation(playBtnAnim, popIn);

    public void ToggleSettingsPanelAnimation(bool popIn) => PlayAnimation(settingsPanelAnim, popIn);

    public void ToggleSettingsButtonAnimation(bool rollIn)
    {
        PlayAnimation(settingsBtnParentAnim, rollIn);
        PlayAnimation(settingBtnAnim, rollIn);
    }

    public void ToggleGameOverPanelAnimation(bool popIn) => PlayAnimation(gameOverPanelAnim, popIn);

    // Helper for toggling animations
    private void PlayAnimation(UIElementAnimation anim, bool isForward)
    {
        bool isReversed = !isForward;
        anim.Play(isReversed);
    }
}
