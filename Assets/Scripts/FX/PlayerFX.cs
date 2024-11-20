using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�����Ч�����࣬������Ļ�𶯡���ӰЧ���ͳ�����Ч��
public class PlayerFX : EntityFX
{
    private bool canScreenShake = true;  //������Ļ��

    [Header("��Ļ����Ч")]
    [SerializeField] private float shakeMultiplier;  //��Ļ�𶯵�ǿ������
    public Vector3 shakeDirection_light;  //��΢��
    public Vector3 shakeDirection_medium;  //�е���
    public Vector3 shakeDirection_heavy;  //ǿ����
    private CinemachineImpulseSource screenShake;  //Cinemachine��Դ���

    [Header("��Ӱ��Ч")]
    [SerializeField] private GameObject afterimagePrefab;  //��ӰԤ����
    [SerializeField] private float afterimageColorLosingSpeed;  //��Ӱ��ɫ��ʧ�ٶ�
    [SerializeField] private float afterimageCooldown;  //��Ӱ����ȴʱ��
    private float afterimageCooldownTimer;  //��Ӱ��ȴ��ʱ��

    [Space]
    [SerializeField] private ParticleSystem dustFX;  //������Ч
    [SerializeField] private ParticleSystem downStrikeDustFX;  //�»�ʱ�ĳ�����Ч

    protected override void Awake()
    {
        base.Awake();  

        screenShake = GetComponent<CinemachineImpulseSource>();  //��ȡCinemachine����Դ���
    }

    // ��Start�г�ʼ����ȴ��ʱ��
    protected override void Start()
    {
        base.Start(); 

        afterimageCooldownTimer = 0;  //��ʼ����Ӱ��ȴ��ʱ��
    }

    protected override void Update()
    {
        base.Update();

        afterimageCooldownTimer -= Time.deltaTime;  //ÿ֡������ȴʱ��
    }

    //��Ļ��Ч�������ݴ���ķ��������
    public void ScreenShake(Vector3 _shakeDirection)
    {
        // ��ֹ��ͬʱ���ж������ʱ����������
        if (canScreenShake)
        {
            //�����𶯷������ǿ��
            screenShake.m_DefaultVelocity = new Vector3(_shakeDirection.x * player.facingDirection, _shakeDirection.y) * shakeMultiplier;
            screenShake.GenerateImpulse();  //������
            canScreenShake = false;  //��ֹ�ٴ���
            Invoke("EnableScreenShake", 0.05f);  //0.05���������һ����
        }
    }

    //������Ļ�𶯵�����
    private void EnableScreenShake()
    {
        canScreenShake = true;  //������Ļ��
    }

    //������Ӱ��Ч��ֻ������ȴʱ�����ʱ�Ż�����
    public void CreateAfterimage()
    {
        if (afterimageCooldownTimer < 0)
        {
            //ʵ������ӰԤ����
            GameObject newAfterimage = Instantiate(afterimagePrefab, transform.position, transform.rotation);
            //���ò�Ӱ����ɫ��ʧ�ٶ�
            newAfterimage.GetComponent<AfterimageFX>()?.SetupAfterImage(sr.sprite, afterimageColorLosingSpeed);

            afterimageCooldownTimer = afterimageCooldown;  //������ȴ��ʱ��
        }
    }

    //���ų�����Ч
    public void PlayDustFX()
    {
        if (dustFX != null)
        {
            dustFX.Play();
        }
    }

    //�����»�������Ч
    public void PlayDownStrikeDustFX()
    {
        if (downStrikeDustFX != null)
        {
            downStrikeDustFX.Play();  
        }
    }
}
