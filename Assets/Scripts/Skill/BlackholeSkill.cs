using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation; 
using UnityEngine;
using UnityEngine.UI;

                                                                    //黑洞技能
public class BlackholeSkill : Skill
{
    [Header("Blackhole Unlock Info")]
    [SerializeField] private SkillTreeSlot_UI blackholeUnlockButton; //黑洞技能解锁按钮的 UI 组件
    public bool blackholeUnlocked { get; private set; } //黑洞技能是否已解锁

    [SerializeField] private GameObject blackholePrefab; //黑洞效果的预制体
    [Space]
    [SerializeField] private float maxSize; //黑洞的最大大小
    [SerializeField] private float growSpeed; //黑洞增长的速度
    [SerializeField] private float shrinkSpeed; //黑洞缩小的速度
    [Space]
    [SerializeField] private int cloneAttackAmount; //克隆体攻击的次数
    [SerializeField] private float cloneAttackCooldown; //克隆体攻击的冷却时间
    [Space]
    [SerializeField] private float QTEInputWindow; //快速时间事件（QTE）的输入窗口时间

    private GameObject currentBlackhole; //当前激活的黑洞对象
    private BlackholeSkillController currentBlackholeScript; //当前黑洞对象的控制器

    protected override void Start()
    {
        base.Start();

        //为黑洞解锁按钮添加点击事件监听器，点击时调用 UnlockBlackhole 方法
        blackholeUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockBlackhole);
    }

    protected override void Update()
    {
        base.Update(); 
    }

    public override void UseSkill()
    {
        base.UseSkill();

        //实例化黑洞预制体，并设置其位置和旋转
        currentBlackhole = Instantiate(blackholePrefab, player.transform.position, Quaternion.identity);

        //获取黑洞对象的控制器，并设置黑洞技能的参数
        currentBlackholeScript = currentBlackhole.GetComponent<BlackholeSkillController>();
        currentBlackholeScript.SetupBlackholeSkill(maxSize, growSpeed, shrinkSpeed, cloneAttackAmount, cloneAttackCooldown, QTEInputWindow);

        //播放音效
        AudioManager.instance.PlaySFX(3, player.transform);
        AudioManager.instance.PlaySFX(6, player.transform);
    }

    public override bool UseSkillIfAvailable()
    {
        return base.UseSkillIfAvailable();
    }

    public bool CanExitBlackholeSkill()
    {
        //如果当前没有激活的黑洞技能，则返回 false
        if (currentBlackholeScript == null)
        {
            return false;
        }

        //如果克隆体攻击已经完成，则清除当前黑洞技能的控制器，并返回 true
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
        //如果黑洞技能已经解锁，则直接返回
        if (blackholeUnlocked)
        {
            return;
        }

        //解锁按钮已经解锁，则将黑洞技能设置为已解锁
        if (blackholeUnlockButton.unlocked)
        {
            blackholeUnlocked = true;
        }
    }
    #endregion
}