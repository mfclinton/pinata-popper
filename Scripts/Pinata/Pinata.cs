using System;
using UnityEngine;

public class Pinata : AShootable
{
    // Delegate Events
    public delegate void HealthChange(int maxHealth, int currentHealth, int delta);
    public event HealthChange OnHealthChanged;
    public event Action MergeTriggered;
    
    [SerializeField] private int startingHealth;
    [SerializeField] private int tier;
    
    private const float escapeVecMultiplier = 2f;
    private const float timeBetweenEscapeChecks = .1f;
    private float nextTimeToAct = 0f;
    
    public int Health { get; private set; }
    public int Tier => tier;
    
    public bool IsMerging { get; private set; }
    
    public void TriggerMerge()
    {
        IsMerging = true;
        MergeTriggered?.Invoke();
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        Pinata otherPinata = other.gameObject.GetComponent<Pinata>();
        if (otherPinata != null)
            PinataManager.Instance.ProcessPinataCollision(this, otherPinata, other);
    }
    
    public void ModifyHealth(int delta)
    {
        Health += delta;
        OnHealthChanged?.Invoke(startingHealth, Health, delta);
    }
    
    public void EvaluateEscapeVector()
    {
        if (Time.time < nextTimeToAct)
            return;
        nextTimeToAct = Time.time + timeBetweenEscapeChecks;
        
        Vector2 escapeVector = GetEscapeVectorFromOverlaps();
        if (escapeVector != Vector2.zero)
        {
            transform.position += (Vector3)escapeVector * Time.deltaTime;
        }
    }
    
    public Vector2 GetEscapeVectorFromOverlaps()
    {
        if(IsMerging)
            return Vector2.zero;
        
        ContactFilter2D contactFilter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Pinata")
        };

        Collider2D[] results = new Collider2D[5];
        int numColliders = ShootableCollider.OverlapCollider(contactFilter, results);

        if (numColliders > 0)
        {
            // Avg Position
            Vector2 averagePosition = Vector2.zero;
            int count = 0;
            for (int i = 0; i < numColliders; i++)
            {
                ColliderDistance2D distance = Physics2D.Distance(ShootableCollider, results[i]);
                if(Mathf.Abs(distance.distance) < 0.1f)
                    continue;
                
                Debug.Log("Distance: " + distance.distance + " between " + gameObject.name + " and " + results[i].gameObject.name);
                
                averagePosition += (Vector2)results[i].transform.position;
                count++;
            }
            if(count == 0)
                return Vector2.zero;
            
            averagePosition /= count;
            Vector2 escapeVector = ((Vector2)transform.position - averagePosition).normalized;
            
            
            return escapeVector * escapeVecMultiplier;
        }

        return Vector2.zero;
    }

    #region Poolable Events

    protected override void OnSpawned()
    {
        IsMerging = false;
        Health = startingHealth;
        
        // Reset the health change event
        OnHealthChanged?.Invoke(startingHealth, Health, 0);
    }

    protected override void OnReturned()
    {
        // Reset Pinata To Default State
        IsMerging = false;
        Health = startingHealth;
        ShootableRigidbody.bodyType = RigidbodyType2D.Dynamic;
        ShootableCollider.enabled = true;
    }

    #endregion
}