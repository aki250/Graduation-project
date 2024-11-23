using System.Collections;
using TMPro;
using UnityEngine;

//定义各种角色属性类型的枚举
public enum StatType
{
    strength,   //力量（增加伤害和暴击力量）
    agility,    //敏捷（增加闪避和暴击概率）
    intelligence,   //智力（增加魔法伤害和魔法抗性）
    vitality,   //体力（增加最大HP）
    damage, //物伤
    critChance, //暴击率
    critPower,  //暴伤
    maxHP,  //最大HP
    armor,  //护甲
    evasion,    //闪避
    magicResistance,    //魔法抗性
    fireDamage, //火焰伤害
    iceDamage,  //冰霜伤害
    lightningDamage //雷电伤害
}

//角色的各种状态和属性
public class CharacterStats : MonoBehaviour
{
    [Header("主属性")]
    public Stat strength;   //力量：增加伤害 +1，暴击伤害 +1%
    public Stat agility;    //敏捷：增加闪避 +1%，暴击几率 +1%
    public Stat intelligence;   //智力：增加魔法伤害 +1，魔法抗性 +3
    public Stat vitality;   //体力：增加最大HP +5
    public Stat maxHP;  //最大HP
    public Stat armor;  //护甲
    public Stat evasion;    //闪避
    public Stat magicResistance;    //魔法抗性

    [Header("物伤")]
    public Stat damage;        // 物理伤害
    public Stat critChance;    // 暴击几率
    public Stat critPower;     // 暴击伤害（默认150%）

    [Header("法伤")]
    public Stat fireDamage;    //火焰伤害
    public Stat iceDamage;     //冰霜伤害
    public Stat lightningDamage;//雷电伤害

    [Header("元素伤")]
    public bool isIgnited;  //是否被点燃（持续伤害）
    public bool isChilled;  //是否被冰冻（护甲 -20%）
    public bool isShocked;  //是否被雷击（准确度 -20%，敌人闪避 +20%）
    public Stat igniteDuration; //点燃持续时间
    public Stat chillDuration;  //冰冻持续时间
    public Stat shockDuration;  //雷击持续时间
    [SerializeField] private GameObject thunderStrikePrefab;    //雷击特效
    private int thunderStrikeDamage;    //雷击伤害

    //是否处于无敌状态
    public bool isInvincible { get; private set; }

    [Space]
    public int currentHP; //当前HP

    //Ailments定时器（用于持续伤害或效果的计时）
    private float ignitedAilmentTimer;  //点燃定时器
    private float chilledAilmentTimer;  //冰冻定时器
    private float shockedAilmentTimer;  //雷击定时器

    private float ignitedDamageCooldown = 0.3f; //点燃伤害冷却时间
    private float ignitedDamageTimer; //点燃伤害计时器
    private int igniteDamage; //点燃伤害（在DoMagicDamage中设置）

    //脆弱状态：承受更多伤害
    public bool isVulnerable { get; private set; }

    //角色是否死亡
    public bool isDead { get; private set; }

        /*                      各种属性的计算公式
        总闪避 = 闪避 + 敏捷 + [攻击者的震惊效果]
        总伤害 = (伤害 + 力量) * [总暴击力量] - 目标护甲 * [目标冰冻效果]
        总暴击几率 = 暴击几率 + 敏捷
        总暴击伤害 = 暴击伤害 + 力量
        总魔法伤害 = (火焰伤害 + 冰霜伤害 + 雷电伤害 + 智力) - (目标魔法抗性 + 3 * 目标智力)          */

    //角色血量变化时触发的事件
    public System.Action onHealthChanged;

    //特效
    protected EntityFX fx;

    protected virtual void Awake()
    {
        fx = GetComponent<EntityFX>();
    }

    protected virtual void Start()
    {
        currentHP = getMaxHP();  //当前HP为最大HP
        critPower.SetDefaultValue(150);  //默认暴击伤害为150%

        //HPBarCanBeInitialized = true; //初始化时标记为可以更新血条
    }

