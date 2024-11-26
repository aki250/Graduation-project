using System.Collections;
using System.Collections.Generic;
using UnityEngine;

                                                //���Ʒ���Spell ���������м��������߼���
public class SpellController : MonoBehaviour
{
    //��⹥����Χ�����
    [SerializeField] private Transform check;

    //��ײ�еĳߴ磬���ڼ�ⷶΧ
    [SerializeField] private Vector2 boxSize;

    //ָ���������Ŀ���
    [SerializeField] private LayerMask whatIsPlayer;

    //�����ߵ�״̬���󣬴洢�����������ԣ����˺�ֵ��
    private CharacterStats enemyStats;

    //��ʼ�������Ĳ���
    public void SetupSpell(CharacterStats _enemyStats)
    {
        enemyStats = _enemyStats; //������״̬
    }

    //�ڶ�����������ã����ڼ�Ⲣ�˺���ҡ�
    private void AnimationTrigger()
    {
        // ʹ�� Physics2D.OverlapBoxAll ��ⷶΧ�ڵ���ײ��
        Collider2D[] colliders = Physics2D.OverlapBoxAll(check.position, boxSize, 0, whatIsPlayer);

        // �������м�⵽����ײ��
        foreach (var hit in colliders)
        {
            //��ײ�������
            if (hit.GetComponent<Player>() != null)
            {
                //���������˺������ù����ߵ��˺��߼�
                enemyStats.DoDamge(hit.GetComponent<PlayerStats>());
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(check.position, boxSize);
    }

    private void SelfDestroy()
    {
        Destroy(gameObject); 
    }
}
