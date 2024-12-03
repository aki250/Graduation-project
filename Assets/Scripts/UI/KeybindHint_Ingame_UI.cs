using TMPro;
using UnityEngine;

public class KeybindHint_Ingame_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI keybindText;   //绑定UI文本组件，显示按键绑定提示
    [SerializeField] private string keybindTextPreset;  //设置提示文本预设内容
    [SerializeField] private string behaveName; //绑定行为名称，根据该名称查找对应按键绑定

    private void Start()
    {
        GetKeybindIfAvailable();  //启动时尝试获取对应的按键绑定
    }

    private void OnEnable()
    {
        GetKeybindIfAvailable();  //尝试更新按键绑定
    }

    // 获取指定行为名称对应的按键绑定并更新UI显示
    private void GetKeybindIfAvailable()
    {
        //检查KeyBindManager实例
        if (KeyBindManager.instance == null) 
        {
            return;
        }

        //字典中包含对应的行为名称，更新显示的按键绑定
        if (KeyBindManager.instance.keybindsDictionary.ContainsKey(behaveName))
        {
            //获取按键绑定并统一格式化
            keybindText.text = UniformKeybindName(KeyBindManager.instance.keybindsDictionary[behaveName].ToString());
            keybindText.text = keybindTextPreset + keybindText.text;  //添加预设文本
        }
    }

    //统一格式化按键名称（处理不同语言环境下的差异）
    private string UniformKeybindName(string _behaveKeybind_InUI)
    {
        if (_behaveKeybind_InUI.StartsWith("Alpha"))
        {
            _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 5);  //移除Alpha前缀
        }

        // 根据当前语言环境，调整按键名称的显示
        //英文
        if (LanguageManager.instance.localeID == 0)
        {
            if (_behaveKeybind_InUI.Equals("Mouse0"))
            {
                _behaveKeybind_InUI = "LMB";  //左键对应 "LMB"
            }

            if (_behaveKeybind_InUI.Equals("Mouse1"))
            {
                _behaveKeybind_InUI = "RMB";  //右键对应 "RMB"
            }

            if (_behaveKeybind_InUI.StartsWith("Left"))
            {
                _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(1, 3);  //移除 Left 前缀
            }
        }
        //中文
        else if (LanguageManager.instance.localeID == 1)
        {
            if (_behaveKeybind_InUI.Equals("Mouse0"))
            {
                _behaveKeybind_InUI = "左键";  
            }

            if (_behaveKeybind_InUI.Equals("Mouse1"))
            {
                _behaveKeybind_InUI = "右键";  
            }

            if (_behaveKeybind_InUI.StartsWith("Left"))
            {
                _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 4);  //移除Left前缀
                _behaveKeybind_InUI = _behaveKeybind_InUI.Insert(0, "左"); 
            }
        }

        return _behaveKeybind_InUI;  //返回格式化后的按键名称
    }
}
