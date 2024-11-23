using UnityEngine;
using UnityEngine.UI;

public enum SwordType
{
    Regular,  //普通剑
    Bounce,   //反弹剑
    Pierce,   //穿透剑
    Spin      //旋转剑
}

//刀剑技能类，继承Skill类管理玩家的剑技
public class SwordSkill : Skill
{
    public SwordType swordType = SwordType.Regular;  //当前使用的剑类型（默认为普通剑）

    [Header("Skill Info")]
    [SerializeField] private SkillTreeSlot_UI throwSwordSkillUnlockButton;  //投掷剑技能解锁按钮
    public bool throwSwordSkillUnlocked { get; private set; }  //投掷剑技能是否解锁
    [SerializeField] private GameObject swordPrefab; 
    [SerializeField] private Vector2 launchSpeed;  //剑的初始速度
    [SerializeField] private float swordReturnSpeed;  //剑回收的速度
    private float swordGravity;  //剑的重力（每种剑类型不同，其实也想相同来着，但是都这样做大框架了，还是分开吧）

    [Header("Regular Sword Info")]
    [SerializeField] private float regularSwordGravity;  //普通剑的重力

    [Header("Bounce Sword Info")]
    [SerializeField] private SkillTreeSlot_UI bounceSwordUnlockButton;  //反弹剑解锁按钮
    public bool bounceSwordUnlocked { get; private set; }  //反弹剑是否解锁
    [SerializeField] private int bounceAmount;  //反弹次数
    [SerializeField] private float bounceSwordGravity;  //重力
    [SerializeField] private float bounceSpeed;  //速度

    [Header("Pierce Sword Info")]
    [SerializeField] private SkillTreeSlot_UI pierceSwordUnlockButton;  //穿透剑解锁
    public bool pierceSwordUnlocked { get; private set; }
    [SerializeField] private int pierceAmount;  //穿透次数
    [SerializeField] private float pierceSwordGravity;  //重力

    [Header("Spin Sword Info")]
    [SerializeField] private SkillTreeSlot_UI spinSwordUnlockButton;  //旋转剑解锁
    public bool spinSwordUnlocked { get; private set; }  
    [SerializeField] private float maxTravelDistance;  //最大飞行距离
    [SerializeField] private float spinDuration;  //持续时间
    [SerializeField] private float spinHitCooldown;  //攻击冷却时间
    [SerializeField] private float spinSwordGravity;  //重力

    [Header("Passive Skill Info")]
    [SerializeField] private SkillTreeSlot_UI timeStopUnlockButton;  //时间停止技能解锁
    [SerializeField] private float enemyFreezeDuration;  //冻结敌人持续时间
    public bool timeStopUnlocked { get; private set; } 
    [SerializeField] private SkillTreeSlot_UI vulnerabilityUnlockButton;  //敌人脆弱技能解锁
    [SerializeField] private float enemyVulnerableDuration;  //持续时间
    public bool vulnerabilityUnlocked { get; private set; } 

    private Vector2 finalDirection;  //最终的投掷方向（根据玩家瞄准计算得出）

    [Header("Aim Dots")]
    [SerializeField] private int dotNumber;  //瞄准点的数量
    [SerializeField] private float spaceBetweenDots;  //瞄准点之间的间距
    [SerializeField] private GameObject dotPrefab;  //瞄准点预制体
    [SerializeField] private Transform dotsParent;  //瞄准点的父物体

    private GameObject[] dots;  //瞄准点数组

    protected override void Start()
    {
        base.Start();

        //生成瞄准点
        GenerateDots();

        // 设置各技能解锁按钮的监听事件
        throwSwordSkillUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockThrowSwordSkill);
        bounceSwordUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockBounceSword);
        pierceSwordUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockPierceSword);
        spinSwordUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockSpinSword);
        timeStopUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockTimeStop);
        vulnerabilityUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockVulnerability);
    }

    protected override void Update()
    {
        // 计算剑的投掷轨迹
        if (Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Attack"]))  //触发投掷剑
        {
            finalDirection = new Vector2(AimDirection().normalized.x * launchSpeed.x, AimDirection().normalized.y * launchSpeed.y);
        }

        if (Input.GetKey(KeyBindManager.instance.keybindsDictionary["Aim"]))  //瞄准点的位置、持续瞄准
        {
            SetupSwordGravity();
            if (player.stateMachine.currentState != player.throwSwordState)
            {
                for (int i = 0; i < dots.Length; i++)
                {
                    dots[i].transform.position = SetDotsPosition(i * spaceBetweenDots);  //设置每个瞄准点的位置
                }
            }
        }
    }

    //创建投掷剑
    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        SwordSkillController newSwordScript = newSword.GetComponent<SwordSkillController>();

        SetupSwordGravity();  //设置剑的重力

        //根据剑的类型设置相关属性
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
        player.AssignNewSword(newSword);  //玩家创造新的剑
        ShowDots(false);  //隐藏瞄准点
    }

    //设置不同剑的重力
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
    //计算玩家的瞄准方向
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);  //获取鼠标位置
        Vector2 direction = mousePosition - playerPosition;  //计算玩家与鼠标的方向
        return direction;
    }

    //生成瞄准点
    private void GenerateDots()
    {
        dots = new GameObject[dotNumber];
        for (int i = 0; i < dotNumber; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);  //初始时不显示
        }
    }

    //显示或隐藏瞄准点
    public void ShowDots(bool _showDots)
    {
        for (int i = 0; i < dotNumber; i++)
        {
            dots[i].SetActive(_showDots);
        }
    }

    //根据时间计算瞄准点的位置             
    private Vector2 SetDotsPosition(float t)
    {
        //斜抛运动的公式：水平方向匀速，竖直方向受到重力影响
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchSpeed.x,
            AimDirection().normalized.y * launchSpeed.y) * t  //水平方向运动
            + 0.5f * (Physics2D.gravity * swordGravity) * (t * t);  //竖直方向受重力影响

        return position;
    }

    #endregion


    //检查存档中已解锁的技能
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

    //解锁投掷剑技能
    private void UnlockThrowSwordSkill()
    {
        if (throwSwordSkillUnlocked)
        {
            return;
        }

        if (throwSwordSkillUnlockButton.unlocked)
        {
            throwSwordSkillUnlocked = true;
            swordType = SwordType.Regular;  //默认使用普通剑
        }
    }

    //解锁反弹剑技能
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

    //解锁穿透剑技能
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

    //解锁旋转剑技能
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

    //解锁时间停止被动
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

    //解锁敌人脆弱技能
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
