using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class DeathBringerAnimationTrigger : Enemy_AnimationTrigger
{
    //获取父对象上的 DeathBringer 组件
    private DeathBringer deathBringer => GetComponentInParent<DeathBringer>();

    //动画传送
    private void Teleport()
    {
        //调用DeathBringer中的方法，寻找传送位置
        deathBringer.FindTeleportPosition();
    }

    //动画使实体变透明
    private void MakeInvisible()
    {
        // 调用 DeathBringer 的特效方法，将实体透明化
        deathBringer.fx.MakeEntityTransparent(true);
    }

    //动画使实体变可见
    private void MakeVisible()
    {
        //调用DeathBringer的特效方法，取消实体的透明效果
        deathBringer.fx.MakeEntityTransparent(false);
    }
}
