using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class CloneSkill : Skill
{
    //存储克隆体的攻击伤害倍增值
    private float currentCloneAttackDamageMultipler;

    [Header("克隆体")]
    [SerializeField] private GameObject clonePrefab;    //克隆体预设
    [SerializeField] private float cloneDuration;    //克隆体存在时间
    [SerializeField] private float colorLosingSpeed;    //克隆体消失衰减速度

    [Header("解锁克隆体攻击能力o")]
    [SerializeField] private SkillTreeSlot_UI mirageAttackUnlockButton;
    [Range(0f, 1f)]    //克隆体攻击伤害倍率
    [SerializeField] private float cloneAttackDamageMultiplier;
    // 表示克隆体攻击是否已解锁的状态
    public bool mirageAttackUnlocked { get; private set; }

    [Header("克隆体攻击解锁")] //进攻型克隆体解锁信息，进攻型克隆体会造成更多伤害并且可以应用命中效果
    [SerializeField] private SkillTreeSlot_UI aggressiveMirageUnlockButton; //进攻型克隆体解锁按钮
    //进攻型克隆体攻击伤害的倍增器
    [Range(0f, 1f)]
    [SerializeField] private float aggressiveCloneAttackDamageMultiplier;
    // 表示进攻型克隆体是否解锁
    public bool aggressiveMirageUnlocked { get; private set; }
    // 进攻型克隆体是否能够应用命中效果
    public bool aggressiveCloneCanApplyOnHitEffect { get; private set; }

    [Header("多重克隆体解锁按钮")] 
    [SerializeField] private SkillTreeSlot_UI multipleMirageUnlockButton;
    [Range(0f, 1f)]    //多重克隆体攻击伤害倍增器，           默认值是玩家的30%伤害
    [SerializeField] private float duplicateCloneAttackDamageMultiplier;
    public bool multipleMirageUnlocked { get; private set; }    //是否解锁多重克隆体功能
    [SerializeField] private float duplicatePossibility;    //克隆体复制可能性
    public int maxDuplicateCloneAmount;    //最大允许复制克隆体的数量
    [HideInInspector] public int currentDuplicateCloneAmount;    //当前复制克隆体数量

    [Header("水晶克隆体")]
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

    //防止创建无尽的克隆体
    public void RefreshCurrentDuplicateCloneAmount()
    {
        currentDuplicateCloneAmount = 0;
    }

    //如果启用了通过水晶替换克隆体功能，则不再创建克隆体
    public void CreateClone(Vector3 _position)
    {

        // 如果水晶克隆体已经解锁，则不再创建克隆体
        if (crystalMirageUnlocked)
        {
            // 下面的代码块用于禁用“水晶替换克隆体”功能
            // 如果水晶技能已经解锁且正在使用，将禁用替换克隆体
            //SkillManager.instance.crystal.crystalMirageUnlocked = false;

            // 防止创建多个水晶
            //SkillManager.instance.crystal.DestroyCurrentCrystal_InCrystalMirageOnly();

            // 如果水晶技能已准备好使用，立即使用该技能
            if (SkillManager.instance.crystal.SkillIsReadyToUse())
            {
                SkillManager.instance.crystal.UseSkillIfAvailable();
            }
            return; // 退出，避免创建克隆体
        }

        RefreshCurrentDuplicateCloneAmount();

        //创建一个新的克隆体
        GameObject newClone = Instantiate(clonePrefab, _position, Quaternion.identity);
        //获取新克隆体的控制脚本
        CloneSkillController newCloneScript = newClone.GetComponent<CloneSkillController>();

        //设置新克隆体的相关属性
        newCloneScript.SetupClone(
            cloneDuration,         
            colorLosingSpeed,    
            mirageAttackUnlocked, 
            FindClosestEnemy(newClone.transform),   //查找最近的敌人
            multipleMirageUnlocked, 
            duplicatePossibility,   
            currentCloneAttackDamageMultipler   
        );
    }

    public void CreateDuplicateClone(Vector3 _position)
    {
        //创建一个重复克隆体
        GameObject newClone = Instantiate(clonePrefab, _position, Quaternion.identity);
        //获取新克隆体的控制脚本
        CloneSkillController newCloneScript = newClone.GetComponent<CloneSkillController>();

        //设置新克隆体的相关属性
        newCloneScript.SetupClone(
            cloneDuration,        
            colorLosingSpeed,    
            mirageAttackUnlocked,
            FindClosestEnemy(newClone.transform), //查找最近的敌人
            multipleMirageUnlocked, 
            duplicatePossibility, 
            currentCloneAttackDamageMultipler
        );

        //防止创建无尽的重复克隆体
        currentDuplicateCloneAmount++;
    }

    //创建一个带延迟的克隆体
    public void CreateCloneWithDelay(Vector3 _position, float _delay)
    {
        //启动协程来处理延迟克隆体的创建
        StartCoroutine(CreateCloneWithDelay_Coroutine(_position, _delay));
    }

    //在延迟后创建克隆体
    private IEnumerator CreateCloneWithDelay_Coroutine(Vector3 _position, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);

        //在延迟后创建克隆体
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
    //镜像攻击
    private void UnlockMirageAttack()
    {
        if (mirageAttackUnlocked)
        {
            return;
        }

        if (mirageAttackUnlockButton.unlocked)
        {
            mirageAttackUnlocked = true;
            currentCloneAttackDamageMultipler = cloneAttackDamageMultiplier;    //当前克隆体的攻击伤害倍率为初始值
        }
    }

    //攻击性克隆体
    private void UnlockAggressiveMirage()
    {
        if (aggressiveMirageUnlocked)
        {
            return;
        }

        if (aggressiveMirageUnlockButton.unlocked)
        {
            aggressiveMirageUnlocked = true;
            aggressiveCloneCanApplyOnHitEffect = true;  //使克隆体可以应用击中效果
            //设置当前克隆体攻击伤害倍率为攻击性镜像倍率
            currentCloneAttackDamageMultipler = aggressiveCloneAttackDamageMultiplier;
        }
    }

    //多重镜像
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

    //水晶镜像
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
