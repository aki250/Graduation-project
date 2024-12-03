using System.Collections;
using System.Collections.Generic;
using UnityEngine;
                    //能在玩家生命值低于50%，冻结范围内的敌人一段时间。

[CreateAssetMenu(fileName = "Freeze Enemy Effect", menuName = "Data/Item Effect/Freeze Enemy Effect")]
public class FreezeEnemy_Effect : ItemEffect
{
    [SerializeField] private float freezeDuration;  //冻结效果持续时间

    public override void ExecuteEffect(Transform _spawnTransform)
    {
        //玩家当前生命值高于50%，不触发冻结效果
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        if (playerStats.currentHP > playerStats.getMaxHP() * 0.5)
        {
            return;
        }

        //获取一个以_spawnTransform 为圆心、半径为2的圆,范围内的所有碰撞体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_spawnTransform.position, 2);
        foreach (var hit in colliders)
        {
            //碰撞体包含Enemy组件，则触发冻结效果
            if (hit.GetComponent<Enemy>() != null)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                enemy.FreezeEnemyForTime(freezeDuration);
            }
        }
    }
}
