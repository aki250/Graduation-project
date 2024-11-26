using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class DeathBringerAnimationTrigger : Enemy_AnimationTrigger
{
    //��ȡ�������ϵ� DeathBringer ���
    private DeathBringer deathBringer => GetComponentInParent<DeathBringer>();

    //��������
    private void Teleport()
    {
        //����DeathBringer�еķ�����Ѱ�Ҵ���λ��
        deathBringer.FindTeleportPosition();
    }

    //����ʹʵ���͸��
    private void MakeInvisible()
    {
        // ���� DeathBringer ����Ч��������ʵ��͸����
        deathBringer.fx.MakeEntityTransparent(true);
    }

    //����ʹʵ���ɼ�
    private void MakeVisible()
    {
        //����DeathBringer����Ч������ȡ��ʵ���͸��Ч��
        deathBringer.fx.MakeEntityTransparent(false);
    }
}
