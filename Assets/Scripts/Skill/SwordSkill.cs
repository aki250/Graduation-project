using UnityEngine;
using UnityEngine.UI;

public enum SwordType
{
    Regular,  //��ͨ��
    Bounce,   //������
    Pierce,   //��͸��
    Spin      //��ת��
}

//���������࣬�̳�Skill�������ҵĽ���
public class SwordSkill : Skill
{
    public SwordType swordType = SwordType.Regular;  //��ǰʹ�õĽ����ͣ�Ĭ��Ϊ��ͨ����

    [Header("Skill Info")]
    [SerializeField] private SkillTreeSlot_UI throwSwordSkillUnlockButton;  //Ͷ�������ܽ�����ť
    public bool throwSwordSkillUnlocked { get; private set; }  //Ͷ���������Ƿ����
    [SerializeField] private GameObject swordPrefab; 
    [SerializeField] private Vector2 launchSpeed;  //���ĳ�ʼ�ٶ�
    [SerializeField] private float swordReturnSpeed;  //�����յ��ٶ�
    private float swordGravity;  //����������ÿ�ֽ����Ͳ�ͬ����ʵҲ����ͬ���ţ����Ƕ������������ˣ����Ƿֿ��ɣ�

    [Header("Regular Sword Info")]
    [SerializeField] private float regularSwordGravity;  //��ͨ��������

    [Header("Bounce Sword Info")]
    [SerializeField] private SkillTreeSlot_UI bounceSwordUnlockButton;  //������������ť
    public bool bounceSwordUnlocked { get; private set; }  //�������Ƿ����
    [SerializeField] private int bounceAmount;  //��������
    [SerializeField] private float bounceSwordGravity;  //����
    [SerializeField] private float bounceSpeed;  //�ٶ�

    [Header("Pierce Sword Info")]
    [SerializeField] private SkillTreeSlot_UI pierceSwordUnlockButton;  //��͸������
    public bool pierceSwordUnlocked { get; private set; }
    [SerializeField] private int pierceAmount;  //��͸����
    [SerializeField] private float pierceSwordGravity;  //����

    [Header("Spin Sword Info")]
    [SerializeField] private SkillTreeSlot_UI spinSwordUnlockButton;  //��ת������
    public bool spinSwordUnlocked { get; private set; }  
    [SerializeField] private float maxTravelDistance;  //�����о���
    [SerializeField] private float spinDuration;  //����ʱ��
    [SerializeField] private float spinHitCooldown;  //������ȴʱ��
    [SerializeField] private float spinSwordGravity;  //����

    [Header("Passive Skill Info")]
    [SerializeField] private SkillTreeSlot_UI timeStopUnlockButton;  //ʱ��ֹͣ���ܽ���
    [SerializeField] private float enemyFreezeDuration;  //������˳���ʱ��
    public bool timeStopUnlocked { get; private set; } 
    [SerializeField] private SkillTreeSlot_UI vulnerabilityUnlockButton;  //���˴������ܽ���
    [SerializeField] private float enemyVulnerableDuration;  //����ʱ��
    public bool vulnerabilityUnlocked { get; private set; } 

    private Vector2 finalDirection;  //���յ�Ͷ�����򣨸��������׼����ó���

    [Header("Aim Dots")]
    [SerializeField] private int dotNumber;  //��׼�������
    [SerializeField] private float spaceBetweenDots;  //��׼��֮��ļ��
    [SerializeField] private GameObject dotPrefab;  //��׼��Ԥ����
    [SerializeField] private Transform dotsParent;  //��׼��ĸ�����

    private GameObject[] dots;  //��׼������

