using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AnimationTrigger : MonoBehaviour
{
    //��ȡ�������ϵ�Enemy���
    private Enemy enemy => GetComponentInParent<Enemy>();

    //�������������������˵Ķ����¼�
    private void AnimationTrigger()
    {
        enemy.AnimationTrigger();
    }

    //�������������ڶ�������ʱ����Ƿ�������ܵ�����
    private void AttackTrigger()
    {
        //��ȡ���˹�����Χ�ڵ�������ײ��
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);

        //����������ײ��
        foreach (var hit in colliders)
        {
            //�����ײ�������
            if (hit.GetComponent<Player>() != null)
            {
                Player player = hit.GetComponent<Player>();

                // ������Ե�������ܵ��˺��ķ���
                //player.Damage(enemy.transform, player.transform);

                // ��ȡ��ҵ� Stats ����������˺�����
                PlayerStats _target = hit.GetComponent<PlayerStats>();
                enemy.stats.DoDamge(_target);  //���˶��������˺�
            }
        }
    }

    //���⹥��������
    private void SpecialAttackTrigger()
    {
        //���õ��˵����⹥������
        enemy.SpecialAttackTrigger();
    }

    //�򿪷�������
    private void OpenCounterAttackWindow()
    {
        //���õ��˴򿪷������ڵķ���
        enemy.OpenCounterAttackWindow();
    }

    //�رշ�������
    private void CloseCounterAttackWindow()
    {
        enemy.CloseCounterAttackWindow();
    }
}
