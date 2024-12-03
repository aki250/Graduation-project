using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGame_UI : MonoBehaviour
{
    public static InGame_UI instance;   //����ģʽʵ��

    [SerializeField] private PlayerStats playerStats;//���״̬��Ϣ
    [SerializeField] private Slider slider; //������ȴ�Ļ�����UI

    //���༼��ͼ��
    [SerializeField] private Image dashImage;   //���ܼ���
    [SerializeField] private Image parryImage;  //��������
    [SerializeField] private Image crystalImage;    //ˮ��
    [SerializeField] private Image throwSwordImage; //Ͷ��
    [SerializeField] private Image blackholeImage;  //�ڶ�


    [Header("����")]
    [SerializeField] private TextMeshProUGUI currentCurrency;   //������ʾ�ı�
    [SerializeField] private float currencyAmount;  //��ǰ��������
    [SerializeField] private float increaseRate = 100;  //������������
    [SerializeField] private float defaultcurrencyFontSize; //Ĭ�������С
    [SerializeField] private float currencyFontSizeWhenIncreasing;  //��������ʱ�������С

    private SkillManager skill;  //������Ҽ���
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
        // ��� playerStats ��Ϊ�գ������������ֵ�仯���¼�
        if (playerStats != null)
        {
            //����onHealthChanged�¼������������ֵ�����仯ʱ����UpdateHPUI��������ֵ
            playerStats.onHealthChanged += UpdateHPUI;
        }

        //��ȡ���ܹ�����ʵ�������ڹ�����Ҽ���
        skill = SkillManager.instance;

        //��ȡ���ʵ�������ڹ���ͻ�ȡ���״̬
        player = PlayerManager.instance.player;

        UpdateHPUI();
    }

    private void Update()
    {
        //���»���UI���������Ӷ���
        UpdateCurrencyUI();

        //��̼����ѽ���ʱ�����ó�̼��ܵ���ȴͼ��
        if (Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Dash"]) && skill.dash.dashUnlocked)
        {
            SetSkillCooldownImage(dashImage);
        }
        //�񵲼����ѽ���ʱ�����ø񵲼��ܵ���ȴͼ��
        if (Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Parry"]) && skill.parry.parryUnlocked)
        {
            SetSkillCooldownImage(parryImage);
        }

        //Ͷ�������ѽ���������Ͷ�����ܵ���ȴͼ��
        if (Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Aim"]) && skill.sword.throwSwordSkillUnlocked)
        {
            SetSkillCooldownImage(throwSwordImage);
        }

        //�ڶ������ѽ���ʱ�����úڶ����ܵ���ȴͼ��
        if (Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Blackhole"]) && skill.blackhole.blackholeUnlocked)
        {
            SetSkillCooldownImage(blackholeImage);
        }

        //���������ܵ���ȴʱ��ͼ�꣬������ȴ״̬
        FillSkillCooldownImage(dashImage, skill.dash.cooldown);
        FillSkillCooldownImage(parryImage, skill.parry.cooldown);
        FillSkillCooldownImage(crystalImage, skill.crystal.GetCrystalCooldown());
        FillSkillCooldownImage(throwSwordImage, skill.sword.cooldown);
        FillSkillCooldownImage(blackholeImage, skill.blackhole.cooldown);
        FillFlaskCooldownImage();
    }

    // ���»���UI��ʾ
    private void UpdateCurrencyUI()
    {
        //��ǰ��������С����ҵ�ʵ�ʻ���
        if (currencyAmount < PlayerManager.instance.GetCurrentCurrency())
        {
            //�����������ӣ���������
            IncraseCurrencyFontSize();

            //���ӻ�����������һ������������
            currencyAmount += Time.deltaTime * increaseRate;
        }
        else
        {
            //���򣬻��������ﵽĿ��ֵʱ������Ϊ���ʵ�ʻ��������ָ�Ĭ��ֵ
            currencyAmount = PlayerManager.instance.GetCurrentCurrency();
            DecreaseCurrencyFontSizeToDefault();
        }

        //��ʽ��������ʾ
        if (currencyAmount == 0)
        {
            currentCurrency.text = currencyAmount.ToString(); //�������Ϊ0����ʾΪ0
        }
        else
        {
            currentCurrency.text = currencyAmount.ToString("#,#"); //����ǧλ�ָ�����ʽ��ʾ
        }
    }

    //���ӻ��������С
    private void IncraseCurrencyFontSize()
    {
        currentCurrency.fontSize = Mathf.Lerp(currentCurrency.fontSize, currencyFontSizeWhenIncreasing, 100 * Time.deltaTime);
    }

    //�ָ����������С��Ĭ��ֵ
    private void DecreaseCurrencyFontSizeToDefault()
    {
        currentCurrency.fontSize = Mathf.Lerp(currentCurrency.fontSize, defaultcurrencyFontSize, 5 * Time.deltaTime);
    }

    //��������ֵUI��ʾ
    private void UpdateHPUI()
    {
        //�������ֵ
        slider.maxValue = playerStats.getMaxHP();
        //��ǰ����ֵ
        slider.value = playerStats.currentHP;
    }
    //������ȴͼ�����״̬
    private void SetSkillCooldownImage(Image _skillImage)
    {
        //���ܵ���ȴʱ���ѹ���fillAmountΪ0����ʾ���ܿ���ʹ�ã��ָ�Ĭ����ɫ
        if (_skillImage.fillAmount <= 0)
        {
            //fillAmountΪ1����ʾ���ܽ�����ȴ������ͼ��
            _skillImage.fillAmount = 1;
        }
    }

    //ˮ��������ȴͼ��
    public void SetCrystalCooldownImage()
    {
        SetSkillCooldownImage(crystalImage);
    }

    //��似����ȴͼ��Ľ�����
    private void FillSkillCooldownImage(Image _skillImage, float _cooldown)
    {
        //��鼼��ͼ���Ƿ�Ϊ��
        if (_skillImage == null)
        {
            return; 
        }

        //��ȴ��������δ����
        if (_skillImage.fillAmount > 0)
        {
            //������ȴ���ȵ��������𽥼���fillAmount����ʾ������ȴ�Ľ���
            //(_cooldown)��ʾ���ܵ���ȴʱ�䣬Time.deltaTime��ÿ֡��ʱ����
            //fillAmount��1��С��0
            _skillImage.fillAmount -= (1 / _cooldown) * Time.deltaTime;
        }
    }

    //���ҩˮ��ȴͼ�������
    private void FillFlaskCooldownImage()
    {
        //��ȡҩˮ��ȴͼ������
        Image _flaskImage = Flask_UI.instance.flaskCooldownImage;
        if (_flaskImage == null)
        {
            return;
        }

        //��ȡ��ǰװ��ҩˮ
        ItemData_Equipment flask = Inventory.instance.GetEquippedEquipmentByType(EquipmentType.Flask);
        if (flask == null)
        {
            return;
        }

        //ҩˮЧ��δ��ʹ�ã�����ȴ����������Ϊ0
        if (!flask.itemEffects[0].effectUsed)
        {
            _flaskImage.fillAmount = 0;  //���ý�����Ϊ��
            return;
        }

        //���ҩˮ��Ч����ʹ������ȴ���������ڸ���
        if (_flaskImage.fillAmount >= 0)
        {
            //����ҩˮ�ϴ�ʹ��,������ʱ��
            float timer = Time.time - flask.itemEffects[0].effectLastUseTime;

            //������ȴ���ȣ���������������𽥼���
            _flaskImage.fillAmount = 1 - ((1 / flask.itemEffects[0].effectCooldown) * timer);
        }
    }

}
