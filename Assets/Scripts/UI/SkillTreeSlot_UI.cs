using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTreeSlot_UI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IGameProgressionSaveManager
{
    [SerializeField] private int skillPrice;    //技能价格

    //绑定名称列表，用于描述技能与特定行为的绑定关系
    [SerializeField] private List<string> boundBehaveNameList;

    [Header("英文")]
    [SerializeField] private string skillName;

    [TextArea]
    [SerializeField] private string skillDescription;

    [Header("中文")]
    [SerializeField] private string skillName_Chinese;
    [TextArea]
    [SerializeField] private string skillDescription_Chinese;

    [Space]
    //技能被锁定颜色
    [SerializeField] private Color lockedSkillColor;

    //技能是否解锁
    public bool unlocked;

    //技能树UI中，该技能解锁的界面元素
    [SerializeField] private SkillTreeSlot_UI[] shouldBeUnlocked;

    //技能树UI中，该技能锁定的界面元素
    [SerializeField] private SkillTreeSlot_UI[] shouldBeLocked;

    //技能图像
    private Image skillImage;

    //更新UI显示
    private UI ui;

                                              //当对象属性改变时，这个方法会被调用。
    private void OnValidate()
    {
        gameObject.name = $"SkillTreeSlot_UI - {skillName}";
    }

    private void Awake()
    {
        //获取当前对象的Image组件，用于修改技能图标的显示效果
        skillImage = GetComponent<Image>();

        //获取父体组件
        ui = GetComponentInParent<UI>();
    }


    private void Start()
    {
        //默认技能图标的颜色是锁定颜色
        skillImage.color = lockedSkillColor;

        //如果技能解锁，技能图标为白色
        if (unlocked)
        {
            skillImage.color = Color.white;
        }
    }

    public void UnlockSkill()
    {   
        //测试技能使用，正常注销
        if (unlocked)
        {
            //取消技能
            unlocked = false; //标记为未解锁
            skillImage.color = lockedSkillColor; //更新图标为锁定颜色
            Debug.Log($"技能已取消学习: {skillName}");

            //如果有前置技能依赖本技能，需要锁定它们
            foreach (var dependentSkill in shouldBeUnlocked)
            {
                if (dependentSkill.unlocked)
                {
                    dependentSkill.UnlockSkill(); //递归锁定依赖技能
                }
            }
            return;
        }

        //检查前置技能是否已解锁
        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (shouldBeUnlocked[i].unlocked == false)
            {
                Debug.Log("前置技能尚未解锁！");
                return;
            }
        }

        //检查是否有互斥技能已解锁
        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            if (shouldBeLocked[i].unlocked == true)
            {
                Debug.Log("替代技能已解锁，当前技能无法解锁");
                return;
            }
        }

        //检查玩家货币支付技能费用
        if (PlayerManager.instance.BuyIfAvailable(skillPrice) == false)
        {
            return;
        }

        //解锁技能
        unlocked = true; //标记为已解锁
        skillImage.color = Color.white; //更新图标颜色为已解锁
        Debug.Log($"成功解锁技能: {skillName}");
    }


    //自定义按键
    private string AddBehaveKeybindNameToDescription(string _skillDescription)
    {
        //遍历绑定行为名称列表，替换技能描述中的占位符
        for (int i = 0; i < boundBehaveNameList.Count; i++)
        {
            // 如果技能描述中包含对应的占位符
            if (_skillDescription.Contains($"BehaveName{i}"))
            {
                //获取对应行为名称的按键绑定
                string _keybindName = KeyBindManager.instance.keybindsDictionary[boundBehaveNameList[i]].ToString();

                //标准化按键绑定名称
                _keybindName = KeyBindManager.instance.UniformKeybindName(_keybindName);

                //将技能描述中的占位符替换为对应的按键绑定名称
                _skillDescription = _skillDescription.Replace($"BehaveName{i}", _keybindName);
            }
        }

        //返回更新后的技能描述
        return _skillDescription;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //计算技能图标位置的偏移量，用于调整提示框的显示位置
        //调整位置时，给定了四个偏移值：左、右、上、下各自的偏移（0.15f）
        Vector2 offset = ui.SetupToolTipPositionOffsetAccordingToUISlotPosition(transform, 0.15f, 0.15f, 0.15f, 0.15f);

        //更新技能提示框的位置，将其显示在技能图标旁边，使用偏移量进行微调
        ui.skillToolTip.transform.position = new Vector2(transform.position.x + offset.x, transform.position.y + offset.y);

        //获取技能描述并替换其中的按键绑定名称
        string completedSkillDescription = AddBehaveKeybindNameToDescription(skillDescription);
        string completedSkillDescription_Chinese = AddBehaveKeybindNameToDescription(skillDescription_Chinese);

        //根据当前语言设置显示对应的技能名称和描述
        if (LanguageManager.instance.localeID == 0)
        {
            //如果是英文，显示英文名称和描述
            ui.skillToolTip.ShowToolTip(skillName, completedSkillDescription, skillPrice.ToString());
        }
        else if (LanguageManager.instance.localeID == 1)
        {
            //如果是中文，显示中文名称和描述
            ui.skillToolTip.ShowToolTip(skillName_Chinese, completedSkillDescription_Chinese, skillPrice.ToString());
        }
    }

    //鼠标离开技能图标，隐藏技能提示框
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.HideToolTip();
    }

    //点技能图标，解锁对应技能
    public void OnPointerDown(PointerEventData eventData)
    {
        UnlockSkill();
    }

    //加载技能树数据
    public void LoadData(GameData _data)
    {
        //从保存的游戏数据中加载技能树数据
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            //游戏数据中包含该技能的信息，则加载该技能解锁状态
            unlocked = value;
        }
    }

    //保存技能树数据
    public void SaveData(ref GameData _data)
    {
        //将技能的解锁状态保存到游戏数据中
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            //游戏数据中已经存在该技能，更新其解锁状态
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, unlocked);
        }
        else
        {
            //游戏数据中没有该技能，添加它并保存解锁状态
            _data.skillTree.Add(skillName, unlocked);
        }
    }

}
