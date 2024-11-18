using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillToolTip_UI : MonoBehaviour
{
    //技能名称
    [SerializeField] private TextMeshProUGUI skillName;
    //技能描述
    [SerializeField] private TextMeshProUGUI skillDescription;
    //技能价格 
    [SerializeField] private TextMeshProUGUI skillPrice;

    public void ShowToolTip(string _skillName, string _skillDescription, string _skillPrice)
    {
        //技能名称
        skillName.text = _skillName;
        //技能描述
        skillDescription.text = _skillDescription;

        //根据当前语言切换
        if (LanguageManager.instance.localeID == 0) //英文
        {
            skillPrice.text = $"Skill price: {_skillPrice}";
        }
        else if (LanguageManager.instance.localeID == 1) //中文
        {
            skillPrice.text = $"技能价格: {_skillPrice}";
        }

        //激活提示框的游戏对象，使其在界面中显示
        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        //禁用提示框，使其从界面中消失
        gameObject.SetActive(false);
    }
}
