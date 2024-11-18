using TMPro;
using UnityEngine;

public class KeybindConflictController : MonoBehaviour
{
    //显示行为名称UI
    [SerializeField] private TextMeshProUGUI behaveName_InUI;
    //显示按键绑定名称UI
    [SerializeField] protected TextMeshProUGUI behaveKeybind_InUI;

    //存储行为和按键绑定
    private string behaveName;
    private string behaveKeybind;

    public void SetupKeybindConflict(string _behaveName, string _behaveKeybind)
    {
        //存储传入的行为名称和按键绑定
        behaveName = _behaveName;
        behaveKeybind = _behaveKeybind;

        //更新UI文本显示
        behaveName_InUI.text = _behaveName;
        behaveKeybind_InUI.text = _behaveKeybind;

        //根据当前语言环境翻译并标准化按键绑定名称
        TranslateBehaveNameAndUniformBehaveKeybindName();
    }

    public void TranslateBehaveNameAndUniformBehaveKeybindName()
    {
        //如果当前语言为中文，翻译行为名称
        if (LanguageManager.instance.localeID == 1)
        {
            behaveName_InUI.text = LanguageManager.instance.EnglishToChineseKeybindsDictionary[behaveName];
        }
        //如果当前语言为英文，则显示原始英文行为名称
        else if (LanguageManager.instance.localeID == 0)
        {
            behaveName_InUI.text = behaveName;
        }

        //标准化并翻译按键绑定名称
        behaveKeybind_InUI.text = UniformKeybindName(behaveKeybind);
    }

    private string UniformKeybindName(string _behaveKeybind_InUI)
    {
                                                //去掉Alpha前缀（如果有），因为它通常代表数字按键
        if (_behaveKeybind_InUI.StartsWith("Alpha"))
        {
            _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 5); //去除Alpha部分
        }

        //如果语言环境为英文
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
        // 如果语言环境为中文
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

            if (_behaveKeybind_InUI.StartsWith("Left"))
            {
                _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 4);
                _behaveKeybind_InUI = _behaveKeybind_InUI.Insert(0, "左");
            }
        }

        return _behaveKeybind_InUI;
    }
}
