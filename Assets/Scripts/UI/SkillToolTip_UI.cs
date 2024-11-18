using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillToolTip_UI : MonoBehaviour
{
    //��������
    [SerializeField] private TextMeshProUGUI skillName;
    //��������
    [SerializeField] private TextMeshProUGUI skillDescription;
    //���ܼ۸� 
    [SerializeField] private TextMeshProUGUI skillPrice;

    public void ShowToolTip(string _skillName, string _skillDescription, string _skillPrice)
    {
        //��������
        skillName.text = _skillName;
        //��������
        skillDescription.text = _skillDescription;

        //���ݵ�ǰ�����л�
        if (LanguageManager.instance.localeID == 0) //Ӣ��
        {
            skillPrice.text = $"Skill price: {_skillPrice}";
        }
        else if (LanguageManager.instance.localeID == 1) //����
        {
            skillPrice.text = $"���ܼ۸�: {_skillPrice}";
        }

        //������ʾ�����Ϸ����ʹ���ڽ�������ʾ
        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        //������ʾ��ʹ��ӽ�������ʧ
        gameObject.SetActive(false);
    }
}
