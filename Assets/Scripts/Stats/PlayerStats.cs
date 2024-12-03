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
        playerDefaultMoveSpeed = player.moveSpeed;  //默认移速
    }

    //玩家攻击目标时的处理
    public override void DoDamge(CharacterStats _targetStats)
    {
        base.DoDamge(_targetStats);

        //如果目标是敌人，进行额外的处理
        if (_targetStats.GetComponent<Enemy>() != null)
        {
            Player player = PlayerManager.instance.player;
            int playerComboCounter = player.primaryAttackState.comboCounter;

            //根据玩家的攻击状态和连击计数，播放不同强度的屏幕震动
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

    //玩家受到伤害时的处理
    public override void TakeDamage(int _damage, Transform _attacker, Transform _attackee, bool _isCrit)
    {

       // player.stats.BecomeInvincible(true);
        //无敌判断
        if (isInvincible)
        {
            return;
        }

        //扣除伤害并返回伤害值
        int takenDamage = DecreaseHPBy(_damage, _isCrit);

        //给攻击者添加伤害闪光效果
        _attackee.GetComponent<Entity>()?.DamageFlashEffect();

        //给玩家加上减速效果
        SlowerPlayerMoveSpeedForTime(0.2f);

        //如果玩家受到的伤害超过最大生命的30%，则会有击退效果
        if (takenDamage >= player.stats.getMaxHP() * 0.3f)
        {
            _attackee.GetComponent<Entity>()?.DamageKnockbackEffect(_attacker, _attackee);
            player.fx.ScreenShake(player.fx.shakeDirection_heavy);
        }

        //如果玩家血量为0且未死亡，则触发死亡逻辑
        if (currentHP <= 0 && !isDead)
        {
            Die();
        }
    }

    // 让玩家在指定时间内减速
    private void SlowerPlayerMoveSpeedForTime(float _duration)
    {
        float defaultMoveSpeed = player.moveSpeed;

        //暂时将玩家的移动速度降低
        player.moveSpeed = player.moveSpeed * _duration;

        //在指定时间后恢复玩家的移动速度
        Invoke("ReturnToDefaultMoveSpeed", _duration);
    }

    //恢复玩家的默认移动速度
    private void ReturnToDefaultMoveSpeed()
    {
        player.moveSpeed = playerDefaultMoveSpeed;
    }

    //玩家死亡时的处理
    protected override void Die()
    {
        base.Die();

        //触发玩家死亡逻辑
        player.Die();

        //记录玩家掉落的货币数量
        GameManager.instance.droppedCurrencyAmount = PlayerManager.instance.GetCurrentCurrency();
        PlayerManager.instance.currency = 0;

        //玩家死亡后生成掉落物品
        GetComponent<PlayerItemDrop>()?.GenrateDrop();
    }

    //玩家受到伤害时的特殊处理（如音效、装备效果等）
    public override int DecreaseHPBy(int _takenDamage, bool _isCrit)
    {
        base.DecreaseHPBy(_takenDamage, _isCrit);

        //播放伤害音效
        int randomIndex = Random.Range(34, 36);
        AudioManager.instance.PlaySFX(randomIndex, player.transform);

        // 检查玩家装备的护甲，并使用其效果
        ItemData_Equipment currentArmor = Inventory.instance.GetEquippedEquipmentByType(EquipmentType.Armor);

        if (currentArmor != null)
        {
            //执行装备的效果（考虑冷却时间）
            Inventory.instance.UseArmorEffect_ConsiderCooldown(player.transform);
        }

        return _takenDamage;
    }

    //玩家闪避攻击时的回调函数
    public override void OnEvasion()
    {
        Debug.Log("Player evaded attack!");
        //创建闪避时的幻象效果
        player.skill.dodge.CreateMirageOnDodge();
    }

    //让玩家的克隆体进行伤害
    public void CloneDoDamage(CharacterStats _targetStats, float _cloneAttackDamageMultipler, Transform _cloneTransform)
    {
        //检查目标是否闪避攻击
        if (TargetCanEvadeThisAttack(_targetStats))
        {
            return;
        }

        int _totalDamage = damage.GetValue() + strength.GetValue();

        //判断是否暴击
        bool crit = CanCrit();

        if (crit)
        {
            Debug.Log("Critical Attack!");
            _totalDamage = CalculatCritDamage(_totalDamage);
        }

        //创建击中效果
        fx.CreateHitFX(_targetStats.transform, crit);

        //克隆体的伤害应该低于玩家的伤害
        if (_cloneAttackDamageMultipler > 0)
        {
            _totalDamage = Mathf.RoundToInt(_totalDamage * _cloneAttackDamageMultipler);
        }

        //检查目标的护甲和脆弱状态，并调整伤害
        _totalDamage = CheckTargetArmor(_targetStats, _totalDamage);
        _totalDamage = CheckTargetVulnerability(_targetStats, _totalDamage);

        //造成伤害
        _targetStats.TakeDamage(_totalDamage, _cloneTransform, _targetStats.transform, crit);
    }
}
