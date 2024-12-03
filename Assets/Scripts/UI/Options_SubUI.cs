using System;
using UnityEngine;

public class Options_SubUI : MonoBehaviour
{
    // 序列化字段，用于引用不同的选项菜单（游戏选项、键位、声音、语言）
    [SerializeField] private GameObject GameplayOptions; 
    [SerializeField] private GameObject KeybindOptions;    
    [SerializeField] private GameObject SoundOptions;       
    [SerializeField] private GameObject LanguageOptions;   

    // Start方法在游戏开始时调用
    private void Start()
    {
        //初始化时游戏选项菜单
        SwitchToOptions(GameplayOptions);
    }

    //切换到指定的选项菜单，并关闭其他所有菜单
    public void SwitchToOptions(GameObject _optionsMenu)
    {
        //关闭所有子UI，将它们设置为不可见
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        //打开选中的菜单，如果菜单不为空
        if (_optionsMenu != null)
        {
            _optionsMenu.SetActive(true);  //显示选中的菜单
            AudioManager.instance.PlaySFX(7, null);  //播放切换菜单时的音效
        }
    }
}

