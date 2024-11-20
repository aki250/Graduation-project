using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEnemy : Enemy
{
    //敌人碰撞器
    public BoxCollider2D textCollider { get; set; }

    [Header("死亡奖励")]
    //死亡后掉落的货币数量
    public int currencyToGive;
    //死亡时显示的物体列表
    public List<GameObject> objectListToShow;
    //死亡时实例化的预制件列表
    public List<GameObject> prefabListToInstantiate;

    #region States
    //空闲状态
    public TextEnemyIdleState idleState { get; private set; }
    //死亡状态
    public TextEnemyDeathState deathState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        //敌人的碰撞器组件
        textCollider = GetComponent<BoxCollider2D>();

        //敌人的状态机状态：只需要站立和死亡了
        idleState = new TextEnemyIdleState(this, stateMachine, null, this);
        deathState = new TextEnemyDeathState(this, stateMachine, null, this);
    }

    protected override void Start()
    {
        base.Start();

        //初始化状态机并设置为空闲状态
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Die()
    {
        base.Die();

        //如果掉落货币数量不为零，增加货币
        if (currencyToGive != 0)
        {
            PlayerManager.instance.currency += currencyToGive;
        }

        //如果有要显示的物体，激活它们
        if (objectListToShow.Count > 0)
        {
            for (int i = 0; i < objectListToShow.Count; i++)
            {
                objectListToShow[i].SetActive(true);
            }
        }

        //如果有要实例化的预制件，实例化它们
        if (prefabListToInstantiate.Count > 0)
        {
        }

        // 更改状态机状态为死亡状态
        stateMachine.ChangeState(deathState);
    }
}
