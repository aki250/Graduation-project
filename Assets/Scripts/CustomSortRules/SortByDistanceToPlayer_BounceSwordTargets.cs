using System.Collections;
using System.Collections.Generic;
using UnityEngine;

                            //ʵ��IComparer<Transform>�ӿڣ����ڱȽ�Transform���������֮��ľ���
public class SortByDistanceToPlayer_BounceSwordTargets : IComparer<Transform>
{
    public int Compare(Transform x, Transform y)
    {
        if (x == null || y == null) //�ģ�
            throw new System.NotImplementedException();

        //��ȡ��ҵ�Transform����󣬼�����x,y���
        Transform player = PlayerManager.instance.player.transform;
        float distanceToSword_x = Vector2.Distance(x.position, player.position);
        float distanceToSword_y = Vector2.Distance(y.position, player.position);


        return distanceToSword_x.CompareTo(distanceToSword_y);  //����   С����  ����˳��
        
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
