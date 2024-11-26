using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Blackhole_HotkeyController : MonoBehaviour
{
    private KeyCode hotkey; //当前QTE
    private TextMeshProUGUI textMesh; //显示QTE文本
    private SpriteRenderer sr; 

    private Transform enemyTransform; //QTE绑定的敌人Transform
    private BlackholeSkillController blackholeScript;       //引用黑洞技能控制器


    public void SetupHotkey(KeyCode _hotkey, Transform _enemyTransform, BlackholeSkillController _blackholeScript)
    {
        //获取子对象中的文本组件，用于显示快捷键
        textMesh = GetComponentInChildren<TextMeshProUGUI>();

        sr = GetComponent<SpriteRenderer>();

        // 绑定敌人的 Transform 和黑洞技能控制器
        enemyTransform = _enemyTransform;
        blackholeScript = _blackholeScript;

        //设置QTE
        hotkey = _hotkey;

        //QTE显示在界面上
        textMesh.text = hotkey.ToString();
    }

    private void Update()
    {
        //检测玩家是否按下了显示QTE
        if (Input.GetKeyDown(hotkey))
        {
            blackholeScript.AddEnemyToList(enemyTransform);

            //按下对应的QTE后，消失
            textMesh.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
