using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class Checkpoint : MonoBehaviour
{
    private Animator anim;

    public string checkpointID;  //检查点唯一标识符

    public bool activated;  //标记检查点是否激活

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 碰撞对象是Player，则激活该检查点并保存游戏状态
        if (collision.GetComponent<Player>() != null)
        {
            ActivateCheckpoint();
            SaveManager.instance.SaveGame();  //保存游戏状态
        }
    }

    public void ActivateCheckpoint()
    {
                            //设置游戏管理器中的最后激活的检查点ID
        GameManager.instance.lastActivatedCheckpointID = checkpointID;

        //播放检查点激活音效
        AudioManager.instance.PlaySFX(5, transform);

        if (activated)
        {
            return;
        }

        anim.SetBool("Active", true);
        activated = true;  //检查点状态设置为已激活
    }

    [ContextMenu("生成检查点ID")]
    private void GenerateCheckpointID()
    {
        //使用GUID生成唯一的检查点ID，确保每个检查点都有不同的标识符
        checkpointID = System.Guid.NewGuid().ToString();
    }
}
