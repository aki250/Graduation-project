using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterimageFX : MonoBehaviour
{
    private SpriteRenderer sr;  //������ʾͼ���SpriteRenderer
    private float afterimageColorLosingSpeed;  //���Ʋ�Ӱ͸���ȼ��ٵ��ٶ�

    //���ò�Ӱ�ľ���ͼ��͸���ȱ仯�ٶ�
    public void SetupAfterImage(Sprite _spriteImage, float _afterimageColorLosingSpeed)
    {
        sr = GetComponent<SpriteRenderer>();  //��ȡ��ǰ����� SpriteRenderer ���
        sr.sprite = _spriteImage;  //���ò�Ӱͼ��Ϊ����ľ���

        afterimageColorLosingSpeed = _afterimageColorLosingSpeed;  //����͸���ȼ��ٵ��ٶ�
    }

    private void Update()
    {
        //͸���ȼ��٣���ʹ͸���ȱ�Ϊ 0
        float alpha = sr.color.a - afterimageColorLosingSpeed * Time.deltaTime;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);  //����SpriteRenderer��ɫ��͸���ȱ仯��

        if (sr.color.a <= 0)
        {
            Destroy(gameObject);  //�Ƴ���ӰЧ��
        }
    }
}
