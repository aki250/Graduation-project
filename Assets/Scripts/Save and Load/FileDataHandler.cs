using System;
using System.IO;
using UnityEngine;

//C:\Users\megum\AppData\LocalLow\DefaultCompany\BrandNewRPGGame_2nd
public class FileDataHandler
{
    private string dataDirPath = "";    //�洢���ݵ�Ŀ¼·��
    private string dataFileName = "";   //�����ļ�������

    private bool encryptData = false;   //�Ƿ��������
    private string codeWord = "Zakozako~"; //���ڼ��ܵ���Կ

    //���캯������ʼ���ļ�·�����ļ������Ƿ�������ݵ�ѡ��
    public FileDataHandler(string _dataDirPath, string _dataFileName, bool _encryptData)
    {
        dataDirPath = _dataDirPath;
        dataFileName = _dataFileName;
        encryptData = _encryptData;
    }

    #region Settings Load and Save

    //������������,��ָ��·�����ļ���
    public void SaveSettings(SettingsData _data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName); //��ȡ�����ı���·��

        try
        {
            //���������ļ���Ŀ¼����������ڣ�
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            //����������ת��ΪJSON�ַ�����true��ʾ��ʽ������������Ķ���
            string dataToStore = JsonUtility.ToJson(_data, true);

            //�����Ҫ���ܣ����ü��ܷ���
            if (encryptData)
            {
                dataToStore = EncryptAndDecrypt(dataToStore);
            }

            //ʹ���ļ����������ݵ��ļ�
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore); //д��JSON����
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error: Failed to save data file: \n{fullPath}\n {e.Message}"); //�������ʧ�ܣ����������Ϣ
        }
    }

    //������������
    public SettingsData LoadSettings()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName); //��ȡ�����ļ���·��
        SettingsData loadData = null;

        if (File.Exists(fullPath)) //����ļ�����
        {
            try
            {
                string dataToLoad = ""; //�����洢��ȡ������

                //ʹ���ļ�����ȡ����
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd(); //��ȡ�ļ�����
                    }
                }

                //�����Ҫ���ܣ����ý��ܷ���
                if (encryptData)
                {
                    dataToLoad = EncryptAndDecrypt(dataToLoad);
                }

                //��JSON���ݽ���ΪSettingsData����
                loadData = JsonUtility.FromJson<SettingsData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to load game data from:\n{fullPath}\n{e.Message}"); //�������ʧ�ܣ����������Ϣ
            }
        }

        return loadData; //���ؼ��ص�����
    }

    #endregion

    #region Game Progression Load And Save

    //������Ϸ�������ݵ�ָ��·�����ļ���
    public void SaveGameProgression(GameData _data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName); //��ȡ�����ı���·��

        try
        {
            //���������ļ���Ŀ¼����������ڣ�
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            //����Ϸ����ת��ΪJSON�ַ�����true��ʾ��ʽ������������Ķ���
            string dataToStore = JsonUtility.ToJson(_data, true);

            //�����Ҫ���ܣ����ü��ܷ���
            if (encryptData)
            {
                dataToStore = EncryptAndDecrypt(dataToStore);
            }

            //ʹ���ļ����������ݵ��ļ�
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore); //д��JSON����
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error: Failed to save data file: \n{fullPath}\n {e.Message}"); //�������ʧ�ܣ����������Ϣ
        }
    }

    //������Ϸ��������
    public GameData LoadGameProgression()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName); //��ȡ�����ļ���·��
        GameData loadData = null;

        if (File.Exists(fullPath)) //����ļ�����
        {
            try
            {
                string dataToLoad = ""; //�����洢��ȡ������

                //ʹ���ļ�����ȡ����
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd(); //��ȡ�ļ�����
                    }
                }

                //���ܷ���
                if (encryptData)
                {
                    dataToLoad = EncryptAndDecrypt(dataToLoad);
                }

                //��JSON���ݽ���ΪGameData����
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to load game data from:\n{fullPath}\n{e.Message}"); // �������ʧ�ܣ����������Ϣ
            }
        }

        return loadData; //���ؼ��ص�����
    }

    #endregion

    //ɾ�������ļ�
    public void DeleteSave()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName); //��ȡ�������ļ�·��

        if (File.Exists(fullPath)) //����ļ�����
        {
            File.Delete(fullPath); //ɾ���ļ�
        }
    }

    //������������ݵĹ��ܣ�XOR���ܣ�
    private string EncryptAndDecrypt(string _data)
    {
        string result = "";

        //ʹ��XOR���ܣ�������㣩������ַ��ض����ݽ��м���/����
        for (int i = 0; i < _data.Length; i++)
        {
            result += (char)(_data[i] ^ codeWord[i % codeWord.Length]); //ʹ����Կ����XOR����
        }

        return result; //���ؼ��ܻ���ܺ�Ľ��
    }
}
