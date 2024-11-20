using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashSkill : Skill
{
    [Header("�������")]
    public bool dashUnlocked;  //�Ƿ��ѽ������
    [SerializeField] private SkillTreeSlot_UI dashUnlockButton;  //������̼��ܰ�ť

    [Header("��̿�ʼʱ���ɿ�¡")]
    public bool cloneOnDashStartUnlocked;  //�����ڳ�̿�ʼʱ���ɿ�¡
    [SerializeField] private SkillTreeSlot_UI cloneOnDashStartUnlockButton;  //������̿�ʼʱ���ɿ�¡�İ�ťUI

    [Header("��̿�ʼʱ�������ɿ�¡")]
    public bool cloneOnDashEndUnlocked;  //�Ƿ�����ڳ�̽���ʱ���ɿ�¡
    [SerializeField] private SkillTreeSlot_UI cloneOnDashEndUnlockButton;  // ������̽���ʱ���ɿ�¡�İ�ťUI

    protected override void Start()
    {
        base.Start(); 

        //������̡���̿�ʼ���ɿ�¡����̽������ɿ�¡�İ�ť��ӵ�������¼�
        dashUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockDash);
        cloneOnDashStartUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCloneOnDashStart);
        cloneOnDashEndUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCloneOnDashEnd);
    }

    //��̼���ʹ�÷���
    public override void UseSkill()
    {
        base.UseSkill();
    }

    //�ڳ�̿�ʼʱ���ɿ�¡
    public void CloneOnDashStart(Vector3 _position)
    {
        if (cloneOnDashStartUnlocked)
        {
            //�����̿�ʼʱ�Ŀ�¡���ܽ���������ָ��λ�ô�����¡
            SkillManager.instance.clone.CreateClone(_position);
        }
    }

    //�ڳ�̽���ʱ���ɿ�¡
    public void CloneOnDashEnd(Vector3 _position)
    {
        if (cloneOnDashEndUnlocked)
        {
            // �����̽���ʱ�Ŀ�¡���ܽ���������ָ��λ�ô�����¡
            SkillManager.instance.clone.CreateClone(_position);
        }
    }

    //�Ӵ浵�м�鲢��������
    protected override void CheckUnlockFromSave()
    {
        UnlockDash();  //��鲢������̼���
        UnlockCloneOnDashStart();  //��鲢������̿�ʼʱ�Ŀ�¡����
        UnlockCloneOnDashEnd();  //��鲢������̽���ʱ�Ŀ�¡����
    }

    #region Unlock Skill
    //������̼���
    private void UnlockDash()
    {
        if (dashUnlocked)
        {
            return;  //����ѽ�����̼��ܣ��������ٴν���
        }

        if (dashUnlockButton.unlocked)
        {
            dashUnlocked = true;  //���ó�̼��ܽ���״̬
        }
    }

    //������̿�ʼʱ�����ɿ�¡��
    private void UnlockCloneOnDashStart()
    {
        if (cloneOnDashStartUnlocked)
        {
            return;
        }

        if (cloneOnDashStartUnlockButton.unlocked)
        {
            cloneOnDashStartUnlocked = true;  //���ó�̿�ʼʱ�Ŀ�¡���ܣ�Ϊ�ѽ���
        }
    }

    //������̽���ʱ�����ɿ�¡��
    private void UnlockCloneOnDashEnd()
    {
        if (cloneOnDashEndUnlocked)
        {
            return; 
        }

        if (cloneOnDashEndUnlockButton.unlocked)
        {
            cloneOnDashEndUnlocked = true;  //�ѽ��������ó�̽���ʱ�Ŀ�¡����Ϊ�ѽ���
        }
    }
    #endregion
}
