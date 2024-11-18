using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPanel_InGame_UI : MonoBehaviour
{
    public static SkillPanel_InGame_UI instance;

    public GameObject dashIcon;         //Dash图标
    public GameObject parryIcon;        //Parry图标
    public GameObject crystalIcon;      //Crystal图标
    public GameObject throwSwordIcon;   //Throw Sword图标
    public GameObject blackholeIcon;    //Blackhole技能图标

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        //当面板启用时，根据技能解锁状态显示技能图标
        ShowAllSkillIconsAccordingToUnlockState();
    }

    private void Start()
    {
        HideAllSkillIcons();        //隐藏所有技能图标，确保面板加载时不会出现问题

        // 根据技能解锁状态显示技能图标
        ShowAllSkillIconsAccordingToUnlockState();

        // 更新所有技能图标的文本信息
        UpdateAllSkillIconTexts();
    }

    // 更新所有技能图标的文本内容
    public void UpdateAllSkillIconTexts()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            // 遍历每个子物体，并更新其对应的技能图标文本
            transform?.GetChild(i)?.GetComponentInChildren<SkillIconText_InGame_UI>()?.UpdateSkillIconText();
        }
    }

    //隐藏所有技能图标
    private void HideAllSkillIcons()
    {
        dashIcon.SetActive(false);    
        parryIcon.SetActive(false);     
        crystalIcon.SetActive(false);   
        throwSwordIcon.SetActive(false); 
        blackholeIcon.SetActive(false);  
    }

    //根据技能解锁状态显示对应的技能图标
    public void ShowAllSkillIconsAccordingToUnlockState()
    {
        //Dash已解锁，显示Dash图标，否则隐藏
        if (SkillManager.instance.dash.dashUnlocked)
        {
            dashIcon.SetActive(true);
        }
        else
        {
            dashIcon.SetActive(false);
        }

        if (SkillManager.instance.parry.parryUnlocked)
        {
            parryIcon.SetActive(true);
        }
        else
        {
            parryIcon.SetActive(false);
        }

        if (SkillManager.instance.crystal.crystalUnlocked)
        {
            crystalIcon.SetActive(true);
        }
        else
        {
            crystalIcon.SetActive(false);
        }

        if (SkillManager.instance.sword.throwSwordSkillUnlocked)
        {
            throwSwordIcon.SetActive(true);
        }
        else
        {
            throwSwordIcon.SetActive(false);
        }

        if (SkillManager.instance.blackhole.blackholeUnlocked)
        {
            blackholeIcon.SetActive(true);
        }
        else
        {
            blackholeIcon.SetActive(false);
        }
    }
}
