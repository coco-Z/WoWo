using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public TextMeshProUGUI attack;
    public TextMeshProUGUI skill1;
    public TextMeshProUGUI skill2;
    public TextMeshProUGUI skill3;
    public TextMeshProUGUI killEnemyCount;
    public TextMeshProUGUI deathCount;
    public TextMeshProUGUI goldICount;

    public TextMeshProUGUI price;

    public Toggle toggle1;
    public Toggle toggle2;
    public Toggle toggle3;
    public Toggle toggle4;

    public Button upgradeBtn;
    public Button closeBtn;


    // 在第一帧更新前调用启动
    void Start()
    {

    }

    private void OnEnable()
    {
        LoadingData();
    }

    // 每帧调用一次更新
    void Update()
    {

    }

    private void LoadingData()
    {
        attack.text = (PlayerDataManage.Instance.playerData.attackLevel * 100 + 100).ToString();
        skill1.text = PlayerDataManage.Instance.playerData.skill1Level.ToString();
        skill2.text = PlayerDataManage.Instance.playerData.skill2Level.ToString();
        skill3.text = PlayerDataManage.Instance.playerData.skill3Level.ToString();
        killEnemyCount.text = PlayerDataManage.Instance.playerData.enemyKillCount.ToString();
        deathCount.text = PlayerDataManage.Instance.playerData.deathCount.ToString();
        goldICount.text = PlayerDataManage.Instance.playerData.coins.ToString();

        SelectChange();
    }

    public void OnSelectChange(bool value)
    {
        SoundManage.Player(SoundName.select);
        SelectChange();
    }

    private void SelectChange()
    {
        int p = 0;

        if (toggle1.isOn)
        {
            p = PlayerDataManage.Instance.playerData.attackLevel * 50 + 50;
        }
        else if (toggle2.isOn)
        {
            p = PlayerDataManage.Instance.playerData.skill1Level * 80 + 80;
        }
        else if (toggle3.isOn)
        {
            p = PlayerDataManage.Instance.playerData.skill2Level * 100 + 100;
        }
        else if (toggle4.isOn)
        {
            p = PlayerDataManage.Instance.playerData.skill3Level * 120 + 120;
        }

        price.text = p.ToString();

        if (p <= PlayerDataManage.Instance.playerData.coins)
        {
            upgradeBtn.interactable = true;
        }
        else
        {
            upgradeBtn.interactable = false;
        }
    }

    public void OnClickBtn()
    {
        SoundManage.Player(SoundName.confirm);

        int p = 0;

        if (toggle1.isOn)
        {
            p = PlayerDataManage.Instance.playerData.attackLevel * 50 + 50;
            PlayerDataManage.Instance.playerData.attackLevel++;
        }
        else if (toggle2.isOn)
        {
            p = PlayerDataManage.Instance.playerData.skill1Level * 80 + 80;
            PlayerDataManage.Instance.playerData.skill1Level++;
        }
        else if (toggle3.isOn)
        {
            p = PlayerDataManage.Instance.playerData.skill2Level * 100 + 100;
            PlayerDataManage.Instance.playerData.skill2Level++;
        }
        else if (toggle4.isOn)
        {
            p = PlayerDataManage.Instance.playerData.skill3Level * 120 + 120;
            PlayerDataManage.Instance.playerData.skill3Level++;
        }

        PlayerDataManage.Instance.playerData.coins -= p;
        PlayerDataManage.Instance.SaveData();

        LoadingData();
    }

    public void OnclickCloseBtn()
    {
        gameObject.SetActive(false);
    }
}
