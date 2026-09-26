using System;
using UnityEngine;

[RequireComponent(typeof(AShootablesManager))]
public class Aimer : MonoBehaviour
{
    [Header("Aiming")]
    [SerializeField] private Vector2 forwardDirection = Vector2.right;
    [SerializeField] private float aimOffsetMagnitude = 1f;
    public float AimOffsetMagnitude => aimOffsetMagnitude;
    [SerializeField] private float maxRotationAngle = 75f;
    
    [Header("Shooting")]
    [SerializeField] private float timeBetweenShots = .5f;
    public float TimeBetweenShots => timeBetweenShots;
    [SerializeField] private float shootForce = 10f;

    [SerializeField] private Transform shotShootableParent;
    
    // Event delegate for when aim is updated
    public delegate void OnAimUpdated(Vector2 aimVector);
    public event OnAimUpdated OnAimUpdatedEvent;
    
    public delegate void OnShoot(AShootable shot);
    public event OnShoot OnShootEvent;
    
    // Internal Variables
    public Vector2 AimVector { get; private set; }
    private float timeLastShot = 0f;
    
    // References
    PlayerController playerController;
    AShootablesManager shootablesManager;

    #region Unity Callbacks

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        shootablesManager = GetComponent<AShootablesManager>();
        
        forwardDirection.Normalize();
        AimVector = forwardDirection;
    }

    private void OnEnable()
    {
        playerController.OnSelectStarted += OnSelectStarted;
        playerController.OnSelectEnded += OnSelectEnded;
    }
    
    private void OnDisable()
    {
        playerController.OnSelectStarted -= OnSelectStarted;
        playerController.OnSelectEnded -= OnSelectEnded;
    }
    
    private void Update()
    {
        AimTowards(playerController.PointerWorldPosition);
        ShowLoadedShootable();
    }

    #endregion

    #region Events

    void OnSelectStarted(Vector2 position)
    {
        
    }
    
    void OnSelectEnded(Vector2 position)
    {
        Shoot();
    }

    #endregion

    public void Reset()
    {
        if(shootablesManager != null)
            shootablesManager.Reset();
        
        enabled = true;
    }

    private void Shoot()
    {
        if(Time.time < timeLastShot + timeBetweenShots)
            return;
        
        AShootable shootable = shootablesManager.PopNextShootable();
        shootable.Fire(AimVector * shootForce);
        shootable.transform.SetParent(shotShootableParent);
        timeLastShot = Time.time;
        
        OnShootEvent?.Invoke(shootable);
    }

    public void AimTowards(Vector2 targetPosition)
    {
        Vector2 directionToTarget = (targetPosition - (Vector2)transform.position).normalized;
        float angleToTarget = Vector2.SignedAngle(forwardDirection, directionToTarget);
        float clampedAngle = Mathf.Clamp(angleToTarget, -maxRotationAngle, maxRotationAngle);

        RotateAimVector(clampedAngle);
    }

    private void RotateAimVector(float angle)
    {
        float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        
        float tx = forwardDirection.x;
        float ty = forwardDirection.y;

        float x = (cos * tx) - (sin * ty);
        float y = (sin * tx) + (cos * ty);
        AimVector = new Vector2(x, y).normalized;
        OnAimUpdatedEvent?.Invoke(AimVector);
    }
    
    private void ShowLoadedShootable()
    {
        AShootable shootable = shootablesManager.PeekNextShootable();
        shootable.transform.position = (Vector2)transform.position + (AimVector * aimOffsetMagnitude);
        shootable.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.down, AimVector));
    }

    private void OnDrawGizmos()
    {
        Vector3 maxRotationDir = Quaternion.Euler(0, 0, maxRotationAngle) * forwardDirection;
        Vector3 minRotationDir = Quaternion.Euler(0, 0, -maxRotationAngle) * forwardDirection;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)forwardDirection);
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + maxRotationDir);
        Gizmos.DrawLine(transform.position, transform.position + minRotationDir);
    
        // Draw the Aim Vector
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)AimVector);
    }
}