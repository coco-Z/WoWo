using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameObject levelManageGO = GameObject.Find("LevelManage");
            LevelManage levelManage = levelManageGO.GetComponent<LevelManage>();
            levelManage.CheckpointTriggered(gameObject);
        }
    }
}
