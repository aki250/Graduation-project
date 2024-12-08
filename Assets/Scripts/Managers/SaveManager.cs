using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net.NetworkInformation;

//�浵λ��  C:\Users\megum\AppData\LocalLow\DefaultCompany\BrandNewRPGGame_2nd

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance; //����

    [SerializeField] private string settingsFileName; //�ļ���
    [SerializeField] private string gameFileName; //��Ϸ�����ļ���
    [SerializeField] private bool encryptData; //�Ƿ��������

    private GameData gameData; //�洢��Ϸ����
    private SettingsData settingsData; //�洢��������

    //�洢���д�����Ϸ���ȱ���Ĺ�����
    private List<IGameProgressionSaveManager> gameProgressionSaveManagers;

    //������Ϸ�����ļ�  �ļ����ݴ�����
    private FileDataHandler gameDataHandler;

    //�洢����  ���ñ���  ������
    private List<ISettingsSaveManager> settingsSaveManagers;

    //����  ���������ļ�   �ļ����ݴ�����
    private FileDataHandler settingsDataHandler;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; //��Ϊ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //��ʼ���������ݡ���Ϸ����   ���ļ�������
        settingsDataHandler = new FileDataHandler(Application.persistentDataPath, settingsFileName, encryptData);
        gameDataHandler = new FileDataHandler(Application.persistentDataPath, gameFileName, encryptData);

        //���ñ������������Ϸ���ȱ��������
        settingsSaveManagers = FindAllSettingsSaveManagers();
        gameProgressionSaveManagers = FindAllGameProgressionSaveManagers();

        //������Ϸ����
        LoadGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        //������������
        LoadSettings();

        //������Ϸ��������
        LoadGameProgression();
    }

    // ������������
    private void LoadSettings()
    {
        //�����ݴ����� ������������
        settingsData = DatabaseMgr.Instance.QuerySetting();

        //������������ �������������������
        foreach (var saveManager in settingsSaveManagers)
        {
            saveManager.LoadData(settingsData);  //���������ݴ���ÿ�����������
        }
    }

    //������Ϸ��������
    private void LoadGameProgression()
    {
        //�����ݴ����� ������Ϸ��������
        gameData=DatabaseMgr.Instance.QueryGame();

        //����������Ϸ���ȱ������������������
        foreach (IGameProgressionSaveManager saveManager in gameProgressionSaveManagers)
        {
            saveManager.LoadData(gameData);  //����Ϸ���ݴ���ÿ�����ȱ��������
        }

        //������صĻ�����Ϣ
        Debug.Log($"Loaded currency: {gameData.currecny}");
    }

    //������Ϸ����
    public void SaveGame()
    {
        //������������
        SaveSettings();

        //������Ϸ��������
        SaveGameProgression();
    }

    //������������
    public void SaveSettings()
    {
        //�����������ñ������������������
        foreach (var saveManagers in settingsSaveManagers)
        {
            saveManagers.SaveData(ref settingsData);  //���������ݴ��ݸ����������
        }

        //ʹ�����ݴ��������������ݱ��浽�ļ�
        DatabaseMgr.Instance.UpdateSettingsData(settingsData);
    }

    //������Ϸ��������
    private void SaveGameProgression()
    {
        //����������Ϸ���ȱ������������������
        foreach (IGameProgressionSaveManager saveManager in gameProgressionSaveManagers)
        {
            saveManager.SaveData(ref gameData);  //����Ϸ���ݴ��ݸ����������
        }

        //ʹ�����ݴ���������Ϸ�������ݱ��浽�ļ�
        DatabaseMgr.Instance.UpdateGameData(gameData);

    }


    //���Ҳ���������ʵ��ISettingsSaveManager�ӿڵı��������
    private List<ISettingsSaveManager> FindAllSettingsSaveManagers()
    {
        //ʹ�� FindObjectsOfType ���ҳ����е����� MonoBehaviour ���󣬲�ɸѡ��ʵ����ISettingsSaveManager�ӿڵĶ���
        IEnumerable<ISettingsSaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<ISettingsSaveManager>();

        //�������ҵ��� ISettingsSaveManager ����ת����һ���б�����
        return new List<ISettingsSaveManager>(saveManagers);
    }

    //���Ҳ���������ʵ��IGameProgressionSaveManager�ӿڵı��������
    private List<IGameProgressionSaveManager> FindAllGameProgressionSaveManagers()
    {
        //ʹ��FindObjectsOfType���ҳ����е�����MonoBehaviour���󣬲�ɸѡ��ʵ����IGameProgressionSaveManager�ӿڶ���
        IEnumerable<IGameProgressionSaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<IGameProgressionSaveManager>();

        //�������ҵ���IGameProgressionSaveManager����ת����һ���б�����
        return new List<IGameProgressionSaveManager>(saveManagers);
    }

    //ɾ����Ϸ���ȵı����ļ��������Ϸ����
    [ContextMenu("ɾ����Ϸ���ȱ����ļ�")]
    public void DeleteGameProgressionSavedData()
    {
        //�����µ�FileDataHandler����ָ�򱣴���Ϸ���ݵ�·�����ļ���
        gameDataHandler = new FileDataHandler(Application.persistentDataPath, gameFileName, encryptData);

        //����DeleteSaveɾ����Ϸ���ȵı����ļ�
        gameDataHandler.DeleteSave();
    }

    //����Ƿ������Ϸ��������
    public bool HasGameSaveData()
    {
        //�����Ϸ���������ܹ��ɹ����أ���˵�����ڱ��������
        if (!gameData.isNew)
        {
            return true;
        }
        return false;
    }


}