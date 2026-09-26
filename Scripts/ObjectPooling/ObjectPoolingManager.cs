using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ObjectPoolingManager : MonoBehaviour
{

    [Serializable]
    public class Pool
    {
        public APoolable Prefab;
        public string Tag => Prefab.tag;
        public int Size;
    }

    [SerializeField] private int increasePoolSize = 5;
    [SerializeField] private List<Pool> pools;

    private Dictionary<string, Pool> poolDataDictionary;
    private Dictionary<string, Queue<APoolable>> poolDictionary;

    public static ObjectPoolingManager Instance;
    
    private void Awake()
    {
        Instance = this;
        InitializePools();
    }


    private void InitializePools()
    {
        poolDataDictionary = new Dictionary<string, Pool>();
        poolDictionary = new Dictionary<string, Queue<APoolable>>();

        foreach (Pool pool in pools)
        {
            poolDataDictionary.Add(pool.Tag, pool);
            poolDictionary.Add(pool.Tag, new Queue<APoolable>());
            ExpandPool(pool.Tag, pool.Size);
        }
    }


    private void ExpandPool(string tag, int amount)
    {
        Pool pool = poolDataDictionary[tag];
        
        for (int i = 0; i < amount; i++)
        {
            APoolable obj = Instantiate(pool.Prefab);
            AddToPool(obj);
        }
    }


    private void AddToPool(APoolable objectToAdd)
    {
        objectToAdd.SetInPool(true);
        objectToAdd.gameObject.SetActive(false);
        objectToAdd.transform.SetParent(transform);
        poolDictionary[objectToAdd.tag].Enqueue(objectToAdd);
    }
    
    
    public APoolable SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        APoolable objectToSpawn = null;
        if (poolDictionary[tag].Count == 0)
            ExpandPool(tag, increasePoolSize);

        objectToSpawn = poolDictionary[tag].Dequeue();
        
        objectToSpawn.gameObject.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.transform.SetParent(parent);

        objectToSpawn.SetInPool(false);
        objectToSpawn.Spawn();

        return objectToSpawn;
    }

    
    public bool ReturnToPool(APoolable objectToReturn)
    {
        if(objectToReturn.InPool)
            return false;
        
        string tag = objectToReturn.tag;
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return false;
        }

        AddToPool(objectToReturn);
        objectToReturn.Return();
        
        return true;
    }
    
}
