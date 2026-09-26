using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AShootablesManager : MonoBehaviour
{
    [SerializeField] protected int startingShootablesQueueLength = 1;
    
    public int ShootablesQueueLength { get; protected set; }
    public LinkedList<AShootable> ShootablesQueue { get; protected set; }

    protected virtual void Start()
    {
        InitializeShootablesManager();
    }

    public void Reset()
    {
        if(ShootablesQueue == null)
            return;

        foreach (AShootable shootable in ShootablesQueue)
            ObjectPoolingManager.Instance.ReturnToPool(shootable);
        
        InitializeShootablesManager();
    }

    protected virtual void InitializeShootablesManager()
    {
        ShootablesQueue = new LinkedList<AShootable>();
        ShootablesQueueLength = startingShootablesQueueLength;
        UpdateShootablesQueue();
    }

    public void SetShootablesQueueLength(int length)
    {
        ShootablesQueueLength = length;
    }

    public abstract AShootable GenerateShootable();

    public void UpdateShootablesQueue()
    {
        int removeCount = ShootablesQueue.Count - ShootablesQueueLength;
        for (int i = 0; i < removeCount; i++)
        {
            AShootable shootable = ShootablesQueue.Last.Value;
            ObjectPoolingManager.Instance.ReturnToPool(shootable);
            ShootablesQueue.RemoveLast();
        }

        int addCount = ShootablesQueueLength - ShootablesQueue.Count;
        for (int i = 0; i < addCount; i++)
        {
            AShootable shootable = GenerateShootable();
            shootable.gameObject.SetActive(false);
            shootable.transform.SetParent(transform);
            ShootablesQueue.AddLast(shootable);
        }
        
        if(addCount > 0)
            PeekNextShootable().OnShootableLoaded();
    }

    public AShootable PeekNextShootable()
    {
        return ShootablesQueue.First.Value;
    }

    public AShootable PopNextShootable()
    {
        AShootable shootable = ShootablesQueue.First.Value;
        ShootablesQueue.RemoveFirst();
        shootable.gameObject.SetActive(true);
        
        UpdateShootablesQueue();
        
        return shootable;
    }
}
