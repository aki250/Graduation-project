using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEnemy : Enemy
{
    //������ײ��
    public BoxCollider2D textCollider { get; set; }

    [Header("��������")]
    //���������Ļ�������
    public int currencyToGive;
    //����ʱ��ʾ�������б�
    public List<GameObject> objectListToShow;
    //����ʱʵ������Ԥ�Ƽ��б�
    public List<GameObject> prefabListToInstantiate;

    #region States
    //����״̬
    public TextEnemyIdleState idleState { get; private set; }
    //����״̬
    public TextEnemyDeathState deathState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        //���˵���ײ�����
        textCollider = GetComponent<BoxCollider2D>();

        //���˵�״̬��״̬��ֻ��Ҫվ����������
        idleState = new TextEnemyIdleState(this, stateMachine, null, this);
        deathState = new TextEnemyDeathState(this, stateMachine, null, this);
    }

    protected override void Start()
    {
        base.Start();

        //��ʼ��״̬��������Ϊ����״̬
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Die()
    {
        base.Die();

        //����������������Ϊ�㣬���ӻ���
        if (currencyToGive != 0)
        {
            PlayerManager.instance.currency += currencyToGive;
        }

        //�����Ҫ��ʾ�����壬��������
        if (objectListToShow.Count > 0)
        {
            for (int i = 0; i < objectListToShow.Count; i++)
            {
                objectListToShow[i].SetActive(true);
            }
        }

        //�����Ҫʵ������Ԥ�Ƽ���ʵ��������
        if (prefabListToInstantiate.Count > 0)
        {
        }

        // ����״̬��״̬Ϊ����״̬
        stateMachine.ChangeState(deathState);
    }
}
