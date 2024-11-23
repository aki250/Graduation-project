using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Charm,
    Flask
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    [Header("Unique Item Effect info")]
    //public float itemCooldown;
    //public bool itemUsed { get; set; }
    //public float itemLastUseTime { get; set; }
    public ItemEffect[] itemEffects;

    [Header("Major Stats")]
    public int strength;  //damage + 1; crit_power + 1%
    public int agility;  //evasion + 1%; crit_chance + 1%
    public int intelligence; //magic_damage + 1; magic_resistance + 3
    public int vitaliy; //maxHP + 5

    [Header("Defensive Stats")]
    public int maxHP;
    public int armor;
    public int evasion;
    public int magicResistance;

    [Header("Offensive Stats")]
    public int damage;
    public int critChance;
    public int critPower;  //critPower = 150% by default

    [Header("Magic Stats")]
    public int fireDamage;
    public int iceDamage;
    public int lightningDamage;

    [Header("Craft Requirements")]
    public List<InventorySlot> requiredCraftMaterials;

    private int statInfoLength;
    //private int minStatInfoLength = 5;

    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitaliy);

        playerStats.maxHP.AddModifier(maxHP);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightningDamage.AddModifier(lightningDamage);
    }

    public void RemoveModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitaliy);

        playerStats.maxHP.RemoveModifier(maxHP);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistance);

        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);

        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightningDamage.RemoveModifier(lightningDamage);
    }

    //will be triggerd in scripts like animationTrigger when attacking enemies
    //private void ExecuteItemEffect(Transform _spawnTransform)
    //{
    //    foreach (var effect in itemEffects)
    //    {
    //        effect.ExecuteEffect(_spawnTransform);
    //    }
    //}

    //public void ExecuteItemEffect_NoHitNeeded()
    //{
    //    foreach (var effect in itemEffects)
    //    {
    //        effect.ExecuteEffect_NoHitNeeded();
    //    }
    //}

    //private void ReleaseSwordArcane()
    //{
    //    foreach (var effect in itemEffects)
    //    {
    //        effect.ReleaseSwordArcane();
    //    }
    //}

    public void RefreshUseState()
    {
        //itemUsed = false;
        //itemLastUseTime = 0;

        foreach (var effect in itemEffects)
        {
            effect.effectUsed = false;
            effect.effectLastUseTime = 0;
        }
    }
    public void ExecuteItemEffect_ConsiderCooldown(Transform _spawnTransform)
    {
        //遍历物品的所有效果
        foreach (var effect in itemEffects)
        {
            //检查每个效果是否可以使用
            bool canUseEffect = Time.time >= effect.effectLastUseTime + effect.effectCooldown;

            //如果效果可以使用，或者效果还没有被使用过
            if (canUseEffect || !effect.effectUsed)
            {
                //执行效果
                effect.ExecuteEffect(_spawnTransform);

                //更新效果的最后使用时间
                effect.effectLastUseTime = Time.time;

                //标记效果已被使用
                effect.effectUsed = true;

                //更新统计信息 UI（例如生命、法力、攻击力等）
                Inventory.instance.UpdateStatUI();

                Debug.Log($"Use Item Effect: {effect.name}");
            }
            else
            {
                //如果效果在冷却中，输出冷却提示（一般都不需要，玩家又不是瞎子
                Debug.Log("Item Effect is in cooldown");
            }
        }
    }

    public void ReleaseSwordArcane_ConsiderCooldown()
    {
        // 这里使用 >= 目的是防止在多个 0 冷却时间效果需要同时执行时
        //但第一个效果后的所有效果都会进入冷却
        foreach (var effect in itemEffects)
        {
            bool canUseEffect = Time.time >= effect.effectLastUseTime + effect.effectCooldown;

            // 如果可以使用效果，或者效果未被使用过
            if (canUseEffect || !effect.effectUsed)
            {
                effect.ReleaseSwordArcane();
                effect.effectLastUseTime = Time.time; //更新效果上次使用时间
                effect.effectUsed = true; //标记效果已被使用
                Inventory.instance.UpdateStatUI(); //更新物品状态UI
                Debug.Log($"Use Sword Arcane: {effect.name}"); //输出调试信息
            }
            else
            {
                Debug.Log("Item Effect is in cooldown"); //输出调试信息：效果正在冷却中
            }
        }
    }

    public override string GetItemStatInfoAndEffectDescription()
    {
        sb.Length = 0; // 清空 StringBuilder
        statInfoLength = 0; // 重置统计信息的行数

        // 添加各项属性信息
        AddItemStatInfo(strength, "Strength");
        AddItemStatInfo(agility, "Agility");
        AddItemStatInfo(intelligence, "Intelligence");
        AddItemStatInfo(vitaliy, "Vitality");

        AddItemStatInfo(damage, "Damage");
        AddItemStatInfo(critChance, "Crit Chance");
        AddItemStatInfo(critPower, "Crit Power");

        AddItemStatInfo(maxHP, "Max HP");
        AddItemStatInfo(evasion, "Evasion");
        AddItemStatInfo(armor, "Armor");
        AddItemStatInfo(magicResistance, "Magic Resist");

        AddItemStatInfo(fireDamage, "Fire Dmg");
        AddItemStatInfo(iceDamage, "Ice Dmg");
        AddItemStatInfo(lightningDamage, "Lightning Dmg");

        // 如果有物品效果并且有属性信息
        if (itemEffects.Length > 0 && statInfoLength > 0)
        {
            // 如果第一个效果有描述信息，添加空行分隔属性信息和效果描述
            if (itemEffects[0].effectDescription.Length > 0)
            {
                sb.AppendLine();
            }
        }

        // 遍历所有物品效果，添加效果描述
        for (int i = 0; i < itemEffects.Length; i++)
        {
            sb.AppendLine();

            if (LanguageManager.instance.localeID == 0) //英文
            {
                if (itemEffects[i].effectDescription.Length > 0)
                {
                    sb.Append($"[unique effect]\n{itemEffects[i].effectDescription}\n");
                }
            }
            else if (LanguageManager.instance.localeID == 1) //中文
            {
                if (itemEffects[i].effectDescription_Chinese.Length > 0)
                {
                    sb.Append($"[固有效果]\n{itemEffects[i].effectDescription_Chinese}\n");
                }
            }

            statInfoLength++; //增加属性信息的行数
        }

        // 确保“固有效果”下面的空行与属性信息下方的空行一致
        if (sb.ToString()[sb.Length - 1] == '\n')
        {
            sb.Remove(sb.Length - 1, 1); // 移除末尾的换行符
        }

        // 如果属性信息行数少于最小行数，添加空行
        //if (statInfoLength < minStatInfoLength)
        //{
        //    int _numberOfLinesToApped = minStatInfoLength - statInfoLength;
        //    if (statInfoLength == 0)
        //    {
        //        _numberOfLinesToApped--;
        //    }
        //    for (int i = 0; i < _numberOfLinesToApped; i++)
        //    {
        //        sb.AppendLine();
        //        sb.Append("");
        //    }
        //}

        sb.AppendLine();
        sb.Append(""); // 添加空行
        sb.AppendLine();
        sb.Append(""); // 添加空行

        return sb.ToString(); // 返回最终的字符串
    }

    private void AddItemStatInfo(int _statValue, string _statName)
    {
        // 如果属性值不为 0，添加属性信息
        if (_statValue != 0)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine(); //已经有内容，添加换行
            }

            //属性值大于0，显示加号
            if (_statValue > 0)
            {
                sb.Append($"+ {_statValue} {_statName}");
                statInfoLength++; //增加属性信息的行数
            }
        }
    }

}
