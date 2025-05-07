using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEditor;
using UnityEngine.ResourceManagement.ResourceProviders;
using XLua;

public class GameManage : MonoBehaviour
{
    public LuaEnv luaEnv;

    private static GameManage instance;
    public static GameManage Instance => instance;

    private string[] m_levels;      // 关卡名字

    public bool isMask;
    public int currentLevel;

    private void Awake()
    {
        if (instance == null)
        { 
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 在第一帧更新前调用启动
    void Start()
    {
        luaEnv = new LuaEnv();

        GameObject bloodDeductionEffect = Resources.Load<GameObject>(@"Prefabs/Blood Deduction Effect");
        ObjectPoolManager.Instance.CreatePool(bloodDeductionEffect.name, bloodDeductionEffect, 20);

        StartCoroutine(LoardLevels());

        isMask = false;
    }

    // 每帧调用一次更新
    void Update()
    {
        
    }

    /// <summary>
    /// 加载关卡场景名字
    /// </summary>
    /// <returns></returns>
    IEnumerator LoardLevels()
    {
        List<string> sceneNames = new List<string>();

        // 加载所有带有指定标签的资源位置
        AsyncOperationHandle<IList<IResourceLocation>> handle = Addressables.LoadResourceLocationsAsync("GameLevel");
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            IList<IResourceLocation> locations = handle.Result;

            foreach (IResourceLocation location in locations)
            {
                // 检查资源类型是否为场景
                Debug.Log(location.ResourceType.ToString());
                if (location.ResourceType == typeof(SceneInstance))
                {
                    // 获取场景的名称
                    string sceneName = location.PrimaryKey.ToString();
                    Debug.Log("Found GameLevel Scene: " + sceneName);

                    // 将场景名称添加到列表中
                    sceneNames.Add(sceneName);
                }
            }

            // 打印保存的场景名称列表
            Debug.Log("All GameLevel Scenes: " + string.Join(", ", sceneNames));
        }
        else
        {
            Debug.LogError("Failed to load GameLevel scenes");
        }

        m_levels = sceneNames.ToArray();

        // 释放操作句柄
        Addressables.Release(handle);
    }

    /// <summary>
    /// 获取所有关卡名称
    /// </summary>
    /// <returns></returns>
    public string[] GetLevels()
    {
        return m_levels;
    }
}
