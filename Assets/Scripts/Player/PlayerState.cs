using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 玩家状态接口
/// </summary>
public interface IPlayerState
{
    void HandleInput(Player player);    // 处理玩家的输入
    void Update(Player player);         // 在每一帧更新玩家的状态
    void OnEnter(Player player);        // 进入该状态时执行的操作
    void OnExit(Player player);         // 退出该状态时执行的操作

}

/// <summary>
/// Idle状态
/// </summary>
public class PlayerStateIdle : IPlayerState
{
    private Animator m_animator;

    public void HandleInput(Player player)
    {
        float moveValue = Input.GetAxisRaw("Horizontal");
        if (moveValue != 0)
        {
            player.ChangeState(new PlayerStateMove());
            return;
        }

        if (Input.GetButtonDown("Jump"))
        {
            player.ChangeState(new PlayerStateJump());
            return;
        }

        if (Input.GetButtonDown("Attack"))
        {
            player.ChangeState(new PlayerStateAttack());
            return;
        }

        if (Input.GetButtonDown("Skill1"))
        {
            if (!player.ChangeMP(-1))
            {
                return;
            }
            player.ChangeState(new PlayerStateSkill1());
            return;
        }

        if (Input.GetButtonDown("Skill2"))
        {
            if (!player.ChangeMP(-1))
            {
                return;
            }
            player.ChangeState(new PlayerStateSkill2());
            return;
        }

        if (Input.GetButtonDown("Skill3"))
        {
            if (!player.ChangeMP(-1))
            {
                return;
            }
            player.ChangeState(new PlayerStateSkill3());
            return;
        }
    }

    public void OnEnter(Player player)
    {
        Debug.Log("进入 Idle 状态");
        m_animator = player.transform.Find("Player Sprite").GetComponent<Animator>();
        m_animator.Play("PlayerIdle");
    }

    public void OnExit(Player player)
    {

    }

    public void Update(Player player)
    {

    }
}

/// <summary>
/// 移动状态
/// </summary>
public class PlayerStateMove : IPlayerState
{
    private Rigidbody2D m_rb;
    private SpriteRenderer m_sprite;
    private Animator m_animator;

    public void HandleInput(Player player)
    {
        if (Input.GetButtonDown("Jump"))
        {
            player.ChangeState(new PlayerStateJump());
            return;
        }

        if (Input.GetButtonDown("Attack"))
        {
            m_rb.velocity = new Vector2(0, 0);
            player.ChangeState(new PlayerStateAttack());
            return;
        }

        if (Input.GetButtonDown("Skill1"))
        {
            if (!player.ChangeMP(-1))
            {
                return;
            }
            m_rb.velocity = new Vector2(0, 0);
            player.ChangeState(new PlayerStateSkill1());
            return;
        }

        if (Input.GetButtonDown("Skill2"))
        {
            if (!player.ChangeMP(-1))
            {
                return;
            }
            m_rb.velocity = new Vector2(0, 0);
            player.ChangeState(new PlayerStateSkill2());
            return;
        }

        if (Input.GetButtonDown("Skill3"))
        {
            if (!player.ChangeMP(-1))
            {
                return;
            }
            m_rb.velocity = new Vector2(0, 0);
            player.ChangeState(new PlayerStateSkill3());
            return;
        }
    }

    public void OnEnter(Player player)
    {
        Debug.Log("进入 移动 模式");
        m_rb = player.GetComponent<Rigidbody2D>();
        m_sprite = player.transform.Find("Player Sprite").GetComponent<SpriteRenderer>();
        m_animator = player.transform.Find("Player Sprite").GetComponent<Animator>();
        m_animator.Play("PlayerRun");
    }

    public void OnExit(Player player)
    {

    }

    public void Update(Player player)
    {
        if (m_rb == null)
        {
            return;
        }

        float moveValue = Input.GetAxisRaw("Horizontal");

        Vector2 moveVector = new Vector2(moveValue * player.moveSpeed, m_rb.velocity.y);
        m_rb.velocity = moveVector;

        if (moveValue > 0.5f)
        {
            m_sprite.flipX = false;
        }
        else if (moveValue < -0.5f)
        {
            m_sprite.flipX = true;
        }
        else
        {
            player.ChangeState(new PlayerStateIdle());
        }
    }
}

