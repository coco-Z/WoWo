using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FollowPlayer : MonoBehaviour
{
    private GameObject m_Player; // 玩家

    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.2f;

    // 在第一帧更新前调用启动
    void Start()
    {
        m_Player = GameObject.Find("Player");
    }

    private void LateUpdate()
    {
        if (m_Player != null) 
        {
            //if (Vector2.Distance(transform.position, m_Player.transform.position) > 0.1f)
            //{
            //    Vector3 targetPosition = new Vector3(m_Player.transform.position.x, m_Player.transform.position.y, transform.position.z);
            //    transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, 6);
            //}

            transform.position = new Vector3(m_Player.transform.position.x, m_Player.transform.position.y + 2, transform.position.z);
        }
    }
}
