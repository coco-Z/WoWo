using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPUIController : MonoBehaviour
{
    public Sprite[] HPSprites;
    public GameObject[] HPImages;

    private void OnEnable()
    {
        HealthManager.OnHealthChanged += ChangeHP;
    }

    // 在第一帧更新前调用启动
    void Start()
    {
        
    }

    private void OnDisable()
    {
        HealthManager.OnHealthChanged -= ChangeHP;
    }

    private void ChangeHP(int value)
    {
        foreach (GameObject gameObject in HPImages)
        {
            gameObject.GetComponent<Image>().sprite = HPSprites[2];
        }

        int full = value / 2;
        for (int i = 0; i < full; ++i)
        {
            HPImages[i].GetComponent<Image>().sprite = HPSprites[0];
        }

        int half = value % 2;
        if (half != 0)
        {
            HPImages[full].GetComponent<Image>().sprite = HPSprites[1];
        }
    }
}
