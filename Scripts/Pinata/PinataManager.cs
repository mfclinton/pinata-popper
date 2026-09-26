using System;
using System.Collections;
using UnityEngine;
using System.Linq;

public class PinataManager : MonoBehaviour
{
    [Header("Merge Settings")]
    [SerializeField] private float mergeTime = .2f;

    [SerializeField] private float mergeExplodeRadius = 5f;
    [SerializeField] private Vector2 mergeExplodeForceRange;
    [SerializeField] private LayerMask explodeLayerMask;
    
    [Header("Pinata Tier Prefabs")]
    [SerializeField] private Pinata[] pinataTierPrefabs;
    
    // Event Delegates
    public delegate void OnPinataMerge(Pinata a, Pinata b, Pinata c);
    public event OnPinataMerge OnPinataMergeEvent;
    public delegate void OnPinataCollision(Pinata a, Pinata b, Collision2D collision);
    public event OnPinataCollision OnPinataCollisionEvent;
    
    
    public static PinataManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        OnPinataMergeEvent += PinataMergeBlast;
    }

    public void ProcessPinataCollision(Pinata a, Pinata b, Collision2D collision)
    {

        if(AlreadyMerging(a, b))
            return;
        else if (!CanMerge(a, b))
        {
            OnPinataCollisionEvent?.Invoke(a, b, collision);
            return;
        }
        
        TriggerMerge(a, b);
    }
    
    private bool AlreadyMerging(Pinata a, Pinata b)
    {
        return a.IsMerging || b.IsMerging;
    }

    private bool CanMerge(Pinata a, Pinata b)
    {
        bool tiersEqual = a.Tier == b.Tier;
        
        return tiersEqual;
    }

    private void TriggerMerge(Pinata a, Pinata b)
    {
        a.TriggerMerge();
        b.TriggerMerge();
        StartCoroutine(MergePinatas(a, b));
    }
    
    private IEnumerator MergePinatas(Pinata a, Pinata b)
    {
        float elapsedTime = 0f;

        bool aIsFaster = a.ShootableRigidbody.velocity.magnitude > b.ShootableRigidbody.velocity.magnitude;
        Pinata pinataToMove = aIsFaster ? a : b;
        Pinata otherPinata = aIsFaster ? b : a;
        
        Vector3 initialPosition = pinataToMove.transform.position;
        Quaternion initialRotation = pinataToMove.transform.rotation;

        while (elapsedTime < mergeTime)
        {
            Vector3 mergePosition = otherPinata.transform.position;
            Quaternion mergeRotation = otherPinata.transform.rotation;

            pinataToMove.transform.position = Vector3.Lerp(initialPosition, mergePosition, elapsedTime / mergeTime);
            pinataToMove.transform.rotation = Quaternion.Lerp(initialRotation, mergeRotation, elapsedTime / mergeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        Vector3 finalPosition = otherPinata.transform.position;
        Quaternion finalRotation = otherPinata.transform.rotation;

        Transform parent = a.transform.parent;
        
        ObjectPoolingManager.Instance.ReturnToPool(a);
        ObjectPoolingManager.Instance.ReturnToPool(b);

        int nextTierIndex = Mathf.Clamp(Mathf.Max(a.Tier, b.Tier) + 1, 0, pinataTierPrefabs.Length - 1);
        Pinata c = ObjectPoolingManager.Instance.SpawnFromPool(pinataTierPrefabs[nextTierIndex].tag, finalPosition, finalRotation).GetComponent<Pinata>();
        c.transform.SetParent(parent);
        
        OnPinataMergeEvent?.Invoke(a, b, c);
    }

    private void PinataMergeBlast(Pinata a, Pinata b, Pinata c)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(c.transform.position, mergeExplodeRadius, explodeLayerMask);

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != c.gameObject)
            {
                Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 difference = collider.transform.position - c.transform.position;
                    
                    Vector2 direction = difference.normalized;
                    float distance = difference.magnitude;

                    float t = 1f - (distance / mergeExplodeRadius);
                    float explodeForce = Mathf.Lerp(mergeExplodeForceRange.x, mergeExplodeForceRange.y, t);
                    
                    rb.AddForce(direction * explodeForce, ForceMode2D.Impulse);
                }
            }
        }
    }
    
    public void Reset()
    {
        foreach (Pinata pinata in FindObjectsOfType<Pinata>())
            ObjectPoolingManager.Instance.ReturnToPool(pinata);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, mergeExplodeRadius);
    }
}