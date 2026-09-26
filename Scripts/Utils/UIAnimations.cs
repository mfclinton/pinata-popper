using UnityEngine;
using UnityEngine.UIElements;
using System;

public static class UIAnimations
{
    public static UIElementAnimation Pop(VisualElement element,
        Vector2 startScale, Vector2 targetScale,
        float duration, AnimationCurve curve, Action onComplete = null)
    {
        return VisualElementAnimation(element, duration, (progress) =>
        {
            Vector2 scale = Vector2.LerpUnclamped(startScale, targetScale, curve.Evaluate(progress));
            element.style.scale = new Scale(scale);
        }, onComplete);
    }
    
    public static UIElementAnimation Rotate(VisualElement element,
        float startRotation, float targetRotation,
        float duration, AnimationCurve curve, Action onComplete = null)
    {
        return VisualElementAnimation(element, duration, (progress) =>
        {
            float currentRotation = Mathf.LerpUnclamped(startRotation, targetRotation, curve.Evaluate(progress));
            element.transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);
        }, onComplete);
    }

    public static UIElementAnimation Slide(VisualElement element,
        float startPercentageX, float targetPercentageX,
        float startPercentageY, float targetPercentageY,
        float duration, AnimationCurve curve, Action onComplete = null)
    {
        return VisualElementAnimation(element, duration, (progress) =>
        {
            float currentPercentageX = Mathf.LerpUnclamped(startPercentageX, targetPercentageX, curve.Evaluate(progress));
            float currentPercentageY = Mathf.LerpUnclamped(startPercentageY, targetPercentageY, curve.Evaluate(progress));

            element.style.left = Length.Percent(currentPercentageX);
            element.style.top = Length.Percent(currentPercentageY);
        }, onComplete);
    }
    
    private static UIElementAnimation VisualElementAnimation(VisualElement element, float duration, Action<float> onUpdate, Action onComplete)
    {
        UIElementAnimation uiElementAnimation = new UIElementAnimation(element, duration, onUpdate);
        return uiElementAnimation;
    }
}
