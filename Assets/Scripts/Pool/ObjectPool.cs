using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private string poolName;        // 对象池名字
    private GameObject prefab;      // 预制体
    private Queue<GameObject> pool = new Queue<GameObject>();

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="poolName">对象池名字</param>
    /// <param name="prefab">预制体</param>
    /// <param name="poolSize">初始大小</param>
    public ObjectPool(string poolName, GameObject prefab, int poolSize)
    {
        this.poolName = poolName;
        this.prefab = prefab;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// 获取对象
    /// </summary>
    /// <returns></returns>
    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();

            if (obj == null)
            {
                obj = GameObject.Instantiate(prefab);
            }

            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = GameObject.Instantiate(prefab);
            obj.SetActive(true);
            return obj;
        }
    }

    /// <summary>
    /// 回收对象
    /// </summary>
    /// <param name="obj">要回收的对象</param>
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
