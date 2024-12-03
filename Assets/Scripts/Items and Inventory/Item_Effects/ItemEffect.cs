using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemEffect : ScriptableObject
{
    // ItemEffect 类是一个可被触发的效果，用于在不同的情况下被激活，如玩家受伤、攻击敌人、使用物品等。
    // 1. 当玩家受到伤害时 - 在 PlayerStats.DecreaseHPBy() 方法中触发
    // 2. 当玩家攻击敌人时 - 在 PlayerAnimationTrigger.AttackTrigger() 方法中触发
    // 3. 当玩家释放剑气（如冰剑、火剑）时 - 在 PlayerAnimationTrigger.AttackTrigger() 方法中触发
    // 4. 当玩家使用物品时（如药水、法力瓶等） - 在 Player.Update() 方法中触发
    // 5. 当玩家的魔法击中敌人时（如神之护符增加效果到水晶） - 在 CrystalSkillController.Explosion() 方法中触发

    public bool effectUsed { get; set; }  //表示效果是否被使用
    public float effectLastUseTime { get; set; }  //记录效果上次使用时间
    public float effectCooldown;  //效果冷却时间

    [TextArea] 
    public string effectDescription;  //英文说明效果描述
    [TextArea] 
    public string effectDescription_Chinese;  //中文说明效果

    //执行效果方法
    public virtual void ExecuteEffect(Transform _spawnTransform)
    {
    }

    //不需要与敌人发生碰撞时使用
    public virtual void ExecuteEffect_NoHitNeeded()
    {
    }

    //释放剑气等技能效果时的处理方法，
    public virtual void ReleaseSwordArcane()
    {
    }
}
