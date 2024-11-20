using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEnemyFX : EntityFX
{
    protected override void Awake()
    {
        //获取子物体中的HP条UI组件
        HPBar = GetComponentInChildren<HPBar_UI>()?.gameObject;
    }

    protected override void Start()
    {
        player = PlayerManager.instance.player;
    }

    //闪烁效果
    private IEnumerator FlashFX()
    {
        yield return new WaitForSeconds(0);
    }
}
