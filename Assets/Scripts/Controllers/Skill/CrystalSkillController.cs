using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalSkillController : MonoBehaviour
{   
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private float crystalExistenceTimer; //水晶存在时间计时器

    private bool canExplode; //水晶是否可以爆炸
    private bool canMove; //水晶是否可以移动
    private float moveSpeed; //水晶移速

    private bool canGrow; //水晶是否可以生长
    private float growSpeed = 5; //水晶的生长速度

    private Transform targetEnemy; //当前目标敌人

    private CrystalSkill crystalSkill; //水晶技能实例

    private void Start()
    {
        crystalSkill = SkillManager.instance.crystal; //获取当前技能管理器中,水晶技能实例
    }

    private void Update()
    {
        crystalExistenceTimer -= Time.deltaTime; //减少水晶存在时间

        //如果没有目标敌人，则水晶无法移动
        if (targetEnemy == null)
        {
            canMove = false; //关闭水晶移动
        }

        //如果水晶的存在时间已结束，尝试触发爆炸
        if (crystalExistenceTimer < 0)
        {
            EndCrystal_ExplodeIfAvailable(); //执行爆炸行为（如果可能）
        }

        // 如果水晶可以移动并且有目标敌人
        if (canMove)
        {
            // 水晶朝敌人方向移动
            transform.position = Vector2.MoveTowards(transform.position, targetEnemy.transform.position, moveSpeed * Time.deltaTime);

            // 如果水晶接近敌人，则停止移动并触发爆炸
            if (Vector2.Distance(transform.position, targetEnemy.transform.position) < 1)
            {
                canMove = false; // 停止水晶的移动
                EndCrystal_ExplodeIfAvailable(); // 执行爆炸行为（如果可能）
            }
        }

        // 如果水晶可以生长，则逐渐增大水晶的大小
        if (canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(2, 2), growSpeed * Time.deltaTime); // 使用插值平滑增长
        }
    }


    public void SetupCrystal(float _crystalExistenceDuration, bool _canExplode, bool _canMove, float _moveSpeed, Transform _targetEnemy)
    {
        crystalExistenceTimer = _crystalExistenceDuration;  //水晶存在时间
        canExplode = _canExplode;   //水晶能否爆炸
        canMove = _canMove; //水晶移动
        moveSpeed = _moveSpeed; //水晶移速
        targetEnemy = _targetEnemy; //水晶飞行目标
    }

    //水晶效果
    public void EndCrystal_ExplodeIfAvailable()
    {
        if (canExplode)
        {
            //能爆就炸
            canGrow = true;
            anim.SetTrigger("Explosion");

            //没有解锁 水晶枪  ，则进入CD
            if (!crystalSkill.crystalGunUnlocked)   
            {
                crystalSkill.EnterCooldown();
            }
        }
        else
        {
            crystalSelfDestroy();
        }
    }

    //水晶自爆
    public void crystalSelfDestroy()
    {
        Destroy(gameObject);

        if (!crystalSkill.crystalGunUnlocked)
        {
            crystalSkill.EnterCooldown();
        }
    }

    private void Explosion()
    {
        //使用物理系统获取所有与水晶重叠的2D碰撞器，这里的范围是碰撞器半径
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        //遍历所有碰撞器
        foreach (var hit in colliders)
        {
            //如果碰撞器附加了敌人组件
            if (hit.GetComponent<Enemy>() != null)
            {
                Enemy enemy = hit.GetComponent<Enemy>();

                PlayerManager.instance.player.stats.DoMagicDamage(enemy.GetComponent<CharacterStats>(), transform);

                //使用当前装备的护符效果
               //Inventory.instance.UseCharmEffect_ConsiderCooldown(enemy.transform);
            }
        }
    }


    public void SpecifyEnemyTarget(Transform _enemy)
    {
        targetEnemy = _enemy;
    }

    //public void CrystalChooseRandomEnemy(float _searchRadius)
    //{
    //    Transform originalTargetEnemy = targetEnemy;

    //    targetEnemy = SkillManager.instance.crystal.ChooseRandomEnemy(transform, _searchRadius);

    //    if (targetEnemy == null)
    //    {
    //        Debug.Log("No enemy is chosen" +
    //            "\n will choose original closest enemy");
    //        targetEnemy = originalTargetEnemy;
    //    }
    //}

}


