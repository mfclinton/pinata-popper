using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public delegate void PointerAction(Vector2 position);
    public event PointerAction OnSelectStarted;
    public event PointerAction OnSelectEnded;
    
    public Action OnSettings;
    
    public Vector2 PointerScreenPosition { get; private set; }
    public Vector2 PointerWorldPosition => Camera.main.ScreenToWorldPoint(PointerScreenPosition); // TODO: make better
    
    private GameplayActions gameplayActions;
    
    #region Unity Callbacks

    private void Awake()
    {
        gameplayActions = new GameplayActions();
        
        gameplayActions.PlayerActions.Position.performed += OnPositionPerformed;

        gameplayActions.PlayerActions.Select.performed += OnSelectStartedPerformed;
        gameplayActions.PlayerActions.Select.canceled += OnSelectEndedPerformed;
        
        gameplayActions.PlayerActions.Settings.performed += OnSettingsPerformed;
    }

    private void OnEnable()
    {
        gameplayActions.Enable();
    }

    private void OnDisable()
    {
        gameplayActions.Disable();
    }

    #endregion

    #region Input Readers
    
    void OnPositionPerformed(InputAction.CallbackContext ctx)
    {
        PointerScreenPosition = ctx.ReadValue<Vector2>();
    }
    
    void OnSelectStartedPerformed(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        OnSelectStarted?.Invoke(PointerScreenPosition);
    }

    void OnSelectEndedPerformed(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        OnSelectEnded?.Invoke(PointerScreenPosition);
    }

    void OnSettingsPerformed(InputAction.CallbackContext ctx)
    {
        OnSettings?.Invoke();
    }

    #endregion
}
