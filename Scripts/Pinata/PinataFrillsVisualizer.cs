using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Pinata))]
public class PinataFrillsVisualizer : MonoBehaviour
{
    [Header("Frill References")]
    [SerializeField] private Transform frillsParent;
    [SerializeField] private Material frillMaterialPrefab; // Reference to the material
    
    [Header("Frill Rotation Settings")]
    [SerializeField] private Vector2 frillRotationRange = new Vector2(-30f, 30f);
    [SerializeField] private float rotationAcceleration = 10f;
    [SerializeField] private float dampingFactor = 0.3f;
    
    [Header("Frill Rotation Input Settings")]
    [SerializeField] private Vector2 speedWeightRange = new Vector2(0.5f, 1f);
    [SerializeField] private Vector2 velocityRange = new Vector2(-10f, 10f);
    [SerializeField] private Vector2 gravityRange = new Vector2(-9.81f, 9.81f);

    [Header("Performance Settings")]
    [SerializeField] private float minSpeedToUpdate = 0.1f;

    // References
    private Pinata pinata;
    private PinataVisualizer pinataVisualizer;
    private Rigidbody2D rb;
    private SpriteRenderer[] frillSpriteRenderers;

    // Internal Variable
    private float rotation;
    private float rotationVelocity;
    private float targetRotation;

    private Material frillMaterialInstance;
    private MaterialPropertyBlock[] propertyBlocks;
    
    // Conts
    private const string evolvePercentageProperty = "_EvolvePercentage";
    private const string alphaProperty = "_Alpha";

    private void Awake()
    {
        pinata = GetComponent<Pinata>();
        pinataVisualizer = GetComponent<PinataVisualizer>();
        rb = GetComponent<Rigidbody2D>();
        
        frillSpriteRenderers = frillsParent.GetComponentsInChildren<SpriteRenderer>();
        
        // Set material
        frillMaterialInstance = frillMaterialPrefab;
        foreach (SpriteRenderer frillSpriteRenderer in frillSpriteRenderers)
            frillSpriteRenderer.sharedMaterial = frillMaterialInstance;

        // Create property blocks
        propertyBlocks = new MaterialPropertyBlock[frillSpriteRenderers.Length];
        for (int i = 0; i < frillSpriteRenderers.Length; i++)
        {
            propertyBlocks[i] = new MaterialPropertyBlock();
            frillSpriteRenderers[i].GetPropertyBlock(propertyBlocks[i]);
        }
    }
    
    private void OnEnable()
    {
        if (pinataVisualizer != null)
        {
            pinataVisualizer.OnEvolveEvent += OnEvolve;
            pinataVisualizer.OnFadeEvent += OnFade;
        }
    }
    
    private void OnDisable()
    {
        if (pinataVisualizer != null)
        {
            pinataVisualizer.OnEvolveEvent -= OnEvolve;
            pinataVisualizer.OnFadeEvent -= OnFade;
        }
    }
    
    private void FixedUpdate()
    {
        UpdateRotation();
    }

    private void Update()
    {
        if(ShouldUpdateRotation())
            UpdateFrills();
    }

    #region Rotation Calculation

    private bool ShouldUpdateRotation()
    {
        bool speedThreshMet = minSpeedToUpdate <= rb.velocity.magnitude;
        bool rotationConverged = Mathf.Abs(rotationVelocity) < 0.1f && Mathf.Abs(targetRotation - rotation) < 0.1f;

        return speedThreshMet || !rotationConverged;
    }
    
    private void UpdateRotation()
    {
        Vector2 localVelocity = rb.velocity;
        Vector2 localGravity = CalculateLocalGravity();

        targetRotation = CalculateTargetRotation(localVelocity, localGravity);
        ApplySpringSystem(targetRotation);

        UpdateObjectRotation();
    }

    private Vector2 CalculateLocalGravity()
    {
        return transform.InverseTransformDirection(Physics2D.gravity);
    }

    private float CalculateTargetRotation(Vector2 velocity, Vector2 gravity)
    {
        float velocityT = Mathf.InverseLerp(velocityRange.x, velocityRange.y, velocity.x);
        float gravityT = Mathf.InverseLerp(gravityRange.x, gravityRange.y, gravity.x);
        float velocityWeight = CalculateVelocityInterpWeightFactor(velocity);
    
        float t = Mathf.Lerp(gravityT, velocityT, velocityWeight);
        return Mathf.Lerp(frillRotationRange.x, frillRotationRange.y, t);
    }
    
    private float CalculateVelocityInterpWeightFactor(Vector2 velocity)
    {
        float speed = velocity.magnitude;
        return Mathf.InverseLerp(speedWeightRange.x, speedWeightRange.y, speed);
    }

    private void ApplySpringSystem(float targetRotation)
    {
        float rotationDifference = targetRotation - rotation;
        float damping = 2f * Mathf.Sqrt(rotationAcceleration) * dampingFactor;
        rotationVelocity += rotationDifference * rotationAcceleration * Time.fixedDeltaTime - rotationVelocity * damping * Time.fixedDeltaTime;
    }

    private void UpdateObjectRotation()
    {
        rotation += rotationVelocity * Time.fixedDeltaTime;
    }

    #endregion
    
    private void UpdateFrills()
    {
        foreach(SpriteRenderer frillSpriteRenderer in frillSpriteRenderers)
        {
            UpdateFrill(frillSpriteRenderer);
        }
    }
    
    private void UpdateFrill(SpriteRenderer frillSpriteRenderer)
    {
        frillSpriteRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
    }

    public void OnEvolve(float evolvePercentage)
    {
        for (int i = 0; i < frillSpriteRenderers.Length; i++)
        {
            propertyBlocks[i].SetFloat(evolvePercentageProperty, evolvePercentage);
            frillSpriteRenderers[i].SetPropertyBlock(propertyBlocks[i]);
        }
    }

    public void OnFade(float alpha)
    {
        for (int i = 0; i < frillSpriteRenderers.Length; i++)
        {
            propertyBlocks[i].SetFloat(alphaProperty, alpha);
            frillSpriteRenderers[i].SetPropertyBlock(propertyBlocks[i]);
        }
    }
}
