using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectLevelPanel : MonoBehaviour
{
    public GameObject content;
    public GameObject levelBtnPrefab;

    // 在第一帧更新前调用启动
    void Start()
    {
        
    }

    private void OnEnable()
    {
        Debug.Log("展示");
        StartCoroutine(LoadLevelBtns());
    }

    // 每帧调用一次更新
    void Update()
    {

    }

    private void OnDisable()
    {
        for (int i = 0; i < content.transform.childCount; i++) 
        {
            GameObject go = content.transform.GetChild(i).gameObject;
            Destroy(go);
        }
    }

    /// <summary>
    /// 点击关闭面板事件
    /// </summary>
    public void ClickCloseBtn()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 选择关卡回调
    /// </summary>
    /// <param name="level">按钮数字</param>
    public void SelectLevel(int level)
    {
        GameManage.Instance.currentLevel = level;
        StartCoroutine(ChangeScene("Level" + level));
        SoundManage.Player(SoundName.confirm);
    }

    /// <summary>
    /// 异步加载关卡选择按钮
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadLevelBtns()
    {
        string[] levels = GameManage.Instance.GetLevels();
        for (int i = 0; i < levels.Length; ++i)
        {
            GameObject levelBtn = GameObject.Instantiate(levelBtnPrefab);
            levelBtn.GetComponent<RectTransform>().SetParent(content.transform);

            TextMeshProUGUI textMeshProUGUI = levelBtn.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            textMeshProUGUI.text = (i + 1).ToString();

            Button button = levelBtn.GetComponent<Button>();
            int level = i + 1;
            button.onClick.AddListener(() => SelectLevel(level));
            button.interactable = false;

            GameObject lockImg = levelBtn.transform.Find("Lock").gameObject;
            lockImg.SetActive(true);

            // 判断是否解锁
            int gameLevel = PlayerDataManage.Instance.playerData.gameLevel;
            if (i < gameLevel)
            {
                lockImg = levelBtn.transform.Find("Lock").gameObject;
                lockImg.SetActive(false);

                Image levelImg = levelBtn.GetComponent<Image>();
                levelImg.color = new Color32(150,255,150, 255);

                button.interactable = true;
            }
            else if (i == gameLevel)
            {
                lockImg = levelBtn.transform.Find("Lock").gameObject;
                lockImg.SetActive(false);
                button.interactable = true;
            }

        }

        yield return null;
    }

    /// <summary>
    /// 异步切换场景
    /// </summary>
    /// <param name="sceneName">场景名</param>
    /// <returns></returns>
    IEnumerator ChangeScene(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        // 切换场景效果
        GameObject maskGO = GameObject.Find("Canvas").transform.Find("Mask").gameObject;
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
