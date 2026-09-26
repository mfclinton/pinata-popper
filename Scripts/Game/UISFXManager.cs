using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UISFXManager : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private AudioSource startGameButtonSFX;
    [SerializeField] private AudioSource settingsButtonSFX;
    [SerializeField] private AudioSource restartButtonSFX;
    [SerializeField] private AudioSource homeButtonSFX;

    private UIReferences uiReferences;
    
    private void Awake()
    {
        uiReferences = FindObjectOfType<UIReferences>();
    }

    private void Start()
    {
        InitializeUISFX();
    }

    #region SFX

    private void InitializeUISFX()
    {
        uiReferences.StartGameButton.RegisterCallback<ClickEvent>(ev => PlayStartGameButtonSFX());

        UIManager uiManager = FindObjectOfType<UIManager>();
        uiManager.OnSettingsToggled += PlaySettingsButtonSFX;
        
        uiReferences.SettingsRestartButton.RegisterCallback<ClickEvent>(ev => PlayRestartButtonSFX());
        uiReferences.SettingsHomeButton.RegisterCallback<ClickEvent>(ev => PlayHomeButtonSFX());
        
        uiReferences.GameOverRestartButton.RegisterCallback<ClickEvent>(ev => PlayRestartButtonSFX());
        uiReferences.GameOverHomeButton.RegisterCallback<ClickEvent>(ev => PlayHomeButtonSFX());
    }
    
    private void PlayStartGameButtonSFX()
    {
        startGameButtonSFX.Play();
    }
    
    private void PlaySettingsButtonSFX()
    {
        settingsButtonSFX.Play();
    }
    
    private void PlayRestartButtonSFX()
    {
        restartButtonSFX.Play();
    }
    
    private void PlayHomeButtonSFX()
    {
        homeButtonSFX.Play();
    }

    #endregion
}
