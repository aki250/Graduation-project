using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashSkill : Skill
{
    [Header("解锁冲刺")]
    public bool dashUnlocked;  //是否已解锁冲刺
    [SerializeField] private SkillTreeSlot_UI dashUnlockButton;  //解锁冲刺技能按钮

    [Header("冲刺开始时生成克隆")]
    public bool cloneOnDashStartUnlocked;  //解锁在冲刺开始时生成克隆
    [SerializeField] private SkillTreeSlot_UI cloneOnDashStartUnlockButton;  //解锁冲刺开始时生成克隆的按钮UI

    [Header("冲刺开始时解锁生成克隆")]
    public bool cloneOnDashEndUnlocked;  //是否解锁在冲刺结束时生成克隆
    [SerializeField] private SkillTreeSlot_UI cloneOnDashEndUnlockButton;  // 解锁冲刺结束时生成克隆的按钮UI

    protected override void Start()
    {
        base.Start(); 

        //解锁冲刺、冲刺开始生成克隆、冲刺结束生成克隆的按钮添加点击监听事件
        dashUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockDash);
        cloneOnDashStartUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCloneOnDashStart);
        cloneOnDashEndUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCloneOnDashEnd);
    }

    //冲刺技能使用方法
    public override void UseSkill()
    {
        base.UseSkill();
    }

    //在冲刺开始时生成克隆
    public void CloneOnDashStart(Vector3 _position)
    {
        if (cloneOnDashStartUnlocked)
        {
            //如果冲刺开始时的克隆技能解锁，则在指定位置创建克隆
            SkillManager.instance.clone.CreateClone(_position);
        }
    }

    //在冲刺结束时生成克隆
    public void CloneOnDashEnd(Vector3 _position)
    {
        if (cloneOnDashEndUnlocked)
        {
            // 如果冲刺结束时的克隆技能解锁，则在指定位置创建克隆
            SkillManager.instance.clone.CreateClone(_position);
        }
    }

    //从存档中检查并解锁技能
    protected override void CheckUnlockFromSave()
    {
        UnlockDash();  //检查并解锁冲刺技能
        UnlockCloneOnDashStart();  //检查并解锁冲刺开始时的克隆技能
        UnlockCloneOnDashEnd();  //检查并解锁冲刺结束时的克隆技能
    }

    #region Unlock Skill
    //解锁冲刺技能
    private void UnlockDash()
    {
        if (dashUnlocked)
        {
            return;  //如果已解锁冲刺技能，则无需再次解锁
        }

        if (dashUnlockButton.unlocked)
        {
            dashUnlocked = true;  //设置冲刺技能解锁状态
        }
    }

    //解锁冲刺开始时，生成克隆体
    private void UnlockCloneOnDashStart()
    {
        if (cloneOnDashStartUnlocked)
        {
            return;
        }

        if (cloneOnDashStartUnlockButton.unlocked)
        {
            cloneOnDashStartUnlocked = true;  //设置冲刺开始时的克隆技能，为已解锁
        }
    }

    //解锁冲刺结束时，生成克隆体
    private void UnlockCloneOnDashEnd()
    {
        if (cloneOnDashEndUnlocked)
        {
            return; 
        }

        if (cloneOnDashEndUnlockButton.unlocked)
        {
            cloneOnDashEndUnlocked = true;  //已解锁，设置冲刺结束时的克隆技能为已解锁
        }
    }
    #endregion
}
