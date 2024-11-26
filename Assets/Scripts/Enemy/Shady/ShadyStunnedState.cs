using UnityEngine;

public class ShadyStunnedState : ShadyState
{
    private float moveTimer;

    public ShadyStunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Shady _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //启动眩晕时红色闪烁
        enemy.fx.InvokeRepeating("RedColorBlink", 0, 0.1f);

        //眩晕状态持续时间
        stateTimer = enemy.stunDuration;
        //控制敌人移动定时器，眩晕状态下敌人的移动时间很短
        moveTimer = 0.1f;

        // 设置敌人的速度，使其根据设置的眩晕移动方向和速度滑行
        // 通过 `-enemy.facingDirection` 来反转方向（敌人可能朝左或朝右）
        rb.velocity = new Vector2(enemy.stunMovement.x * -enemy.facingDirection, enemy.stunMovement.y);
    }

    public override void Exit()
    {
        base.Exit(); 

        //取消眩晕时闪烁效果
        enemy.fx.Invoke("CancelColorChange", 0);
    }

    public override void Update()
    {
        base.Update();  

        //减少moveTimer，控制敌人的眩晕滑行时间
        moveTimer -= Time.deltaTime;

        //小于0，则停止敌人的水平移动
        if (moveTimer < 0)
        {
            //只保留垂直方向速度
            enemy.SetVelocity(0, rb.velocity.y);
        }

        // 眩晕时间结束，切换回空闲状态
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
