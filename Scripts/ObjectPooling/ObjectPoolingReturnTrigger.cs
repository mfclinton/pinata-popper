using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Collider2D))]
public class ObjectPoolingReturnTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        APoolable poolable = other.GetComponent<APoolable>();
        if (poolable != null)
            ObjectPoolingManager.Instance.ReturnToPool(poolable);
    }
}