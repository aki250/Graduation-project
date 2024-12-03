using System.Collections;
using System.Collections.Generic;
using UnityEngine;
                    //�����������ֵ����50%�����᷶Χ�ڵĵ���һ��ʱ�䡣

[CreateAssetMenu(fileName = "Freeze Enemy Effect", menuName = "Data/Item Effect/Freeze Enemy Effect")]
public class FreezeEnemy_Effect : ItemEffect
{
    [SerializeField] private float freezeDuration;  //����Ч������ʱ��

    public override void ExecuteEffect(Transform _spawnTransform)
    {
        //��ҵ�ǰ����ֵ����50%������������Ч��
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        if (playerStats.currentHP > playerStats.getMaxHP() * 0.5)
        {
            return;
        }

        //��ȡһ����_spawnTransform ΪԲ�ġ��뾶Ϊ2��Բ,��Χ�ڵ�������ײ��
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_spawnTransform.position, 2);
        foreach (var hit in colliders)
        {
            //��ײ�����Enemy������򴥷�����Ч��
            if (hit.GetComponent<Enemy>() != null)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                enemy.FreezeEnemyForTime(freezeDuration);
            }
        }
    }
}
