using System.Collections;
using System.Collections.Generic;
using UnityEngine;


                                            //控制技能 雷击  行为逻辑，包括移动、命中和伤害处理。

public class Skill_ThunderStrikeController : MonoBehaviour
{
    //技能目标状态对象
    [SerializeField] private CharacterStats targetStats;

    //雷击后移度
    [SerializeField] private float thunderMoveSpeed;

    //技能伤害值
    private int damage;

    private Animator anim;

    //是否已经触发命中逻辑
    private bool triggered;

    //初始化脚本的相关组件。
    private void Awake()
    {
        // 获取子对象中的动画控制器
        anim = GetComponentInChildren<Animator>();
    }


    private void Start()
    {
    }

    private void Update()
    {
        //目标不存在
        if (targetStats == null)    
        {
            return;
        }

        //触发了就退出
        if (triggered)
        {
            return;
        }

        // 使技能向目标位置移动
        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, thunderMoveSpeed * Time.deltaTime);

        // 设置技能的朝向，使其面向目标
        transform.right = transform.position - targetStats.transform.position;

        // 检测技能是否接近目标
        if (Vector2.Distance(transform.position, targetStats.transform.position) < 0.2f)
        {
            triggered = true; // 标记技能已触发命中逻辑

            // 调整动画的位置和方向
            anim.transform.localPosition = new Vector3(0, 0.5f);
            anim.transform.localRotation = Quaternion.identity;

            // 重置技能的整体旋转并扩大显示尺寸
            transform.rotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);

            // 将技能挂载到目标对象上，确保技能与目标同步移动
            transform.parent = targetStats.transform;

            // 延迟触发伤害和销毁逻辑
            Invoke("DamageAndSelfDestroy", 0.25f);

            // 触发命中动画
            anim.SetTrigger("Hit");
        }
    }

    //设置技能的初始参数。
    public void Setup(int _damage, CharacterStats _targetStats)
    {
        damage = _damage; //技能伤害值
        targetStats = _targetStats; //目标状态对象
    }

    //对目标造成伤害并销毁技能对象。
    private void DamageAndSelfDestroy()
    {
        //目标进入电击异常状态
        targetStats.ApplyShockAilment(true);

        //对目标造成伤害
        targetStats.TakeDamage(damage, transform, targetStats.transform, false);

        //延迟销毁技能对象，以确保命中动画播放完成
        //Destroy(gameObject, 0.4f);
        if (targetStats == null)
        {
            Destroy(gameObject); // 目标丢失时销毁技能
            return;
        }

    }
}
