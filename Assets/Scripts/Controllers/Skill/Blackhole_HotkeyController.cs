using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Blackhole_HotkeyController : MonoBehaviour
{
    private KeyCode hotkey; //��ǰQTE
    private TextMeshProUGUI textMesh; //��ʾQTE�ı�
    private SpriteRenderer sr; 

    private Transform enemyTransform; //QTE�󶨵ĵ���Transform
    private BlackholeSkillController blackholeScript;       //���úڶ����ܿ�����


    public void SetupHotkey(KeyCode _hotkey, Transform _enemyTransform, BlackholeSkillController _blackholeScript)
    {
        //��ȡ�Ӷ����е��ı������������ʾ��ݼ�
        textMesh = GetComponentInChildren<TextMeshProUGUI>();

        sr = GetComponent<SpriteRenderer>();

        // �󶨵��˵� Transform �ͺڶ����ܿ�����
        enemyTransform = _enemyTransform;
        blackholeScript = _blackholeScript;

        //����QTE
        hotkey = _hotkey;

        //QTE��ʾ�ڽ�����
        textMesh.text = hotkey.ToString();
    }

    private void Update()
    {
        //�������Ƿ�������ʾQTE
        if (Input.GetKeyDown(hotkey))
        {
            blackholeScript.AddEnemyToList(enemyTransform);

            //���¶�Ӧ��QTE����ʧ
            textMesh.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