/// <summary>
/// 跳跃状态
/// </summary>
public class PlayerStateJump : IPlayerState
{
    private Rigidbody2D m_rb;
    private SpriteRenderer m_sprite;
    private Animator m_animator;

    private bool isFirstUpdate;

    public void HandleInput(Player player)
    {
        if (Input.GetButtonDown("Skill3"))
        {
            if (!player.ChangeMP(-1))
            {
                return;
            }
            player.ChangeState(new PlayerStateSkill3());
            return;
        }
    }

    public void OnEnter(Player player)
    {
        Debug.Log("进入 跳跃 模式");

        isFirstUpdate = true;

        m_sprite = player.transform.Find("Player Sprite").GetComponent<SpriteRenderer>();

        m_rb = player.GetComponent<Rigidbody2D>();
        if (!player.isNotOnGround)
        {
            m_rb.AddForce(Vector2.up * player.jumpForce, ForceMode2D.Impulse);
            SoundManage.Player(SoundName.jump);
        }

        m_animator = player.transform.Find("Player Sprite").GetComponent<Animator>();
        m_animator.Play("PlayerJump");
    }

    public void OnExit(Player player)
    {

    }

    public void Update(Player player)
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            return;
        }

        float moveValue = Input.GetAxisRaw("Horizontal");
        m_rb.velocity = new Vector2(moveValue * player.moveSpeed, m_rb.velocity.y);

        if (moveValue > 0.5f)
        {
            m_sprite.flipX = false;
        }
        else if (moveValue < -0.5f)
        {
            m_sprite.flipX = true;
        }

        if (m_rb.velocity.y < 0.1f && m_rb.velocity.y > -0.1f && player.isCollide)
        {
            player.ChangeState(new PlayerStateIdle());
        }
    }

}

/// <summary>
/// 攻击状态
/// </summary>
public class PlayerStateAttack : IPlayerState
{
    private Animator m_animator;
    private bool isActionAttackRange;

    public void HandleInput(Player player)
    {
        if (Input.GetButtonDown("Attack"))
        {
            player.ChangeState(new PlayerStateAttack());
        }
    }

    public void OnEnter(Player player)
    {
        m_animator = player.transform.Find("Player Sprite").GetComponent<Animator>();

        if (player.currentComboWaitTime > 0)
        {
            return;
        }
        player.currentComboWaitTime = player.comboWaitTime;

        if (player.currentComboTime < 0 || player.comboIndex >= 3)
        {
            player.comboIndex = 0;
        }
        player.currentComboTime = player.comboTimeLimit;

        player.comboIndex++;

        Debug.Log("进入 攻击 状态 " + player.comboIndex);

        m_animator.Play("PlayerAttack" + player.comboIndex);

        isActionAttackRange = false;
    }

    public void OnExit(Player player)
    {

    }

    public void Update(Player player)
    {
        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("PlayerAttack" + player.comboIndex))
        {
            if (stateInfo.normalizedTime > 0.3f && !isActionAttackRange)
            {
                float offsetX = 0.65f;
                Vector2 size = new Vector2(0.6f, 1);

                if (player.IsRight())
                {
                    Vector2 origin = new Vector2(player.transform.position.x + offsetX, player.transform.position.y);
                    RaycastHit2D[] ray = Physics2D.BoxCastAll(origin, size, 0, Vector2.right, 0);

                    for (int i = 0; i < ray.Length; ++i)
                    {
                        // 击中敌人
                        if (ray[i].collider.tag.Equals("Enemy"))
                        {
                            EnemyBace enemybase = ray[i].collider.GetComponent<EnemyBace>();
                            if (enemybase)
                            {
                                enemybase.Harmed(PlayerDataManage.Instance.playerData.attackLevel + 1);
                            }
                        }
                    }
                }
                else
                {
                    Vector2 origin = new Vector2(player.transform.position.x - offsetX, player.transform.position.y);
                    RaycastHit2D[] ray = Physics2D.BoxCastAll(origin, size, 0, Vector2.left, 0);

                    for (int i = 0; i < ray.Length; ++i)
                    {
                        // 击中敌人
                        if (ray[i].collider.tag.Equals("Enemy"))
                        {
                            EnemyBace enemybase = ray[i].collider.GetComponent<EnemyBace>();
                            if (enemybase)
                            {
                                enemybase.Harmed(PlayerDataManage.Instance.playerData.attackLevel + 1);
                            }
                        }
                    }

                }

                isActionAttackRange = true;
            }

            // 判断动画是否播放结束
            if (stateInfo.normalizedTime >= 1f)
            {
                player.ChangeState(new PlayerStateIdle());
            }
        }
    }

}

