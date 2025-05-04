using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BloodDeductionEffect : MonoBehaviour
{
    private float waitTime = 1;
    private float currentTime = 0;

    // 每帧调用一次更新
    void Update()
    {
        if (currentTime < 0) 
        {
            currentTime = waitTime;
            ObjectPoolManager.Instance.ReturnObjectToPool("Blood Deduction Effect", gameObject);
            return;
        }
        currentTime -= Time.deltaTime;
    }

    public void ShowBloodDeduction(Vector3 startPosition, Vector3 endPosition, int value)
    { 
        transform.position = startPosition;
        transform.DOMove(endPosition, waitTime/2);

        TextMeshProUGUI textMeshProUGUI = transform.GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = value.ToString();

        currentTime = waitTime;
    }
}
