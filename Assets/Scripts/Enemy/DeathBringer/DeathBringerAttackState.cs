using UnityEngine;

public class DeathBringerAttackState : DeathBringerState
{
    public DeathBringerAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, DeathBringer _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //在攻击开始时设置一个时间延迟，敌人会先向前位移
        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeAttacked = Time.time; //敌人最后一次攻击时间
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
        {
            //如果敌人被击退，则停止继续向前移动
            if (enemy.isKnockbacked)
            {
                stateTimer = 0;
                return;
            }

            // 如果没有被击退，敌人会在攻击开始时向前移动
            // enemy.SetVelocity(enemy.battleMoveSpeed * enemy.facingDirection, rb.velocity.y);
        }
        else
        {
            // 攻击结束后，停止敌人的水平移动，只保留垂直速度（例如重力影响）
            enemy.SetVelocity(0, rb.velocity.y);
        }

        //检查是否需要触发转态变更
        if (triggerCalled)
        {
            //敌人可以传送，则切换到传送状态
            if (enemy.CanTeleport())
            {
                stateMachine.ChangeState(enemy.teleportState);
            }
            else
            {
                //否则，增加传送概率并切换回战斗状态
                enemy.chanceToTeleport += 10;
                stateMachine.ChangeState(enemy.battleState);
            }
        }
    }
}
