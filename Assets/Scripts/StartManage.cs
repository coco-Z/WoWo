using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StartManage : MonoBehaviour
{
    public GameObject selectLevelPanal;
    public GameObject infoPanal;

    // 在第一帧更新前调用启动
    void Start()
    {
        if (GameManage.Instance.isMask)
        {
            EntScene();
        }
    }

    // 每帧调用一次更新
    void Update()
    {
        
    }

    public void StartBtnAction()
    { 
        selectLevelPanal.SetActive(true);
        SoundManage.Player(SoundName.select);
    }

    public void OnClickInfo()
    {
        infoPanal.SetActive(true);
        SoundManage.Player(SoundName.select);
    }

    private void EntScene()
    {
        // 切换场景效果
        GameObject maskGO = GameObject.Find("Canvas").transform.Find("Mask").gameObject;
        if (maskGO != null)
        {;
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

            maskGORectTransform.DOAnchorPos(targetPosition, 0.4f).SetEase(Ease.Linear).OnComplete(() => { maskGO.SetActive(false);});

            GameManage.Instance.isMask = false;
        }
    }

}