    protected virtual void Update()
    {
        //更新状态异常的定时器
        ignitedAilmentTimer -= Time.deltaTime;
        chilledAilmentTimer -= Time.deltaTime;
        shockedAilmentTimer -= Time.deltaTime;

        ignitedDamageTimer -= Time.deltaTime;

        //如果点燃定时器过期，移除点燃效果
        if (ignitedAilmentTimer < 0)
        {
            isIgnited = false;
        }

        //如果冰冻定时器过期，移除冰冻效果
        if (chilledAilmentTimer < 0)
        {
            isChilled = false;
        }

        //如果震惊定时器过期，移除震惊效果
        if (shockedAilmentTimer < 0)
        {
            isShocked = false;
        }

        //如果角色被点燃，则处理点燃伤害
        if (isIgnited)
        {
            DealIgniteDamage(); // 执行点燃伤害
        }
    }


    public virtual void DoDamge(CharacterStats _targetStats)
    {
        //如果目标是无敌状态，无法受到伤害
        if (_targetStats.isInvincible)
        {
            return;
        }

        //检查目标是否能够闪避这次攻击
        if (TargetCanEvadeThisAttack(_targetStats))
        {
            return;
        }

        //计算基础伤害，包含角色的物理伤害和力量加成
        int _totalDamage = damage.GetValue() + strength.GetValue();

        //判断是否触发暴击
        bool crit = CanCrit();

        if (crit)
        {
            _totalDamage = CalculatCritDamage(_totalDamage); //计算暴击伤害
        }

        //创建击中特效，传递目标和是否暴击的状态
        fx.CreateHitFX(_targetStats.transform, crit);

        //计算目标护甲对伤害的减免
        _totalDamage = CheckTargetArmor(_targetStats, _totalDamage);

        //检查目标是否处于脆弱状态，增加伤害
        _totalDamage = CheckTargetVulnerability(_targetStats, _totalDamage);

        //让目标承受伤害，并传递必要的参数（伤害值、攻击者、目标、是否暴击）
        _targetStats.TakeDamage(_totalDamage, transform, _targetStats.transform, crit);

        //魔法伤害相关函数
        //DoMagicDamage(_targetStats, transform);
    }

    public virtual void TakeDamage(int _damage, Transform _attacker, Transform _attackee, bool _isCrit)
    {
        //如果角色是无敌状态，则不受伤害
        if (isInvincible)
        {
            return;
        }

        //扣除伤害，判断是否为暴击
        DecreaseHPBy(_damage, _isCrit);

        // 为目标角色创建伤害反馈效果（如闪光、击退等）
        _attackee.GetComponent<Entity>()?.DamageFlashEffect();
        _attackee.GetComponent<Entity>()?.DamageKnockbackEffect(_attacker, _attackee);

        // 如果当前HP小于等于0，并且角色还没有死亡，则执行死亡逻辑
        if (currentHP <= 0 && !isDead)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        //如果角色已经死亡，则不再执行
        if (isDead)
        {
            return;
        }

        isDead = true;
        Debug.Log($"{gameObject.name} is Dead");
    }

    public virtual void DieFromFalling()
    {
        //角色从高处摔落死亡时调用
        if (isDead)
        {
            return;
        }

        //角色血量设置为0，触发血量变化事件
        currentHP = 0;

        if (onHealthChanged != null)
        {
            onHealthChanged(); //通知血量变化
        }

        // 执行死亡逻辑
        Die();
    }

    public void BecomeInvincible(bool _invincible)
    {
        //设置角色是否为无敌状态
        isInvincible = _invincible;
    }

    #region Magic and Ailments
    public virtual void DoMagicDamage(CharacterStats _targetStats, Transform _attacker)
    {
        //获取火焰、冰霜、雷电魔法伤害
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        //计算总魔法伤害
        int _totalMagicDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();

        //计算目标的魔法抗性
        _totalMagicDamage = CheckTargetMagicResistance(_targetStats, _totalMagicDamage);       

        //对目标造成魔法伤害
        _targetStats.TakeDamage(_totalMagicDamage, _attacker, _targetStats.transform, false);

        //如果至少有一种魔法伤害大于0，则可以应用状态异常
        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0)
        {
            return;
        }

