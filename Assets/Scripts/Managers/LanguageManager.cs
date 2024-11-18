using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoBehaviour, ISettingsSaveManager
{
    public static LanguageManager instance;

    //0英文，1中文
    public int localeID { get; set; }

    //英文到中文的映射字典
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
        //初始化字典
        EnglishToChineseKeybindsDictionary = new Dictionary<string, string>();
        EnglishToChineseEquipmentTypeDictionary = new Dictionary<string, string>();
        EnglishToChineseStatDictionary = new Dictionary<string, string>();

        //设置字典内容
        SetupEnglishToChineseKeybindsDictionary();
        SetupEnglishToChineseEquipmentTypeDictionary();
        SetupEnglishToChineseStatDictionary();
    }

    //根据localeID设置当前的语言
    public void SetTextLanguageByLocaleID(int _localeID)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
    }

    //设置英文到中文的按键绑定字典
    private void SetupEnglishToChineseKeybindsDictionary()
    {
        EnglishToChineseKeybindsDictionary.Add("Attack", "攻击");
        EnglishToChineseKeybindsDictionary.Add("Aim", "瞄准");
        EnglishToChineseKeybindsDictionary.Add("Flask", "元素瓶");
        EnglishToChineseKeybindsDictionary.Add("Dash", "冲刺");
        EnglishToChineseKeybindsDictionary.Add("Parry", "弹反");
        EnglishToChineseKeybindsDictionary.Add("Crystal", "水晶");
        EnglishToChineseKeybindsDictionary.Add("Blackhole", "黑洞");
        EnglishToChineseKeybindsDictionary.Add("Character", "角色面板");
        EnglishToChineseKeybindsDictionary.Add("Craft", "制造面板");
        EnglishToChineseKeybindsDictionary.Add("Skill", "技能面板");
    }

    //设置英文到中文的装备类型字典
    private void SetupEnglishToChineseEquipmentTypeDictionary()
    {
        EnglishToChineseEquipmentTypeDictionary.Add("Weapon", "武器");
        EnglishToChineseEquipmentTypeDictionary.Add("Armor", "护甲");
        EnglishToChineseEquipmentTypeDictionary.Add("Charm", "护身符");
        EnglishToChineseEquipmentTypeDictionary.Add("Flask", "元素瓶");
    }

    //设置英文到中文的属性字典
    private void SetupEnglishToChineseStatDictionary()
    {
        EnglishToChineseStatDictionary.Add("Strength", "力量");
        EnglishToChineseStatDictionary.Add("Agility", "敏捷");
        EnglishToChineseStatDictionary.Add("Intelligence", "智力");
        EnglishToChineseStatDictionary.Add("Vitality", "活力");
        EnglishToChineseStatDictionary.Add("Damage", "伤害");
        EnglishToChineseStatDictionary.Add("Crit Chance", "暴击率");
        EnglishToChineseStatDictionary.Add("Crit Power", "暴击伤害");
        EnglishToChineseStatDictionary.Add("Max HP", "最大生命值");
        EnglishToChineseStatDictionary.Add("Evasion", "闪避");
        EnglishToChineseStatDictionary.Add("Armor", "护甲");
        EnglishToChineseStatDictionary.Add("Magic Resist", "魔法抗性");
        EnglishToChineseStatDictionary.Add("Fire Dmg", "火焰伤害");
        EnglishToChineseStatDictionary.Add("Ice Dmg", "寒冰伤害");
        EnglishToChineseStatDictionary.Add("Lightning Dmg", "雷电伤害");
    }

    //根据中文名称获取对应的英文名称
    public string GetEnglishNameByChinese(string _chineseName)
    {
        string result = string.Empty;

        //遍历字典找到匹配的英文名称
        foreach (var search in EnglishToChineseKeybindsDictionary)
        {
            if (search.Value == _chineseName)
            {
                result = search.Key;
            }
        }

        return result;
    }

    //将物品属性信息从英文翻译为中文
    public string TranslateItemStatInfoFromEnglishToChinese(string _itemStatInfo)
    {
        //遍历所有属性，进行替换
        foreach (var search in EnglishToChineseStatDictionary)
        {
            string stat_English = search.Key;

            _itemStatInfo = _itemStatInfo.Replace(stat_English, EnglishToChineseStatDictionary[stat_English]);
        }

        return _itemStatInfo;
    }

    //从保存的数据中加载语言设置
    public void LoadData(SettingsData _data)
    {
        localeID = _data.localeID;

        //设置语言
        SetTextLanguageByLocaleID(localeID);
    }

    //将当前语言设置保存到数据中
    public void SaveData(ref SettingsData _data)
    {
        _data.localeID = localeID;
    }
}
