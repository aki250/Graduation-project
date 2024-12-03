using System.Collections;
using System.Collections.Generic;
using UnityEngine;
                                                                     //剑的雷电特效

[CreateAssetMenu(fileName = "Sword_Thunder Strike Effect", menuName = "Data/Item Effect/Thunder Strike")]
public class Sword_ThunderStrikeEffect : ItemEffect
{
    //雷电特效预制体
    [SerializeField] private GameObject thunderStrikePrefab;

    public override void ExecuteEffect(Transform _enemyTransform)
    {
        //在敌人位置实例化雷电攻击特效
        GameObject newThunderStrike = Instantiate(thunderStrikePrefab, _enemyTransform.position, Quaternion.identity);

        Destroy(newThunderStrike, 1f);
    }
}