        //尝试施加状态异常（点燃、冰冻、电击）
        AttemptToApplyAilments(_targetStats, _fireDamage, _iceDamage, _lightningDamage);
    }

    private void AttemptToApplyAilments(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightningDamage)
    {
        //根据魔法伤害选择应用的状态异常
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        //如果三种魔法伤害相等，随机选择一个施加状态异常
        if (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < 0.3f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                if (canApplyIgnite)
                {
                    _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * 0.2f)); //设置点燃伤害
                }
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < 0.5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < 0.5f && _lightningDamage > 0)
            {
                canApplyShock = true;
                if (canApplyShock)
                {
                    _targetStats.SetupThunderStrikeDamage(Mathf.RoundToInt(_lightningDamage * 0.2f)); //设置雷电伤害
                }
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }

        //为目标设置状态异常
        if (canApplyIgnite)
        {
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * 0.2f)); //点燃伤害
        }

        if (canApplyShock)
        {
            _targetStats.SetupThunderStrikeDamage(Mathf.RoundToInt(_lightningDamage * 0.2f)); //雷电伤害
        }

        //应用状态异常
        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        //检查目标是否已经处于某种状态，如果是，则不重复施加状态异常
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        //如果可以施加点燃效果，设置点燃状态并启动定时器
        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedAilmentTimer = igniteDuration.GetValue();
            StartCoroutine(fx.EnableIgniteFXForTime_Coroutine(ignitedAilmentTimer)); //启动点燃特效
        }

        //如果可以施加冰冻效果，设置冰冻状态并启动定时器
        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            chilledAilmentTimer = chillDuration.GetValue();
            StartCoroutine(fx.EnableChillFXForTime_Coroutine(chilledAilmentTimer)); //启动冰冻特效
            float _slowPercentage = 0.2f;
            GetComponent<Entity>()?.SlowSpeedBy(_slowPercentage, chillDuration.GetValue()); //减速效果
        }

        //如果可以施加震惊效果，设置震惊状态并启动定时器
        if (_shock && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShockAilment(_shock); //施加震惊效果
            }
            else //如果目标已经震惊，则生成雷电打击并攻击最近的敌人
            {
                if (GetComponent<Player>() != null)
                {
                    return; //玩家不能生成雷电打击攻击自己
                }
                GenerateThunderStrikeAndHitClosestEnemy(7.5f); //生成雷电打击并攻击最近的敌人
            }
        }

        //输出当前状态
        if (isIgnited)
        {
            Debug.Log($"{gameObject.name} is Ignited");
        }
        else if (isChilled)
        {
            Debug.Log($"{gameObject.name} is Chilled");
        }
        else if (isShocked)
        {
            Debug.Log($"{gameObject.name} is Shocked");
        }
    }

    public void ApplyShockAilment(bool _shock)
    {
        if (isShocked)
        {
            return;
        }

        //震慑状态并启动定时器
        isShocked = _shock;
        shockedAilmentTimer = shockDuration.GetValue();
        StartCoroutine(fx.EnableShockFXForTime_Coroutine(shockedAilmentTimer)); //震慑特效
    }

    private void GenerateThunderStrikeAndHitClosestEnemy(float _targetScanRadius)
    {
        //查找最近的敌人并生成雷电打击攻击
        Transform closestEnemy = null;

        //获取所有在扫描范围内的敌人
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _targetScanRadius);

        float closestDistanceToEnemy = Mathf.Infinity;

        //查找距离最近的敌人
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float currentDistanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (currentDistanceToEnemy < closestDistanceToEnemy)
                {
                    closestDistanceToEnemy = currentDistanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }

        //如果没有找到敌人，则目标是当前角色自己
        if (closestEnemy == null)
        {
            closestEnemy = transform;
        }

        //生成雷电打击并攻击最近的敌人
        if (closestEnemy != null)
        {
            GameObject newThunderStrike = Instantiate(thunderStrikePrefab, transform.position, Quaternion.identity);
            newThunderStrike.GetComponent<Skill_ThunderStrikeController>()?.Setup(thunderStrikeDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    //每秒一次点燃伤害
    private void DealIgniteDamage()
    {
        //如果点燃伤害计时器小于0，表示可以施加伤害（施加dot火伤
        if (ignitedDamageTimer < 0)
        {
            Debug.Log($"Take burn damage {igniteDamage}");
            DecreaseHPBy(igniteDamage, false);

            //如果血量小于等于0，且角色还未死亡，触发死亡
            if (currentHP <= 0 && !isDead)
            {
                Die();
            }
            //重置点燃伤害计时器
            ignitedDamageTimer = ignitedDamageCooldown;
        }
    }

    //点燃伤害
    public void SetupIgniteDamage(int _igniteDamage)
    {
        igniteDamage = _igniteDamage;
    }

    //雷伤
    public void SetupThunderStrikeDamage(int _thunderStrikeDamage)
    {
        thunderStrikeDamage = _thunderStrikeDamage;
    }
    #endregion



    #region 生命值和伤害计算 - 护甲、暴击、魔法抗性、脆弱性、闪避
    //检查目标的护甲，并计算最终伤害
    protected int CheckTargetArmor(CharacterStats _targetStats, int _totalDamage)
    {
        //目标被冰冻，降低其护甲值
        if (_targetStats.isChilled)
        {
            _totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * 0.8f);
        }
        else
        {
            _totalDamage -= _targetStats.armor.GetValue();
        }


        //确保伤害不为负值
        _totalDamage = Mathf.Clamp(_totalDamage, 0, int.MaxValue);
        return _totalDamage;
    }

    //检查目标的魔法抗性，计算最终魔法伤害
    private int CheckTargetMagicResistance(CharacterStats _targetStats, int _totalMagicDamage)
    {

        _totalMagicDamage -= _targetStats.magicResistance.GetValue() + (3 * _targetStats.intelligence.GetValue());

        //确保魔法伤害不为负值
        _totalMagicDamage = Mathf.Clamp(_totalMagicDamage, 0, int.MaxValue);
        return _totalMagicDamage;
    }

    //检查目标是否能闪避这次攻击
    protected bool TargetCanEvadeThisAttack(CharacterStats _targetStats)
    {
        int _totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        //如果目标被震惊，增加闪避值
        if (isShocked)
        {
            _totalEvasion += 20;
        }
        //判断目标是否闪避了攻击
        if (Random.Range(0, 100) < _totalEvasion)
        {
            //触发闪避效果
            _targetStats.OnEvasion();
            return true;
        }

        return false;
    }

    //判断是否触发暴击
    protected bool CanCrit()
    {
        int _totalCritChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) <= _totalCritChance)
        {
            return true;
        }

        return false;
    }

    //计算暴击伤害
    protected int CalculatCritDamage(int _damage)
    {
        //计算暴击伤害倍率
        float _totalCritPower = (critPower.GetValue() + strength.GetValue()) * 0.01f;

        //返回暴击伤害
        float critDamage = _damage * _totalCritPower;
        return Mathf.RoundToInt(critDamage);
    }

    //检查目标是否脆弱，如果是，增加伤害
    public int CheckTargetVulnerability(CharacterStats _targetStats, int _totalDamage)
    {
        if (_targetStats.isVulnerable)
        {
            _totalDamage = Mathf.RoundToInt(_totalDamage * 1.1f);
        }

        return _totalDamage;
    }

    //目标闪避时的回调函数
    public virtual void OnEvasion()
    {
    }


    #region HP
    //扣除指定的伤害并返回伤害值
    public virtual int DecreaseHPBy(int _takenDamage, bool _isCrit)
    {
        currentHP -= _takenDamage;

        //如果有伤害发生，显示伤害文本
        if (_takenDamage > 0)
        {
            GameObject popUpText = fx.CreatePopUpText(_takenDamage.ToString());

            //如果是暴击，显示特殊的暴击文本效果
            if (_isCrit)
            {
                popUpText.GetComponent<TextMeshPro>().color = Color.yellow;
                popUpText.GetComponent<TextMeshPro>().fontSize = 12;
            }
        }

        Debug.Log($"{gameObject.name} takes {_takenDamage} damage");

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }

        return _takenDamage;
    }

    //增加HP，最大HP不能超过上限
    public virtual void IncreaseHPBy(int _HP)
    {
        currentHP += _HP;

        if (currentHP > getMaxHP())
        {
            currentHP = getMaxHP();
        }

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }
    #endregion

    #endregion
    //使角色在指定时间内施加脆弱
    public void BecomeVulnerableForTime(float _seconds)
    {
        StartCoroutine(BecomeVulnerableForTime_Coroutine(_seconds));
    }

    //在指定时间内使角色脆弱
    private IEnumerator BecomeVulnerableForTime_Coroutine(float _duration)
    {
        isVulnerable = true;
        //Debug.Log("Vulnerable!");
        yield return new WaitForSeconds(_duration);
        isVulnerable = false;
        //Debug.Log("Exit Vulnerable!");
    }

    //在指定时间内增加指定属性
    public virtual void IncreaseStatByTime(Stat _statToModify, int _modifier, float _duration)
    {
        StartCoroutine(StatModify_Coroutine(_statToModify, _modifier, _duration));
    }

    //在指定时间内修改指定属性的值
    private IEnumerator StatModify_Coroutine(Stat _statToModify, int _modifier, float _duration)
    {
        _statToModify.AddModifier(_modifier);
        Inventory.instance.UpdateStatUI();

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
        Inventory.instance.UpdateStatUI();
    }

    //获取指定类型的属性
    public Stat GetStatByType(StatType _statType)
    {
        Stat stat = null;

        //根据属性类型返回相应的属性
        switch (_statType)
        {
            case StatType.strength: stat = strength; break;
            case StatType.agility: stat = agility; break;
            case StatType.intelligence: stat = intelligence; break;
            case StatType.vitality: stat = vitality; break;
            case StatType.damage: stat = damage; break;
            case StatType.critChance: stat = critChance; break;
            case StatType.critPower: stat = critPower; break;
            case StatType.maxHP: stat = maxHP; break;
            case StatType.armor: stat = armor; break;
            case StatType.evasion: stat = evasion; break;
            case StatType.magicResistance: stat = magicResistance; break;
            case StatType.fireDamage: stat = fireDamage; break;
            case StatType.iceDamage: stat = iceDamage; break;
            case StatType.lightningDamage: stat = lightningDamage; break;
        }

        return stat;
    }


    #region Get Final Stat Value

    //获取最大HP值
    public int getMaxHP()
    {
        return maxHP.GetValue() + vitality.GetValue() * 5;
    }

    //获取最终的伤害值
    public int GetDamage()
    {
        return damage.GetValue() + strength.GetValue();
    }

    //获取暴击力量值
    public int GetCritPower()
    {
        return critPower.GetValue() + strength.GetValue();
    }

    //获取暴击几率
    public int GetCritChance()
    {
        return critChance.GetValue() + agility.GetValue();
    }

    //获取闪避值
    public int GetEvasion()
    {
        return evasion.GetValue() + agility.GetValue();
    }

    //获取魔法抗性
    public int GetMagicResistance()
    {
        return magicResistance.GetValue() + intelligence.GetValue() * 3;
    }

    //获取火焰伤害
    public int GetFireDamage()
    {
        return fireDamage.GetValue() + intelligence.GetValue();
    }

    //获取冰霜伤害
    public int GetIceDamage()
    {
        return iceDamage.GetValue() + intelligence.GetValue();
    }

    //获取雷电伤害
    public int GetLightningDamage()
    {
        return lightningDamage.GetValue() + intelligence.GetValue();
    }

    #endregion
}
