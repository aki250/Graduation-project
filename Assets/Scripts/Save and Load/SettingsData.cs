using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    [Header("��������")]
    public SerializableDictionary<string, float> volumeSettingsDictionary;

    [Header("��λ����")]
    public SerializableDictionary<string, KeyCode> keybindsDictionary;

    [Header("��Ϸ�淨����")]
    public SerializableDictionary<string, bool> gameplayToggleSettingsDictionary;

    [Header("��������")]
    public int localeID; 

    public SettingsData()
    {
        //����Ĭ��������
        localeID = 1;

        //��ʼ�����������ֵ�
        volumeSettingsDictionary = new SerializableDictionary<string, float>();

        //��ʼ����λ���ֵ䣬������Ĭ�ϼ�λ
        keybindsDictionary = new SerializableDictionary<string, KeyCode>();
        SetupDefaultKeybinds();

        //��ʼ����Ϸ�淨�����ֵ�
        gameplayToggleSettingsDictionary = new SerializableDictionary<string, bool>();
    }

    private void SetupDefaultKeybinds()
    {
        keybindsDictionary.Add("Attack", KeyCode.Mouse0);
        keybindsDictionary.Add("Aim", KeyCode.Mouse1);
        keybindsDictionary.Add("Flask", KeyCode.Alpha1);
        keybindsDictionary.Add("Dash", KeyCode.LeftShift);
        keybindsDictionary.Add("Parry", KeyCode.Q);
        keybindsDictionary.Add("Crystal", KeyCode.F);
        keybindsDictionary.Add("Blackhole", KeyCode.R);
        keybindsDictionary.Add("Character", KeyCode.C);
        keybindsDictionary.Add("Craft", KeyCode.B);
        keybindsDictionary.Add("Skill", KeyCode.K);
    }
}