/// <summary>
/// 技能1
/// </summary>
public class PlayerStateSkill1 : IPlayerState
{
    private Animator m_animator;
    private string animAame = "PlayerSkill1";
    private int attackCount = 0;

    public void HandleInput(Player player)
    {

    }

    public void OnEnter(Player player)
    {
        m_animator = player.transform.Find("Player Sprite").GetComponent<Animator>();
        m_animator.Play(animAame);

        attackCount = 0;
    }

    public void OnExit(Player player)
    {

    }

    public void Update(Player player)
    {
        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(animAame))
        {
            if (stateInfo.normalizedTime >= 0.3f && attackCount == 0)
            {
                RaycastHit2D[] rays = AttackRange(player);
                foreach (RaycastHit2D ray in rays)
                {
                    if (ray.collider.tag.Equals("Enemy"))
                    {
                        EnemyBace enemybase = ray.collider.GetComponent<EnemyBace>();
                        if (enemybase)
                        {
                            enemybase.Harmed(PlayerDataManage.Instance.playerData.skill1Level + 2);
                        }
                    }
                }

                attackCount++;
            }
            else if (stateInfo.normalizedTime >= 0.7f && attackCount == 1)
            {
                RaycastHit2D[] rays = AttackRange(player);
                foreach (RaycastHit2D ray in rays)
                {
                    if (ray.collider.tag.Equals("Enemy"))
                    {
                        EnemyBace enemybase = ray.collider.GetComponent<EnemyBace>();
                        if (enemybase)
                        {
                            enemybase.Harmed(PlayerDataManage.Instance.playerData.skill1Level + 2);
                        }

                        Rigidbody2D rb = ray.collider.GetComponent<Rigidbody2D>();
                        if (rb)
                        {
                            if (player.IsRight())
                            {
                                rb.AddForce(new Vector2(6, 5), ForceMode2D.Impulse);
                            }
                            else
                            {
                                rb.AddForce(new Vector2(-6, 5), ForceMode2D.Impulse);
                            }
                        }
                    }
                }

                attackCount++;
            }

            // 判断动画是否播放结束
            if (stateInfo.normalizedTime >= 1f)
            {
                player.ChangeState(new PlayerStateIdle());
            }
        }
    }

    private RaycastHit2D[] AttackRange(Player player)
    {
        float offsetX = 0.65f;
        Vector2 size = new Vector2(0.6f, 1);

        if (player.IsRight())
        {
            Vector2 origin = new Vector2(player.transform.position.x + offsetX, player.transform.position.y);
            return Physics2D.BoxCastAll(origin, size, 0, Vector2.right, 0);
        }
        else
        {
            Vector2 origin = new Vector2(player.transform.position.x - offsetX, player.transform.position.y);
            return Physics2D.BoxCastAll(origin, size, 0, Vector2.left, 0);
        }
    }

}

/// <summary>
/// 技能2
/// </summary>
public class PlayerStateSkill2 : IPlayerState
{
    private Animator m_animator;
    private string animName = "PlayerSkill2";
    private bool isShoot;

    public void HandleInput(Player player)
    {

    }

    public void OnEnter(Player player)
    {
        m_animator = player.transform.Find("Player Sprite").GetComponent<Animator>();
        m_animator.Play(animName);

        isShoot = false;
    }

    public void OnExit(Player player)
    {

    }

