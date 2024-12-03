using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParrySkill : Skill
{
    [Header("�мܼ���")]
    [SerializeField] private SkillTreeSlot_UI parryUnlockButton;  //�мܼ��ܽ�����ť
    public bool parryUnlocked { get; private set; }  //�Ƿ�������мܼ���

    [Header("�мָܻ�HP")]
    [SerializeField] private SkillTreeSlot_UI parryRecoverUnlockButton;  //�мָܻ�HP/FP���ܽ�����ť
    public bool parryRecoverUnlocked { get; private set; }  //�Ƿ�����мָܻ�����
    [Range(0f, 1f)]
    [SerializeField] private float recoverPercentage;  //�ָ��İٷֱ�

    [Header("����Ӱ���мܼ�")]
    [SerializeField] private SkillTreeSlot_UI parryWithMirageUnlockButton;  //����Ӱ���мܼ��ܽ�����ť
    public bool parryWithMirageUnlocked { get; private set; }  //�Ƿ�����˴���Ӱ���мܼ���


    protected override void Start()
    {
        base.Start();

        parryUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockParry);
        parryRecoverUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockParryRecover);
        parryWithMirageUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockParryWithMirage);
    }
    public override void UseSkill()
    {
        //ʹ�ü���ʱ�л�������״̬
        player.stateMachine.ChangeState(player.counterAttackState);
    }

    public override bool UseSkillIfAvailable()
    {
        return base.UseSkillIfAvailable();                      //������ܿ��ã�ִ�л����ļ��ܼ��
    }

    // �ڳɹ��мܺ�ָ�HP
    public void RecoverHPFPInSuccessfulParry()
    {
        if (parryRecoverUnlocked == true)
        {
            //����ָ���HP��
            int recoverAmount = Mathf.RoundToInt(player.stats.getMaxHP() * recoverPercentage);
            player.stats.IncreaseHPBy(recoverAmount);  //����HP
        }
    }

    //�ڳɹ��мܺ󴴽���Ӱ
    public void MakeMirageInSuccessfulParry(Vector3 _cloneSpawnPosition)
    {
        if (parryWithMirageUnlocked == true)
        {
            //������Ӱ���ӳ�0.1��
            SkillManager.instance.clone.CreateCloneWithDelay(_cloneSpawnPosition, 0.1f);
        }
    }

    //���ӱ����������Ƿ��ѽ�������
    protected override void CheckUnlockFromSave()
    {
        UnlockParry();
        UnlockParryRecover();
        UnlockParryWithMirage();
    }

    #region ��������
    //�����мܼ���
    private void UnlockParry()
    {
        if (parryUnlocked)
        {
            return;
        }

        if (parryUnlockButton.unlocked == true)
        {
            parryUnlocked = true;  //����мܼ����ѽ���
        }
    }

    //�����мָܻ�����
    private void UnlockParryRecover()
    {
        if (parryRecoverUnlocked)
        {
            return;
        }

        if (parryRecoverUnlockButton.unlocked == true)
        {
            parryRecoverUnlocked = true;  //����мָܻ������ѽ���
        }
    }

    //��������Ӱ�мܼ���
    private void UnlockParryWithMirage()
    {
        if (parryWithMirageUnlocked)
        {
            return;
        }

        if (parryWithMirageUnlockButton.unlocked == true)
        {
            parryWithMirageUnlocked = true;  //��Ǵ���Ӱ�мܼ����ѽ���
        }
    }
    #endregion

}
