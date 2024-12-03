using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;

public class PopUpText : MonoBehaviour
{
    private TextMeshPro myText;

    [SerializeField] private float appearingSpeed;  //�����ٶ�
    [SerializeField] private float disappearingSpeed;   //��ʧ�ٶ�
    [SerializeField] private float colorLosingSpeed;    //��ɫ�ٶ�

    [SerializeField] private float lifeTime;    //�ı�Сʱǰ����ʱ��
    private float textTimer;    //ʣ��ʱ��

    private void Awake()
    {
        myText = GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        textTimer = lifeTime;   //��ʼ���ı�����ʱ���ʱ��
    }

    private void Update()
    {
        textTimer -= Time.deltaTime;

        //�ı�����ʱ���Ķ������������ƶ����γ�Ʈ��Ч��
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + 1), appearingSpeed * Time.deltaTime);

        //<0��ʼ��ʧ
        if (textTimer < 0)
        {
            //�ı���ɫalphaֵ�𽥼��٣���ɫЧ��
            float alpha = myText.color.a - colorLosingSpeed * Time.deltaTime;

            //�����ı���ɫ��RGB���䣬ֻ����alpha͸����
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, alpha);

            // ��alphaС��50���ı���ʼ�����ƶ����л���ʧ�Ķ���
            if (myText.color.a < 50)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + 1), disappearingSpeed * Time.deltaTime);
            }

            if (myText.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }

    }
}
