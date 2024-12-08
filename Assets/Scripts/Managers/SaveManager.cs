using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net.NetworkInformation;

//存档位置  C:\Users\megum\AppData\LocalLow\DefaultCompany\BrandNewRPGGame_2nd

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance; //单例

    [SerializeField] private string settingsFileName; //文件名
    [SerializeField] private string gameFileName; //游戏数据文件名
    [SerializeField] private bool encryptData; //是否加密数据

    private GameData gameData; //存储游戏数据
    private SettingsData settingsData; //存储设置数据

    //存储所有处理游戏进度保存的管理器
    private List<IGameProgressionSaveManager> gameProgressionSaveManagers;

    //处理游戏数据文件  文件数据处理器
    private FileDataHandler gameDataHandler;

    //存储所有  设置保存  管理器
    private List<ISettingsSaveManager> settingsSaveManagers;

    //处理  设置数据文件   文件数据处理器
    private FileDataHandler settingsDataHandler;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; //设为单例
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //初始化设置数据、游戏数据   的文件处理器
        settingsDataHandler = new FileDataHandler(Application.persistentDataPath, settingsFileName, encryptData);
        gameDataHandler = new FileDataHandler(Application.persistentDataPath, gameFileName, encryptData);

        //设置保存管理器、游戏进度保存管理器
        settingsSaveManagers = FindAllSettingsSaveManagers();
        gameProgressionSaveManagers = FindAllGameProgressionSaveManagers();

        //加载游戏数据
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
        //加载设置数据
        LoadSettings();

        //加载游戏进度数据
        LoadGameProgression();
    }

    // 加载设置数据
    private void LoadSettings()
    {
        //从数据处理器 加载设置数据
        settingsData = DatabaseMgr.Instance.QuerySetting();

        //遍历所有设置 保存管理器并加载数据
        foreach (var saveManager in settingsSaveManagers)
        {
            saveManager.LoadData(settingsData);  //将设置数据传给每个保存管理器
        }
    }

    //加载游戏进度数据
    private void LoadGameProgression()
    {
        //从数据处理器 加载游戏进度数据
        gameData=DatabaseMgr.Instance.QueryGame();

        //遍历所有游戏进度保存管理器并加载数据
        foreach (IGameProgressionSaveManager saveManager in gameProgressionSaveManagers)
        {
            saveManager.LoadData(gameData);  //将游戏数据传给每个进度保存管理器
        }

        //输出加载的货币信息
        Debug.Log($"Loaded currency: {gameData.currecny}");
    }

    //保存游戏数据
    public void SaveGame()
    {
        //保存设置数据
        SaveSettings();

        //保存游戏进度数据
        SaveGameProgression();
    }

    //保存设置数据
    public void SaveSettings()
    {
        //遍历所有设置保存管理器并保存数据
        foreach (var saveManagers in settingsSaveManagers)
        {
            saveManagers.SaveData(ref settingsData);  //将设置数据传递给保存管理器
        }

        //使用数据处理器将设置数据保存到文件
        DatabaseMgr.Instance.UpdateSettingsData(settingsData);
    }

    //保存游戏进度数据
    private void SaveGameProgression()
    {
        //遍历所有游戏进度保存管理器并保存数据
        foreach (IGameProgressionSaveManager saveManager in gameProgressionSaveManagers)
        {
            saveManager.SaveData(ref gameData);  //将游戏数据传递给保存管理器
        }

        //使用数据处理器将游戏进度数据保存到文件
        DatabaseMgr.Instance.UpdateGameData(gameData);

    }


    //查找并返回所有实现ISettingsSaveManager接口的保存管理器
    private List<ISettingsSaveManager> FindAllSettingsSaveManagers()
    {
        //使用 FindObjectsOfType 查找场景中的所有 MonoBehaviour 对象，并筛选出实现了ISettingsSaveManager接口的对象
        IEnumerable<ISettingsSaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<ISettingsSaveManager>();

        //将所有找到的 ISettingsSaveManager 对象转换成一个列表并返回
        return new List<ISettingsSaveManager>(saveManagers);
    }

    //查找并返回所有实现IGameProgressionSaveManager接口的保存管理器
    private List<IGameProgressionSaveManager> FindAllGameProgressionSaveManagers()
    {
        //使用FindObjectsOfType查找场景中的所有MonoBehaviour对象，并筛选出实现了IGameProgressionSaveManager接口对象
        IEnumerable<IGameProgressionSaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<IGameProgressionSaveManager>();

        //将所有找到的IGameProgressionSaveManager对象转换成一个列表并返回
        return new List<IGameProgressionSaveManager>(saveManagers);
    }

    //删除游戏进度的保存文件，清空游戏数据
    [ContextMenu("删除游戏进度保存文件")]
    public void DeleteGameProgressionSavedData()
    {
        //创建新的FileDataHandler对象，指向保存游戏数据的路径和文件名
        gameDataHandler = new FileDataHandler(Application.persistentDataPath, gameFileName, encryptData);

        //调用DeleteSave删除游戏进度的保存文件
        gameDataHandler.DeleteSave();
    }

    //检查是否存在游戏保存数据
    public bool HasGameSaveData()
    {
        //如果游戏进度数据能够成功加载，则说明存在保存的数据
        if (!gameData.isNew)
        {
            return true;
        }
        return false;
    }


}