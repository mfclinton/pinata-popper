using UnityEngine;
using System;

public abstract class APoolable : MonoBehaviour
{
    protected bool inPool = false;
    public bool InPool => inPool;
    
    public event Action OnSpawnedEvent;
    public event Action OnReturnedEvent;

    protected abstract void OnSpawned();
    protected abstract void OnReturned();

    public void Spawn()
    {
        OnSpawned();
        OnSpawnedEvent?.Invoke();
    }

    public void Return()
    {
        OnReturned();
        OnReturnedEvent?.Invoke();
    }
    
    public void SetInPool(bool inPool)
    {
        this.inPool = inPool;
    }
}