using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal Effect", menuName = "Data/Item Effect/Heal Effect")]
public class Heal_Effect : ItemEffect
{
    //HealPercent������������������ֵ�İٷֱȣ���ʾ�ָ���������ֵ����0��1��0���ָ�������1�ָ�ȫ������ֵ
    [Range(0f, 1f)]
    [SerializeField] private float healPercent;

    public override void ExecuteEffect(Transform _enemyTransform)
    {
        //��ȡ���PlayerStats
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        //ȡ����������������ֵ��ָ��ٷֱȳ˻�
        int healHP = Mathf.RoundToInt(playerStats.getMaxHP() * healPercent);    
        playerStats.IncreaseHPBy(healHP);
    }

}
