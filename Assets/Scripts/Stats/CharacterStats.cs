using System.Collections;
using TMPro;
using UnityEngine;

//������ֽ�ɫ�������͵�ö��
public enum StatType
{
    strength,   //�����������˺��ͱ���������
    agility,    //���ݣ��������ܺͱ������ʣ�
    intelligence,   //����������ħ���˺���ħ�����ԣ�
    vitality,   //�������������HP��
    damage, //����
    critChance, //������
    critPower,  //����
    maxHP,  //���HP
    armor,  //����
    evasion,    //����
    magicResistance,    //ħ������
    fireDamage, //�����˺�
    iceDamage,  //��˪�˺�
    lightningDamage //�׵��˺�
}

//��ɫ�ĸ���״̬������
public class CharacterStats : MonoBehaviour
{
    [Header("������")]
    public Stat strength;   //�����������˺� +1�������˺� +1%
    public Stat agility;    //���ݣ��������� +1%���������� +1%
    public Stat intelligence;   //����������ħ���˺� +1��ħ������ +3
    public Stat vitality;   //�������������HP +5
    public Stat maxHP;  //���HP
    public Stat armor;  //����
    public Stat evasion;    //����
    public Stat magicResistance;    //ħ������

    [Header("����")]
    public Stat damage;        // �����˺�
    public Stat critChance;    // ��������
    public Stat critPower;     // �����˺���Ĭ��150%��

    [Header("����")]
    public Stat fireDamage;    //�����˺�
    public Stat iceDamage;     //��˪�˺�
    public Stat lightningDamage;//�׵��˺�

    [Header("Ԫ����")]
    public bool isIgnited;  //�Ƿ񱻵�ȼ�������˺���
    public bool isChilled;  //�Ƿ񱻱��������� -20%��
    public bool isShocked;  //�Ƿ��׻���׼ȷ�� -20%���������� +20%��
    public Stat igniteDuration; //��ȼ����ʱ��
    public Stat chillDuration;  //��������ʱ��
    public Stat shockDuration;  //�׻�����ʱ��
    [SerializeField] private GameObject thunderStrikePrefab;    //�׻���Ч
    private int thunderStrikeDamage;    //�׻��˺�

    //�Ƿ����޵�״̬
    public bool isInvincible { get; private set; }

    [Space]
    public int currentHP; //��ǰHP

    //Ailments��ʱ�������ڳ����˺���Ч���ļ�ʱ��
    private float ignitedAilmentTimer;  //��ȼ��ʱ��
    private float chilledAilmentTimer;  //������ʱ��
    private float shockedAilmentTimer;  //�׻���ʱ��

    private float ignitedDamageCooldown = 0.3f; //��ȼ�˺���ȴʱ��
    private float ignitedDamageTimer; //��ȼ�˺���ʱ��
    private int igniteDamage; //��ȼ�˺�����DoMagicDamage�����ã�

    //����״̬�����ܸ����˺�
    public bool isVulnerable { get; private set; }

    //��ɫ�Ƿ�����
    public bool isDead { get; private set; }

        /*                      �������Եļ��㹫ʽ
        ������ = ���� + ���� + [�����ߵ���Ч��]
        ���˺� = (�˺� + ����) * [�ܱ�������] - Ŀ�껤�� * [Ŀ�����Ч��]
        �ܱ������� = �������� + ����
        �ܱ����˺� = �����˺� + ����
        ��ħ���˺� = (�����˺� + ��˪�˺� + �׵��˺� + ����) - (Ŀ��ħ������ + 3 * Ŀ������)          */

    //��ɫѪ���仯ʱ�������¼�
    public System.Action onHealthChanged;

    //��Ч
    protected EntityFX fx;

    protected virtual void Awake()
    {
        fx = GetComponent<EntityFX>();
    }

    protected virtual void Start()
    {
        currentHP = getMaxHP();  //��ǰHPΪ���HP
        critPower.SetDefaultValue(150);  //Ĭ�ϱ����˺�Ϊ150%

        //HPBarCanBeInitialized = true; //��ʼ��ʱ���Ϊ���Ը���Ѫ��
    }

    protected virtual void Update()
    {
        //����״̬�쳣�Ķ�ʱ��
        ignitedAilmentTimer -= Time.deltaTime;
        chilledAilmentTimer -= Time.deltaTime;
        shockedAilmentTimer -= Time.deltaTime;

        ignitedDamageTimer -= Time.deltaTime;

        //�����ȼ��ʱ�����ڣ��Ƴ���ȼЧ��
        if (ignitedAilmentTimer < 0)
        {
            isIgnited = false;
        }

        //���������ʱ�����ڣ��Ƴ�����Ч��
        if (chilledAilmentTimer < 0)
        {
            isChilled = false;
        }

        //����𾪶�ʱ�����ڣ��Ƴ���Ч��
        if (shockedAilmentTimer < 0)
        {
            isShocked = false;
        }

        //�����ɫ����ȼ�������ȼ�˺�
        if (isIgnited)
        {
            DealIgniteDamage(); // ִ�е�ȼ�˺�
        }
    }


