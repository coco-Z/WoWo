using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using XLua;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

/// <summary>
/// 血量事件委托
/// </summary>
public class HealthManager
{
    // 定义一个委托类型
    public delegate void HealthChangedEvent(int currentHealth);

    // 定义一个事件
    public static event HealthChangedEvent OnHealthChanged;

    // 触发事件的方法
    public static void TriggerHealthChanged(int currentHealth)
    {
        // 检查是否有订阅者
        if (OnHealthChanged != null)
        {
            OnHealthChanged(currentHealth);
        }
    }
}

public class MPManager
{
    public delegate void MPChangedEvent(int currentMP);
    public static event MPChangedEvent OnMPChangeEvent;
    public static void TriggerMPChanged(int currentMP)
    {
        if (OnMPChangeEvent != null)
        {
            OnMPChangeEvent(currentMP);
        }
    }
}

[LuaCallCSharp]
[Hotfix]
public class Player : MonoBehaviour
{
    private LuaTable luaTable;
    private Action<Player> startLua;
    private Action<Player> updatatLua;

    public IPlayerState playerState;

    [HideInInspector]
    public float moveSpeed = 5;
    [HideInInspector]
    public float jumpForce = 10;
    [HideInInspector]
    public bool isNotOnGround;
    [HideInInspector]
    public bool isCollide;

    [HideInInspector]
    public float comboTimeLimit = 0.8f;
    [HideInInspector]
    public float currentComboTime = 0f;
    [HideInInspector]
    public float comboWaitTime = 0.3f;
    [HideInInspector]
    public float currentComboWaitTime = 0f;
    [HideInInspector]
    public int comboIndex = 0;

    public int HP;
    public int MP;
    public bool isPause;

    public Rigidbody2D m_rb;
    private SpriteRenderer m_sprite;
    public BoxCollider2D m_boxCollider;
    private TrailRenderer m_trailRenderer;

    private PhysicsMaterial2D normalMaterial;       // 正常的物理材质
    private PhysicsMaterial2D noFrictionMaterial;   // 没有阻力的物理材质

    private void Awake()
    {
        // 为每个脚本设置一个独立的脚本域，可一定程度上防止脚本间全局变量、函数冲突
        luaTable = GameManage.Instance.luaEnv.NewTable();

        // 设置其元表的 __index, 使其能够访问全局变量
        using (LuaTable meta = GameManage.Instance.luaEnv.NewTable())
        {
            meta.Set("__index", GameManage.Instance.luaEnv.Global);
            luaTable.SetMetaTable(meta);
        }

        var handle = Addressables.LoadAssetAsync<TextAsset>("PlayerLua.lua");
        handle.WaitForCompletion();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // 获取 Lua 脚本内容
            TextAsset luaScript = handle.Result;
            // 执行 Lua 脚本
            GameManage.Instance.luaEnv.DoString(luaScript.text, luaScript.name, luaTable);

            startLua = luaTable.Get<Action<Player>>("Start");
            updatatLua = luaTable.Get<Action<Player>>("Updata");
        }
        else
        {
            Debug.Log("Player.lua 加载失败");
        }
    }

    // 在第一帧更新前调用启动
    void Start()
    {
        if (startLua != null)
        {
            startLua(this);
        }

        ChangeState(new PlayerStateIdle());

        m_rb = GetComponent<Rigidbody2D>();
        m_sprite = transform.Find("Player Sprite").GetComponent<SpriteRenderer>();
        m_boxCollider = GetComponent<BoxCollider2D>();
        m_trailRenderer = GetComponent<TrailRenderer>();

        HealthManager.TriggerHealthChanged(HP);
        MPManager.TriggerMPChanged(MP);

        normalMaterial = Resources.Load<PhysicsMaterial2D>(@"Materials/Normal Material");
        noFrictionMaterial = Resources.Load<PhysicsMaterial2D>(@"Materials/No Friction Material");

        StartCoroutine(RecoverMP());
        PauseChange.OnPauseChanged += Pause;
    }

    // 每帧调用一次更新
    void Update()
    {
        if (updatatLua != null)
        {
            updatatLua(this);
        }

    }

    /// <summary>
    /// 检测是否落下
    /// </summary>
    private void CheckOnGround()
    {
        isNotOnGround = false;
        if (m_rb.velocity.y > 0.2f || m_rb.velocity.y < -0.2f)
        {
            isNotOnGround = true;
            if (!(playerState is PlayerStateJump))
            {
                ChangeState(new PlayerStateJump());
            }
        }

        // 切换材质
        if (isNotOnGround)
        {
            if (m_rb.sharedMaterial != noFrictionMaterial)
            {
                m_rb.sharedMaterial = noFrictionMaterial;
            }

            if (m_boxCollider.sharedMaterial != noFrictionMaterial)
            {
                m_boxCollider.sharedMaterial = noFrictionMaterial;
            }
        }
        else
        {
            if (m_rb.sharedMaterial != normalMaterial)
            {
                m_rb.sharedMaterial = normalMaterial;
            }

            if (m_boxCollider.sharedMaterial != normalMaterial)
            {
                m_boxCollider.sharedMaterial = normalMaterial;
            }
        }
    }

    private void OnDisable()
    {
        PauseChange.OnPauseChanged -= Pause;
    }

    private void OnDestroy()
    {
        // 销毁 Lua 环境
        luaTable = null;
        startLua = null;
        updatatLua = null;
    }

    private void Pause(bool pause)
    {
        isPause = pause;
    }

    /// <summary>
    /// 改变状态
    /// </summary>
    /// <param name="state">新的状态</param>
    public void ChangeState(IPlayerState state)
    {
        if (playerState != null)
        {
            playerState.OnExit(this);
        }
        playerState = state;
        playerState.OnEnter(this);
    }

    /// <summary>
    /// 判断朝向
    /// </summary>
    /// <returns>是否朝右</returns>
    public bool IsRight()
    {
        return !m_sprite.flipX;
    }

    public void Harmer(int value)
    {
        var luaHuarmer = luaTable.Get<LuaFunction>("LuaHarmer");
        if (luaHuarmer != null)
        {
            luaHuarmer.Call(this, -value);
        }
    }

    public void ChangeHP(int changeValue)
    {
        HP = HP + changeValue;
        HP = Mathf.Max(HP, 0);
        HP = Mathf.Min(HP, 10);
        HealthManager.TriggerHealthChanged(HP);

        if (HP <= 0 && !(playerState is PlayerStateDead))
        {
            ChangeState(new PlayerStateDead());

            PlayerDataManage.Instance.playerData.deathCount++;
            PlayerDataManage.Instance.SaveData();
        }
    }

    public bool ChangeMP(int changeValue)
    {
        if (MP + changeValue < 0)
        {
            return false;
        }
        MP = MP + changeValue;
        MP = Mathf.Max(MP, 0);
        MP = Mathf.Min(MP, 5);
        MPManager.TriggerMPChanged(MP);

        return true;
    }

    public bool IsDead()
    {
        return HP <= 0;
    }

    public void SetTrail(bool isShow)
    {
        m_trailRenderer.enabled = isShow;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isCollide = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isCollide = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Gold")
        {
            Destroy(col.gameObject);
            PlayerDataManage.Instance.playerData.coins++;

            SoundManage.Player(SoundName.collect);
        }
    }

    IEnumerator RecoverMP()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            if (!IsDead())
            {
                ChangeMP(1);
            }
        }
    }
}
