using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeybindOptionController : MonoBehaviour
{
    //��ʾ��Ϊ����UI
    [SerializeField] private TextMeshProUGUI behaveName_InUI;
    //��ʾ����������UI
    [SerializeField] protected TextMeshProUGUI behaveKeybind_InUI;
    //�������󶨵İ�ť
    [SerializeField] private Button keybindButton;

    //������ͻ��ʾ����Ԥ����
    [Space]
    [SerializeField] private GameObject keybindConflictPromptWindowPrefab;

    //���ڴ洢��Ϊ���ƺͰ����󶨵��ַ���������Ĭ����Ӣ�ģ�
    private string behaveName;
    private string behaveKeybind;

    private void Start()
    {
        //Ϊ��ť��ӵ���¼�������������ť�����ʱִ��ChangeKeybind����
        keybindButton.onClick.AddListener(ChangeKeybind);
    }

    //���ð���ѡ���ʼ����Ϊ���ƺͰ����󶨣�������UI
    public void SetupKeybindOption(string _behaveName, string _behaveKeybind)
    {
        behaveName = _behaveName;
        behaveKeybind = _behaveKeybind;

        //������Ϊ���ƺͰ�������UI�ϵ���ʾ
        behaveName_InUI.text = _behaveName;
        behaveKeybind_InUI.text = _behaveKeybind;

        //������Ϊ���Ʋ���׼������������
        TranslateBehaveNameAndUniformBehaveKeybindName();
    }

    //���ݵ�ǰ���Ի���������Ϊ���ƣ�����׼�����������ơ�
    public void TranslateBehaveNameAndUniformBehaveKeybindName()
    {
        // �������Ϊ���ģ�������Ϊ����
        if (LanguageManager.instance.localeID == 1)
        {
            behaveName_InUI.text = LanguageManager.instance.EnglishToChineseKeybindsDictionary[behaveName];
        }
        // �������ΪӢ�ģ�ֱ����ʾ��Ϊ����
        else if (LanguageManager.instance.localeID == 0)
        {
            behaveName_InUI.text = behaveName;
        }

        // ��׼�������밴��������
        behaveKeybind_InUI.text = UniformKeybindName(behaveKeybind);
    }


    //��׼�����������ƣ�ȷ����ͬ���Ի�����ͳһ��ʾ��
    private string UniformKeybindName(string _behaveKeybind_InUI)
    {
                                            //ȥ��Alphaǰ׺����Ϊ���������ְ���
        if (_behaveKeybind_InUI.StartsWith("Alpha"))
        {
            _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 5);
        }

        //�������ΪӢ�ģ���׼����������
        if (LanguageManager.instance.localeID == 0)
        {
            if (_behaveKeybind_InUI.Equals("Mouse0"))
            {
                _behaveKeybind_InUI = "Mouse Left";
            }

            if (_behaveKeybind_InUI.Equals("Mouse1"))
            {
                _behaveKeybind_InUI = "Mouse Right";
            }

            if (_behaveKeybind_InUI.StartsWith("Left"))
            {
                _behaveKeybind_InUI = _behaveKeybind_InUI.Insert(4, " ");
            }
        }
        //�������Ϊ���ģ���׼����������
        else if (LanguageManager.instance.localeID == 1)
        {
            if (_behaveKeybind_InUI.Equals("Mouse0"))
            {
                _behaveKeybind_InUI = "������";
            }

            if (_behaveKeybind_InUI.Equals("Mouse1"))
            {
                _behaveKeybind_InUI = "����Ҽ�";
            }

            // ����������� "Left" ��ͷ������ "LeftShift"������ "Left" �滻Ϊ "��"
            if (_behaveKeybind_InUI.StartsWith("Left"))
            {
                _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 4);
                _behaveKeybind_InUI = _behaveKeybind_InUI.Insert(0, "��");
            }
        }

        return _behaveKeybind_InUI;
    }

    //�����󶨰�ť�����ʱ����ʼִ�а������ĵĹ��̡�
    public void ChangeKeybind()
    {
        // ����Э�����ȴ��û������µİ���
        StartCoroutine(ChangeKeybindInput());
    }

    //�ȴ��û������µİ����󶨡�
    private IEnumerator ChangeKeybindInput()
    {
        // ��ʾ�ȴ��������ʾ��Ϣ
        if (LanguageManager.instance.localeID == 0)
        {
            behaveKeybind_InUI.text = "Awaiting input";
        }
        else if (LanguageManager.instance.localeID == 1)
        {
            behaveKeybind_InUI.text = "�ȴ����밴��";
        }

        //�����������룬ȷ���û�ֻ�������°���
        UI.instance?.EnableUIKeyInput(false);
        MainMenu_UI.instance?.EnableKeyInput(false);

        //�ȴ�ֱ���û�����ĳ������
        yield return new WaitUntil(CheckInput);

        //�Ƴ���ť�����м���������ֹ�ظ�����
        keybindButton.onClick.RemoveAllListeners();

        //���¼�������е����м���ͼ���ı�
        if (SkillPanel_InGame_UI.instance != null)
        {
            SkillPanel_InGame_UI.instance?.UpdateAllSkillIconTexts();
        }

        //�ȴ�ֱ��û�а���������
        yield return new WaitWhile(HasAnyKey);

        //�ָ���ť�Ľ������ܲ�������Ӽ�����
        keybindButton.interactable = true;
        keybindButton.onClick.AddListener(ChangeKeybind);

        //�ָ�����Ĺ���
        UI.instance?.EnableUIKeyInput(true);
        MainMenu_UI.instance?.EnableKeyInput(true);
    }

    //����Ƿ��а��������¡�
    private bool CheckInput()
    {
        keybindButton.interactable = false;

        //���ÿ�����̰����Ƿ񱻰���
        foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
        {
            //���ĳ������������
            if (Input.GetKeyDown(keycode))
            {
                //ȡ����������
                if (keycode == KeyCode.Escape)
                {
                    behaveKeybind_InUI.text = UniformKeybindName(KeyBindManager.instance.keybindsDictionary[behaveName].ToString());
                    Debug.Log("Keybind change cancelled");
                    return true;
                }

                //������ʾΪ�µİ�����
                behaveKeybind_InUI.text = UniformKeybindName(keycode.ToString());
                behaveKeybind = keycode.ToString();
                Debug.Log($"{behaveName_InUI.text} keybind has changed to {keycode.ToString()}");

                //���°������ֵ�
                KeyBindManager.instance.keybindsDictionary[behaveName] = keycode;

                //�������������ͻ����ʾ��ͻ��ʾ����
                if (HasKeybindConflict(keycode))
                {
                    //ʵ����������ͻ��ʾ����
                    GameObject newKeybindConflictPromptWindow = Instantiate(keybindConflictPromptWindowPrefab, transform.parent.parent.parent.parent);

                    //������ͻ��ʾ����
                    newKeybindConflictPromptWindow.GetComponent<KeybindConflictPromptWindowController>()?.SetupKeybindConflictPromptWindow(keycode);
                }

                return true;
            }
        }

        return false;
    }

    //����Ƿ��а��������¡�
    private bool HasAnyKey()
    {
        return Input.anyKey || Input.anyKeyDown;
    }

    //����°����Ƿ������а����󶨳�ͻ��
    private bool HasKeybindConflict(KeyCode _keycode)
    {
        foreach (var search in KeyBindManager.instance.keybindsDictionary)
        {
            //��������İ����󶨳�ͻ
            if (search.Key == behaveName)
            {
                continue;
            }

            //����°�����������Ϊ�İ����󶨳�ͻ
            if (search.Value == _keycode)
            {
                return true;
            }
        }

        return false;
    }
}
