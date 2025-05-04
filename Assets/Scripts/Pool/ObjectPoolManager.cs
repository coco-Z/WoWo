using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    private static ObjectPoolManager instance;
    public static ObjectPoolManager Instance => instance;

    private Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    // 创建对象池
    public void CreatePool(string poolName, GameObject prefab, int poolSize)
    {
        if (!pools.ContainsKey(poolName))
        {
            pools.Add(poolName, new ObjectPool(poolName, prefab, poolSize));
        }
    }

    // 从对象池中获取对象
    public GameObject GetPooledObject(string poolName)
    {
        if (pools.ContainsKey(poolName))
        {
            return pools[poolName].GetObject();
        }
        return null;
    }

    // 将对象回收到对象池
    public void ReturnObjectToPool(string poolName, GameObject obj)
    {
        if (pools.ContainsKey(poolName))
        {
            pools[poolName].ReturnObject(obj);
        }
    }
}
