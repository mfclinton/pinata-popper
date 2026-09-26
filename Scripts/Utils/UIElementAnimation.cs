using UnityEngine;
using UnityEngine.UIElements;
using System;

public class UIElementAnimation
{
    // Animation Data
    private VisualElement element;
    private float duration;
    private Action<float> updateAction;
    
    // State
    private bool isPlaying = false;
    private bool isReversed = false;
    private float relElapsedTime = 0;
    private float T => relElapsedTime / duration;
    
    // Internal Variables
    private IVisualElementScheduledItem scheduledItem;
    
    public UIElementAnimation(VisualElement element, float duration, Action<float> updateAction)
    {
        this.element = element;
        this.duration = duration;
        this.updateAction = updateAction;
        
        SetProgress(0f);
    }
    
    public void Play(bool isReversed = false)
    {
        this.isReversed = isReversed;
        if (isPlaying)
            return;
        
        StartAnimation();
    }
    
    public void Pause()
    {
        isPlaying = false;
        scheduledItem?.Pause();
    }
    
    public void SetProgress(float progress)
    {
        Pause();
        relElapsedTime = Mathf.Clamp(progress * duration, 0, duration);
        updateAction.Invoke(T);
    }

    private void StartAnimation()
    {
        isPlaying = true;
        
        scheduledItem = element.schedule.Execute(() =>
        {
            float deltaSign = isReversed ? -1 : 1;
            float delta = Time.deltaTime * deltaSign;
            relElapsedTime = Mathf.Clamp(relElapsedTime + delta, 0, duration);
            
            updateAction.Invoke(T);

            if (T == 0 || T == 1)
            {
                isPlaying = false;
            }
        }).Every(1000 / 60).Until(() => !isPlaying);
    }
}