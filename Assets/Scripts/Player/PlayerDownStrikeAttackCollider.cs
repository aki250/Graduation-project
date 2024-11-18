using System.Collections;
using System.Collections.Generic;
using UnityEngine;

                                //玩家下击攻击碰撞体类，负责检测玩家下击状态时与敌人发生的碰撞，并进行相应的攻击逻辑处理
public class PlayerDownStrikeAttackCollider : MonoBehaviour
{
    private Player player;  //获取玩家的状态和行为

    private float downStrikAttackCooldown = 10f;  //下击攻击的冷却时间
    private float downStrikeAttackTimer;  //计时器，用于控制冷却时间

    //在脚本开始时获取玩家对象
    private void Start()
    {
        player = PlayerManager.instance.player;  //获取玩家对象（通过 PlayerManager 单例）
    }

    private void Update()
    {
        downStrikeAttackTimer -= Time.deltaTime;  //每帧减少冷却时间

        //如果玩家已接触地面，禁用当前物体（可能是攻击碰撞体）
        if (player.IsGroundDetected())
        {
            gameObject.SetActive(false);
        }
    }

    //重置计时器
    private void OnEnable()
    {
        downStrikeAttackTimer = 0;  //激活时将冷却计时器清零
    }

    //当碰撞体与其他物体发生持续接触时触发
    private void OnTriggerStay2D(Collider2D collision)
    {
        //如果玩家当前处于下击状态
        if (player.stateMachine.currentState == player.downStrikeState)
        {
            // 检查碰撞体是否与敌人发生碰撞
            if (collision.GetComponent<Enemy>() != null)
            {
                Enemy enemy = collision.GetComponent<Enemy>();  //获取敌人组件

                //保存原始的敌人击退向量（用于在攻击后恢复）
                Vector2 originalEnemyKnockbackMovement = enemy.knockbackMovement;
                //设置敌人新的击退向量，使其与玩家的垂直速度（Y轴速度）保持一致
                enemy.knockbackMovement = new Vector2(0, player.rb.velocity.y);

                // 设置敌人新的速度（水平与垂直都跟随玩家的速度）
                enemy.SetVelocity(player.rb.velocity.x, player.rb.velocity.y);

                //如果冷却时间已过，进行伤害处理
                if (downStrikeAttackTimer < 0)
                {
                    player.stats.DoDamge(enemy.stats);  //玩家对敌人造成伤害
                    downStrikeAttackTimer = downStrikAttackCooldown;  //重置冷却计时器
                }

                //恢复敌人的击退向量,为了好看点，加就加了
                enemy.knockbackMovement = originalEnemyKnockbackMovement;
            }
        }
    }

    //当碰撞体与其他物体结束接触时触发
    private void OnTriggerExit2D(Collider2D collision)
    {
        //如果玩家当前处于下击状态
        if (player.stateMachine.currentState == player.downStrikeState)
        {
            //检查碰撞体是否与敌人发生碰撞
            if (collision.GetComponent<Enemy>() != null)
            {
                Enemy enemy = collision.GetComponent<Enemy>();  //获取敌人组件

                //直接设置敌人的速度（不再考虑击退）
                enemy.SetVelocity(player.rb.velocity.x, player.rb.velocity.y);

                //玩家对敌人造成伤害
                player.stats.DoDamge(enemy.stats);
            }
        }
    }
}
