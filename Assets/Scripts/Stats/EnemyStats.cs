using System.Linq;
using UnityEngine;

                                          //敌人的状态
public class EnemyStats : CharacterStats
{
    private Enemy enemy; //敌人组件
    private ItemDrop itemDropSystem; //物品掉落系统组件

    public Stat currencyDropAmount; //敌人死亡时掉落的货币数量

    [Header("敌人等级")]
    [SerializeField] private int enemyLevel = 1; //敌人等级

    [Range(0f, 1f)]
    [SerializeField] private float percentageModifier = 0.4f; //敌人等级对属性的百分比加成

    protected override void Start()
    {
        //根据敌人等级调整所有属性
        ModifyAllStatsAccordingToEnemyLevel();

        base.Start();

        enemy = GetComponent<Enemy>(); //获取敌人组件
        itemDropSystem = GetComponent<ItemDrop>(); //获取物品掉落系统组件
    }

    //敌人受到伤害时调用
    public override void TakeDamage(int _damage, Transform _attacker, Transform _attackee, bool _isCrit)
    {
        base.TakeDamage(_damage, _attacker, _attackee, _isCrit);

        //敌人进入战斗状态
        enemy.GetIntoBattleState();
    }

    protected override void Die()
    {
        base.Die();

        enemy.Die(); //敌人死亡

        //敌人死亡时掉落物品
        itemDropSystem.GenrateDrop();

        //玩家获得货币
        PlayerManager.instance.currency += currencyDropAmount.GetValue();

        //3秒后销毁敌人对象
        Destroy(gameObject, 3f);
    }

    //敌人生命值为0
    public void ZeroHP()
    {
        currentHP = 0; //设置当前生命值为0

        base.Die();

        if (onHealthChanged != null) //如果有健康状态改变的委托
        {
            onHealthChanged(); //调用委托
        }
    }

    //敌人掉落货币和物品时调用
    public void DropCurrencyAndItem()
    {
        itemDropSystem.GenrateDrop(); //掉落物品

        //玩家获得货币
        PlayerManager.instance.currency += currencyDropAmount.GetValue();
    }

    //根据敌人等级调整所有属性
    private void ModifyAllStatsAccordingToEnemyLevel()
    {
        //调整所有属性，包括主要属性、防御属性、攻击属性、魔法属性和货币掉落
        foreach (Stat stat in GetType().GetFields().Where(f => typeof(Stat).IsAssignableFrom(f.FieldType)).Select(f => (Stat)f.GetValue(this)))
        {
            ModifyStatAccordingToEnemyLevel(stat);
        }
    }

    //根据敌人等级调整单个属性
    private void ModifyStatAccordingToEnemyLevel(Stat _stat)
    {
        //当敌人等级大于1时，增加敌人的属性
        for (int i = 1; i < enemyLevel; i++)
        {
            float _modifier = _stat.GetValue() * percentageModifier; //计算属性加成
            _stat.AddModifier(Mathf.RoundToInt(_modifier)); //增加属性加成
        }
    }
}