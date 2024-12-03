using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour, ISettingsSaveManager
{
    public static UI instance; //UI���Ƶ���ʵ��������ȫ�ַ���

    [SerializeField] private GameObject character_UI;   //��ɫ
    [SerializeField] private GameObject skillTree_UI;   //������
    [SerializeField] private GameObject craft_UI;   //����
    [SerializeField] private GameObject options_UI; //����
    [SerializeField] private GameObject ingame_UI;  //Ĭ����ʾ��UI

    //���ܡ���Ʒ�����Ե���ʾ��
    public SkillToolTip_UI skillToolTip;
    public ItemToolTip_UI itemToolTip;
    public StatToolTip_UI statToolTip;
    public CraftWindow_UI craftWindow;  //��������UI

    [Space]
    [Header("������ʾ")]
    public FadeScreen_UI fadeScreen; //��Ϸ����ʱ�Ľ���Ч���������������ʱ��
    [SerializeField] private GameObject endText;  //��������
    [SerializeField] private GameObject tryAgainButton;  //���԰�ť

    [Header("��л����")]
    [SerializeField] private GameObject thankYouForPlayingText;
    [SerializeField] private TextMeshProUGUI achievedEndingText; //��ʾ�Ѵ�ɵĽ��(����չ��֣�
    [SerializeField] private GameObject returnToTitleButton; //���ر���ҳ��ť

    [Header("��������")]
    [SerializeField] private VolumeSlider_UI[] volumeSettings; //�������ڻ���

    [Header("��Ϸ����")]
    [SerializeField] private GameplayOptionToggle_UI[] gameplayToggleSettings; //��Ϸ�淨����ѡ����翪�أ�

    private bool UIKeyFunctioning = true; //ȷ��UI�����ļ�λ��������

    private GameObject currentUI; //��ǰ��ʾ��UI

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

        //ȷ���������ڿ�ʼʱ���ڼ���״̬
        skillTree_UI.SetActive(true);
    }

    private void Start()
    {
        //��Ϸ��ʼʱ��ֻ��ingame_UI
        SwitchToMenu(ingame_UI);
        itemToolTip.gameObject.SetActive(false); //��ʼʱ�ر����й�����ʾ
        statToolTip.gameObject.SetActive(false);
        skillToolTip.gameObject.SetActive(false);

        fadeScreen.gameObject.SetActive(true); //���ý�����Ļ

        UIKeyFunctioning = true;
    }

    private void Update()
    {
                                        //C���л�����ɫ
        if (UIKeyFunctioning && Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Character"]))
        {
            OpenMenuByKeyBoard(character_UI);
        }

                                        //B���л������� 
        if (UIKeyFunctioning && Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Craft"]))
        {
            OpenMenuByKeyBoard(craft_UI);
        }

                                        //K���л��������� 
        if (UIKeyFunctioning && Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Skill"]))
        {
            OpenMenuByKeyBoard(skillTree_UI);
        }

        //��ǰ��ʾ��UI����ingame_UI����Esc���رյ�ǰUI ����ʾingame_UI
        //�����ingame_UI������Esc����ѡ��UI
        if (UIKeyFunctioning && Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentUI != ingame_UI)
            {
                //�ر����й�����ʾ���л�����Ϸ����
                skillToolTip.gameObject.SetActive(false);
                itemToolTip.gameObject.SetActive(false);
                statToolTip.gameObject.SetActive(false);
                SwitchToMenu(ingame_UI);
            }
            else
            {
                //��ѡ��˵�
                OpenMenuByKeyBoard(options_UI);
            }
        }
    }

    public void SwitchToMenu(GameObject _menu)
    {
        //�ر�����UI���
        for (int i = 0; i < transform.childCount; i++)
        {
            //������ɫ������Ļ
            bool isFadeScreen = (transform.GetChild(i).GetComponent<FadeScreen_UI>() != null);

            if (!isFadeScreen)
            {
                //�رյ�ǰUI
                transform.GetChild(i).gameObject.SetActive(false);
                currentUI = null; //���õ�ǰ��ʾUI
            }
        }

        //����Ŀ��UIΪ����״̬
        if (_menu != null)
        {
            _menu.SetActive(true); //����ָ���Ĳ˵� UI
            currentUI = _menu; //���õ�ǰ UI ΪĿ��˵�
            AudioManager.instance.PlaySFX(7, null); //�����л�UI����Ч
        }

        // ���Ŀ��˵�����Ϸ�е� UI����ָ���Ϸ����
        if (_menu == ingame_UI)
        {
            GameManager.instance?.PauseGame(false); 
        }
        else
        {
            //���Ŀ��˵�������Ϸ�е�UI������ͣ��Ϸ
            GameManager.instance?.PauseGame(true); 
        }
    }

    public void OpenMenuByKeyBoard(GameObject _menu)
    {
        // �ж�Ŀ��˵��Ƿ��Ѿ��򿪣�����Ѿ�����رղ˵���������Ϸ����
        if (_menu != null && _menu.activeSelf)
        {
            // ���Ŀ��˵��Ѿ��򿪣���رոò˵����л�����Ϸ UI
            skillToolTip.gameObject.SetActive(false); // �ر����й�����ʾ
            itemToolTip.gameObject.SetActive(false);
            statToolTip.gameObject.SetActive(false);
            SwitchToMenu(ingame_UI); // �л�����Ϸ UI
        }
        else if (_menu != null && !_menu.activeSelf)  // ���Ŀ��˵�δ�򿪣���򿪸ò˵�
        {
            SwitchToMenu(_menu); // �л���Ŀ��˵�
        }
    }

    //
    public Vector2 SetupToolTipPositionOffsetAccordingToMousePosition(float _xOffsetRate_left, float _xOffsetRate_right, float _yOffsetRate_up, float _yOffsetRate_down)
    {
        Vector2 mousePosition = Input.mousePosition; //��ȡ��굱ǰλ��
        float _xOffset = 0;
        float _yOffset = 0;

        //������λ����Ļ�Ҳ�
        if (mousePosition.x >= Screen.width * 0.5)
        {
            _xOffset = -Screen.width * _xOffsetRate_left; //�������ƫ����
        }
        else //������λ����Ļ���
        {
            _xOffset = Screen.width * _xOffsetRate_right; //�����Ҳ�ƫ����
        }

        //������λ����Ļ�Ϸ�
        if (mousePosition.y >= Screen.height * 0.5)
        {
            _yOffset = -Screen.height * _yOffsetRate_down; //�����·�ƫ����
        }
        else //������λ����Ļ�·�
        {
            _yOffset = Screen.height * _yOffsetRate_up; //�����Ϸ�ƫ����
        }

        //���ظ������λ�ü������ʾ��λ��ƫ����
        Vector2 toolTipPositionOffset = new Vector2(_xOffset, _yOffset);
        return toolTipPositionOffset;
    }

    public Vector2 SetupToolTipPositionOffsetAccordingToUISlotPosition(Transform _slotUITransform, float _xOffsetRate_left, float _xOffsetRate_right, float _yOffsetRate_up, float _yOffsetRate_down)
    {
        float _xOffset = 0;
        float _yOffset = 0;

        // ��� UI ���λ����Ļ�Ҳ�
        if (_slotUITransform.position.x >= Screen.width * 0.5)
        {
            _xOffset = -Screen.width * _xOffsetRate_left; // ����ƫ��������
        }
        else // ��� UI ���λ����Ļ���
        {
            _xOffset = Screen.width * _xOffsetRate_right; // ����ƫ��������
        }

        // ��� UI ���λ����Ļ�Ϸ�
        if (_slotUITransform.position.y >= Screen.height * 0.5)
        {
            _yOffset = -Screen.height * _yOffsetRate_down; // ����ƫ��������
        }
        else // ��� UI ���λ����Ļ�·�
        {
            _yOffset = Screen.height * _yOffsetRate_up; // ����ƫ��������
        }

        // ���ظ��� UI ���λ�ü���Ĺ�����ʾ��λ��ƫ����
        Vector2 toolTipPositionOffset = new Vector2(_xOffset, _yOffset);
        return toolTipPositionOffset;
    }

    public void SwitchToThankYouForPlaying(string _achievedEndingText)
    {
        UIKeyFunctioning = false; // ���� UI ��������
        fadeScreen.FadeOut(); // ִ�н�������
        StartCoroutine(ThankYouForPlayingCoroutine(_achievedEndingText)); // ������лҳ���Э��
    }

    private IEnumerator ThankYouForPlayingCoroutine(string _achievedEndingText)
    {
        yield return new WaitForSeconds(1.5f); // �ȴ� 1.5 �����ʾ��л��Ϣ

        thankYouForPlayingText.SetActive(true); // �����л����
        achievedEndingText.text = _achievedEndingText; // �����Ѵ�ɵĽ������
        achievedEndingText.gameObject.SetActive(true); // ��ʾ�������

        yield return new WaitForSeconds(1f); // �ȴ� 1 �����ʾ�������˵���ť

        returnToTitleButton.SetActive(true); // ��������˵���ť
    }

    public void DeleteGameProgressionAndReturnToTitle()
    {
        StartCoroutine(DeleteGameProgressionAndReturnToTitle_Coroutine()); // ����ɾ����Ϸ���Ȳ��������˵���Э��
    }

    private IEnumerator DeleteGameProgressionAndReturnToTitle_Coroutine()
    {
        GameManager.instance.PauseGame(false); // �ָ���Ϸ
        fadeScreen.FadeOut(); // ִ�н�������
        SaveManager.instance.DeleteGameProgressionSavedData(); // ɾ����Ϸ�浵����

        yield return new WaitForSeconds(0.5f); // �ȴ� 0.5 ���������˵�����

        SceneManager.LoadScene("MainMenu"); // �������˵�����
    }

    public void SwitchToEndScreen()
    {
        // fadeScreen.gameObject.SetActive(true); // ��ѡ����ʽ�������Ļ
        UIKeyFunctioning = false; // ���� UI ��������
        fadeScreen.FadeOut(); // ִ�н�������
        StartCoroutine(EndScreenCoroutine()); // ��������ҳ���Э��
    }

    private IEnumerator EndScreenCoroutine()
    {
        yield return new WaitForSeconds(1.5f); // �ȴ� 1.5 �����ʾ��������

        endText.SetActive(true); // ��ʾ��������

        yield return new WaitForSeconds(1f); // �ȴ� 1 �����ʾ���԰�ť

        tryAgainButton.SetActive(true); // �������԰�ť
    }

    public void RestartGame()
    {
        SaveManager.instance.SaveGame(); // ���浱ǰ��Ϸ����
        GameManager.instance.RestartScene(); // ���¼��ص�ǰ��������ʼ��һ����Ϸ
    }

    public void EnableUIKeyInput(bool _value)
    {
        UIKeyFunctioning = _value; // ���ݴ����ֵ���û���� UI ��������
    }

    public void LoadData(SettingsData _data)
    {
        // ������Ƶ����
        foreach (var search in _data.volumeSettingsDictionary)
        {
            foreach (var volume in volumeSettings)
            {
                if (volume.parameter == search.Key)
                {
                    volume.LoadVolumeSlider(search.Value); // �������������ֵ
                }
            }
        }

        // ������Ϸ�淨�л�����
        foreach (var search in _data.gameplayToggleSettingsDictionary)
        {
            foreach (var toggle in gameplayToggleSettings)
            {
                if (toggle.optionName == search.Key)
                {
                    toggle.SetToggleValue(search.Value); // ���ÿ��ص�ֵ
                }
            }
        }
    }

    public void SaveData(ref SettingsData _data)
    {
        // ������Ƶ����
        _data.volumeSettingsDictionary.Clear(); // ����������������ֵ�

        foreach (var volume in volumeSettings)
        {
            _data.volumeSettingsDictionary.Add(volume.parameter, volume.slider.value); // �������������ֵ
        }

        // ������Ϸ�淨�л�����
        _data.gameplayToggleSettingsDictionary.Clear(); // ���������Ϸ�淨�����ֵ�

        foreach (var toggle in gameplayToggleSettings)
        {
            _data.gameplayToggleSettingsDictionary.Add(toggle.optionName, toggle.GetToggleValue()); // ���濪�ص�ֵ
        }
    }

}
