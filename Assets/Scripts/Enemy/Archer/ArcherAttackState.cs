using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ArcherAttackState继承自ArcherState，表示弓箭手的攻击状态
public class ArcherAttackState : ArcherState
{
    // 构造函数，初始化状态
    public ArcherAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Archer _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    // 状态进入时的逻辑
    public override void Enter()
    {
        base.Enter();

        // 设置一个计时器，使敌人在攻击动作开始时向前移动一小段时间
        stateTimer = 0.1f;
    }

    // 状态退出时的逻辑
    public override void Exit()
    {
        base.Exit();

        // 更新敌人的最后一次攻击时间
        enemy.lastTimeAttacked = Time.time;
    }

    // 状态更新时的逻辑
    public override void Update()
    {
        base.Update();

        // 如果计时器大于0
        if (stateTimer > 0)
        {
            // 如果敌人被击退，不再向前移动
            if (enemy.isKnockbacked)
            {
                stateTimer = 0;  // 重置计时器
                return;          // 提前结束
            }

            // 敌人在攻击动作开始时向前移动（注释掉的代码表明此逻辑暂未启用）
            // enemy.SetVelocity(enemy.battleMoveSpeed * enemy.facingDirection, rb.velocity.y);
        }
        else
        {
            // 攻击后停止移动，保持垂直方向的速度不变
            enemy.SetVelocity(0, rb.velocity.y);
        }

        // 如果触发攻击动画的条件已满足
        if (triggerCalled)
        {
            // 切换到战斗状态
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
