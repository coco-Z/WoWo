using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManage : MonoBehaviour
{
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
        int sceneCount = SceneManager.sceneCountInBuildSettings; // 获取已打包的场景总数
        string[] sceneFiles = new string[sceneCount];

        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i); // 获取场景路径
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath); // 提取场景名称
            sceneFiles[i] = sceneName;
        }

        // 提取文件名（不包括扩展名），并筛选出以"Level"开头的文件
        List<string> levelSceneNames = new List<string>();
        foreach (string file in sceneFiles)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
            if (fileNameWithoutExtension.StartsWith("Level"))
            {
                levelSceneNames.Add(fileNameWithoutExtension);
            }
        }

        m_levels = levelSceneNames.ToArray();
        yield return null;
    }

    public string[] GetLevels()
    {
        return m_levels;
    }
}
