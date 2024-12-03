using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_UI : MonoBehaviour, ISettingsSaveManager
{
    public static MainMenu_UI instance;

    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;
    [SerializeField] private FadeScreen_UI fadeScreen;

    [Header("����Ϸ")]
    [SerializeField] private GameObject newGameConfirmWindow;

    [Header("����")]
    [SerializeField] private GameObject optionsUI;

    [Header("�˳�ȷ��")]
    [SerializeField] private GameObject exitConfirmWindow;

    [Header("��������")]
    [SerializeField] private VolumeSlider_UI[] volumeSettings;

    [Header("��Ϸ����")]
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
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));  //ʹ��Э�̼��س��������Ͻ���Ч��
    }

    public void ShowNewGameConfirmWindow()
    {
        //����������UI����ʾ����Ϸȷ�ϴ���
        for (int i = 0; i < transform.childCount; i++)
        {
            //�ر�����UI
            transform.GetChild(i).gameObject.SetActive(false);
            newGameConfirmWindow.SetActive(true);  //��ʾ����Ϸȷ�ϴ���
        }
    }

    public void CloseAllConfirmWindow()
    {
        //��������UI���ر�ȷ�ϴ��ں�����UI
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
            newGameConfirmWindow.SetActive(false);  //�ر�����Ϸȷ�ϴ���
            exitConfirmWindow.SetActive(false);  //�ر��˳�ȷ�ϴ���
            optionsUI.SetActive(false);  //�ر�����UI
        }
    }

    public void NewGame_DetectSaveFile()
    {
        // ����Ƿ���ڴ浵�ļ�
        if (SaveManager.instance.HasGameSaveData())
        {
            ShowNewGameConfirmWindow();  //����д浵���ݣ���ʾ����Ϸȷ�ϴ���
        }
        else
        {
            NewGame();  //���û�д浵���ݣ�ֱ�ӿ�ʼ����Ϸ
        }
    }

    public void NewGame()
    {
        //ɾ����ǰ��Ϸ�浵����
        SaveManager.instance.DeleteGameProgressionSavedData();
        //SceneManager.LoadScene(sceneName);  //�����Ϊֱ�Ӽ��س���
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));  //ʹ��Э�̼��س��������Ͻ���Ч��
    }

    public void Exit()
    {
        //Debug.Log("Game exited");
        //�˳���Ϸ
        Application.Quit();
    }

    //���س��������뽥��Ч����Э��
    private IEnumerator LoadSceneWithFadeEffect(float _delayTime)
    {
        fadeScreen.FadeOut();  //����Ч��

        //�ȴ�ָ��ʱ�����س���
        yield return new WaitForSeconds(_delayTime);

        //�����³���
        SceneManager.LoadScene(sceneName);
    }

    public void SwitchToOptionsUI()
    {
        if (optionsUI != null)
        {
            //�ر�����UI
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            //��ʾ����UI
            optionsUI.SetActive(true);
        }
    }

    public void ShowExitConfirmWindow()
    {
        //������������UI����ʾ�˳�ȷ�ϴ���
        for (int i = 0; i < transform.childCount; i++)
        {
            // �ر�����UI
            transform.GetChild(i).gameObject.SetActive(false);
            exitConfirmWindow.SetActive(true);  //��ʾ�˳�ȷ�ϴ���
        }
    }

    public void EnableKeyInput(bool _value)
    {
        //���û���ü�������
        UIKeyFunctioning = _value;
    }

    public void LoadData(SettingsData _data)
    {
       //Debug.Log("����ѡ������");

        //������Ƶ����
        foreach (var search in _data.volumeSettingsDictionary)
        {
            foreach (var volume in volumeSettings)
            {
                if (volume.parameter == search.Key)
                {
                    volume.LoadVolumeSlider(search.Value);  //���ݱ�������ݼ�������������
                }
            }
        }

        // ������Ϸ�淨��������
        foreach (var search in _data.gameplayToggleSettingsDictionary)
        {
            foreach (var toggle in gameplayToggleSettings)
            {
                if (toggle.optionName == search.Key)
                {
                    toggle.SetToggleValue(search.Value);  //���ݱ�����������ÿ���ֵ
                }
            }
        }
    }

    public void SaveData(ref SettingsData _data)
    {
        //Debug.Log("����ѡ������");
        //������Ƶ����
        _data.volumeSettingsDictionary.Clear();  //���ԭ�е���������

        foreach (var volume in volumeSettings)
        {
            _data.volumeSettingsDictionary.Add(volume.parameter, volume.slider.value);  //����ÿ�����������ֵ
        }

        //������Ϸ�淨��������
        _data.gameplayToggleSettingsDictionary.Clear();  //���ԭ�п�������

        foreach (var toggle in gameplayToggleSettings)
        {
            _data.gameplayToggleSettingsDictionary.Add(toggle.optionName, toggle.GetToggleValue());  //����ÿ������ֵ
        }
    }

}
