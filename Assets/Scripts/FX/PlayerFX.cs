using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//玩家特效管理类，包括屏幕震动、残影效果和尘土特效等
public class PlayerFX : EntityFX
{
    private bool canScreenShake = true;  //触发屏幕震动

    [Header("屏幕震动特效")]
    [SerializeField] private float shakeMultiplier;  //屏幕震动的强度因子
    public Vector3 shakeDirection_light;  //轻微震动
    public Vector3 shakeDirection_medium;  //中等震动
    public Vector3 shakeDirection_heavy;  //强烈震动
    private CinemachineImpulseSource screenShake;  //Cinemachine震动源组件

    [Header("残影特效")]
    [SerializeField] private GameObject afterimagePrefab;  //残影预制体
    [SerializeField] private float afterimageColorLosingSpeed;  //残影颜色消失速度
    [SerializeField] private float afterimageCooldown;  //残影的冷却时间
    private float afterimageCooldownTimer;  //残影冷却计时器

    [Space]
    [SerializeField] private ParticleSystem dustFX;  //尘土特效
    [SerializeField] private ParticleSystem downStrikeDustFX;  //下击时的尘土特效

    protected override void Awake()
    {
        base.Awake();  

        screenShake = GetComponent<CinemachineImpulseSource>();  //获取Cinemachine的震动源组件
    }

    // 在Start中初始化冷却计时器
    protected override void Start()
    {
        base.Start(); 

        afterimageCooldownTimer = 0;  //初始化残影冷却计时器
    }

    protected override void Update()
    {
        base.Update();

        afterimageCooldownTimer -= Time.deltaTime;  //每帧减少冷却时间
    }

    //屏幕震动效果，根据传入的方向进行震动
    public void ScreenShake(Vector3 _shakeDirection)
    {
        // 防止在同时击中多个敌人时发生过度震动
        if (canScreenShake)
        {
            //设置震动方向和震动强度
            screenShake.m_DefaultVelocity = new Vector3(_shakeDirection.x * player.facingDirection, _shakeDirection.y) * shakeMultiplier;
            screenShake.GenerateImpulse();  //生成震动
            canScreenShake = false;  //禁止再次震动
            Invoke("EnableScreenShake", 0.05f);  //0.05秒后允许下一次震动
        }
    }

    //启用屏幕震动的条件
    private void EnableScreenShake()
    {
        canScreenShake = true;  //允许屏幕震动
    }

    //创建残影特效，只有在冷却时间结束时才会生成
    public void CreateAfterimage()
    {
        if (afterimageCooldownTimer < 0)
        {
            //实例化残影预制体
            GameObject newAfterimage = Instantiate(afterimagePrefab, transform.position, transform.rotation);
            //设置残影的颜色消失速度
            newAfterimage.GetComponent<AfterimageFX>()?.SetupAfterImage(sr.sprite, afterimageColorLosingSpeed);

            afterimageCooldownTimer = afterimageCooldown;  //重置冷却计时器
        }
    }

    //播放尘土特效
    public void PlayDustFX()
    {
        if (dustFX != null)
        {
            dustFX.Play();
        }
    }

    //播放下击尘土特效
    public void PlayDownStrikeDustFX()
    {
        if (downStrikeDustFX != null)
        {
            downStrikeDustFX.Play();  
        }
    }
}
