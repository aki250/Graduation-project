using UnityEngine;

                                                                         //玩家释放黑洞技能的状态
public class PlayerReleaseBlackholeSkillState : PlayerState
{
    //飞行时间，即玩家在释放黑洞技能时悬浮的时间
    private float flyTime = 0.4f;
    //技能是否已使用的标志
    private bool skillUsed;
    //原始重力值，用于在状态结束时恢复重力
    private float originalGravity;

    public PlayerReleaseBlackholeSkillState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    //当进入释放黑洞技能状态时调用
    public override void Enter()
    {
        base.Enter();

        //保存原始重力值
        originalGravity = rb.gravityScale;

        //重置技能使用标志
        skillUsed = false;
        //设置状态计时器为飞行时间
        stateTimer = flyTime;
        //设置重力为 0，使玩家悬浮
        rb.gravityScale = 0;
    }

    public override void Exit()
    {
        base.Exit();
        //恢复原始重力值
        rb.gravityScale = originalGravity;
        //使玩家实体不透明，即恢复正常显示
        player.fx.MakeEntityTransparent(false);
    }

    public override void Update()
    {
        base.Update(); 

        if (stateMachine.currentState != player.blackholeSkillState)
        {
            return;
        }

        //如果主摄像机的正交大小小于或等于 10，逐渐增加其正交大小
        if (Camera.main.orthographicSize <= 10)
        {
            Camera.main.orthographicSize += 0.1f;
        }

        //如果状态计时器>0，使玩家上升
        if (stateTimer > 0)
        {
            rb.velocity = new Vector2(0, 15);
        }

        //如果状态计时器<0，使玩家缓慢下降
        if (stateTimer < 0)
        {
            rb.velocity = new Vector2(0, -0.1f);

            //如果技能尚未使用
            if (!skillUsed)
            {
                //如果黑洞技能可用并使用成功，则设置技能使用标志为true
                if (player.skill.blackhole.UseSkillIfAvailable())
                {
                    skillUsed = true;
                }
            }

            // 如果可以退出黑洞技能状态
            if (player.skill.blackhole.CanExitBlackholeSkill())
            {
                // 切换到空中状态
                player.stateMachine.ChangeState(player.airState);
            }
        }
    }
}