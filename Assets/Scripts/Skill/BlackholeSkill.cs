using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation; 
using UnityEngine;
using UnityEngine.UI;

                                                                    //�ڶ�����
public class BlackholeSkill : Skill
{
    [Header("Blackhole Unlock Info")]
    [SerializeField] private SkillTreeSlot_UI blackholeUnlockButton; //�ڶ����ܽ�����ť�� UI ���
    public bool blackholeUnlocked { get; private set; } //�ڶ������Ƿ��ѽ���

    [SerializeField] private GameObject blackholePrefab; //�ڶ�Ч����Ԥ����
    [Space]
    [SerializeField] private float maxSize; //�ڶ�������С
    [SerializeField] private float growSpeed; //�ڶ��������ٶ�
    [SerializeField] private float shrinkSpeed; //�ڶ���С���ٶ�
    [Space]
    [SerializeField] private int cloneAttackAmount; //��¡�幥���Ĵ���
    [SerializeField] private float cloneAttackCooldown; //��¡�幥������ȴʱ��
    [Space]
    [SerializeField] private float QTEInputWindow; //����ʱ���¼���QTE�������봰��ʱ��

    private GameObject currentBlackhole; //��ǰ����ĺڶ�����
    private BlackholeSkillController currentBlackholeScript; //��ǰ�ڶ�����Ŀ�����

    protected override void Start()
    {
        base.Start();

        //Ϊ�ڶ�������ť��ӵ���¼������������ʱ���� UnlockBlackhole ����
        blackholeUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockBlackhole);
    }

    protected override void Update()
    {
        base.Update(); 
    }

    public override void UseSkill()
    {
        base.UseSkill();

        //ʵ�����ڶ�Ԥ���壬��������λ�ú���ת
        currentBlackhole = Instantiate(blackholePrefab, player.transform.position, Quaternion.identity);

        //��ȡ�ڶ�����Ŀ������������úڶ����ܵĲ���
        currentBlackholeScript = currentBlackhole.GetComponent<BlackholeSkillController>();
        currentBlackholeScript.SetupBlackholeSkill(maxSize, growSpeed, shrinkSpeed, cloneAttackAmount, cloneAttackCooldown, QTEInputWindow);

        //������Ч
        AudioManager.instance.PlaySFX(3, player.transform);
        AudioManager.instance.PlaySFX(6, player.transform);
    }

    public override bool UseSkillIfAvailable()
    {
        return base.UseSkillIfAvailable();
    }

    public bool CanExitBlackholeSkill()
    {
        //�����ǰû�м���ĺڶ����ܣ��򷵻� false
        if (currentBlackholeScript == null)
        {
            return false;
        }

        //�����¡�幥���Ѿ���ɣ��������ǰ�ڶ����ܵĿ������������� true
        if (currentBlackholeScript.CloneAttackHasFinished())
        {
            currentBlackholeScript = null;
            return true;
        }

        return false;
    }

    protected override void CheckUnlockFromSave()
    {
        UnlockBlackhole();
    }

    #region Unlock Skill
    private void UnlockBlackhole()
    {
        //����ڶ������Ѿ���������ֱ�ӷ���
        if (blackholeUnlocked)
        {
            return;
        }

        //������ť�Ѿ��������򽫺ڶ���������Ϊ�ѽ���
        if (blackholeUnlockButton.unlocked)
        {
            blackholeUnlocked = true;
        }
    }
    #endregion
}