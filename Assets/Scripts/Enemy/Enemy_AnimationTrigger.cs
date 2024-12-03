using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AnimationTrigger : MonoBehaviour
{
    //获取父物体上的Enemy组件
    private Enemy enemy => GetComponentInParent<Enemy>();

    //动画触发器，触发敌人的动画事件
    private void AnimationTrigger()
    {
        enemy.AnimationTrigger();
    }

    //攻击触发器，在动画触发时检查是否有玩家受到攻击
    private void AttackTrigger()
    {
        //获取敌人攻击范围内的所有碰撞体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);

        //遍历所有碰撞体
        foreach (var hit in colliders)
        {
            //如果碰撞体是玩家
            if (hit.GetComponent<Player>() != null)
            {
                Player player = hit.GetComponent<Player>();

                // 这里可以调用玩家受到伤害的方法
                //player.Damage(enemy.transform, player.transform);

                // 获取玩家的 Stats 组件并进行伤害计算
                PlayerStats _target = hit.GetComponent<PlayerStats>();
                enemy.stats.DoDamge(_target);  //敌人对玩家造成伤害
            }
        }
    }

    //特殊攻击触发器
    private void SpecialAttackTrigger()
    {
        //调用敌人的特殊攻击方法
        enemy.SpecialAttackTrigger();
    }

    //打开反击窗口
    private void OpenCounterAttackWindow()
    {
        //调用敌人打开反击窗口的方法
        enemy.OpenCounterAttackWindow();
    }

    //关闭反击窗口
    private void CloseCounterAttackWindow()
    {
        enemy.CloseCounterAttackWindow();
    }
}
