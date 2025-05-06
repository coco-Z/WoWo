using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class LevelManage : MonoBehaviour
{
    private Player player;

    // 在第一帧更新前调用启动
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();

        if (GameManage.Instance.isMask)
        {
            PauseChange.TriggerPauseChanged(true);
            EntScene();
        }
    }

    // 每帧调用一次更新
    void Update()
    {

    }

    private void EntScene()
    {
        // 切换场景效果
        GameObject maskGO = GameObject.Find("LevelCanvas").transform.Find("Mask").gameObject;
        if (maskGO != null)
        {
            ;
            maskGO.SetActive(true);
            RectTransform canvasRectTransform = maskGO.transform.parent.GetComponent<RectTransform>();
            RectTransform maskGORectTransform = maskGO.transform.GetComponent<RectTransform>();

            // 设置Image的锚点为Stretch模式
            maskGORectTransform.anchorMin = new Vector2(0, 0);
            maskGORectTransform.anchorMax = new Vector2(1, 1);
            maskGORectTransform.pivot = new Vector2(0.5f, 0.5f); // 设置中心点为中间

            // 设置Image的大小为Canvas的大小
            maskGORectTransform.sizeDelta = new Vector2(0, 0); // sizeDelta设置为0，让其完全拉伸

            Vector3 targetPosition = maskGORectTransform.anchoredPosition + new Vector2(0, Screen.height);

            maskGORectTransform.DOAnchorPos(targetPosition, 0.4f).SetEase(Ease.Linear).OnComplete(() => { 
                maskGO.SetActive(false);
                PauseChange.TriggerPauseChanged(false);
            });

            GameManage.Instance.isMask = false;
        }
    }

    IEnumerator ChangeScene(string sceneName)
    {
        // 切换场景效果
        GameObject maskGO = GameObject.Find("LevelCanvas").transform.Find("Mask").gameObject;
        bool moveEnd = true;
        if (maskGO != null)
        {
            moveEnd = false;
            maskGO.SetActive(true);
            RectTransform canvasRectTransform = maskGO.transform.parent.GetComponent<RectTransform>();
            RectTransform maskGORectTransform = maskGO.transform.GetComponent<RectTransform>();

            // 设置Image的锚点为Stretch模式
            maskGORectTransform.anchorMin = new Vector2(0, 0);
            maskGORectTransform.anchorMax = new Vector2(1, 1);
            maskGORectTransform.pivot = new Vector2(0.5f, 0.5f); // 设置中心点为中间

            // 设置Image的大小为Canvas的大小
            maskGORectTransform.sizeDelta = new Vector2(0, 0); // sizeDelta设置为0，让其完全拉伸

            maskGORectTransform.anchoredPosition = new Vector2(0, Screen.height);

            Vector3 targetPosition = new Vector2(0, 0);
            maskGORectTransform.DOAnchorPos(targetPosition, 0.4f).SetEase(Ease.Linear).OnComplete(() => { moveEnd = true; });

            GameManage.Instance.isMask = true;
        }

        AsyncOperationHandle<SceneInstance> sceneHandle = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single, false);
        yield return sceneHandle;

        while (!moveEnd)
        {
            yield return null;
        }

        sceneHandle.Result.ActivateAsync();
    }

    public void CheckpointTriggered(GameObject checkpoint)
    {
        if (checkpoint.name.Equals("Win"))
        {
            Winning();
            return;
        }
        if (checkpoint.name.Equals("Kill"))
        {
            player.Harmer(10);
            return;
        }

        GameObject goStart = GameObject.Find(checkpoint.name + "_Start");
        if (goStart != null) 
        {
            GameObject goEnd = GameObject.Find(checkpoint.name + "_End");
            if (goEnd != null) 
            {
                float time = ExtractNumber(checkpoint.name);
                if (time <= 0)
                {
                    goStart.transform.DOMove(goEnd.transform.position, 1);
                }
                else
                {
                    goStart.transform.DOMove(goEnd.transform.position, time);
                }
            }
        }
        
    }

    private float ExtractNumber(string input)
    {
        // 找到最后一个下划线的位置
        int underscoreIndex = input.LastIndexOf('_');
        if (underscoreIndex == -1)
        {
            return 0;
        }

        // 提取下划线后面的子字符串
        string numberPart = input.Substring(underscoreIndex + 1);

        // 尝试将子字符串转换为float
        if (float.TryParse(numberPart, NumberStyles.Float, CultureInfo.InvariantCulture, out float number))
        {
            return number;
        }
        else
        {
            return 0;
        }
    }

/// <summary>
/// 胜利
/// </summary>
    protected void Winning()
    {
        if (player.IsDead() || GameObject.FindGameObjectWithTag("Enemy"))
        {
            return;
        }

        PauseChange.TriggerPauseChanged(true);

        if (PlayerDataManage.Instance.playerData.gameLevel < GameManage.Instance.currentLevel)
        {
            PlayerDataManage.Instance.playerData.gameLevel = GameManage.Instance.currentLevel;
            PlayerDataManage.Instance.SaveData();
        }

        if (GameManage.Instance.GetLevels().Length <= GameManage.Instance.currentLevel)
        {
            StartCoroutine(ChangeScene("GameStart"));
            return;
        }

        StartCoroutine(ChangeScene("Level" + (GameManage.Instance.currentLevel + 1)));

        GameManage.Instance.currentLevel += 1;
    }
}
