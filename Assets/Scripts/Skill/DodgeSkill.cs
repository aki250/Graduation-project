using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DodgeSkill : Skill
{
    [Header("���ܼ��ܽ�����Ϣ")]
    [SerializeField] private SkillTreeSlot_UI dodgeUnlockButton;  //���ܼ��ܽ�����ť
    [SerializeField] private int evasionIncreasement;  //����������
    public bool dodgeUnlocked;  //�Ƿ���������ܼ���

    [Header("��Ӱ���ܼ���")]
    [SerializeField] private SkillTreeSlot_UI mirageDodgeUnlockButton;  //��Ӱ���ܼ��ܽ�����ť
    public bool mirageDodgeUnlocked;  //�Ƿ������Ӱ����

    protected override void Start()
    {
        base.Start();

        //Ϊ��ť��ӵ���¼�������
        dodgeUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockDodge);
        mirageDodgeUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockMirageDodge);
    }

    //����ʱ������Ӱ
    public void CreateMirageOnDodge()
    {
        if (mirageDodgeUnlocked)
        {
            //�����˻�Ӱ���ܣ��򴴽���Ӱ
            SkillManager.instance.clone.CreateClone(new Vector3(player.transform.position.x + 2 * player.facingDirection, player.transform.position.y));
        }
    }

    //��鱣������ݲ���������
    protected override void CheckUnlockFromSave()
    {
        UnlockDodge();
        UnlockMirageDodge();
    }

    #region ��������
    //�������ܼ���
    private void UnlockDodge()
    {
        if (dodgeUnlocked)
        {
            return;
        }

        if (dodgeUnlockButton.unlocked)
        {
            dodgeUnlocked = true;
            player.stats.evasion.AddModifier(evasionIncreasement);  //������������
            //�������ܼ��ܺ�����������ҵ�״̬UI
            Inventory.instance.UpdateStatUI();
        }
    }

    //������Ӱ���ܼ���
    private void UnlockMirageDodge()
    {
        if (mirageDodgeUnlocked)
        {
            return;
        }

        if (mirageDodgeUnlockButton.unlocked)
        {
            mirageDodgeUnlocked = true;  //���Ϊ�ѽ�����Ӱ����
        }
    }
    #endregion
}
