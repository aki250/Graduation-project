using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    protected Player player;

    public float cooldown;  //技能冷却时间
    protected float cooldownTimer;  //冷却计时器
    public float skillLastUseTime { get; protected set; } = 0;  //上次使用技能的时间

    // 在技能启动时进行初始化
    protected virtual void Start()
    {
        player = PlayerManager.instance.player;  //获取玩家实例

        CheckUnlockFromSave();  //从保存中检查技能是否解锁
    }

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;  //计时器倒计时
    }

    //检查技能是否已解锁
    protected virtual void CheckUnlockFromSave()
    {

    }

    //检查技能是否可以使用
    public virtual bool SkillIsReadyToUse()
    {
        if (cooldownTimer < 0)  //如果冷却计时器小于0，表示技能已准备好
        {
            return true;
        }
        else
        {
            //根据语言设置弹出相应的提示文本
            if (LanguageManager.instance.localeID == 0)
            {
                player.fx.CreatePopUpText("Skill is in cooldown");  
            }
            else if (LanguageManager.instance.localeID == 1)
            {
                player.fx.CreatePopUpText("技能冷却中！"); 
            }
            return false;
        }
    }

    //如果技能可以使用，则执行技能
    public virtual bool UseSkillIfAvailable()
    {
        if (cooldownTimer < 0)  //如果技能处于冷却状态
        {
            UseSkill();
            cooldownTimer = cooldown;  //重置冷却计时器
            return true;
        }

        //根据语言设置提示文本
        if (LanguageManager.instance.localeID == 0)
        {
            player.fx.CreatePopUpText("Skill is in cooldown");  //英文
        }
        else if (LanguageManager.instance.localeID == 1)
        {
            player.fx.CreatePopUpText("技能冷却中！");  //中文
        }
        return false;
    }

    public virtual void UseSkill()
    {
    }

    //寻找最近的敌人
    protected virtual Transform FindClosestEnemy(Transform _searchCenter)
    {
        Transform closestEnemy = null;

        //在搜索半径内查找所有敌人
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_searchCenter.position, 12);

        float closestDistanceToEnemy = Mathf.Infinity;  //最近敌人距离

        //寻找最近的敌人
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)  //确保目标是敌人
            {
                float currentDistanceToEnemy = Vector2.Distance(_searchCenter.position, hit.transform.position);

                //如果当前敌人的距离更近，则更新
                if (currentDistanceToEnemy < closestDistanceToEnemy)
                {
                    closestDistanceToEnemy = currentDistanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }

        return closestEnemy;
    }

    //从搜索半径内随机选择一个敌人
    protected virtual Transform ChooseRandomEnemy(Transform _searchCenter, float _targetSearchRadius)
    {
        Transform targetEnemy = null;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _targetSearchRadius);

        //查找半径内的敌人
        List<Transform> enemies = new List<Transform>();

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)  //确保目标是敌人
            {
                enemies.Add(hit.transform);
            }
        }

        //如果找到敌人，则随机选择一个作为目标
        if (enemies.Count > 0)
        {
            targetEnemy = enemies[Random.Range(0, enemies.Count)];
        }

        return targetEnemy;
    }
}
