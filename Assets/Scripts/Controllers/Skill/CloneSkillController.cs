using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CloneSkillController : MonoBehaviour
{
    private SpriteRenderer sr;  //角色SpriteRenderer，用于控制克隆的视觉表现
    private Animator anim;  //角色Animator，用于控制克隆的动画

    private float cloneDuration;  //克隆体持续时间
    private float cloneTimer;  //克隆体计时器
    private float colorLosingSpeed;  //克隆体消退速度

    [SerializeField] private Transform attackCheck;  //检测攻击中心位置
    [SerializeField] private float attackCheckRadius;  //攻击检测范围半径
    private Transform closestEnemy;  //最近敌人

    private bool canDuplicateClone;  //是否生成克隆副本
    private float duplicatePossibility;  //克隆副本生成的可能性

    private bool cloneFacingRight = true;  //克隆体是否面向右侧
    private float cloneFacingDirection = 1;  //面向方向，1右，-1左

    private float cloneAttackDamageMultiplier;  //克隆体攻击伤害倍数

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>(); 
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;  //每帧减少克隆体计时器，即生命值

        if (cloneTimer < 0)
        {
            //克隆结束后开始消退颜色
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLosingSpeed));

            if (sr.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    // 设置克隆的相关参数
    public void SetupClone(float _cloneDuration, float _colorLosingSpeed, bool _canAttack, Transform _closestEnemy, bool _canDuplicateClone, float _duplicatePossibility, float _cloneAttackDamageMultiplier)
    {
        if (_canAttack)
        {
            //如果克隆体攻击，则随机设置攻击动画
            anim.SetInteger("AttackNumber", Random.Range(1, 4));
        }

        // 设置克隆的持续时间、消失速度、敌人、克隆副本生成的可能性等参数
        cloneDuration = _cloneDuration;
        colorLosingSpeed = _colorLosingSpeed;
        cloneTimer = cloneDuration;  // 设置克隆计时器为克隆持续时间

        closestEnemy = _closestEnemy;  // 设置最近的敌人

        // 使克隆面向最近的敌人
        FaceClosestTarget();

        canDuplicateClone = _canDuplicateClone;  // 是否可以生成副本
        duplicatePossibility = _duplicatePossibility;  // 副本生成概率
        cloneAttackDamageMultiplier = _cloneAttackDamageMultiplier;  // 克隆攻击伤害倍数
    }

    // 触发动画结束，停止克隆
    private void AnimationTrigger()
    {
        cloneTimer = -0.1f;  // 立即结束克隆计时
    }

    // 触发攻击事件
    private void AttackTrigger()
    {
        // 如果学会了激进的幻象技能，克隆攻击时也会应用击中效果
        if (SkillManager.instance.clone.aggressiveCloneCanApplyOnHitEffect)
        {
            Inventory.instance.ReleaseSwordArcane_ConsiderCooldown();  // 释放剑的奥术技能（如果有的话）
        }

        // 检测攻击范围内的所有碰撞体（敌人）
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
        {
            // 如果碰撞体是敌人
            if (hit.GetComponent<Enemy>() != null)
            {
                Enemy enemy = hit.GetComponent<Enemy>();  // 获取敌人

                // 获取玩家的属性（用于计算伤害）
                PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

                // 克隆的伤害应该比玩家本身的伤害小
                if (playerStats != null)
                {
                    playerStats.CloneDoDamage(enemy.GetComponent<CharacterStats>(), cloneAttackDamageMultiplier, transform);  // 克隆攻击伤害
                }

                // 如果学会了激进幻象，克隆攻击时也会应用击中效果
                if (SkillManager.instance.clone.aggressiveCloneCanApplyOnHitEffect)
                {
                    Inventory.instance.UseSwordEffect_ConsiderCooldown(enemy.transform);  // 触发剑的技能效果
                }

                // 如果可以生成副本
                if (canDuplicateClone)
                {
                    // 随机决定是否在敌人附近生成一个副本
                    if (Random.Range(0, 100) < duplicatePossibility && SkillManager.instance.clone.currentDuplicateCloneAmount < SkillManager.instance.clone.maxDuplicateCloneAmount)
                    {
                        // 在敌人旁边随机位置创建副本
                        SkillManager.instance.clone.CreateDuplicateClone(new Vector3(hit.transform.position.x + 1f * cloneFacingDirection, hit.transform.position.y));
                    }
                }
            }
        }
    }

    // 使克隆面向最近的敌人
    private void FaceClosestTarget()
    {
        if (closestEnemy != null)
        {
            // 如果克隆在敌人右边，翻转克隆面朝方向
            if (transform.position.x > closestEnemy.position.x)
            {
                CloneFlip();
            }
        }
    }

    // 翻转克隆的面朝方向
    private void CloneFlip()
    {
        transform.Rotate(0, 180, 0);  // 翻转物体180度

        cloneFacingRight = !cloneFacingRight;  // 切换面朝方向
        cloneFacingDirection = -cloneFacingDirection;  // 改变克隆面朝方向的数值（-1为左，1为右）
    }
}
