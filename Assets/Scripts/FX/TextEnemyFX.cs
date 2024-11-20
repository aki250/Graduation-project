using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEnemyFX : EntityFX
{
    protected override void Awake()
    {
        //��ȡ�������е�HP��UI���
        HPBar = GetComponentInChildren<HPBar_UI>()?.gameObject;
    }

    protected override void Start()
    {
        player = PlayerManager.instance.player;
    }

    //��˸Ч��
    private IEnumerator FlashFX()
    {
        yield return new WaitForSeconds(0);
    }
}
