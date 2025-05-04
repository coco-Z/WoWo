using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundName
{
    public static string select = "Select 1";
    public static string jump = "Jump 1";
    public static string hit = "Hit damage 1";
    public static string collect = "Fruit collect 1";
    public static string confirm = "Confirm 1";
    public static string skill3 = "Skills";
    public static string playerHit = "Playerhurt";
    public static string playerFire = "power_up";
}

public class SoundManage : MonoBehaviour
{
    private static SoundManage _instance;

    public static SoundManage Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SoundManage>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(SoundManage).Name;
                    _instance = obj.AddComponent<SoundManage>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }

    private static AudioSource _audioSource;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // 在第一帧更新前调用启动
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // 每帧调用一次更新
    void Update()
    {
        
    }

    public static void Player(string name)
    { 
        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + name);
        _audioSource.clip = clip;
        _audioSource.Play();
    }
}
