using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MPUIController : MonoBehaviour
{
    public Image[] MPImages;

    private void OnEnable()
    {
        MPManager.OnMPChangeEvent += ChangeMP;
    }

    // 在第一帧更新前调用启动
    void Start()
    {
        
    }

    // 每帧调用一次更新
    void Update()
    {
        
    }

    private void OnDisable()
    {
        MPManager.OnMPChangeEvent -= ChangeMP;
    }

    private void ChangeMP(int value)
    {
        foreach (Image image in MPImages)
        {
            image.color = new Color(0, 0, 0, 1);
        }

        for (int i = 0; i < value; ++i)
        {
            MPImages[i].color = new Color(255, 255, 255, 1);
        }
    }
}
