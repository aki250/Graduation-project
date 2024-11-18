using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScreen_UI : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    //��������Ч����
    public void FadeOut()
    {
        //ʹ��Ϸ�����ڼ���״̬��
        gameObject.SetActive(true);

        anim.SetTrigger("FadeOut");
    }

    //���ڴ�������Ч����
    public void FadeIn()
    {
        //ʹ��Ϸ�����ڼ���״̬��
        gameObject.SetActive(true);

        anim.SetTrigger("FadeIn");
    }
}