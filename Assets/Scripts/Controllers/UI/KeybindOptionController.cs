using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeybindOptionController : MonoBehaviour
{
    //显示行为名称UI
    [SerializeField] private TextMeshProUGUI behaveName_InUI;
    //显示按键绑定名称UI
    [SerializeField] protected TextMeshProUGUI behaveKeybind_InUI;
    //处理按键绑定的按钮
    [SerializeField] private Button keybindButton;

    //按键冲突提示窗口预制体
    [Space]
    [SerializeField] private GameObject keybindConflictPromptWindowPrefab;

    //用于存储行为名称和按键绑定的字符串变量（默认是英文）
    private string behaveName;
    private string behaveKeybind;

    private void Start()
    {
        //为按钮添加点击事件监听器，当按钮被点击时执行ChangeKeybind方法
        keybindButton.onClick.AddListener(ChangeKeybind);
    }

    //设置按键选项，初始化行为名称和按键绑定，并更新UI
    public void SetupKeybindOption(string _behaveName, string _behaveKeybind)
    {
        behaveName = _behaveName;
        behaveKeybind = _behaveKeybind;

        //更新行为名称和按键绑定在UI上的显示
        behaveName_InUI.text = _behaveName;
        behaveKeybind_InUI.text = _behaveKeybind;

        //翻译行为名称并标准化按键绑定名称
        TranslateBehaveNameAndUniformBehaveKeybindName();
    }

    //根据当前语言环境翻译行为名称，并标准化按键绑定名称。
    public void TranslateBehaveNameAndUniformBehaveKeybindName()
    {
        // 如果语言为中文，翻译行为名称
        if (LanguageManager.instance.localeID == 1)
        {
            behaveName_InUI.text = LanguageManager.instance.EnglishToChineseKeybindsDictionary[behaveName];
        }
        // 如果语言为英文，直接显示行为名称
        else if (LanguageManager.instance.localeID == 0)
        {
            behaveName_InUI.text = behaveName;
        }

        // 标准化并翻译按键绑定名称
        behaveKeybind_InUI.text = UniformKeybindName(behaveKeybind);
    }


    //标准化按键绑定名称，确保不同语言环境下统一显示。
    private string UniformKeybindName(string _behaveKeybind_InUI)
    {
                                            //去掉Alpha前缀，因为它代表数字按键
        if (_behaveKeybind_InUI.StartsWith("Alpha"))
        {
            _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 5);
        }

        //如果语言为英文，标准化按键名称
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
        //如果语言为中文，标准化按键名称
        else if (LanguageManager.instance.localeID == 1)
        {
            if (_behaveKeybind_InUI.Equals("Mouse0"))
            {
                _behaveKeybind_InUI = "鼠标左键";
            }

            if (_behaveKeybind_InUI.Equals("Mouse1"))
            {
                _behaveKeybind_InUI = "鼠标右键";
            }

            // 如果按键名以 "Left" 开头（例如 "LeftShift"），将 "Left" 替换为 "左"
            if (_behaveKeybind_InUI.StartsWith("Left"))
            {
                _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 4);
                _behaveKeybind_InUI = _behaveKeybind_InUI.Insert(0, "左");
            }
        }

        return _behaveKeybind_InUI;
    }

    //按键绑定按钮被点击时，开始执行按键更改的过程。
    public void ChangeKeybind()
    {
        // 启动协程来等待用户输入新的按键
        StartCoroutine(ChangeKeybindInput());
    }

    //等待用户输入新的按键绑定。
    private IEnumerator ChangeKeybindInput()
    {
        // 显示等待输入的提示信息
        if (LanguageManager.instance.localeID == 0)
        {
            behaveKeybind_InUI.text = "Awaiting input";
        }
        else if (LanguageManager.instance.localeID == 1)
        {
            behaveKeybind_InUI.text = "等待输入按键";
        }

        //禁用其他输入，确保用户只能输入新按键
        UI.instance?.EnableUIKeyInput(false);
        MainMenu_UI.instance?.EnableKeyInput(false);

        //等待直到用户按下某个按键
        yield return new WaitUntil(CheckInput);

        //移除按钮的所有监听器，防止重复触发
        keybindButton.onClick.RemoveAllListeners();

        //更新技能面板中的所有技能图标文本
        if (SkillPanel_InGame_UI.instance != null)
        {
            SkillPanel_InGame_UI.instance?.UpdateAllSkillIconTexts();
        }

        //等待直到没有按键被按下
        yield return new WaitWhile(HasAnyKey);

        //恢复按钮的交互功能并重新添加监听器
        keybindButton.interactable = true;
        keybindButton.onClick.AddListener(ChangeKeybind);

        //恢复输入的功能
        UI.instance?.EnableUIKeyInput(true);
        MainMenu_UI.instance?.EnableKeyInput(true);
    }

    //检查是否有按键被按下。
    private bool CheckInput()
    {
        keybindButton.interactable = false;

        //检查每个键盘按键是否被按下
        foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
        {
            //如果某个按键被按下
            if (Input.GetKeyDown(keycode))
            {
                //取消按键更改
                if (keycode == KeyCode.Escape)
                {
                    behaveKeybind_InUI.text = UniformKeybindName(KeyBindManager.instance.keybindsDictionary[behaveName].ToString());
                    Debug.Log("Keybind change cancelled");
                    return true;
                }

                //更新显示为新的按键绑定
                behaveKeybind_InUI.text = UniformKeybindName(keycode.ToString());
                behaveKeybind = keycode.ToString();
                Debug.Log($"{behaveName_InUI.text} keybind has changed to {keycode.ToString()}");

                //更新按键绑定字典
                KeyBindManager.instance.keybindsDictionary[behaveName] = keycode;

                //如果按键发生冲突，显示冲突提示窗口
                if (HasKeybindConflict(keycode))
                {
                    //实例化按键冲突提示窗口
                    GameObject newKeybindConflictPromptWindow = Instantiate(keybindConflictPromptWindowPrefab, transform.parent.parent.parent.parent);

                    //按键冲突提示窗口
                    newKeybindConflictPromptWindow.GetComponent<KeybindConflictPromptWindowController>()?.SetupKeybindConflictPromptWindow(keycode);
                }

                return true;
            }
        }

        return false;
    }

    //检查是否有按键被按下。
    private bool HasAnyKey()
    {
        return Input.anyKey || Input.anyKeyDown;
    }

    //检查新按键是否与现有按键绑定冲突。
    private bool HasKeybindConflict(KeyCode _keycode)
    {
        foreach (var search in KeyBindManager.instance.keybindsDictionary)
        {
            //不与自身的按键绑定冲突
            if (search.Key == behaveName)
            {
                continue;
            }

            //如果新按键与其他行为的按键绑定冲突
            if (search.Value == _keycode)
            {
                return true;
            }
        }

        return false;
    }
}
