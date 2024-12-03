using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour, ISettingsSaveManager
{
    public static UI instance; //UI控制单例实例，方便全局访问

    [SerializeField] private GameObject character_UI;   //角色
    [SerializeField] private GameObject skillTree_UI;   //技能树
    [SerializeField] private GameObject craft_UI;   //制作
    [SerializeField] private GameObject options_UI; //设置
    [SerializeField] private GameObject ingame_UI;  //默认显示的UI

    //技能、物品和属性的提示框
    public SkillToolTip_UI skillToolTip;
    public ItemToolTip_UI itemToolTip;
    public StatToolTip_UI statToolTip;
    public CraftWindow_UI craftWindow;  //制作窗口UI

    [Space]
    [Header("结束显示")]
    public FadeScreen_UI fadeScreen; //游戏结束时的渐隐效果（例如玩家死亡时）
    [SerializeField] private GameObject endText;  //结束文字
    [SerializeField] private GameObject tryAgainButton;  //重试按钮

    [Header("感谢游玩")]
    [SerializeField] private GameObject thankYouForPlayingText;
    [SerializeField] private TextMeshProUGUI achievedEndingText; //显示已达成的结局(可扩展结局）
    [SerializeField] private GameObject returnToTitleButton; //返回标题页按钮

    [Header("声音设置")]
    [SerializeField] private VolumeSlider_UI[] volumeSettings; //音量调节滑块

    [Header("游戏设置")]
    [SerializeField] private GameplayOptionToggle_UI[] gameplayToggleSettings; //游戏玩法设置选项（例如开关）

    private bool UIKeyFunctioning = true; //确保UI操作的键位正常工作

    private GameObject currentUI; //当前显示的UI

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }

        //确保技能树在开始时处于激活状态
        skillTree_UI.SetActive(true);
    }

    private void Start()
    {
        //游戏开始时，只打开ingame_UI
        SwitchToMenu(ingame_UI);
        itemToolTip.gameObject.SetActive(false); //初始时关闭所有工具提示
        statToolTip.gameObject.SetActive(false);
        skillToolTip.gameObject.SetActive(false);

        fadeScreen.gameObject.SetActive(true); //启用渐隐屏幕

        UIKeyFunctioning = true;
    }

    private void Update()
    {
                                        //C键切换到角色
        if (UIKeyFunctioning && Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Character"]))
        {
            OpenMenuByKeyBoard(character_UI);
        }

                                        //B键切换到制作 
        if (UIKeyFunctioning && Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Craft"]))
        {
            OpenMenuByKeyBoard(craft_UI);
        }

                                        //K键切换到技能树 
        if (UIKeyFunctioning && Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Skill"]))
        {
            OpenMenuByKeyBoard(skillTree_UI);
        }

        //当前显示的UI不是ingame_UI，按Esc键关闭当前UI 并显示ingame_UI
        //如果是ingame_UI，按下Esc键打开选项UI
        if (UIKeyFunctioning && Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentUI != ingame_UI)
            {
                //关闭所有工具提示并切换回游戏界面
                skillToolTip.gameObject.SetActive(false);
                itemToolTip.gameObject.SetActive(false);
                statToolTip.gameObject.SetActive(false);
                SwitchToMenu(ingame_UI);
            }
            else
            {
                //打开选项菜单
                OpenMenuByKeyBoard(options_UI);
            }
        }
    }

    public void SwitchToMenu(GameObject _menu)
    {
        //关闭所有UI面板
        for (int i = 0; i < transform.childCount; i++)
        {
            //保留黑色渐隐屏幕
            bool isFadeScreen = (transform.GetChild(i).GetComponent<FadeScreen_UI>() != null);

            if (!isFadeScreen)
            {
                //关闭当前UI
                transform.GetChild(i).gameObject.SetActive(false);
                currentUI = null; //重置当前显示UI
            }
        }

        //设置目标UI为激活状态
        if (_menu != null)
        {
            _menu.SetActive(true); //激活指定的菜单 UI
            currentUI = _menu; //设置当前 UI 为目标菜单
            AudioManager.instance.PlaySFX(7, null); //播放切换UI的音效
        }

        // 如果目标菜单是游戏中的 UI，则恢复游戏进度
        if (_menu == ingame_UI)
        {
            GameManager.instance?.PauseGame(false); 
        }
        else
        {
            //如果目标菜单不是游戏中的UI，则暂停游戏
            GameManager.instance?.PauseGame(true); 
        }
    }

    public void OpenMenuByKeyBoard(GameObject _menu)
    {
        // 判断目标菜单是否已经打开，如果已经打开则关闭菜单并返回游戏界面
        if (_menu != null && _menu.activeSelf)
        {
            // 如果目标菜单已经打开，则关闭该菜单并切换回游戏 UI
            skillToolTip.gameObject.SetActive(false); // 关闭所有工具提示
            itemToolTip.gameObject.SetActive(false);
            statToolTip.gameObject.SetActive(false);
            SwitchToMenu(ingame_UI); // 切换回游戏 UI
        }
        else if (_menu != null && !_menu.activeSelf)  // 如果目标菜单未打开，则打开该菜单
        {
            SwitchToMenu(_menu); // 切换到目标菜单
        }
    }

    //
    public Vector2 SetupToolTipPositionOffsetAccordingToMousePosition(float _xOffsetRate_left, float _xOffsetRate_right, float _yOffsetRate_up, float _yOffsetRate_down)
    {
        Vector2 mousePosition = Input.mousePosition; //获取鼠标当前位置
        float _xOffset = 0;
        float _yOffset = 0;

        //如果鼠标位于屏幕右侧
        if (mousePosition.x >= Screen.width * 0.5)
        {
            _xOffset = -Screen.width * _xOffsetRate_left; //设置左侧偏移量
        }
        else //如果鼠标位于屏幕左侧
        {
            _xOffset = Screen.width * _xOffsetRate_right; //设置右侧偏移量
        }

        //如果鼠标位于屏幕上方
        if (mousePosition.y >= Screen.height * 0.5)
        {
            _yOffset = -Screen.height * _yOffsetRate_down; //设置下方偏移量
        }
        else //如果鼠标位于屏幕下方
        {
            _yOffset = Screen.height * _yOffsetRate_up; //设置上方偏移量
        }

        //返回根据鼠标位置计算的提示框位置偏移量
        Vector2 toolTipPositionOffset = new Vector2(_xOffset, _yOffset);
        return toolTipPositionOffset;
    }

    public Vector2 SetupToolTipPositionOffsetAccordingToUISlotPosition(Transform _slotUITransform, float _xOffsetRate_left, float _xOffsetRate_right, float _yOffsetRate_up, float _yOffsetRate_down)
    {
        float _xOffset = 0;
        float _yOffset = 0;

        // 如果 UI 插槽位于屏幕右侧
        if (_slotUITransform.position.x >= Screen.width * 0.5)
        {
            _xOffset = -Screen.width * _xOffsetRate_left; // 设置偏移量向左
        }
        else // 如果 UI 插槽位于屏幕左侧
        {
            _xOffset = Screen.width * _xOffsetRate_right; // 设置偏移量向右
        }

        // 如果 UI 插槽位于屏幕上方
        if (_slotUITransform.position.y >= Screen.height * 0.5)
        {
            _yOffset = -Screen.height * _yOffsetRate_down; // 设置偏移量向下
        }
        else // 如果 UI 插槽位于屏幕下方
        {
            _yOffset = Screen.height * _yOffsetRate_up; // 设置偏移量向上
        }

        // 返回根据 UI 插槽位置计算的工具提示框位置偏移量
        Vector2 toolTipPositionOffset = new Vector2(_xOffset, _yOffset);
        return toolTipPositionOffset;
    }

    public void SwitchToThankYouForPlaying(string _achievedEndingText)
    {
        UIKeyFunctioning = false; // 禁用 UI 键盘输入
        fadeScreen.FadeOut(); // 执行渐隐动画
        StartCoroutine(ThankYouForPlayingCoroutine(_achievedEndingText)); // 启动感谢页面的协程
    }

    private IEnumerator ThankYouForPlayingCoroutine(string _achievedEndingText)
    {
        yield return new WaitForSeconds(1.5f); // 等待 1.5 秒后显示感谢信息

        thankYouForPlayingText.SetActive(true); // 激活感谢文字
        achievedEndingText.text = _achievedEndingText; // 设置已达成的结局文字
        achievedEndingText.gameObject.SetActive(true); // 显示结局文字

        yield return new WaitForSeconds(1f); // 等待 1 秒后显示返回主菜单按钮

        returnToTitleButton.SetActive(true); // 激活返回主菜单按钮
    }

    public void DeleteGameProgressionAndReturnToTitle()
    {
        StartCoroutine(DeleteGameProgressionAndReturnToTitle_Coroutine()); // 启动删除游戏进度并返回主菜单的协程
    }

    private IEnumerator DeleteGameProgressionAndReturnToTitle_Coroutine()
    {
        GameManager.instance.PauseGame(false); // 恢复游戏
        fadeScreen.FadeOut(); // 执行渐隐动画
        SaveManager.instance.DeleteGameProgressionSavedData(); // 删除游戏存档数据

        yield return new WaitForSeconds(0.5f); // 等待 0.5 秒后加载主菜单场景

        SceneManager.LoadScene("MainMenu"); // 加载主菜单场景
    }

    public void SwitchToEndScreen()
    {
        // fadeScreen.gameObject.SetActive(true); // 可选：显式激活渐隐屏幕
        UIKeyFunctioning = false; // 禁用 UI 键盘输入
        fadeScreen.FadeOut(); // 执行渐隐动画
        StartCoroutine(EndScreenCoroutine()); // 启动结束页面的协程
    }

    private IEnumerator EndScreenCoroutine()
    {
        yield return new WaitForSeconds(1.5f); // 等待 1.5 秒后显示结束文字

        endText.SetActive(true); // 显示结束文字

        yield return new WaitForSeconds(1f); // 等待 1 秒后显示重试按钮

        tryAgainButton.SetActive(true); // 激活重试按钮
    }

    public void RestartGame()
    {
        SaveManager.instance.SaveGame(); // 保存当前游戏进度
        GameManager.instance.RestartScene(); // 重新加载当前场景，开始新一轮游戏
    }

    public void EnableUIKeyInput(bool _value)
    {
        UIKeyFunctioning = _value; // 根据传入的值启用或禁用 UI 键盘输入
    }

    public void LoadData(SettingsData _data)
    {
        // 加载音频设置
        foreach (var search in _data.volumeSettingsDictionary)
        {
            foreach (var volume in volumeSettings)
            {
                if (volume.parameter == search.Key)
                {
                    volume.LoadVolumeSlider(search.Value); // 加载音量滑块的值
                }
            }
        }

        // 加载游戏玩法切换设置
        foreach (var search in _data.gameplayToggleSettingsDictionary)
        {
            foreach (var toggle in gameplayToggleSettings)
            {
                if (toggle.optionName == search.Key)
                {
                    toggle.SetToggleValue(search.Value); // 设置开关的值
                }
            }
        }
    }

    public void SaveData(ref SettingsData _data)
    {
        // 保存音频设置
        _data.volumeSettingsDictionary.Clear(); // 清空现有音量设置字典

        foreach (var volume in volumeSettings)
        {
            _data.volumeSettingsDictionary.Add(volume.parameter, volume.slider.value); // 保存音量滑块的值
        }

        // 保存游戏玩法切换设置
        _data.gameplayToggleSettingsDictionary.Clear(); // 清空现有游戏玩法设置字典

        foreach (var toggle in gameplayToggleSettings)
        {
            _data.gameplayToggleSettingsDictionary.Add(toggle.optionName, toggle.GetToggleValue()); // 保存开关的值
        }
    }

}