    public virtual void DoDamge(CharacterStats _targetStats)
    {
        //���Ŀ�����޵�״̬���޷��ܵ��˺�
        if (_targetStats.isInvincible)
        {
            return;
        }

        //���Ŀ���Ƿ��ܹ�������ι���
        if (TargetCanEvadeThisAttack(_targetStats))
        {
            return;
        }

        //��������˺���������ɫ�������˺��������ӳ�
        int _totalDamage = damage.GetValue() + strength.GetValue();

        //�ж��Ƿ񴥷�����
        bool crit = CanCrit();

        if (crit)
        {
            _totalDamage = CalculatCritDamage(_totalDamage); //���㱩���˺�
        }

        //����������Ч������Ŀ����Ƿ񱩻���״̬
        fx.CreateHitFX(_targetStats.transform, crit);

        //����Ŀ�껤�׶��˺��ļ���
        _totalDamage = CheckTargetArmor(_targetStats, _totalDamage);

        //���Ŀ���Ƿ��ڴ���״̬�������˺�
        _totalDamage = CheckTargetVulnerability(_targetStats, _totalDamage);

        //��Ŀ������˺��������ݱ�Ҫ�Ĳ������˺�ֵ�������ߡ�Ŀ�ꡢ�Ƿ񱩻���
        _targetStats.TakeDamage(_totalDamage, transform, _targetStats.transform, crit);

        //ħ���˺���غ���
        //DoMagicDamage(_targetStats, transform);
    }

    public virtual void TakeDamage(int _damage, Transform _attacker, Transform _attackee, bool _isCrit)
    {
        //�����ɫ���޵�״̬�������˺�
        if (isInvincible)
        {
            return;
        }

        //�۳��˺����ж��Ƿ�Ϊ����
        DecreaseHPBy(_damage, _isCrit);

        // ΪĿ���ɫ�����˺�����Ч���������⡢���˵ȣ�
        _attackee.GetComponent<Entity>()?.DamageFlashEffect();
        _attackee.GetComponent<Entity>()?.DamageKnockbackEffect(_attacker, _attackee);

        // �����ǰHPС�ڵ���0�����ҽ�ɫ��û����������ִ�������߼�
        if (currentHP <= 0 && !isDead)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        //�����ɫ�Ѿ�����������ִ��
        if (isDead)
        {
            return;
        }

        isDead = true;
        Debug.Log($"{gameObject.name} is Dead");
    }

    public virtual void DieFromFalling()
    {
        //��ɫ�Ӹߴ�ˤ������ʱ����
        if (isDead)
        {
            return;
        }

        //��ɫѪ������Ϊ0������Ѫ���仯�¼�
        currentHP = 0;

        if (onHealthChanged != null)
        {
            onHealthChanged(); //֪ͨѪ���仯
        }

        // ִ�������߼�
        Die();
    }

    public void BecomeInvincible(bool _invincible)
    {
        //���ý�ɫ�Ƿ�Ϊ�޵�״̬
        isInvincible = _invincible;
    }

    #region Magic and Ailments
    public virtual void DoMagicDamage(CharacterStats _targetStats, Transform _attacker)
    {
        //��ȡ���桢��˪���׵�ħ���˺�
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        //������ħ���˺�
        int _totalMagicDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();

        //����Ŀ���ħ������
        _totalMagicDamage = CheckTargetMagicResistance(_targetStats, _totalMagicDamage);       

        //��Ŀ�����ħ���˺�
        _targetStats.TakeDamage(_totalMagicDamage, _attacker, _targetStats.transform, false);

        //���������һ��ħ���˺�����0�������Ӧ��״̬�쳣
        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0)
        {
            return;
        }

