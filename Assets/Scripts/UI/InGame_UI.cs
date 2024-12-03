using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGame_UI : MonoBehaviour
{
    public static InGame_UI instance;   //单例模式实例

    [SerializeField] private PlayerStats playerStats;//玩家状态信息
    [SerializeField] private Slider slider; //技能冷却的滑动条UI

    //各类技能图标
    [SerializeField] private Image dashImage;   //闪避技能
    [SerializeField] private Image parryImage;  //反击技能
    [SerializeField] private Image crystalImage;    //水晶
    [SerializeField] private Image throwSwordImage; //投剑
    [SerializeField] private Image blackholeImage;  //黑洞


    [Header("货币")]
    [SerializeField] private TextMeshProUGUI currentCurrency;   //货币显示文本
    [SerializeField] private float currencyAmount;  //当前货币数量
    [SerializeField] private float increaseRate = 100;  //货币增加速率
    [SerializeField] private float defaultcurrencyFontSize; //默认字体大小
    [SerializeField] private float currencyFontSizeWhenIncreasing;  //货币增加时的字体大小

    private SkillManager skill;  //管理玩家技能
    private Player player;

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
    }
    private void Start()
    {
        // 如果 playerStats 不为空，订阅玩家生命值变化的事件
        if (playerStats != null)
        {
            //订阅onHealthChanged事件，当玩家生命值发生变化时调用UpdateHPUI更新生命值
            playerStats.onHealthChanged += UpdateHPUI;
        }

        //获取技能管理器实例，用于管理玩家技能
        skill = SkillManager.instance;

        //获取玩家实例，用于管理和获取玩家状态
        player = PlayerManager.instance.player;

        UpdateHPUI();
    }

    private void Update()
    {
        //更新货币UI，货币增加动画
        UpdateCurrencyUI();

        //冲刺技能已解锁时，设置冲刺技能的冷却图标
        if (Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Dash"]) && skill.dash.dashUnlocked)
        {
            SetSkillCooldownImage(dashImage);
        }
        //格挡技能已解锁时，设置格挡技能的冷却图标
        if (Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Parry"]) && skill.parry.parryUnlocked)
        {
            SetSkillCooldownImage(parryImage);
        }

        //投剑技能已解锁，设置投剑技能的冷却图标
        if (Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Aim"]) && skill.sword.throwSwordSkillUnlocked)
        {
            SetSkillCooldownImage(throwSwordImage);
        }

        //黑洞技能已解锁时，设置黑洞技能的冷却图标
        if (Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Blackhole"]) && skill.blackhole.blackholeUnlocked)
        {
            SetSkillCooldownImage(blackholeImage);
        }

        //填充各个技能的冷却时间图标，更新冷却状态
        FillSkillCooldownImage(dashImage, skill.dash.cooldown);
        FillSkillCooldownImage(parryImage, skill.parry.cooldown);
        FillSkillCooldownImage(crystalImage, skill.crystal.GetCrystalCooldown());
        FillSkillCooldownImage(throwSwordImage, skill.sword.cooldown);
        FillSkillCooldownImage(blackholeImage, skill.blackhole.cooldown);
        FillFlaskCooldownImage();
    }

    // 更新货币UI显示
    private void UpdateCurrencyUI()
    {
        //当前货币数量小于玩家的实际货币
        if (currencyAmount < PlayerManager.instance.GetCurrentCurrency())
        {
            //货币数量增加，增大字体
            IncraseCurrencyFontSize();

            //增加货币数量，按一定的增速增长
            currencyAmount += Time.deltaTime * increaseRate;
        }
        else
        {
            //否则，货币数量达到目标值时，设置为玩家实际货币数，恢复默认值
            currencyAmount = PlayerManager.instance.GetCurrentCurrency();
            DecreaseCurrencyFontSizeToDefault();
        }

        //格式化货币显示
        if (currencyAmount == 0)
        {
            currentCurrency.text = currencyAmount.ToString(); //如果货币为0，显示为0
        }
        else
        {
            currentCurrency.text = currencyAmount.ToString("#,#"); //否则按千位分隔符格式显示
        }
    }

    //增加货币字体大小
    private void IncraseCurrencyFontSize()
    {
        currentCurrency.fontSize = Mathf.Lerp(currentCurrency.fontSize, currencyFontSizeWhenIncreasing, 100 * Time.deltaTime);
    }

    //恢复货币字体大小到默认值
    private void DecreaseCurrencyFontSizeToDefault()
    {
        currentCurrency.fontSize = Mathf.Lerp(currentCurrency.fontSize, defaultcurrencyFontSize, 5 * Time.deltaTime);
    }

    //更新生命值UI显示
    private void UpdateHPUI()
    {
        //最大生命值
        slider.maxValue = playerStats.getMaxHP();
        //当前生命值
        slider.value = playerStats.currentHP;
    }
    //技能冷却图标填充状态
    private void SetSkillCooldownImage(Image _skillImage)
    {
        //技能的冷却时间已过，fillAmount为0，表示技能可以使用，恢复默认颜色
        if (_skillImage.fillAmount <= 0)
        {
            //fillAmount为1，表示技能进入冷却，填满图标
            _skillImage.fillAmount = 1;
        }
    }

    //水晶技能冷却图标
    public void SetCrystalCooldownImage()
    {
        SetSkillCooldownImage(crystalImage);
    }

    //填充技能冷却图标的进度条
    private void FillSkillCooldownImage(Image _skillImage, float _cooldown)
    {
        //检查技能图标是否为空
        if (_skillImage == null)
        {
            return; 
        }

        //冷却进度条尚未填满
        if (_skillImage.fillAmount > 0)
        {
            //计算冷却进度的增量，逐渐减少fillAmount，表示技能冷却的进度
            //(_cooldown)表示技能的冷却时间，Time.deltaTime是每帧的时间间隔
            //fillAmount从1减小到0
            _skillImage.fillAmount -= (1 / _cooldown) * Time.deltaTime;
        }
    }

    //填充药水冷却图标进度条
    private void FillFlaskCooldownImage()
    {
        //获取药水冷却图标引用
        Image _flaskImage = Flask_UI.instance.flaskCooldownImage;
        if (_flaskImage == null)
        {
            return;
        }

        //获取当前装备药水
        ItemData_Equipment flask = Inventory.instance.GetEquippedEquipmentByType(EquipmentType.Flask);
        if (flask == null)
        {
            return;
        }

        //药水效果未被使用，则冷却进度条保持为0
        if (!flask.itemEffects[0].effectUsed)
        {
            _flaskImage.fillAmount = 0;  //设置进度条为空
            return;
        }

        //如果药水的效果已使用且冷却进度条正在更新
        if (_flaskImage.fillAmount >= 0)
        {
            //计算药水上次使用,以来的时间
            float timer = Time.time - flask.itemEffects[0].effectLastUseTime;

            //计算冷却进度，将进度条填充量逐渐减少
            _flaskImage.fillAmount = 1 - ((1 / flask.itemEffects[0].effectCooldown) * timer);
        }
    }

}