    public void Update(Player player)
    {
        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(animName))
        {
            if (!isShoot && stateInfo.normalizedTime >= 0.6f)
            {
                // 拷贝火焰
                GameObject go = player.transform.Find("Fire").gameObject;
                GameObject fire = Object.Instantiate(go);
                fire.transform.position = player.transform.position;
                fire.SetActive(true);

                // 获取脚本
                PlayerFire playerFire = fire.GetComponent<PlayerFire>();

                // 调用发射方法
                float toX = player.transform.position.x + 20;
                if (!player.IsRight())
                {
                    toX = player.transform.position.x - 20;

                    // 设置方向
                    fire.transform.Rotate(0, 180, 0, Space.World);
                }
                playerFire.Shoot(toX);

                isShoot = true;

                SoundManage.Player(SoundName.playerFire);
            }

            // 判断动画是否播放结束
            if (stateInfo.normalizedTime >= 1f)
            {
                player.ChangeState(new PlayerStateIdle());
            }
        }
    }

}

/// <summary>
/// 技能3
/// </summary>
public class PlayerStateSkill3 : IPlayerState
{
    private float dashDuration = 0.04f;  // 冲刺持续时间
    private float move;
    private float currentGravitational;  // 当前重力
    private Animator m_animator;
    private Rigidbody2D m_Rigidbody2;
    private Vector2 targetPosition;

    public void HandleInput(Player player)
    {

    }

    public void OnEnter(Player player)
    {
        m_Rigidbody2 = player.GetComponent<Rigidbody2D>();
        currentGravitational = m_Rigidbody2.gravityScale;
        m_Rigidbody2.gravityScale = 0;

        m_animator = player.transform.Find("Player Sprite").GetComponent<Animator>();
        m_animator.Play("PlayerSkill3");
        dashDuration = 0.5f;

        targetPosition = new Vector2(player.transform.position.x + 10, player.transform.position.y);
        if (!player.IsRight())
        {
            targetPosition = new Vector2(player.transform.position.x - 10, player.transform.position.y);
        }

        move = ((PlayerDataManage.Instance.playerData.skill3Level / 0.5f) + 3) / dashDuration;

        if (!player.IsRight())
        {
            move = -move;
        }

        player.SetTrail(true);

        SoundManage.Player(SoundName.skill3);
    }

    public void OnExit(Player player)
    {
        if (m_Rigidbody2)
        {
            m_Rigidbody2.gravityScale = currentGravitational;
        }

        if (m_Rigidbody2)
        {
            m_Rigidbody2.velocity = Vector2.zero;
        }

        player.SetTrail(false);
    }

    public void Update(Player player)
    {
        m_Rigidbody2.velocity = new Vector2(move, 0);

        // 冲刺结束
        dashDuration -= Time.deltaTime;
        if (dashDuration < 0)
        {
            player.ChangeState(new PlayerStateIdle());
        }
    }
}

/// <summary>
/// 受伤状态
/// </summary>
public class PlayerStateHarmed : IPlayerState
{
    private Animator m_animator;
    private string animName = "PlayerHarmed";

    public void HandleInput(Player player)
    {

    }

    public void OnEnter(Player player)
    {
        m_animator = player.transform.Find("Player Sprite").GetComponent<Animator>();
        m_animator.Play(animName);
    }

    public void OnExit(Player player)
    {

    }

    public void Update(Player player)
    {
        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);

        // 判断动画是否播放结束
        if (stateInfo.IsName(animName))
        {
            if (stateInfo.normalizedTime >= 1f)
            {
                player.ChangeState(new PlayerStateIdle());
            }
        }
    }

}

/// <summary>
/// 死亡状态
/// </summary>
public class PlayerStateDead : IPlayerState
{
    private Animator m_animator;
    private string animName = "PlayerDead";
    private float waitTime;
    public void HandleInput(Player player)
    {

    }

    public void OnEnter(Player player)
    {
        Debug.Log("死亡");
        m_animator = player.transform.Find("Player Sprite").GetComponent<Animator>();
        m_animator.Play(animName);
        waitTime = 0.5f;
    }

    public void OnExit(Player player)
    {

    }

    public void Update(Player player)
    {
        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(animName))
        {
            // 判断动画是否播放结束
            if (stateInfo.normalizedTime >= 1f)
            {
                waitTime -= Time.deltaTime;
                if (waitTime > 0f)
                {
                    return;
                }
                // 重新加载当前场景
                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);
            }
        }
    }
}