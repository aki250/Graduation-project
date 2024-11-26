using UnityEngine;

public class ArcherJumpState : ArcherState
{
    public ArcherJumpState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.lastTimeJumped = Time.time;
        rb.velocity = new Vector2(enemy.jumpVelocity.x * -enemy.facingDirection, enemy.jumpVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        anim.SetFloat("yVelocity", rb.velocity.y);

        if (rb.velocity.y < 0 && enemy.IsGroundDetected())
        {
            //停止水平滑动，确保角色落地后保持静止，后切换到战斗状态
            rb.velocity = new Vector2(0, rb.velocity.y);
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
