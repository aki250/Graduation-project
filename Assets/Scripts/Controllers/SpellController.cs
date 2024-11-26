using System.Collections;
using System.Collections.Generic;
using UnityEngine;

                                                //控制法术Spell ，包括命中检测和销毁逻辑。
public class SpellController : MonoBehaviour
{
    //检测攻击范围的起点
    [SerializeField] private Transform check;

    //碰撞盒的尺寸，用于检测范围
    [SerializeField] private Vector2 boxSize;

    //指定检测的玩家目标层
    [SerializeField] private LayerMask whatIsPlayer;

    //攻击者的状态对象，存储攻击方的属性（如伤害值）
    private CharacterStats enemyStats;

    //初始化法术的参数
    public void SetupSpell(CharacterStats _enemyStats)
    {
        enemyStats = _enemyStats; //攻击者状态
    }

    //在动画触发点调用，用于检测并伤害玩家。
    private void AnimationTrigger()
    {
        // 使用 Physics2D.OverlapBoxAll 检测范围内的碰撞体
        Collider2D[] colliders = Physics2D.OverlapBoxAll(check.position, boxSize, 0, whatIsPlayer);

        // 遍历所有检测到的碰撞体
        foreach (var hit in colliders)
        {
            //碰撞体是玩家
            if (hit.GetComponent<Player>() != null)
            {
                //对玩家造成伤害，调用攻击者的伤害逻辑
                enemyStats.DoDamge(hit.GetComponent<PlayerStats>());
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(check.position, boxSize);
    }

    private void SelfDestroy()
    {
        Destroy(gameObject); 
    }
}
