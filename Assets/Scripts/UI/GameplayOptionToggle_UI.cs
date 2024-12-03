using UnityEngine;
using UnityEngine.UI;

public class GameplayOptionToggle_UI : MonoBehaviour
{
    //选项名称，标识当前UI对应选项
    public string optionName;

    //ToggleUI组件，用于显示和控制当前选项的开关状态
    [SerializeField] private Toggle optionToggle;

    //获取当前开关的值
    public bool GetToggleValue()
    {
        //返回当前Toggle组件的开关状态
        return optionToggle.isOn;
    }

    //设置开关的状态
    public void SetToggleValue(bool _value)
    {
        //设置Toggle组件的开关状态
        optionToggle.isOn = _value;
    }
}

