using System.Linq;
using UnityEngine;

                                          //���˵�״̬
public class EnemyStats : CharacterStats
{
    private Enemy enemy; //�������
    private ItemDrop itemDropSystem; //��Ʒ����ϵͳ���

    public Stat currencyDropAmount; //��������ʱ����Ļ�������

    [Header("���˵ȼ�")]
    [SerializeField] private int enemyLevel = 1; //���˵ȼ�

    [Range(0f, 1f)]
    [SerializeField] private float percentageModifier = 0.4f; //���˵ȼ������Եİٷֱȼӳ�

    protected override void Start()
    {
        //���ݵ��˵ȼ�������������
        ModifyAllStatsAccordingToEnemyLevel();

        base.Start();

        enemy = GetComponent<Enemy>(); //��ȡ�������
        itemDropSystem = GetComponent<ItemDrop>(); //��ȡ��Ʒ����ϵͳ���
    }

    //�����ܵ��˺�ʱ����
    public override void TakeDamage(int _damage, Transform _attacker, Transform _attackee, bool _isCrit)
    {
        base.TakeDamage(_damage, _attacker, _attackee, _isCrit);

        //���˽���ս��״̬
        enemy.GetIntoBattleState();
    }

    protected override void Die()
    {
        base.Die();

        enemy.Die(); //��������

        //��������ʱ������Ʒ
        itemDropSystem.GenrateDrop();

        //��һ�û���
        PlayerManager.instance.currency += currencyDropAmount.GetValue();

        //3������ٵ��˶���
        Destroy(gameObject, 3f);
    }

    //��������ֵΪ0
    public void ZeroHP()
    {
        currentHP = 0; //���õ�ǰ����ֵΪ0

        base.Die();

        if (onHealthChanged != null) //����н���״̬�ı��ί��
        {
            onHealthChanged(); //����ί��
        }
    }

    //���˵�����Һ���Ʒʱ����
    public void DropCurrencyAndItem()
    {
        itemDropSystem.GenrateDrop(); //������Ʒ

        //��һ�û���
        PlayerManager.instance.currency += currencyDropAmount.GetValue();
    }

    //���ݵ��˵ȼ�������������
    private void ModifyAllStatsAccordingToEnemyLevel()
    {
        //�����������ԣ�������Ҫ���ԡ��������ԡ��������ԡ�ħ�����Ժͻ��ҵ���
        foreach (Stat stat in GetType().GetFields().Where(f => typeof(Stat).IsAssignableFrom(f.FieldType)).Select(f => (Stat)f.GetValue(this)))
        {
            ModifyStatAccordingToEnemyLevel(stat);
        }
    }

    //���ݵ��˵ȼ�������������
    private void ModifyStatAccordingToEnemyLevel(Stat _stat)
    {
        //�����˵ȼ�����1ʱ�����ӵ��˵�����
        for (int i = 1; i < enemyLevel; i++)
        {
            float _modifier = _stat.GetValue() * percentageModifier; //�������Լӳ�
            _stat.AddModifier(Mathf.RoundToInt(_modifier)); //�������Լӳ�
        }
    }
}