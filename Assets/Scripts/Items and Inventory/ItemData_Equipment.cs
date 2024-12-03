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
    public EquipmentType equipmentType; //װ�����ͣ�ָ������Ʒ�����������׵�

    [Header("��ƷЧ��")]
    public ItemEffect[] itemEffects; //��Ʒ��Ч������

    [Header("������")]
    public int strength;  //�����������˺� +1   �������� +1% 
    public int agility;  //���ݣ������� +1%   �������� +1%
    public int intelligence; //������ħ���˺� +1  ħ������ +3
    public int vitaliy; //���������������ֵ +5

    [Header("��������")]
    public int maxHP;   //�������ֵ
    public int armor;   //���ף����������˺��ı���
    public int evasion; //���ܣ����ӱ��ⱻ�����ĸ���
    public int magicResistance; //ħ������������ħ���˺��ı���

    [Header("�����˺�")]
    public int damage;  //���������˺�
    public int critChance;  //������
    public int critPower;  //���ˣ�����ʱ�˺��ı�����Ĭ��150%��

    [Header("����")]
    public int fireDamage;  //����
    public int iceDamage;   //����
    public int lightningDamage; //��

    [Header("��������Ҫ��")]
    //�б�ָ����������Ʒ����Ĳ��ϣ�ÿ�����ϰ���һ����Ʒ�ۣ�InventorySlot��
    public List<InventorySlot> requiredCraftMaterials;

    private int statInfoLength;


    //Ϊ�����������ֵ
    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        //����װ�������ԣ���ӵ���ҵĸ���������
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

    //Ϊ��Ҽ�������ֵ
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

    //ˢ����Ʒʹ��״̬��������Ч����ʹ��״̬�����ʹ��ʱ������
    public void RefreshUseState()
    {
        //������ Ч��  ��ʹ��״̬��Ϊfalse�����������ʹ��ʱ��
        foreach (var effect in itemEffects)
        {
            effect.effectUsed = false; 
            effect.effectLastUseTime = 0;  
        }
    }

    //������ȴʱ����ִ����Ʒ��Ч��
    public void ExecuteItemEffect_ConsiderCooldown(Transform _spawnTransform)
    {
        //������ƷЧ��
        foreach (var effect in itemEffects)
        {
            //���ÿ��Ч���Ƿ����ʹ�ã���ǰʱ�� >= �ϴ�ʹ��ʱ�� + ��ȴʱ�䣩
            bool canUseEffect = Time.time >= effect.effectLastUseTime + effect.effectCooldown;

            //���Ч������ʹ�ã���Ч����û�б�ʹ�ù�
            if (canUseEffect || !effect.effectUsed)
            {
                //ִ�и�Ч��
                effect.ExecuteEffect(_spawnTransform);

                //���ʹ��ʱ��Ϊ��ǰʱ��
                effect.effectLastUseTime = Time.time;

                //���Ч��Ϊ��ʹ��
                effect.effectUsed = true;

                //����ͳ����Ϣ
                Inventory.instance.UpdateStatUI();

                //Debug.Log($"Use Item Effect: {effect.name}");
            }
        }
    }


    public void ReleaseSwordArcane_ConsiderCooldown()
    {
        //����ʹ��>=Ŀ���Ƿ�ֹ�ڶ��0��ȴʱ��Ч����Ҫͬʱִ��ʱ
        //����һ��Ч���������Ч�����������ȴ
        foreach (var effect in itemEffects)
        {
            bool canUseEffect = Time.time >= effect.effectLastUseTime + effect.effectCooldown;

            //�������ʹ��Ч��������Ч��δ��ʹ�ù�
            if (canUseEffect || !effect.effectUsed)
            {
                effect.ReleaseSwordArcane();
                effect.effectLastUseTime = Time.time; //����Ч���ϴ�ʹ��ʱ��
                effect.effectUsed = true; //���Ч���ѱ�ʹ��
                Inventory.instance.UpdateStatUI(); //������Ʒ״̬UI

            }

        }
    }
    // ��ȡ��Ʒ��ͳ����Ϣ��Ч������
    public override string GetItemStatInfoAndEffectDescription()
    {
        sb.Length = 0; //���StringBuilder�����¹�������
        statInfoLength = 0; //����ͳ����Ϣ������

        //��Ӹ���������Ϣ��StringBuilder
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

        //�����Ʒ��Ч������������Ϣ
        if (itemEffects.Length > 0 && statInfoLength > 0)
        {
            //��һ��Ч����������Ϣ����ӿ��зָ�������Ϣ��Ч������
            if (itemEffects[0].effectDescription.Length > 0)
            {
                sb.AppendLine(); //��ӻ��з�
            }
        }

        //����������ƷЧ�������ÿ��Ч����������Ϣ
        for (int i = 0; i < itemEffects.Length; i++)
        {
            sb.AppendLine(); //��ӻ��з�

            //�жϵ�ǰ������Ӣ�Ļ�������
            if (LanguageManager.instance.localeID == 0) //Ӣ��
            {
                if (itemEffects[i].effectDescription.Length > 0)
                {
                    sb.Append($"[unique effect]\n{itemEffects[i].effectDescription}\n");
                }
            }
            else if (LanguageManager.instance.localeID == 1) // ����
            {
                if (itemEffects[i].effectDescription_Chinese.Length > 0)
                {
                    sb.Append($"[����Ч��]\n{itemEffects[i].effectDescription_Chinese}\n");
                }
            }

            statInfoLength++; //����������Ϣ����
        }

        //ȷ��"����Ч��"����Ŀ�����������Ϣ�·��Ŀ���һ��
        if (sb.ToString()[sb.Length - 1] == '\n')
        {
            sb.Remove(sb.Length - 1, 1); //�Ƴ�ĩβ�Ļ��з�����ֹ�������
        }

        //���ַ���ĩβ��ӿ��У��Ա��ָ�ʽ
        sb.AppendLine();
        sb.Append(""); 
        sb.AppendLine();
        sb.Append(""); 

        return sb.ToString(); //��������������Ϣ��Ч������
    }

    private void AddItemStatInfo(int _statValue, string _statName)
    {
        //�������ֵ��Ϊ0�����������Ϣ
        if (_statValue != 0)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine(); //�Ѿ������ݣ���ӻ���
            }

            //����ֵ����0����ʾ�Ӻ�
            if (_statValue > 0)
            {
                sb.Append($"+ {_statValue} {_statName}");
                statInfoLength++; //����������Ϣ������
            }
        }
    }

}
