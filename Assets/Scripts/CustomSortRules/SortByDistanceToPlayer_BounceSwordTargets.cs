using System.Collections;
using System.Collections.Generic;
using UnityEngine;

                            //实现IComparer<Transform>接口，用于比较Transform对象与玩家之间的距离
public class SortByDistanceToPlayer_BounceSwordTargets : IComparer<Transform>
{
    public int Compare(Transform x, Transform y)
    {
        if (x == null || y == null) //寄！
            throw new System.NotImplementedException();

        //获取玩家的Transform组件后，计算下x,y结果
        Transform player = PlayerManager.instance.player.transform;
        float distanceToSword_x = Vector2.Distance(x.position, player.position);
        float distanceToSword_y = Vector2.Distance(y.position, player.position);


        return distanceToSword_x.CompareTo(distanceToSword_y);  //返回   小到大  排序顺序
        
        //if (distanceToSword_x < distanceToSword_y)
        //{
        //    return -1;
        //}
        //else if(distanceToSword_x == distanceToSword_y)
        //{
        //    return 0;
        //}
        //else
        //{
        //    return 1;
        //}
    }

}
