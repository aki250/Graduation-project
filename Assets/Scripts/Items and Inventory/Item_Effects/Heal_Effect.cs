using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal Effect", menuName = "Data/Item Effect/Heal Effect")]
public class Heal_Effect : ItemEffect
{
    //HealPercent用于设置玩家最大生命值的百分比，表示恢复多少生命值，从0到1，0不恢复生命，1恢复全部生命值
    [Range(0f, 1f)]
    [SerializeField] private float healPercent;

    public override void ExecuteEffect(Transform _enemyTransform)
    {
        //获取玩家PlayerStats
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        //取整数，玩家最大生命值与恢复百分比乘积
        int healHP = Mathf.RoundToInt(playerStats.getMaxHP() * healPercent);    
        playerStats.IncreaseHPBy(healHP);
    }

}
