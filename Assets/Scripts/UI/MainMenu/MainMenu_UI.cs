using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_UI : MonoBehaviour, ISettingsSaveManager
{
    public static MainMenu_UI instance;

    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;
    [SerializeField] private FadeScreen_UI fadeScreen;

    [Header("新游戏")]
    [SerializeField] private GameObject newGameConfirmWindow;

    [Header("设置")]
    [SerializeField] private GameObject optionsUI;

    [Header("退出确认")]
    [SerializeField] private GameObject exitConfirmWindow;

    [Header("音量设置")]
    [SerializeField] private VolumeSlider_UI[] volumeSettings;

    [Header("游戏设置")]
    [SerializeField] private GameplayOptionToggle_UI[] gameplayToggleSettings;

    private bool UIKeyFunctioning = true;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (!SaveManager.instance.HasGameSaveData())
        {
            continueButton.SetActive(false);
        }
    }
    public void ContinueGame()
    {
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));  //使用协程加载场景并加上渐变效果
    }

    public void ShowNewGameConfirmWindow()
    {
        //隐藏所有子UI，显示新游戏确认窗口
        for (int i = 0; i < transform.childCount; i++)
        {
            //关闭其他UI
            transform.GetChild(i).gameObject.SetActive(false);
            newGameConfirmWindow.SetActive(true);  //显示新游戏确认窗口
        }
    }

    public void CloseAllConfirmWindow()
    {
        //打开所有子UI，关闭确认窗口和其他UI
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
            newGameConfirmWindow.SetActive(false);  //关闭新游戏确认窗口
            exitConfirmWindow.SetActive(false);  //关闭退出确认窗口
            optionsUI.SetActive(false);  //关闭设置UI
        }
    }

    public void NewGame_DetectSaveFile()
    {
        // 检查是否存在存档文件
        if (SaveManager.instance.HasGameSaveData())
        {
            ShowNewGameConfirmWindow();  //如果有存档数据，显示新游戏确认窗口
        }
        else
        {
            NewGame();  //如果没有存档数据，直接开始新游戏
        }
    }

    public void NewGame()
    {
        //删除当前游戏存档数据
        SaveManager.instance.DeleteGameProgressionSavedData();
        //SceneManager.LoadScene(sceneName);  //可替代为直接加载场景
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));  //使用协程加载场景并加上渐变效果
    }

    public void Exit()
    {
        //Debug.Log("Game exited");
        //退出游戏
        Application.Quit();
    }

    //加载场景并加入渐变效果的协程
    private IEnumerator LoadSceneWithFadeEffect(float _delayTime)
    {
        fadeScreen.FadeOut();  //淡出效果

        //等待指定时间后加载场景
        yield return new WaitForSeconds(_delayTime);

        //加载新场景
        SceneManager.LoadScene(sceneName);
    }

    public void SwitchToOptionsUI()
    {
        if (optionsUI != null)
        {
            //关闭所有UI
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            //显示设置UI
            optionsUI.SetActive(true);
        }
    }

    public void ShowExitConfirmWindow()
    {
        //隐藏所有其他UI并显示退出确认窗口
        for (int i = 0; i < transform.childCount; i++)
        {
            // 关闭其他UI
            transform.GetChild(i).gameObject.SetActive(false);
            exitConfirmWindow.SetActive(true);  //显示退出确认窗口
        }
    }

    public void EnableKeyInput(bool _value)
    {
        //启用或禁用键盘输入
        UIKeyFunctioning = _value;
    }

    public void LoadData(SettingsData _data)
    {
       //Debug.Log("加载选项数据");

        //加载音频设置
        foreach (var search in _data.volumeSettingsDictionary)
        {
            foreach (var volume in volumeSettings)
            {
                if (volume.parameter == search.Key)
                {
                    volume.LoadVolumeSlider(search.Value);  //根据保存的数据加载音量滑动条
                }
            }
        }

        // 加载游戏玩法开关设置
        foreach (var search in _data.gameplayToggleSettingsDictionary)
        {
            foreach (var toggle in gameplayToggleSettings)
            {
                if (toggle.optionName == search.Key)
                {
                    toggle.SetToggleValue(search.Value);  //根据保存的数据设置开关值
                }
            }
        }
    }

    public void SaveData(ref SettingsData _data)
    {
        //Debug.Log("保存选项数据");
        //保存音频设置
        _data.volumeSettingsDictionary.Clear();  //清空原有的音量设置

        foreach (var volume in volumeSettings)
        {
            _data.volumeSettingsDictionary.Add(volume.parameter, volume.slider.value);  //保存每个音量滑块的值
        }

        //保存游戏玩法开关设置
        _data.gameplayToggleSettingsDictionary.Clear();  //清空原有开关设置

        foreach (var toggle in gameplayToggleSettings)
        {
            _data.gameplayToggleSettingsDictionary.Add(toggle.optionName, toggle.GetToggleValue());  //保存每个开关值
        }
    }

}
