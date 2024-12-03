using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;
    private PlayerFX playerFX;

    private float playerDefaultMoveSpeed;

    protected override void Awake()
    {
        base.Awake();
        playerFX = GetComponent<PlayerFX>();
    }

    protected override void Start()
    {
        base.Start();
        player = GetComponent<Player>();
        playerDefaultMoveSpeed = player.moveSpeed;  //Ĭ������
    }

    //��ҹ���Ŀ��ʱ�Ĵ���
    public override void DoDamge(CharacterStats _targetStats)
    {
        base.DoDamge(_targetStats);

        //���Ŀ���ǵ��ˣ����ж���Ĵ���
        if (_targetStats.GetComponent<Enemy>() != null)
        {
            Player player = PlayerManager.instance.player;
            int playerComboCounter = player.primaryAttackState.comboCounter;

            //������ҵĹ���״̬���������������Ų�ͬǿ�ȵ���Ļ��
            if (player.stateMachine.currentState == player.primaryAttackState)
            {
                if (playerComboCounter <= 1)
                {
                    playerFX.ScreenShake(playerFX.shakeDirection_light);
                }
                else
                {
                    playerFX.ScreenShake(playerFX.shakeDirection_medium);
                }
            }
        }
    }

    //����ܵ��˺�ʱ�Ĵ���
    public override void TakeDamage(int _damage, Transform _attacker, Transform _attackee, bool _isCrit)
    {

       // player.stats.BecomeInvincible(true);
        //�޵��ж�
        if (isInvincible)
        {
            return;
        }

        //�۳��˺��������˺�ֵ
        int takenDamage = DecreaseHPBy(_damage, _isCrit);

        //������������˺�����Ч��
        _attackee.GetComponent<Entity>()?.DamageFlashEffect();

        //����Ҽ��ϼ���Ч��
        SlowerPlayerMoveSpeedForTime(0.2f);

        //�������ܵ����˺��������������30%������л���Ч��
        if (takenDamage >= player.stats.getMaxHP() * 0.3f)
        {
            _attackee.GetComponent<Entity>()?.DamageKnockbackEffect(_attacker, _attackee);
            player.fx.ScreenShake(player.fx.shakeDirection_heavy);
        }

        //������Ѫ��Ϊ0��δ�������򴥷������߼�
        if (currentHP <= 0 && !isDead)
        {
            Die();
        }
    }

    // �������ָ��ʱ���ڼ���
    private void SlowerPlayerMoveSpeedForTime(float _duration)
    {
        float defaultMoveSpeed = player.moveSpeed;

        //��ʱ����ҵ��ƶ��ٶȽ���
        player.moveSpeed = player.moveSpeed * _duration;

        //��ָ��ʱ���ָ���ҵ��ƶ��ٶ�
        Invoke("ReturnToDefaultMoveSpeed", _duration);
    }

    //�ָ���ҵ�Ĭ���ƶ��ٶ�
    private void ReturnToDefaultMoveSpeed()
    {
        player.moveSpeed = playerDefaultMoveSpeed;
    }

    //�������ʱ�Ĵ���
    protected override void Die()
    {
        base.Die();

        //������������߼�
        player.Die();

        //��¼��ҵ���Ļ�������
        GameManager.instance.droppedCurrencyAmount = PlayerManager.instance.GetCurrentCurrency();
        PlayerManager.instance.currency = 0;

        //������������ɵ�����Ʒ
        GetComponent<PlayerItemDrop>()?.GenrateDrop();
    }

    //����ܵ��˺�ʱ�����⴦������Ч��װ��Ч���ȣ�
    public override int DecreaseHPBy(int _takenDamage, bool _isCrit)
    {
        base.DecreaseHPBy(_takenDamage, _isCrit);

        //�����˺���Ч
        int randomIndex = Random.Range(34, 36);
        AudioManager.instance.PlaySFX(randomIndex, player.transform);

        // ������װ���Ļ��ף���ʹ����Ч��
        ItemData_Equipment currentArmor = Inventory.instance.GetEquippedEquipmentByType(EquipmentType.Armor);

        if (currentArmor != null)
        {
            //ִ��װ����Ч����������ȴʱ�䣩
            Inventory.instance.UseArmorEffect_ConsiderCooldown(player.transform);
        }

        return _takenDamage;
    }

    //������ܹ���ʱ�Ļص�����
    public override void OnEvasion()
    {
        Debug.Log("Player evaded attack!");
        //��������ʱ�Ļ���Ч��
        player.skill.dodge.CreateMirageOnDodge();
    }

    //����ҵĿ�¡������˺�
    public void CloneDoDamage(CharacterStats _targetStats, float _cloneAttackDamageMultipler, Transform _cloneTransform)
    {
        //���Ŀ���Ƿ����ܹ���
        if (TargetCanEvadeThisAttack(_targetStats))
        {
            return;
        }

        int _totalDamage = damage.GetValue() + strength.GetValue();

        //�ж��Ƿ񱩻�
        bool crit = CanCrit();

        if (crit)
        {
            Debug.Log("Critical Attack!");
            _totalDamage = CalculatCritDamage(_totalDamage);
        }

        //��������Ч��
        fx.CreateHitFX(_targetStats.transform, crit);

        //��¡����˺�Ӧ�õ�����ҵ��˺�
        if (_cloneAttackDamageMultipler > 0)
        {
            _totalDamage = Mathf.RoundToInt(_totalDamage * _cloneAttackDamageMultipler);
        }

        //���Ŀ��Ļ��׺ʹ���״̬���������˺�
        _totalDamage = CheckTargetArmor(_targetStats, _totalDamage);
        _totalDamage = CheckTargetVulnerability(_targetStats, _totalDamage);

        //����˺�
        _targetStats.TakeDamage(_totalDamage, _cloneTransform, _targetStats.transform, crit);
    }
}
