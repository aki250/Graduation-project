using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoBehaviour, ISettingsSaveManager
{
    public static LanguageManager instance;

    //0Ӣ�ģ�1����
    public int localeID { get; set; }

    //Ӣ�ĵ����ĵ�ӳ���ֵ�
    public Dictionary<string, string> EnglishToChineseKeybindsDictionary;
    public Dictionary<string, string> EnglishToChineseEquipmentTypeDictionary;
    public Dictionary<string, string> EnglishToChineseStatDictionary;

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
        //��ʼ���ֵ�
        EnglishToChineseKeybindsDictionary = new Dictionary<string, string>();
        EnglishToChineseEquipmentTypeDictionary = new Dictionary<string, string>();
        EnglishToChineseStatDictionary = new Dictionary<string, string>();

        //�����ֵ�����
        SetupEnglishToChineseKeybindsDictionary();
        SetupEnglishToChineseEquipmentTypeDictionary();
        SetupEnglishToChineseStatDictionary();
    }

    //����localeID���õ�ǰ������
    public void SetTextLanguageByLocaleID(int _localeID)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
    }

    //����Ӣ�ĵ����ĵİ������ֵ�
    private void SetupEnglishToChineseKeybindsDictionary()
    {
        EnglishToChineseKeybindsDictionary.Add("Attack", "����");
        EnglishToChineseKeybindsDictionary.Add("Aim", "��׼");
        EnglishToChineseKeybindsDictionary.Add("Flask", "Ԫ��ƿ");
        EnglishToChineseKeybindsDictionary.Add("Dash", "���");
        EnglishToChineseKeybindsDictionary.Add("Parry", "����");
        EnglishToChineseKeybindsDictionary.Add("Crystal", "ˮ��");
        EnglishToChineseKeybindsDictionary.Add("Blackhole", "�ڶ�");
        EnglishToChineseKeybindsDictionary.Add("Character", "��ɫ���");
        EnglishToChineseKeybindsDictionary.Add("Craft", "�������");
        EnglishToChineseKeybindsDictionary.Add("Skill", "�������");
    }

    //����Ӣ�ĵ����ĵ�װ�������ֵ�
    private void SetupEnglishToChineseEquipmentTypeDictionary()
    {
        EnglishToChineseEquipmentTypeDictionary.Add("Weapon", "����");
        EnglishToChineseEquipmentTypeDictionary.Add("Armor", "����");
        EnglishToChineseEquipmentTypeDictionary.Add("Charm", "�����");
        EnglishToChineseEquipmentTypeDictionary.Add("Flask", "Ԫ��ƿ");
    }

    //����Ӣ�ĵ����ĵ������ֵ�
    private void SetupEnglishToChineseStatDictionary()
    {
        EnglishToChineseStatDictionary.Add("Strength", "����");
        EnglishToChineseStatDictionary.Add("Agility", "����");
        EnglishToChineseStatDictionary.Add("Intelligence", "����");
        EnglishToChineseStatDictionary.Add("Vitality", "����");
        EnglishToChineseStatDictionary.Add("Damage", "�˺�");
        EnglishToChineseStatDictionary.Add("Crit Chance", "������");
        EnglishToChineseStatDictionary.Add("Crit Power", "�����˺�");
        EnglishToChineseStatDictionary.Add("Max HP", "�������ֵ");
        EnglishToChineseStatDictionary.Add("Evasion", "����");
        EnglishToChineseStatDictionary.Add("Armor", "����");
        EnglishToChineseStatDictionary.Add("Magic Resist", "ħ������");
        EnglishToChineseStatDictionary.Add("Fire Dmg", "�����˺�");
        EnglishToChineseStatDictionary.Add("Ice Dmg", "�����˺�");
        EnglishToChineseStatDictionary.Add("Lightning Dmg", "�׵��˺�");
    }

    //�����������ƻ�ȡ��Ӧ��Ӣ������
    public string GetEnglishNameByChinese(string _chineseName)
    {
        string result = string.Empty;

        //�����ֵ��ҵ�ƥ���Ӣ������
        foreach (var search in EnglishToChineseKeybindsDictionary)
        {
            if (search.Value == _chineseName)
            {
                result = search.Key;
            }
        }

        return result;
    }

    //����Ʒ������Ϣ��Ӣ�ķ���Ϊ����
    public string TranslateItemStatInfoFromEnglishToChinese(string _itemStatInfo)
    {
        //�����������ԣ������滻
        foreach (var search in EnglishToChineseStatDictionary)
        {
            string stat_English = search.Key;

            _itemStatInfo = _itemStatInfo.Replace(stat_English, EnglishToChineseStatDictionary[stat_English]);
        }

        return _itemStatInfo;
    }

    //�ӱ���������м�����������
    public void LoadData(SettingsData _data)
    {
        localeID = _data.localeID;

        //��������
        SetTextLanguageByLocaleID(localeID);
    }

    //����ǰ�������ñ��浽������
    public void SaveData(ref SettingsData _data)
    {
        _data.localeID = localeID;
    }
}
