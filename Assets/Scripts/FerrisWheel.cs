using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class FerrisWheel : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;    //�洢��ת�ٶȡ�

    void Start()
    {

    }

    void Update()
    {
        //ʹ��Ϸ����Χ��Z����ת��rotateSpeed����ת�ٶȣ�Time.deltaTimeȷ����ת�ٶȲ���֡��Ӱ�졣
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);

        //����transform�������Ӷ���
        for (int i = 0; i < transform.childCount; i++)
        {
            //��ÿ���Ӷ������ת����ΪQuaternion.identity����û����ת��
            //����ζ������FerrisWheel�����ת������λ���������Ӷ���ʼ�ձ���ˮƽ��
            transform.GetChild(i).transform.rotation = Quaternion.identity;
        }
    }
}