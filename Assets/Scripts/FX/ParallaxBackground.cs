using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;  //�������������

    [SerializeField] private float parallaxEffect;  //���Ʊ����Ĺ����ٶ�

    private float xPosition;  // ������x����
    private float length;     // �����Ŀ�ȣ����ڼ����Ӳ�Ч����ƽ��

    private void Start()
    {
        // ���Ҳ���ֵ�������
        cam = GameObject.Find("Main Camera");

        // ��ȡ�����ĳ�ʼx����
        xPosition = transform.position.x;

        // ��ȡ����ͼ��Ŀ�ȣ�����ʹ����SpriteRenderer��ȡ�ñ����ĳߴ�
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        // �����������x�����������Ӳ�Ч����ƫ����
        float BGPositionOffset = cam.transform.position.x * (1 - parallaxEffect);

        // ���㱳����Ҫ�ƶ��ľ��룬�����������x������Ӳ�����������
        float distanceToMove = cam.transform.position.x * parallaxEffect;

        // ���±�����λ�ã����䰴�Ӳ�Ч�������ƶ�
        transform.position = new Vector3(xPosition + distanceToMove, transform.position.y);

        // ʵ�ֱ���������ѭ��Ч����ʹ����ͼ����Ҳ��������³��֣�
        if (BGPositionOffset > xPosition + length)
        {
            // ���������ұ߽糬�������λ��ʱ�����������¶�λ���Ҳ�
            xPosition += length;
        }
        else if (BGPositionOffset < xPosition - length)
        {
            // ����������߽糬�������λ��ʱ�����������¶�λ�����
            xPosition -= length;
        }
    }
}
