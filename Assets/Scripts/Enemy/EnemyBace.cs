using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBace : MonoBehaviour
{
    public float moveSpeed = 3;
    [HideInInspector]

    private bool isAction;
    private bool isEntPlayer;
    private bool isMove;
    private bool isPause;
    private float gotoPlayerWait = 1f;
    private float currentGotoPlayer = 0;
    private int HP = 5;

    private GameObject player;

    private Rigidbody2D m_rb;
    private SpriteRenderer m_spriteRenderer;
    private Animator m_animator;

    // 在第一帧更新前调用启动
    void Start()
    {
        isAction = false;
        isMove = false;

        player = GameObject.Find("Player");

        m_rb = GetComponent<Rigidbody2D>();
        m_spriteRenderer = transform.Find("Enemy Sprite").GetComponent<SpriteRenderer>();
        m_animator = transform.Find("Enemy Sprite").GetComponent<Animator>();

        PauseChange.OnPauseChanged += Pause;
    }

    // 每帧调用一次更新
    void Update()
    {
        if (IsDead() || isPause) 
        {
            return;
        }

        currentGotoPlayer -= Time.deltaTime;
        currentGotoPlayer = Mathf.Max(currentGotoPlayer, -1);

        if (!isAction)
        {
            return;
        }

        // 朝Player移动
        if (isAction && !isEntPlayer && currentGotoPlayer < 0)
        {
            float moveDir = transform.position.x < player.transform.position.x ? 1 : -1;
            if (moveDir < 0)
            {
                m_spriteRenderer.flipX = false;
            }
            else
            {
                m_spriteRenderer.flipX = true;
            }
            m_rb.velocity = new Vector2(moveDir * moveSpeed, m_rb.velocity.y);
            isMove = true;

            m_animator.SetBool("IsMove", true);
        }
        else if (isMove)
        {
            m_rb.velocity = Vector2.zero;
            isMove = false;

            m_animator.SetBool("IsMove", false);
        }
    }

    private void OnDisable()
    {
        PauseChange.OnPauseChanged -= Pause;
    }

    private void OnDestroy()
    {
        ParticleSystem particleSystem = transform.Find("Death effect").GetComponent<ParticleSystem>();
        particleSystem.transform.parent = null;
        particleSystem.gameObject.SetActive(true);
    }

    public void Harmed(int value)
    {
        isAction = true;

        if (IsDead())
        {
            return;
        }
        m_rb.velocity = Vector2.zero;
        currentGotoPlayer = gotoPlayerWait;

        SoundManage.Player(SoundName.hit);
        
        HP -= value;
        ShowDamageText(value);

        if (IsDead())
        {
            m_rb.AddForce(new Vector2(0, 6), ForceMode2D.Impulse);
            m_animator.SetTrigger("Dead");
            Destroy(gameObject, 2);

            PlayerDataManage.Instance.playerData.enemyKillCount++;
            
            return;
        }

        m_animator.SetTrigger("Harmed");
    }

    public bool IsDead()
    {
        return HP <= 0;
    }

    private void ShowDamageText(int damage)
    {
        GameObject blood = ObjectPoolManager.Instance.GetPooledObject("Blood Deduction Effect");
        if (blood == null)
        {
            return;
        }
        blood.transform.SetParent(GameObject.Find("World Canvas").GetComponent<Transform>(), false);

        float randomX = Random.Range(-0.5f, 0.5f);
        float randomY = Random.Range(0.5f, 1.0f);
        Vector3 randomPosition = new Vector3(randomX, randomY, 0);
        Vector3 endPosition = transform.position + randomPosition;

        blood.GetComponent<BloodDeductionEffect>().ShowBloodDeduction(transform.position, endPosition, -damage * 100);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isPause)
        {
            return;
        }

        if (collision.gameObject == player)
        {
            isAction = true;

            if (currentGotoPlayer < 0) 
            {
                isEntPlayer = true;
                currentGotoPlayer = gotoPlayerWait;

                Player p = player.GetComponent<Player>();
                p.Harmer(1);
            }
            
        }
        else
        {
            isEntPlayer = false;
        }
    }

    private void Pause(bool pause)
    { 
        isPause = pause;
    }

}
