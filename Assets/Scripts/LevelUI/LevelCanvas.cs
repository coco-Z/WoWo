using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseChange
{
    // 定义一个委托类型
    public delegate void PauseChangedEvent(bool isPause);

    // 定义一个事件
    public static event PauseChangedEvent OnPauseChanged;

    // 触发事件的方法
    public static void TriggerPauseChanged(bool isPause)
    {
        // 检查是否有订阅者
        if (OnPauseChanged != null)
        {
            OnPauseChanged(isPause);
        }
    }
}

public class LevelCanvas : MonoBehaviour
{
    private GameObject pausePanel;
    private Button pauseBtn;
    private Button backHomeBtn;
    private Button continueBtn;

    // 在第一帧更新前调用启动
    void Start()
    {
        pausePanel = transform.Find("PausePanel").gameObject;

        pauseBtn = transform.Find("PauseBtn").GetComponent<Button>();
        pauseBtn.onClick.AddListener(PauseGame);

        backHomeBtn = pausePanel.transform.Find("BackHome").GetComponent<Button>();
        backHomeBtn.onClick.AddListener(ClickBackHomeBtn);

        continueBtn = pausePanel.transform.Find("Continue").GetComponent<Button>();
        continueBtn.onClick.AddListener(ClickContinueBtn);

    }

    // 每帧调用一次更新
    void Update()
    {
        
    }

    private void PauseGame()
    {
        Debug.Log("游戏暂停");
        pausePanel.SetActive(true);
        PauseChange.TriggerPauseChanged(true);

        SoundManage.Player(SoundName.select);
    }

    private void ClickBackHomeBtn()
    {
        StartCoroutine(ChangeScene());
        SoundManage.Player(SoundName.select);
    }

    private void ClickContinueBtn() 
    {
        pausePanel.SetActive(false);
        PauseChange.TriggerPauseChanged(false);
        SoundManage.Player(SoundName.confirm);
    }

    IEnumerator ChangeScene()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(0);
        asyncOperation.allowSceneActivation = false;

        // 切换场景效果
        GameObject maskGO = transform.Find("Mask").gameObject;
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
        else
        {
            Debug.Log("没有找到Mask");
        }

        while (!moveEnd && !asyncOperation.isDone)
        {
            yield return null;
        }

        asyncOperation.allowSceneActivation = true;
    }
}
