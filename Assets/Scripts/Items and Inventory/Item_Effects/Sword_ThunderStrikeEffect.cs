using System.Collections;
using System.Collections.Generic;
using UnityEngine;
                                                                     //�����׵���Ч

[CreateAssetMenu(fileName = "Sword_Thunder Strike Effect", menuName = "Data/Item Effect/Thunder Strike")]
public class Sword_ThunderStrikeEffect : ItemEffect
{
    //�׵���ЧԤ����
    [SerializeField] private GameObject thunderStrikePrefab;

    public override void ExecuteEffect(Transform _enemyTransform)
    {
        //�ڵ���λ��ʵ�����׵繥����Ч
        GameObject newThunderStrike = Instantiate(thunderStrikePrefab, _enemyTransform.position, Quaternion.identity);

        Destroy(newThunderStrike, 1f);
    }
}
