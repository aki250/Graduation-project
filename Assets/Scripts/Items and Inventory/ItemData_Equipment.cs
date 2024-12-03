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
    public EquipmentType equipmentType; //装备类型，指定该物品是武器、护甲等

    [Header("物品效果")]
    public ItemEffect[] itemEffects; //物品的效果数组

    [Header("主属性")]
    public int strength;  //力量，物理伤害 +1   暴击力量 +1% 
    public int agility;  //敏捷，闪避率 +1%   暴击几率 +1%
    public int intelligence; //智力，魔法伤害 +1  魔法抗性 +3
    public int vitaliy; //生命力，最大生命值 +5

    [Header("防御属性")]
    public int maxHP;   //最大生命值
    public int armor;   //护甲，减少物理伤害的比例
    public int evasion; //闪避，增加避免被攻击的概率
    public int magicResistance; //魔抗，减少所受魔法伤害的比例

    [Header("物理伤害")]
    public int damage;  //基础物理伤害
    public int critChance;  //暴击率
    public int critPower;  //暴伤，暴击时伤害的倍数（默认150%）

    [Header("法伤")]
    public int fireDamage;  //火伤
    public int iceDamage;   //冰伤
    public int lightningDamage; //电

    [Header("制作材料要求")]
    //列表，指定制作该物品所需的材料，每个材料包含一个物品槽（InventorySlot）
    public List<InventorySlot> requiredCraftMaterials;

    private int statInfoLength;


    //为玩家增加属性值
    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        //根据装备的属性，添加到玩家的各个属性上
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

    //为玩家减少属性值
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

    //刷新物品使用状态，将所有效果的使用状态和最后使用时间重置
    public void RefreshUseState()
    {
        //将所有 效果  的使用状态设为false，并重置最后使用时间
        foreach (var effect in itemEffects)
        {
            effect.effectUsed = false; 
            effect.effectLastUseTime = 0;  
        }
    }

    //根据冷却时间来执行物品的效果
    public void ExecuteItemEffect_ConsiderCooldown(Transform _spawnTransform)
    {
        //遍历物品效果
        foreach (var effect in itemEffects)
        {
            //检查每个效果是否可以使用（当前时间 >= 上次使用时间 + 冷却时间）
            bool canUseEffect = Time.time >= effect.effectLastUseTime + effect.effectCooldown;

            //如果效果可以使用，或效果还没有被使用过
            if (canUseEffect || !effect.effectUsed)
            {
                //执行该效果
                effect.ExecuteEffect(_spawnTransform);

                //最后使用时间为当前时间
                effect.effectLastUseTime = Time.time;

                //标记效果为已使用
                effect.effectUsed = true;

                //更新统计信息
                Inventory.instance.UpdateStatUI();

                //Debug.Log($"Use Item Effect: {effect.name}");
            }
        }
    }


    public void ReleaseSwordArcane_ConsiderCooldown()
    {
        //这里使用>=目的是防止在多个0冷却时间效果需要同时执行时
        //但第一个效果后的所有效果都会进入冷却
        foreach (var effect in itemEffects)
        {
            bool canUseEffect = Time.time >= effect.effectLastUseTime + effect.effectCooldown;

            //如果可以使用效果，或者效果未被使用过
            if (canUseEffect || !effect.effectUsed)
            {
                effect.ReleaseSwordArcane();
                effect.effectLastUseTime = Time.time; //更新效果上次使用时间
                effect.effectUsed = true; //标记效果已被使用
                Inventory.instance.UpdateStatUI(); //更新物品状态UI

            }

        }
    }
    // 获取物品的统计信息和效果描述
    public override string GetItemStatInfoAndEffectDescription()
    {
        sb.Length = 0; //清空StringBuilder，重新构建内容
        statInfoLength = 0; //重置统计信息的行数

        //添加各项属性信息到StringBuilder
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

        //如果物品有效果且有属性信息
        if (itemEffects.Length > 0 && statInfoLength > 0)
        {
            //第一个效果有描述信息，添加空行分隔属性信息和效果描述
            if (itemEffects[0].effectDescription.Length > 0)
            {
                sb.AppendLine(); //添加换行符
            }
        }

        //遍历所有物品效果，添加每个效果的描述信息
        for (int i = 0; i < itemEffects.Length; i++)
        {
            sb.AppendLine(); //添加换行符

            //判断当前语言是英文还是中文
            if (LanguageManager.instance.localeID == 0) //英文
            {
                if (itemEffects[i].effectDescription.Length > 0)
                {
                    sb.Append($"[unique effect]\n{itemEffects[i].effectDescription}\n");
                }
            }
            else if (LanguageManager.instance.localeID == 1) // 中文
            {
                if (itemEffects[i].effectDescription_Chinese.Length > 0)
                {
                    sb.Append($"[固有效果]\n{itemEffects[i].effectDescription_Chinese}\n");
                }
            }

            statInfoLength++; //增加属性信息行数
        }

        //确保"固有效果"下面的空行与属性信息下方的空行一致
        if (sb.ToString()[sb.Length - 1] == '\n')
        {
            sb.Remove(sb.Length - 1, 1); //移除末尾的换行符，防止多余空行
        }

        //在字符串末尾添加空行，以保持格式
        sb.AppendLine();
        sb.Append(""); 
        sb.AppendLine();
        sb.Append(""); 

        return sb.ToString(); //返回最终属性信息和效果描述
    }

    private void AddItemStatInfo(int _statValue, string _statName)
    {
        //如果属性值不为0，添加属性信息
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
