using UnityEngine;

//������Unity�༭���д����ýű���ʵ����Ϊ�ɸ��õ���ƷЧ��
[CreateAssetMenu(fileName = "Buff Effect", menuName = "Data/Item Effect/Buff Effect")]
public class Buff_Effect : ItemEffect
{
    //����Buff��������
    [SerializeField] private StatType buffStatType;

    //buff������ֵ
    [SerializeField] private int buffValue;

    //buff����ʱ��
    [SerializeField] private int buffDuration;

    //���ڻ�����ҵ����Թ������
    private PlayerStats playerStats;

    //ע�͵��İ汾��һ������������Ŀ����˵������ֱ��Ӧ��Buff����
    /*
    public override void ExecuteEffect_NoHitNeeded()
    {
        playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        //����ҵ�ָ������buffStatType����buffValue������buffDuration
        playerStats.IncreaseStatByTime(GetBuffStat(buffStatType), buffValue, buffDuration);
    }
    */

    //���Ǹ��෽�������ָ��Ŀ��ִ�� Buff Ч��
    public override void ExecuteEffect(Transform _enemyTransform)
    {
        playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        //buffStatType��ȡ��Ҫ���ӵ����ԣ���Ӧ��Ч��
        //BuffЧ��������buffValue��������buffDurationʱ��
        playerStats.IncreaseStatByTime(playerStats.GetStatByType(buffStatType), buffValue, buffDuration);
    }
}

