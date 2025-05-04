using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

public class Player : MonoBehaviour
{
    private IPlayerState playerState;

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

    private int HP;
    private int MP;
    private bool isPause;

    private Rigidbody2D m_rb;
    private SpriteRenderer m_sprite;
    private BoxCollider2D m_boxCollider;
    private TrailRenderer m_trailRenderer;

    private PhysicsMaterial2D normalMaterial;       // 正常的物理材质
    private PhysicsMaterial2D noFrictionMaterial;   // 没有阻力的物理材质

    // 在第一帧更新前调用启动
    void Start()
    {
        ChangeState(new PlayerStateIdle());

        m_rb = GetComponent<Rigidbody2D>();
        m_sprite = transform.Find("Player Sprite").GetComponent<SpriteRenderer>();
        m_boxCollider = GetComponent<BoxCollider2D>();
        m_trailRenderer = GetComponent<TrailRenderer>();

        moveSpeed = 5;
        jumpForce = 12.5f;

        HP = 10;
        MP = 5;
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
        if (isPause)
        {
            return;
        }

        currentComboTime -= Time.deltaTime;
        currentComboTime = Mathf.Max(currentComboTime, -1);
        currentComboWaitTime -= Time.deltaTime;
        currentComboWaitTime = Mathf.Max(currentComboWaitTime, -1);

        if (playerState != null)
        {
            playerState.HandleInput(this);
            playerState.Update(this);
        }

        if (IsDead())
        {
            return;
        }

        // 判断是否在下落
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
            if (m_rb.sharedMaterial == normalMaterial)
            {
                m_rb.sharedMaterial = noFrictionMaterial;
            }

            if (m_boxCollider.sharedMaterial == normalMaterial)
            {
                m_boxCollider.sharedMaterial = noFrictionMaterial;
            }
        }
        else
        {
            if (m_rb.sharedMaterial == noFrictionMaterial)
            {
                m_rb.sharedMaterial = normalMaterial;
            }

            if (m_boxCollider.sharedMaterial == noFrictionMaterial)
            {
                m_boxCollider.sharedMaterial = normalMaterial;
            }
        }
    }

    private void OnDisable()
    {
        PauseChange.OnPauseChanged -= Pause;
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
        if (IsDead()) 
        {
            return;
        }

        SoundManage.Player(SoundName.playerHit);

        ChangeHP(-value);
        if (IsDead()) 
        {
            return; 
        }

        ChangeState(new PlayerStateHarmed());
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
