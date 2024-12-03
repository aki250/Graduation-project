using System.Collections;
using System.Collections.Generic;
using UnityEngine;
                                                                    //冰火攻击特效
[CreateAssetMenu(fileName = "Ice and Fire Effect", menuName = "Data/Item Effect/Ice and Fire Effect")]
public class IceAndFire_Effect : ItemEffect
{
    [SerializeField] private GameObject iceAndFirePrefab;
    [SerializeField] private float xFlyVelocity;
    
    //第三普攻触发冰火波
    public override void ReleaseSwordArcane()
    {
        Player player = PlayerManager.instance.player;

        //判断是否第三连段
        bool thirdAttack = player.primaryAttackState.comboCounter == 2;

        if (thirdAttack)
        {
            //实例化特效在玩家位置，往前飞，3s后销毁
            GameObject newIceAndFire = Instantiate(iceAndFirePrefab, player.transform.position, player.transform.rotation);
            newIceAndFire.GetComponent<Rigidbody2D>().velocity = new Vector2(xFlyVelocity * player.facingDirection, 0);

            Destroy(newIceAndFire, 3);
        }
    }
    

    //public override void ExecuteEffect_HitNeeded(Transform _spawnTransform)
    //{
    //    Player player = PlayerManager.instance.player;

    //    bool thirdAttack = player.primaryAttackState.comboCounter == 2;

    //    if (thirdAttack)
    //    {
    //        GameObject newIceAndFire = Instantiate(iceAndFirePrefab, _spawnTransform.position, player.transform.rotation);

    //        newIceAndFire.GetComponent<Rigidbody2D>().velocity = new Vector2(xFlyVelocity * player.facingDirection, 0);

    //        Destroy(newIceAndFire, 3);
    //    }

    //}

    
    //public override void ExecuteEffect_NoHitNeeded()
    //{
    //    Player player = PlayerManager.instance.player;

    //    bool thirdAttack = player.primaryAttackState.comboCounter == 2;

    //    if (thirdAttack)
    //    {
    //        GameObject newIceAndFire = Instantiate(iceAndFirePrefab, player.transform.position, player.transform.rotation);

    //        newIceAndFire.GetComponent<Rigidbody2D>().velocity = new Vector2(xFlyVelocity * player.facingDirection, 0);

    //        Destroy(newIceAndFire, 3);
    //    }

    //}


}
