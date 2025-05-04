using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    private float moveSpeed = 3;
    private bool isPause;
    private Tweener tweener;


    // 在第一帧更新前调用启动
    void Start()
    {
        PauseChange.OnPauseChanged += Pause;
    }

    // 每帧调用一次更新
    void Update()
    {
        if (isPause)
        {
            tweener.Pause();
        }
        else
        {
            tweener.Play();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Enemy"))
        {
            EnemyBace enemyBace = collision.GetComponent<EnemyBace>();
            enemyBace.Harmed(PlayerDataManage.Instance.playerData.skill2Level + 2);
        }
        else if (!collision.tag.Equals("Player"))
        {
            Destroy(gameObject);
        }
    }

    public void Shoot(float to_x)
    {
        tweener = transform.DOMoveX(to_x, moveSpeed, false).SetEase(Ease.Linear).OnComplete(() => Destroy(gameObject));
    }
}
