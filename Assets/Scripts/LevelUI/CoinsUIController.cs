using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinsUIController : MonoBehaviour
{
    public TextMeshProUGUI countText;

    // 在第一帧更新前调用启动
    void Start()
    {
        
    }

    // 每帧调用一次更新
    void Update()
    {
        countText.text = PlayerDataManage.Instance.playerData.coins.ToString();
    }
}
