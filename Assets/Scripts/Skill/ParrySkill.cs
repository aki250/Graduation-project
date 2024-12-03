using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParrySkill : Skill
{
    [Header("招架技能")]
    [SerializeField] private SkillTreeSlot_UI parryUnlockButton;  //招架技能解锁按钮
    public bool parryUnlocked { get; private set; }  //是否解锁了招架技能

    [Header("招架恢复HP")]
    [SerializeField] private SkillTreeSlot_UI parryRecoverUnlockButton;  //招架恢复HP/FP技能解锁按钮
    public bool parryRecoverUnlocked { get; private set; }  //是否解锁招架恢复技能
    [Range(0f, 1f)]
    [SerializeField] private float recoverPercentage;  //恢复的百分比

    [Header("带幻影的招架技")]
    [SerializeField] private SkillTreeSlot_UI parryWithMirageUnlockButton;  //带幻影的招架技能解锁按钮
    public bool parryWithMirageUnlocked { get; private set; }  //是否解锁了带幻影的招架技能


    protected override void Start()
    {
        base.Start();

        parryUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockParry);
        parryRecoverUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockParryRecover);
        parryWithMirageUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockParryWithMirage);
    }
    public override void UseSkill()
    {
        //使用技能时切换到反击状态
        player.stateMachine.ChangeState(player.counterAttackState);
    }

    public override bool UseSkillIfAvailable()
    {
        return base.UseSkillIfAvailable();                      //如果技能可用，执行基础的技能检查
    }

    // 在成功招架后恢复HP
    public void RecoverHPFPInSuccessfulParry()
    {
        if (parryRecoverUnlocked == true)
        {
            //计算恢复的HP量
            int recoverAmount = Mathf.RoundToInt(player.stats.getMaxHP() * recoverPercentage);
            player.stats.IncreaseHPBy(recoverAmount);  //增加HP
        }
    }

    //在成功招架后创建幻影
    public void MakeMirageInSuccessfulParry(Vector3 _cloneSpawnPosition)
    {
        if (parryWithMirageUnlocked == true)
        {
            //创建幻影，延迟0.1秒
            SkillManager.instance.clone.CreateCloneWithDelay(_cloneSpawnPosition, 0.1f);
        }
    }

    //检查从保存数据中是否已解锁技能
    protected override void CheckUnlockFromSave()
    {
        UnlockParry();
        UnlockParryRecover();
        UnlockParryWithMirage();
    }

    #region 解锁技能
    //解锁招架技能
    private void UnlockParry()
    {
        if (parryUnlocked)
        {
            return;
        }

        if (parryUnlockButton.unlocked == true)
        {
            parryUnlocked = true;  //标记招架技能已解锁
        }
    }

    //解锁招架恢复技能
    private void UnlockParryRecover()
    {
        if (parryRecoverUnlocked)
        {
            return;
        }

        if (parryRecoverUnlockButton.unlocked == true)
        {
            parryRecoverUnlocked = true;  //标记招架恢复技能已解锁
        }
    }

    //解锁带幻影招架技能
    private void UnlockParryWithMirage()
    {
        if (parryWithMirageUnlocked)
        {
            return;
        }

        if (parryWithMirageUnlockButton.unlocked == true)
        {
            parryWithMirageUnlocked = true;  //标记带幻影招架技能已解锁
        }
    }
    #endregion

}
