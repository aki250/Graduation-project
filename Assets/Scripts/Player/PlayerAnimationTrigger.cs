using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimationTrigger : MonoBehaviour
{
    //获取父体Player组件
    private Player player => GetComponentInParent<Player>();

    //下击碰撞器，用于处理下击攻击时的碰撞检测
    [SerializeField] private CircleCollider2D downStrikeCollider;

    //动画触发器，调用Player组件中的 AnimationTrigger 方法
    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    //攻击触发器，用于触发攻击并检测与敌人的碰撞
    private void AttackTrigger()
    {
        //释放剑的奥术效果，检查是否可以重新释放（考虑冷却时间）
        Inventory.instance.ReleaseSwordArcane_ConsiderCooldown();

        //获取玩家攻击范围内的所有碰撞器
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        //遍历所有检测到的碰撞器，检查是否是敌人
        foreach (var hit in colliders)
        {
            //如果是敌人，执行攻击逻辑
            if (hit.GetComponent<Enemy>() != null)
            {
                //获取敌人的统计数据并造成伤害
                EnemyStats _target = hit.GetComponent<EnemyStats>();
                player.stats.DoDamge(_target);

                //使用剑的效果，考虑冷却时间
                Inventory.instance.UseSwordEffect_ConsiderCooldown(_target.transform);
            }
        }
    }

    //空中发射攻击触发器，用于发起空中攻击并将敌人击飞
    private void AirLaunchAttackTrigger()
    {
        //释放剑的奥术效果，检查是否可以重新释放
        Inventory.instance.ReleaseSwordArcane_ConsiderCooldown();

        //获取玩家攻击范围内的所有碰撞器（可能是敌人）
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        //遍历所有检测到的碰撞器，检查是否是敌人
        foreach (var hit in colliders)
        {
            //如果是敌人，执行攻击逻辑
            if (hit.GetComponent<Enemy>() != null)
            {
                //获取敌人并暂时修改其击退运动，使其被击飞
                Enemy _enemy = hit.GetComponent<Enemy>();
                Vector2 originalKnockbackMovement = _enemy.knockbackMovement;
                _enemy.knockbackMovement = new Vector2(0, 17); //设置敌人被升天

                //获取敌人的统计数据并造成伤害
                EnemyStats _target = hit.GetComponent<EnemyStats>();
                player.stats.DoDamge(_target);

                //播放屏幕震动特效
                player.fx.ScreenShake(player.fx.shakeDirection_light);

                //恢复敌人的击退运动状态
                _enemy.knockbackMovement = originalKnockbackMovement;

                //使用剑的效果
                Inventory.instance.UseSwordEffect_ConsiderCooldown(_target.transform);
            }
        }
    }

    //打开下击攻击的碰撞器
    private void DownStrikeColliderOpenTrigger()
    {
        downStrikeCollider.gameObject.SetActive(true);
    }

    //关闭下击攻击的碰撞器
    private void DownStrikeColliderCloseTrigger()
    {
        downStrikeCollider.gameObject.SetActive(false);
    }

    //空中发射跳跃触发器，调用玩家的空中发射跳跃方法
    private void AirLaunchJumpTrigger()
    {
        player.AirLaunchJumpTrigger();
    }

    //下击触发器，调用玩家的下击方法
    private void DownStrikeTrigger()
    {
        player.DownStrikeTrigger();
    }

    //停止下击动画触发器，调用玩家停止下击动画的方法
    private void DownStrikeAnimStopTrigger()
    {
        player.DownStrikeAnimStopTrigger();
    }

    //投掷剑触发器，调用SkillManager创建投掷剑的技能方法
    private void ThrowSword()
    {
        SkillManager.instance.sword.CreateSword();
    }
}
