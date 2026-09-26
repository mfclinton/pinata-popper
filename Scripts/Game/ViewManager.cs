using System.Collections;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    [SerializeField, Tooltip("Maximum Y position of the camera.")]
    private float maxYPos = 20f;

    [SerializeField, Tooltip("Minimum Y position of the camera.")]
    private float minYPos = 0f;

    [SerializeField, Tooltip("Duration of the camera transition in seconds.")]
    private float animDuration = 2f;

    [SerializeField, Tooltip("Curve to smooth the camera transition.")]
    private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // References
    private Material bgMaterial;
    private Coroutine transitionCoroutine;

    public bool IsTransitioning => transitionCoroutine != null;

    private void Awake()
    {
        InitializeBackgroundMaterial();
    }

    private void Update()
    {
        UpdateView();
    }

    private void InitializeBackgroundMaterial()
    {
        MeshRenderer bgRenderer = GetComponentInChildren<MeshRenderer>();
        bgMaterial = Instantiate(bgRenderer.sharedMaterial);
        bgRenderer.sharedMaterial = bgMaterial;
    }
    
    private IEnumerator SmoothTransition(float targetPercentage)
    {
        float durationPercentage = Mathf.Abs(targetPercentage - GetCurPercentage());
        float duration = animDuration * durationPercentage;
        
        float targetYPos = Mathf.Lerp(minYPos, maxYPos, targetPercentage);
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(transform.position.x, targetYPos, transform.position.z);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curveT = transitionCurve.Evaluate(t);
            transform.position = Vector3.Lerp(startPosition, targetPosition, curveT);
            yield return null;
        }

        transform.position = targetPosition;
        transitionCoroutine = null;
    }

    public Coroutine TransitionToPercentage(float percentage)
    {
        percentage = Mathf.Clamp01(percentage);
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);
        
        transitionCoroutine = StartCoroutine(SmoothTransition(percentage));
        return transitionCoroutine;
    }
    
    public void TriggerTransition(bool toStart)
    {
        float targetPercentage = toStart ? 1f : 0f;
        TransitionToPercentage(targetPercentage);
    }
    
    private float GetCurPercentage()
    {
        float p = (transform.position.y - minYPos) / (maxYPos - minYPos);
        return Mathf.Clamp01(p);
    }
    
    private void UpdateBackground(float p)
    {
        bgMaterial.SetFloat("_p", p);
    }

    private void UpdateView()
    {
        float p = GetCurPercentage();
        UpdateBackground(p);
    }
}
