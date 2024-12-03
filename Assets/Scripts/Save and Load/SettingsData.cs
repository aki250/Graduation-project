using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    [Header("声音设置")]
    public SerializableDictionary<string, float> volumeSettingsDictionary;

    [Header("键位设置")]
    public SerializableDictionary<string, KeyCode> keybindsDictionary;

    [Header("游戏玩法设置")]
    public SerializableDictionary<string, bool> gameplayToggleSettingsDictionary;

    [Header("语言设置")]
    public int localeID; 

    public SettingsData()
    {
        //语言默认是中文
        localeID = 1;

        //初始化音量设置字典
        volumeSettingsDictionary = new SerializableDictionary<string, float>();

        //初始化键位绑定字典，并设置默认键位
        keybindsDictionary = new SerializableDictionary<string, KeyCode>();
        SetupDefaultKeybinds();

        //初始化游戏玩法设置字典
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
