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

    //触发淡出效果。
    public void FadeOut()
    {
        //使游戏对象处于激活状态。
        gameObject.SetActive(true);

        anim.SetTrigger("FadeOut");
    }

    //用于触发淡入效果。
    public void FadeIn()
    {
        //使游戏对象处于激活状态。
        gameObject.SetActive(true);

        anim.SetTrigger("FadeIn");
    }
}