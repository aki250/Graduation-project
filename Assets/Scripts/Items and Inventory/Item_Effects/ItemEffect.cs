using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemEffect : ScriptableObject
{
    // ItemEffect ����һ���ɱ�������Ч���������ڲ�ͬ������±������������ˡ��������ˡ�ʹ����Ʒ�ȡ�
    // 1. ������ܵ��˺�ʱ - �� PlayerStats.DecreaseHPBy() �����д���
    // 2. ����ҹ�������ʱ - �� PlayerAnimationTrigger.AttackTrigger() �����д���
    // 3. ������ͷŽ�������������𽣣�ʱ - �� PlayerAnimationTrigger.AttackTrigger() �����д���
    // 4. �����ʹ����Ʒʱ����ҩˮ������ƿ�ȣ� - �� Player.Update() �����д���
    // 5. ����ҵ�ħ�����е���ʱ������֮��������Ч����ˮ���� - �� CrystalSkillController.Explosion() �����д���

    public bool effectUsed { get; set; }  //��ʾЧ���Ƿ�ʹ��
    public float effectLastUseTime { get; set; }  //��¼Ч���ϴ�ʹ��ʱ��
    public float effectCooldown;  //Ч����ȴʱ��

    [TextArea] 
    public string effectDescription;  //Ӣ��˵��Ч������
    [TextArea] 
    public string effectDescription_Chinese;  //����˵��Ч��

    //ִ��Ч������
    public virtual void ExecuteEffect(Transform _spawnTransform)
    {
    }

    //����Ҫ����˷�����ײʱʹ��
    public virtual void ExecuteEffect_NoHitNeeded()
    {
    }

    //�ͷŽ����ȼ���Ч��ʱ�Ĵ�������
    public virtual void ReleaseSwordArcane()
    {
    }
}
