using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class CloneSkill : Skill
{
    //�洢��¡��Ĺ����˺�����ֵ
    private float currentCloneAttackDamageMultipler;

    [Header("��¡��")]
    [SerializeField] private GameObject clonePrefab;    //��¡��Ԥ��
    [SerializeField] private float cloneDuration;    //��¡�����ʱ��
    [SerializeField] private float colorLosingSpeed;    //��¡����ʧ˥���ٶ�

    [Header("������¡�幥������o")]
    [SerializeField] private SkillTreeSlot_UI mirageAttackUnlockButton;
    [Range(0f, 1f)]    //��¡�幥���˺�����
    [SerializeField] private float cloneAttackDamageMultiplier;
    // ��ʾ��¡�幥���Ƿ��ѽ�����״̬
    public bool mirageAttackUnlocked { get; private set; }

    [Header("��¡�幥������")] //�����Ϳ�¡�������Ϣ�������Ϳ�¡�����ɸ����˺����ҿ���Ӧ������Ч��
    [SerializeField] private SkillTreeSlot_UI aggressiveMirageUnlockButton; //�����Ϳ�¡�������ť
    //�����Ϳ�¡�幥���˺��ı�����
    [Range(0f, 1f)]
    [SerializeField] private float aggressiveCloneAttackDamageMultiplier;
    // ��ʾ�����Ϳ�¡���Ƿ����
    public bool aggressiveMirageUnlocked { get; private set; }
    // �����Ϳ�¡���Ƿ��ܹ�Ӧ������Ч��
    public bool aggressiveCloneCanApplyOnHitEffect { get; private set; }

    [Header("���ؿ�¡�������ť")] 
    [SerializeField] private SkillTreeSlot_UI multipleMirageUnlockButton;
    [Range(0f, 1f)]    //���ؿ�¡�幥���˺���������           Ĭ��ֵ����ҵ�30%�˺�
    [SerializeField] private float duplicateCloneAttackDamageMultiplier;
    public bool multipleMirageUnlocked { get; private set; }    //�Ƿ�������ؿ�¡�幦��
    [SerializeField] private float duplicatePossibility;    //��¡�帴�ƿ�����
    public int maxDuplicateCloneAmount;    //��������ƿ�¡�������
    [HideInInspector] public int currentDuplicateCloneAmount;    //��ǰ���ƿ�¡������

    [Header("ˮ����¡��")]
    [SerializeField] private SkillTreeSlot_UI crystalMirageUnlockButton;
    public bool crystalMirageUnlocked { get; private set; }


    protected override void Start()
    {
        base.Start();

        mirageAttackUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockMirageAttack);
        aggressiveMirageUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockAggressiveMirage);
        multipleMirageUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockMultipleMirage);
        crystalMirageUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCrystalMirage);
    }

    //��ֹ�����޾��Ŀ�¡��
    public void RefreshCurrentDuplicateCloneAmount()
    {
        currentDuplicateCloneAmount = 0;
    }

    //���������ͨ��ˮ���滻��¡�幦�ܣ����ٴ�����¡��
    public void CreateClone(Vector3 _position)
    {

        // ���ˮ����¡���Ѿ����������ٴ�����¡��
        if (crystalMirageUnlocked)
        {
            // ����Ĵ�������ڽ��á�ˮ���滻��¡�塱����
            // ���ˮ�������Ѿ�����������ʹ�ã��������滻��¡��
            //SkillManager.instance.crystal.crystalMirageUnlocked = false;

            // ��ֹ�������ˮ��
            //SkillManager.instance.crystal.DestroyCurrentCrystal_InCrystalMirageOnly();

            // ���ˮ��������׼����ʹ�ã�����ʹ�øü���
            if (SkillManager.instance.crystal.SkillIsReadyToUse())
            {
                SkillManager.instance.crystal.UseSkillIfAvailable();
            }
            return; // �˳������ⴴ����¡��
        }

        RefreshCurrentDuplicateCloneAmount();

        //����һ���µĿ�¡��
        GameObject newClone = Instantiate(clonePrefab, _position, Quaternion.identity);
        //��ȡ�¿�¡��Ŀ��ƽű�
        CloneSkillController newCloneScript = newClone.GetComponent<CloneSkillController>();

        //�����¿�¡����������
        newCloneScript.SetupClone(
            cloneDuration,         
            colorLosingSpeed,    
            mirageAttackUnlocked, 
            FindClosestEnemy(newClone.transform),   //��������ĵ���
            multipleMirageUnlocked, 
            duplicatePossibility,   
            currentCloneAttackDamageMultipler   
        );
    }

    public void CreateDuplicateClone(Vector3 _position)
    {
        //����һ���ظ���¡��
        GameObject newClone = Instantiate(clonePrefab, _position, Quaternion.identity);
        //��ȡ�¿�¡��Ŀ��ƽű�
        CloneSkillController newCloneScript = newClone.GetComponent<CloneSkillController>();

        //�����¿�¡����������
        newCloneScript.SetupClone(
            cloneDuration,        
            colorLosingSpeed,    
            mirageAttackUnlocked,
            FindClosestEnemy(newClone.transform), //��������ĵ���
            multipleMirageUnlocked, 
            duplicatePossibility, 
            currentCloneAttackDamageMultipler
        );

        //��ֹ�����޾����ظ���¡��
        currentDuplicateCloneAmount++;
    }

    //����һ�����ӳٵĿ�¡��
    public void CreateCloneWithDelay(Vector3 _position, float _delay)
    {
        //����Э���������ӳٿ�¡��Ĵ���
        StartCoroutine(CreateCloneWithDelay_Coroutine(_position, _delay));
    }

    //���ӳٺ󴴽���¡��
    private IEnumerator CreateCloneWithDelay_Coroutine(Vector3 _position, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);

        //���ӳٺ󴴽���¡��
        CreateClone(_position);
    }


    protected override void CheckUnlockFromSave()
    {
        UnlockMirageAttack();
        UnlockAggressiveMirage();
        UnlockCrystalMirage();
        UnlockMultipleMirage();
    }

    #region Unlock Skill
    //���񹥻�
    private void UnlockMirageAttack()
    {
        if (mirageAttackUnlocked)
        {
            return;
        }

        if (mirageAttackUnlockButton.unlocked)
        {
            mirageAttackUnlocked = true;
            currentCloneAttackDamageMultipler = cloneAttackDamageMultiplier;    //��ǰ��¡��Ĺ����˺�����Ϊ��ʼֵ
        }
    }

    //�����Կ�¡��
    private void UnlockAggressiveMirage()
    {
        if (aggressiveMirageUnlocked)
        {
            return;
        }

        if (aggressiveMirageUnlockButton.unlocked)
        {
            aggressiveMirageUnlocked = true;
            aggressiveCloneCanApplyOnHitEffect = true;  //ʹ��¡�����Ӧ�û���Ч��
            //���õ�ǰ��¡�幥���˺�����Ϊ�����Ծ�����
            currentCloneAttackDamageMultipler = aggressiveCloneAttackDamageMultiplier;
        }
    }

    //���ؾ���
    private void UnlockMultipleMirage()
    {
        if (multipleMirageUnlocked)
        {
            return;
        }

        if (multipleMirageUnlockButton.unlocked)
        {
            multipleMirageUnlocked = true;
            currentCloneAttackDamageMultipler = duplicateCloneAttackDamageMultiplier;
        }
    }

    //ˮ������
    private void UnlockCrystalMirage()
    {
        if (crystalMirageUnlocked)
        {
            return;
        }

        if (crystalMirageUnlockButton.unlocked)
        {
            crystalMirageUnlocked = true;
        }
    }
    #endregion
}
