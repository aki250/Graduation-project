using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DodgeSkill : Skill
{
    [Header("闪避技能解锁信息")]
    [SerializeField] private SkillTreeSlot_UI dodgeUnlockButton;  //闪避技能解锁按钮
    [SerializeField] private int evasionIncreasement;  //闪避增加量
    public bool dodgeUnlocked;  //是否解锁了闪避技能

    [Header("幻影闪避技能")]
    [SerializeField] private SkillTreeSlot_UI mirageDodgeUnlockButton;  //幻影闪避技能解锁按钮
    public bool mirageDodgeUnlocked;  //是否解锁幻影闪避

    protected override void Start()
    {
        base.Start();

        //为按钮添加点击事件监听器
        dodgeUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockDodge);
        mirageDodgeUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockMirageDodge);
    }

    //闪避时创建幻影
    public void CreateMirageOnDodge()
    {
        if (mirageDodgeUnlocked)
        {
            //解锁了幻影闪避，则创建幻影
            SkillManager.instance.clone.CreateClone(new Vector3(player.transform.position.x + 2 * player.facingDirection, player.transform.position.y));
        }
    }

    //检查保存的数据并解锁技能
    protected override void CheckUnlockFromSave()
    {
        UnlockDodge();
        UnlockMirageDodge();
    }

    #region 解锁技能
    //解锁闪避技能
    private void UnlockDodge()
    {
        if (dodgeUnlocked)
        {
            return;
        }

        if (dodgeUnlockButton.unlocked)
        {
            dodgeUnlocked = true;
            player.stats.evasion.AddModifier(evasionIncreasement);  //增加闪避属性
            //解锁闪避技能后立即更新玩家的状态UI
            Inventory.instance.UpdateStatUI();
        }
    }

    //解锁幻影闪避技能
    private void UnlockMirageDodge()
    {
        if (mirageDodgeUnlocked)
        {
            return;
        }

        if (mirageDodgeUnlockButton.unlocked)
        {
            mirageDodgeUnlocked = true;  //标记为已解锁幻影闪避
        }
    }
    #endregion
}