        //����ʩ��״̬�쳣����ȼ�������������
        AttemptToApplyAilments(_targetStats, _fireDamage, _iceDamage, _lightningDamage);
    }

    private void AttemptToApplyAilments(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightningDamage)
    {
        //����ħ���˺�ѡ��Ӧ�õ�״̬�쳣
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        //�������ħ���˺���ȣ����ѡ��һ��ʩ��״̬�쳣
        if (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < 0.3f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                if (canApplyIgnite)
                {
                    _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * 0.2f)); //���õ�ȼ�˺�
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
                    _targetStats.SetupThunderStrikeDamage(Mathf.RoundToInt(_lightningDamage * 0.2f)); //�����׵��˺�
                }
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }

        //ΪĿ������״̬�쳣
        if (canApplyIgnite)
        {
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * 0.2f)); //��ȼ�˺�
        }

        if (canApplyShock)
        {
            _targetStats.SetupThunderStrikeDamage(Mathf.RoundToInt(_lightningDamage * 0.2f)); //�׵��˺�
        }

        //Ӧ��״̬�쳣
        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        //���Ŀ���Ƿ��Ѿ�����ĳ��״̬������ǣ����ظ�ʩ��״̬�쳣
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        //�������ʩ�ӵ�ȼЧ�������õ�ȼ״̬��������ʱ��
        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedAilmentTimer = igniteDuration.GetValue();
            StartCoroutine(fx.EnableIgniteFXForTime_Coroutine(ignitedAilmentTimer)); //������ȼ��Ч
        }

        //�������ʩ�ӱ���Ч�������ñ���״̬��������ʱ��
        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            chilledAilmentTimer = chillDuration.GetValue();
            StartCoroutine(fx.EnableChillFXForTime_Coroutine(chilledAilmentTimer)); //����������Ч
            float _slowPercentage = 0.2f;
            GetComponent<Entity>()?.SlowSpeedBy(_slowPercentage, chillDuration.GetValue()); //����Ч��
        }

        //�������ʩ����Ч����������״̬��������ʱ��
        if (_shock && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShockAilment(_shock); //ʩ����Ч��
            }
            else //���Ŀ���Ѿ��𾪣��������׵�������������ĵ���
            {
                if (GetComponent<Player>() != null)
                {
                    return; //��Ҳ��������׵��������Լ�
                }
                GenerateThunderStrikeAndHitClosestEnemy(7.5f); //�����׵�������������ĵ���
            }
        }

        //�����ǰ״̬
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

        //����״̬��������ʱ��
        isShocked = _shock;
        shockedAilmentTimer = shockDuration.GetValue();
        StartCoroutine(fx.EnableShockFXForTime_Coroutine(shockedAilmentTimer)); //������Ч
    }

    private void GenerateThunderStrikeAndHitClosestEnemy(float _targetScanRadius)
    {
        //��������ĵ��˲������׵�������
        Transform closestEnemy = null;

        //��ȡ������ɨ�跶Χ�ڵĵ���
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _targetScanRadius);

        float closestDistanceToEnemy = Mathf.Infinity;

        //���Ҿ�������ĵ���
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

        //���û���ҵ����ˣ���Ŀ���ǵ�ǰ��ɫ�Լ�
        if (closestEnemy == null)
        {
            closestEnemy = transform;
        }

        //�����׵�������������ĵ���
        if (closestEnemy != null)
        {
            GameObject newThunderStrike = Instantiate(thunderStrikePrefab, transform.position, Quaternion.identity);
            newThunderStrike.GetComponent<Skill_ThunderStrikeController>()?.Setup(thunderStrikeDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    //ÿ��һ�ε�ȼ�˺�
    private void DealIgniteDamage()
    {
        //�����ȼ�˺���ʱ��С��0����ʾ����ʩ���˺���ʩ��dot����
        if (ignitedDamageTimer < 0)
        {
            Debug.Log($"Take burn damage {igniteDamage}");
            DecreaseHPBy(igniteDamage, false);

            //���Ѫ��С�ڵ���0���ҽ�ɫ��δ��������������
            if (currentHP <= 0 && !isDead)
            {
                Die();
            }
            //���õ�ȼ�˺���ʱ��
            ignitedDamageTimer = ignitedDamageCooldown;
        }
    }

    //��ȼ�˺�
    public void SetupIgniteDamage(int _igniteDamage)
    {
        igniteDamage = _igniteDamage;
    }

    //����
    public void SetupThunderStrikeDamage(int _thunderStrikeDamage)
    {
        thunderStrikeDamage = _thunderStrikeDamage;
    }
    #endregion



    #region ����ֵ���˺����� - ���ס�������ħ�����ԡ������ԡ�����
    //���Ŀ��Ļ��ף������������˺�
    protected int CheckTargetArmor(CharacterStats _targetStats, int _totalDamage)
    {
        //Ŀ�걻�����������令��ֵ
        if (_targetStats.isChilled)
        {
            _totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * 0.8f);
        }
        else
        {
            _totalDamage -= _targetStats.armor.GetValue();
        }


        //ȷ���˺���Ϊ��ֵ
        _totalDamage = Mathf.Clamp(_totalDamage, 0, int.MaxValue);
        return _totalDamage;
    }

    //���Ŀ���ħ�����ԣ���������ħ���˺�
    private int CheckTargetMagicResistance(CharacterStats _targetStats, int _totalMagicDamage)
    {

        _totalMagicDamage -= _targetStats.magicResistance.GetValue() + (3 * _targetStats.intelligence.GetValue());

        //ȷ��ħ���˺���Ϊ��ֵ
        _totalMagicDamage = Mathf.Clamp(_totalMagicDamage, 0, int.MaxValue);
        return _totalMagicDamage;
    }

    //���Ŀ���Ƿ���������ι���
    protected bool TargetCanEvadeThisAttack(CharacterStats _targetStats)
    {
        int _totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        //���Ŀ�걻�𾪣���������ֵ
        if (isShocked)
        {
            _totalEvasion += 20;
        }
        //�ж�Ŀ���Ƿ������˹���
        if (Random.Range(0, 100) < _totalEvasion)
        {
            //��������Ч��
            _targetStats.OnEvasion();
            return true;
        }

        return false;
    }

    //�ж��Ƿ񴥷�����
    protected bool CanCrit()
    {
        int _totalCritChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) <= _totalCritChance)
        {
            return true;
        }

        return false;
    }

    //���㱩���˺�
    protected int CalculatCritDamage(int _damage)
    {
        //���㱩���˺�����
        float _totalCritPower = (critPower.GetValue() + strength.GetValue()) * 0.01f;

        //���ر����˺�
        float critDamage = _damage * _totalCritPower;
        return Mathf.RoundToInt(critDamage);
    }

    //���Ŀ���Ƿ����������ǣ������˺�
    public int CheckTargetVulnerability(CharacterStats _targetStats, int _totalDamage)
    {
        if (_targetStats.isVulnerable)
        {
            _totalDamage = Mathf.RoundToInt(_totalDamage * 1.1f);
        }

        return _totalDamage;
    }

    //Ŀ������ʱ�Ļص�����
    public virtual void OnEvasion()
    {
    }


    #region HP
    //�۳�ָ�����˺��������˺�ֵ
    public virtual int DecreaseHPBy(int _takenDamage, bool _isCrit)
    {
        currentHP -= _takenDamage;

        //������˺���������ʾ�˺��ı�
        if (_takenDamage > 0)
        {
            GameObject popUpText = fx.CreatePopUpText(_takenDamage.ToString());

            //����Ǳ�������ʾ����ı����ı�Ч��
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

    //����HP�����HP���ܳ�������
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
    //ʹ��ɫ��ָ��ʱ����ʩ�Ӵ���
    public void BecomeVulnerableForTime(float _seconds)
    {
        StartCoroutine(BecomeVulnerableForTime_Coroutine(_seconds));
    }

    //��ָ��ʱ����ʹ��ɫ����
    private IEnumerator BecomeVulnerableForTime_Coroutine(float _duration)
    {
        isVulnerable = true;
        //Debug.Log("Vulnerable!");
        yield return new WaitForSeconds(_duration);
        isVulnerable = false;
        //Debug.Log("Exit Vulnerable!");
    }

    //��ָ��ʱ��������ָ������
    public virtual void IncreaseStatByTime(Stat _statToModify, int _modifier, float _duration)
    {
        StartCoroutine(StatModify_Coroutine(_statToModify, _modifier, _duration));
    }

    //��ָ��ʱ�����޸�ָ�����Ե�ֵ
    private IEnumerator StatModify_Coroutine(Stat _statToModify, int _modifier, float _duration)
    {
        _statToModify.AddModifier(_modifier);
        Inventory.instance.UpdateStatUI();

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
        Inventory.instance.UpdateStatUI();
    }

    //��ȡָ�����͵�����
    public Stat GetStatByType(StatType _statType)
    {
        Stat stat = null;

        //�����������ͷ�����Ӧ������
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

    //��ȡ���HPֵ
    public int getMaxHP()
    {
        return maxHP.GetValue() + vitality.GetValue() * 5;
    }

    //��ȡ���յ��˺�ֵ
    public int GetDamage()
    {
        return damage.GetValue() + strength.GetValue();
    }

    //��ȡ��������ֵ
    public int GetCritPower()
    {
        return critPower.GetValue() + strength.GetValue();
    }

    //��ȡ��������
    public int GetCritChance()
    {
        return critChance.GetValue() + agility.GetValue();
    }

    //��ȡ����ֵ
    public int GetEvasion()
    {
        return evasion.GetValue() + agility.GetValue();
    }

    //��ȡħ������
    public int GetMagicResistance()
    {
        return magicResistance.GetValue() + intelligence.GetValue() * 3;
    }

    //��ȡ�����˺�
    public int GetFireDamage()
    {
        return fireDamage.GetValue() + intelligence.GetValue();
    }

    //��ȡ��˪�˺�
    public int GetIceDamage()
    {
        return iceDamage.GetValue() + intelligence.GetValue();
    }

    //��ȡ�׵��˺�
    public int GetLightningDamage()
    {
        return lightningDamage.GetValue() + intelligence.GetValue();
    }

    #endregion
}