    protected override void Start()
    {
        base.Start();

        //������׼��
        GenerateDots();

        // ���ø����ܽ�����ť�ļ����¼�
        throwSwordSkillUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockThrowSwordSkill);
        bounceSwordUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockBounceSword);
        pierceSwordUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockPierceSword);
        spinSwordUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockSpinSword);
        timeStopUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockTimeStop);
        vulnerabilityUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockVulnerability);
    }

    protected override void Update()
    {
        // ���㽣��Ͷ���켣
        if (Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Attack"]))  //����Ͷ����
        {
            finalDirection = new Vector2(AimDirection().normalized.x * launchSpeed.x, AimDirection().normalized.y * launchSpeed.y);
        }

        if (Input.GetKey(KeyBindManager.instance.keybindsDictionary["Aim"]))  //��׼���λ�á�������׼
        {
            SetupSwordGravity();
            if (player.stateMachine.currentState != player.throwSwordState)
            {
                for (int i = 0; i < dots.Length; i++)
                {
                    dots[i].transform.position = SetDotsPosition(i * spaceBetweenDots);  //����ÿ����׼���λ��
                }
            }
        }
    }

    //����Ͷ����
    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        SwordSkillController newSwordScript = newSword.GetComponent<SwordSkillController>();

        SetupSwordGravity();  //���ý�������

        //���ݽ������������������
        if (swordType == SwordType.Bounce)
        {
            newSwordScript.SetupBounceSword(true, bounceAmount, bounceSpeed);
        }
        else if (swordType == SwordType.Pierce)
        {
            newSwordScript.SetupPierceSword(true, pierceAmount);
        }
        else if (swordType == SwordType.Spin)
        {
            newSwordScript.SetupSpinSword(true, maxTravelDistance, spinDuration, spinHitCooldown);
        }

        newSwordScript.SetupSword(finalDirection, swordGravity, swordReturnSpeed, enemyFreezeDuration, enemyVulnerableDuration);
        player.AssignNewSword(newSword);  //��Ҵ����µĽ�
        ShowDots(false);  //������׼��
    }

    //���ò�ͬ��������
    private void SetupSwordGravity()
    {
        if (swordType == SwordType.Bounce)
        {
            swordGravity = bounceSwordGravity;
        }
        else if (swordType == SwordType.Pierce)
        {
            swordGravity = pierceSwordGravity;
        }
        else if (swordType == SwordType.Regular)
        {
            swordGravity = regularSwordGravity;
        }
        else if (swordType == SwordType.Spin)
        {
            swordGravity = spinSwordGravity;
        }
    }

    #region Aim
    //������ҵ���׼����
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);  //��ȡ���λ��
        Vector2 direction = mousePosition - playerPosition;  //������������ķ���
        return direction;
    }

    //������׼��
    private void GenerateDots()
    {
        dots = new GameObject[dotNumber];
        for (int i = 0; i < dotNumber; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);  //��ʼʱ����ʾ
        }
    }

    //��ʾ��������׼��
    public void ShowDots(bool _showDots)
    {
        for (int i = 0; i < dotNumber; i++)
        {
            dots[i].SetActive(_showDots);
        }
    }

    //����ʱ�������׼���λ��             
    private Vector2 SetDotsPosition(float t)
    {
        //б���˶��Ĺ�ʽ��ˮƽ�������٣���ֱ�����ܵ�����Ӱ��
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchSpeed.x,
            AimDirection().normalized.y * launchSpeed.y) * t  //ˮƽ�����˶�
            + 0.5f * (Physics2D.gravity * swordGravity) * (t * t);  //��ֱ����������Ӱ��

        return position;
    }

    #endregion


    //���浵���ѽ����ļ���
    protected override void CheckUnlockFromSave()
    {
        UnlockThrowSwordSkill();
        UnlockTimeStop();
        UnlockVulnerability();
        UnlockBounceSword();
        UnlockPierceSword();
        UnlockSpinSword();
    }

    #region Unlock Skill

    //����Ͷ��������
    private void UnlockThrowSwordSkill()
    {
        if (throwSwordSkillUnlocked)
        {
            return;
        }

        if (throwSwordSkillUnlockButton.unlocked)
        {
            throwSwordSkillUnlocked = true;
            swordType = SwordType.Regular;  //Ĭ��ʹ����ͨ��
        }
    }

    //��������������
    private void UnlockBounceSword()
    {
        if (bounceSwordUnlocked)
        {
            return;
        }

        if (bounceSwordUnlockButton.unlocked)
        {
            bounceSwordUnlocked = true;
            swordType = SwordType.Bounce;
        }
    }

    //������͸������
    private void UnlockPierceSword()
    {
        if (pierceSwordUnlocked)
        {
            return;
        }

        if (pierceSwordUnlockButton.unlocked)
        {
            pierceSwordUnlocked = true;
            swordType = SwordType.Pierce;
        }
    }

    //������ת������
    private void UnlockSpinSword()
    {
        if (spinSwordUnlocked)
        {
            return;
        }

        if (spinSwordUnlockButton.unlocked)
        {
            spinSwordUnlocked = true;
            swordType = SwordType.Spin;
        }
    }

    //����ʱ��ֹͣ����
    private void UnlockTimeStop()
    {
        if (timeStopUnlocked)
        {
            return;
        }

        if (timeStopUnlockButton.unlocked)
        {
            timeStopUnlocked = true;
        }
    }

    //�������˴�������
    private void UnlockVulnerability()
    {
        if (vulnerabilityUnlocked)
        {
            return;
        }

        if (vulnerabilityUnlockButton.unlocked)
        {
            vulnerabilityUnlocked = true;
        }
    }
    #endregion
}
