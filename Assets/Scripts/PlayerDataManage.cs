using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int coins;           // 金币数量

    public int attackLevel;     // 普通攻击等级
    public int skill1Level;     // 技能1等级
    public int skill2Level;     // 技能2等级
    public int skill3Level;     // 技能3等级

    public int gameLevel;       // 游戏胜利关卡数

    public int deathCount;      // 死亡次数
    public int enemyKillCount;  // 杀敌数
}

public class PlayerDataManage
{
    public static PlayerDataManage Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerDataManage();
            }
            return instance;
        }
    }
    private static PlayerDataManage instance;

    public PlayerData playerData;
    private string saveFilePath;

    private PlayerDataManage()
    {
        // 初始化代码
        saveFilePath = Application.streamingAssetsPath + "/GameData/PlayerData.json";
        LoadData();
    }

    public void LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            playerData = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            playerData = new PlayerData()
            {
                coins = 0,
                attackLevel = 0,
                skill1Level = 0,
                skill2Level = 0,
                skill3Level = 0,
                gameLevel = 0,
                deathCount = 0,
                enemyKillCount = 0,
            };
            SaveData();
        }
    }

    // 保存数据
    public void SaveData()
    {
        string json = JsonUtility.ToJson(playerData);
        File.WriteAllText(saveFilePath, json);
    }
}
