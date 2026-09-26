using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Pinata), typeof(SpriteRenderer))]
public class PinataVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer[] accessoryRenderers;
    
    [Header("Evolve Settings")]
    [SerializeField] private float evolveDuration = .2f;
    [SerializeField] private float fadeDuration = .45f; // Ensure this is less than Aimer's timeBetweenShots
    [SerializeField] private float scaleDuration = .2f;
    [SerializeField] private float scaleStartPercentage = .9f;
    
    // Events
    public delegate void OnEvolve(float evolvePercentage);
    public event OnEvolve OnEvolveEvent;
    
    public delegate void OnFade(float alpha);
    public event OnFade OnFadeEvent;
    
    // Internal Variables
    private Pinata pinata;
    private SpriteRenderer spriteRenderer;
    
    private Coroutine evolveCoroutine;
    
    private Coroutine fadeCoroutine;
    private Coroutine scaleCoroutine;
    
    private Vector3 originalScale;
    
    // Material Properties
    private const string evolvePercentageProperty = "_EvolvePercentage";
    
    private void Awake()
    {
        pinata = GetComponent<Pinata>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        originalScale = transform.localScale;
    }
    
    private void OnEnable()
    {
        pinata.MergeTriggered += TriggerMergeAnimation;
        
        pinata.OnShootableLoadedEvent += TriggerSpawnAnimation;
        pinata.OnSpawnedEvent += Reset;
        
        pinata.OnReturnedEvent += CancelEvolve;
        
        TriggerScaleAnimation();
    }
    
    private void OnDisable()
    {
        pinata.MergeTriggered -= TriggerMergeAnimation;
        
        pinata.OnShootableLoadedEvent -= TriggerSpawnAnimation;
        pinata.OnSpawnedEvent -= Reset;
        
        pinata.OnReturnedEvent -= CancelEvolve;
    }
    
    private void Reset()
    {
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(props);
        props.SetFloat(evolvePercentageProperty, 0f);
        spriteRenderer.SetPropertyBlock(props);
        UpdateAccessoryEvolve(0f);
        OnEvolveEvent?.Invoke(0f);
    }
    
    private void CancelEvolve()
    {
        if(evolveCoroutine != null)
            StopCoroutine(evolveCoroutine);
    }
    
    private void CancelFade()
    {
        if(fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        
        // Reset alpha
        Color color = spriteRenderer.color;
        color.a = 1f;
        spriteRenderer.color = color;
        OnFadeEvent?.Invoke(1f);
    }
    
    private void CancelScale()
    {
        if(scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);
        
        transform.localScale = originalScale;
    }
    
    private void TriggerSpawnAnimation()
    {
        CancelFade();

        fadeCoroutine = StartCoroutine(FadeIn());
    }
    
    private void TriggerMergeAnimation()
    {
        CancelEvolve();
        evolveCoroutine = StartCoroutine(AnimateEvolve(true));
    }
    
    private void TriggerScaleAnimation()
    {
        CancelScale();
        scaleCoroutine = StartCoroutine(ScaleAnimation(true));
    }

    private IEnumerator AnimateEvolve(bool isEvolving) // Evolving vs Devolving
    {
        float elapsedTime = 0f;
        
        float startValue = isEvolving ? 0f : 1f;
        float endValue = 1f - startValue;

        MaterialPropertyBlock props = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(props);

        while (elapsedTime < evolveDuration)
        {
            float progress = elapsedTime / evolveDuration;
            float newValue = Mathf.Lerp(startValue, endValue, progress);

            props.SetFloat(evolvePercentageProperty, newValue);
            spriteRenderer.SetPropertyBlock(props);
            UpdateAccessoryEvolve(newValue);
            OnEvolveEvent?.Invoke(newValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        props.SetFloat(evolvePercentageProperty, endValue);
        spriteRenderer.SetPropertyBlock(props);
        UpdateAccessoryEvolve(endValue);
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        
        Color startColor = spriteRenderer.color;
        startColor.a = 0f;
        Color endColor = spriteRenderer.color;
        endColor.a = 1f;

        while (elapsedTime < fadeDuration)
        {
            float progress = elapsedTime / fadeDuration;
            
            Color currentColor = Color.Lerp(startColor, endColor, progress);
            spriteRenderer.color = currentColor;
            OnFadeEvent?.Invoke(currentColor.a);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = endColor;
        OnFadeEvent?.Invoke(endColor.a);
    }
    
    private IEnumerator ScaleAnimation(bool isGrowing)
    {
        float elapsedTime = 0f;
        
        Vector3 startScale = isGrowing ? originalScale * scaleStartPercentage : transform.localScale;
        Vector3 endScale = isGrowing ? originalScale : originalScale * scaleStartPercentage;

        while (elapsedTime < scaleDuration)
        {
            float progress = elapsedTime / scaleDuration;
            
            transform.localScale = Vector3.Lerp(startScale, endScale, progress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale;
    }
    
    void UpdateAccessoryEvolve(float evolvePercentage)
    {
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        
        foreach(SpriteRenderer accessoryRenderer in accessoryRenderers)
        {
            accessoryRenderer.GetPropertyBlock(props);
            props.SetFloat(evolvePercentageProperty, evolvePercentage);
            accessoryRenderer.SetPropertyBlock(props);
        }
    }
}
