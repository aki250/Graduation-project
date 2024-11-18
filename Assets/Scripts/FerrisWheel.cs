using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class FerrisWheel : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;    //存储旋转速度。

    void Start()
    {

    }

    void Update()
    {
        //使游戏对象围绕Z轴旋转，rotateSpeed是旋转速度，Time.deltaTime确保旋转速度不受帧率影响。
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);

        //遍历transform的所有子对象。
        for (int i = 0; i < transform.childCount; i++)
        {
            //将每个子对象的旋转设置为Quaternion.identity，即没有旋转。
            //这意味着无论FerrisWheel如何旋转，其座位（或其他子对象）始终保持水平。
            transform.GetChild(i).transform.rotation = Quaternion.identity;
        }
    }
}