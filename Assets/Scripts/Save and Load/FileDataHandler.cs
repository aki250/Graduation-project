using System;
using System.IO;
using UnityEngine;

//C:\Users\megum\AppData\LocalLow\DefaultCompany\BrandNewRPGGame_2nd
public class FileDataHandler
{
    private string dataDirPath = "";    //存储数据的目录路径
    private string dataFileName = "";   //保存文件的名称

    private bool encryptData = false;   //是否加密数据
    private string codeWord = "Zakozako~"; //用于加密的密钥

    //构造函数，初始化文件路径、文件名和是否加密数据的选项
    public FileDataHandler(string _dataDirPath, string _dataFileName, bool _encryptData)
    {
        dataDirPath = _dataDirPath;
        dataFileName = _dataFileName;
        encryptData = _encryptData;
    }

    #region Settings Load and Save

    //保存设置数据,到指定路径的文件中
    public void SaveSettings(SettingsData _data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName); //获取完整的保存路径

        try
        {
            //创建保存文件的目录（如果不存在）
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            //将设置数据转换为JSON字符串（true表示格式化输出，方便阅读）
            string dataToStore = JsonUtility.ToJson(_data, true);

            //如果需要加密，调用加密方法
            if (encryptData)
            {
                dataToStore = EncryptAndDecrypt(dataToStore);
            }

            //使用文件流保存数据到文件
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore); //写入JSON数据
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error: Failed to save data file: \n{fullPath}\n {e.Message}"); //如果保存失败，输出错误信息
        }
    }

    //加载设置数据
    public SettingsData LoadSettings()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName); //获取完整的加载路径
        SettingsData loadData = null;

        if (File.Exists(fullPath)) //如果文件存在
        {
            try
            {
                string dataToLoad = ""; //用来存储读取的数据

                //使用文件流读取数据
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd(); //读取文件内容
                    }
                }

                //如果需要解密，调用解密方法
                if (encryptData)
                {
                    dataToLoad = EncryptAndDecrypt(dataToLoad);
                }

                //将JSON数据解析为SettingsData对象
                loadData = JsonUtility.FromJson<SettingsData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to load game data from:\n{fullPath}\n{e.Message}"); //如果加载失败，输出错误信息
            }
        }

        return loadData; //返回加载的数据
    }

    #endregion

    #region Game Progression Load And Save

    //保存游戏进度数据到指定路径的文件中
    public void SaveGameProgression(GameData _data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName); //获取完整的保存路径

        try
        {
            //创建保存文件的目录（如果不存在）
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            //将游戏数据转换为JSON字符串（true表示格式化输出，方便阅读）
            string dataToStore = JsonUtility.ToJson(_data, true);

            //如果需要加密，调用加密方法
            if (encryptData)
            {
                dataToStore = EncryptAndDecrypt(dataToStore);
            }

            //使用文件流保存数据到文件
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore); //写入JSON数据
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error: Failed to save data file: \n{fullPath}\n {e.Message}"); //如果保存失败，输出错误信息
        }
    }

    //加载游戏进度数据
    public GameData LoadGameProgression()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName); //获取完整的加载路径
        GameData loadData = null;

        if (File.Exists(fullPath)) //如果文件存在
        {
            try
            {
                string dataToLoad = ""; //用来存储读取的数据

                //使用文件流读取数据
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd(); //读取文件内容
                    }
                }

                //解密方法
                if (encryptData)
                {
                    dataToLoad = EncryptAndDecrypt(dataToLoad);
                }

                //将JSON数据解析为GameData对象
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to load game data from:\n{fullPath}\n{e.Message}"); // 如果加载失败，输出错误信息
            }
        }

        return loadData; //返回加载的数据
    }

    #endregion

    //删除保存文件
    public void DeleteSave()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName); //获取完整的文件路径

        if (File.Exists(fullPath)) //如果文件存在
        {
            File.Delete(fullPath); //删除文件
        }
    }

    //加密与解密数据的功能（XOR加密）
    private string EncryptAndDecrypt(string _data)
    {
        string result = "";

        //使用XOR加密（异或运算），逐个字符地对数据进行加密/解密
        for (int i = 0; i < _data.Length; i++)
        {
            result += (char)(_data[i] ^ codeWord[i % codeWord.Length]); //使用密钥进行XOR运算
        }

        return result; //返回加密或解密后的结果
    }
}
