using UnityEngine;

//允许在Unity编辑器中创建该脚本的实例作为可复用的物品效果
[CreateAssetMenu(fileName = "Buff Effect", menuName = "Data/Item Effect/Buff Effect")]
public class Buff_Effect : ItemEffect
{
    //定义Buff属性类型
    [SerializeField] private StatType buffStatType;

    //buff提升数值
    [SerializeField] private int buffValue;

    //buff持续时间
    [SerializeField] private int buffDuration;

    //用于缓存玩家的属性管理组件
    private PlayerStats playerStats;

    //注释掉的版本是一个可以在无需目标敌人的情况下直接应用Buff方法
    /*
    public override void ExecuteEffect_NoHitNeeded()
    {
        playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        //对玩家的指定属性buffStatType增加buffValue，持续buffDuration
        playerStats.IncreaseStatByTime(GetBuffStat(buffStatType), buffValue, buffDuration);
    }
    */

    //覆盖父类方法，针对指定目标执行 Buff 效果
    public override void ExecuteEffect(Transform _enemyTransform)
    {
        playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        //buffStatType获取需要增加的属性，并应用效果
        //Buff效果将增加buffValue，并持续buffDuration时间
        playerStats.IncreaseStatByTime(playerStats.GetStatByType(buffStatType), buffValue, buffDuration);
    }
}

