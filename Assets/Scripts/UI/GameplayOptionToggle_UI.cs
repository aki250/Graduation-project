using UnityEngine;
using UnityEngine.UI;

public class GameplayOptionToggle_UI : MonoBehaviour
{
    //ѡ�����ƣ���ʶ��ǰUI��Ӧѡ��
    public string optionName;

    //ToggleUI�����������ʾ�Ϳ��Ƶ�ǰѡ��Ŀ���״̬
    [SerializeField] private Toggle optionToggle;

    //��ȡ��ǰ���ص�ֵ
    public bool GetToggleValue()
    {
        //���ص�ǰToggle����Ŀ���״̬
        return optionToggle.isOn;
    }

    //���ÿ��ص�״̬
    public void SetToggleValue(bool _value)
    {
        //����Toggle����Ŀ���״̬
        optionToggle.isOn = _value;
    }
}

