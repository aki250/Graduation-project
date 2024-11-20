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

    //英文描述部分
    [Header("英文")]
    //技能名称英文
    [SerializeField] private string skillName;
    //技能描述英文
    [TextArea]
    [SerializeField] private string skillDescription;

    //中文描述部分
    [Header("中文")]
    //技能名称中文
    [SerializeField] private string skillName_Chinese;
    //技能描述中文
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
        //检查前置技能是否已解锁
        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            //任何一个前置技能未解锁，则打印提示并返回
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

        //确保技能没有重复解锁
        if (unlocked)
        {
            Debug.Log("您已经解锁过该技能！");
            return;
        }

        //检查玩家货币支付技能费用
        if (PlayerManager.instance.BuyIfAvailable(skillPrice) == false)
        {
            return; 
        }

        unlocked = true; //将技能标记为已解锁
        skillImage.color = Color.white; //更新图标颜色
        Debug.Log($"成功解锁技能: {skillName}");
    }

    //自定义按键
    private string AddBehaveKeybindNameToDescription(string _skillDescription)
    {
        //遍历绑定行为名称列表，替换技能描述中的占位符
        for (int i = 0; i < boundBehaveNameList.Count; i++)
        {
            // 如果技能描述中包含对应的占位符（BehaveName0、BehaveName1 等）
            if (_skillDescription.Contains($"BehaveName{i}"))
            {
                //获取对应行为名称的按键绑定（通过行为名称从字典中获取）
                string _keybindName = KeyBindManager.instance.keybindsDictionary[boundBehaveNameList[i]].ToString();

                //标准化按键绑定名称（例如：将"Mouse0"转换为"LMB"等）
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
    public void OnPointerExit(PointerEventData eventData)
    {
        //鼠标离开技能图标，隐藏技能提示框
        ui.skillToolTip.HideToolTip();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //鼠标点击技能图标，解锁该技能
        UnlockSkill();
    }

    public void LoadData(GameData _data)
    {
        //从保存的游戏数据中加载技能树数据
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            //游戏数据中包含该技能的信息，则加载该技能解锁状态
            unlocked = value;
        }
    }

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
