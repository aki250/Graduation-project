using System;
using UnityEngine;

public class Options_SubUI : MonoBehaviour
{
    // ���л��ֶΣ��������ò�ͬ��ѡ��˵�����Ϸѡ���λ�����������ԣ�
    [SerializeField] private GameObject GameplayOptions; 
    [SerializeField] private GameObject KeybindOptions;    
    [SerializeField] private GameObject SoundOptions;       
    [SerializeField] private GameObject LanguageOptions;   

    // Start��������Ϸ��ʼʱ����
    private void Start()
    {
        //��ʼ��ʱ��Ϸѡ��˵�
        SwitchToOptions(GameplayOptions);
    }

    //�л���ָ����ѡ��˵������ر��������в˵�
    public void SwitchToOptions(GameObject _optionsMenu)
    {
        //�ر�������UI������������Ϊ���ɼ�
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        //��ѡ�еĲ˵�������˵���Ϊ��
        if (_optionsMenu != null)
        {
            _optionsMenu.SetActive(true);  //��ʾѡ�еĲ˵�
            AudioManager.instance.PlaySFX(7, null);  //�����л��˵�ʱ����Ч
        }
    }
}

