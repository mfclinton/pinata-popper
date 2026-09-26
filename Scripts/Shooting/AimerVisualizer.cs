using System;
using UnityEngine;

[RequireComponent(typeof(Aimer), typeof(LineRenderer))]
public class AimerVisualizer : MonoBehaviour
{
    [SerializeField] private float offsetMagnitude = 1f;
    [SerializeField] private float previewLineLength = 5f;
    
    // References
    private Aimer aimer;
    private LineRenderer lineRenderer;

    private float offset = 0f;
    
    private void Awake()
    {
        aimer = GetComponent<Aimer>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        
        // Setup gradient
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 0.95f) }
        );
        lineRenderer.colorGradient = gradient;

    }

    private void OnEnable()
    {
        aimer.OnAimUpdatedEvent += OnAimUpdated;
    }
    
    private void OnDisable()
    {
        aimer.OnAimUpdatedEvent -= OnAimUpdated;
    }

    private void Update()
    {
        lineRenderer.material.mainTextureOffset = new Vector2(offset, 0f);
    }

    private void OnAimUpdated(Vector2 aimVector)
    {
        Vector3 aimVector3D = aimVector;
        Vector3 offsetPosition = transform.position + aimVector3D * offsetMagnitude;
        Vector3 endPoint = offsetPosition + aimVector3D * previewLineLength;

        lineRenderer.SetPosition(0, offsetPosition);
        lineRenderer.SetPosition(1, endPoint);
    }
}