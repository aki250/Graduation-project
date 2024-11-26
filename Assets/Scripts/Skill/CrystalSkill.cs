using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class CrystalSkill : Skill
{
    [Space]
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalExistenceDuration;    //ˮ������ʱ��
    private GameObject currentCrystal;  //����ˮ������
    [SerializeField] private SkillTreeSlot_UI crystalUnlockButton;  //����ˮ�����ܰ�ť
    public bool crystalUnlocked { get; private set; }   //ˮ���Ƿ����

    [Header("��Ӱһ������")]  //�ڴ��͵�ˮ��λ��ʱ����ԭʼλ�����ɿ�¡��
    [SerializeField] private SkillTreeSlot_UI mirageBlinkUnlockButton; 
    public bool mirageBlinkUnlocked { get; private set; }   

    [Header("ˮ����ը����")]
    [SerializeField] private SkillTreeSlot_UI explosiveCrystalUnlockButton;
    public bool explosiveCrystalUnlocked { get; private set; }

    [Header("ˮ���ƶ�����")]
    [SerializeField] private SkillTreeSlot_UI movingCrystalUnlockButton;
    public bool movingCrystalUnlocked { get; private set; }
    [SerializeField] private float moveSpeed;

    [Header("ˮ��ǹ����")]
    [SerializeField] private SkillTreeSlot_UI crystalGunUnlockButton;
    public bool crystalGunUnlocked { get; private set; }
    [SerializeField] private int magSize;   //��ϻ��С
    [SerializeField] private float shootCooldown;  //���Ƶ��
    [SerializeField] private float reloadTime;  //���ʱ��
    [SerializeField] private float shootWindow; //�������
    private float shootWindowTimer; //ˮ��ǹ��ϻ���洢ˮ������
    [SerializeField] private List<GameObject> crystalMag = new List<GameObject>();
    private bool reloading = false; //��������װ���ӵ�


    protected override void Start()
    {
        base.Start();

        crystalUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCrystal);
        mirageBlinkUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCrystalMirage);
        explosiveCrystalUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockExplosiveCrystal);
        movingCrystalUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockMovingCrystal);
        crystalGunUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCrystalGun);
    }

    protected override void Update()
    {
        base.Update();

        shootWindowTimer -= Time.deltaTime;

        //if haven't shoot all the ammo in the mag for a while
        //auto reload the mag
        //shootWindowTimer = shootWindow; in ShootCrystalGunIfAvailable()
        //add !reloading to prevent calling coroutine multiple times
        //when using invoke in the past, the invoke is gonna get called lots of times
        //because there's gonna be much time in the shootWindowTimer <= 0 && 0 < ammo < magsize state
        if (shootWindowTimer <= 0 && crystalMag.Count > 0 && crystalMag.Count < magSize && !reloading)
        {
            ReloadCrystalMag();
        }
    }

    protected override void CheckUnlockFromSave()
    {
        UnlockCrystal();
        UnlockCrystalGun();
        UnlockCrystalMirage();
        UnlockExplosiveCrystal();
        UnlockMovingCrystal();
    }

    public override bool UseSkillIfAvailable()
    {
        if (crystalGunUnlocked)
        {
            UseSkill();
            return true;
        }
        else
        {
            if (cooldownTimer < 0)
            {
                UseSkill();
                return true;
            }

            //Ӣ��
            if (LanguageManager.instance.localeID == 0)
            {
                player.fx.CreatePopUpText("Skill is in cooldown");

            }
            //����
            else if (LanguageManager.instance.localeID == 1)
            {
                player.fx.CreatePopUpText("������ȴ�У�");
            }

            return false;
        }
    }

    //ˮ������ѡ��
    public override void UseSkill()
    {
        base.UseSkill();

        //���ˮ��ǹ�����ѽ������������е���ˮ������
        if (crystalGunUnlocked)
        {
            //���ˮ��ǹ���ò��ɹ����
            if (ShootCrystalGunIfAvailable())
            {
                //���¼�������е�ˮ��UIͼ��
                EnterCooldown();
                return;
            }

            //Ӣ��
            if (LanguageManager.instance.localeID == 0)
            {
                player.fx.CreatePopUpText("Skill is in cooldown");

            }
            //����
            else if (LanguageManager.instance.localeID == 1)
            {
                player.fx.CreatePopUpText("������ȴ�У�");
            }

            return;
        }

        //��ǰû��ˮ�����򴴽�
        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            //���ˮ�������ƶ�����ˣ���ô���͹��ܽ�������
            if (movingCrystalUnlocked)
            {
                return;
            }

            //���˲�Ӱһ�����ܽ���
            if (mirageBlinkUnlocked)
            {
                //���λ�����ɿ�¡��
                SkillManager.instance.clone.CreateClone(player.transform.position);
                //Destroy(currentCrystal);
                currentCrystal.GetComponent<CrystalSkillController>()?.crystalSelfDestroy();
            }

            //�����ˮ��λ�û���
            Vector2 playerPosition = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPosition;

            currentCrystal.GetComponent<CrystalSkillController>()?.EndCrystal_ExplodeIfAvailable();

            EnterCooldown();
        }
    }

    public void CreateCrystal()
    {
        //����ˮ��ʵ�������λ��
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity); ;
        //��ȡˮ�����ܿ��ƽű�
        CrystalSkillController currentCrystalScript = currentCrystal.GetComponent<CrystalSkillController>();
        //��ȡ                                         ����ʱ��               �Ƿ�ը                �Ƿ��ƶ�            ����          Ѱ���������
        currentCrystalScript.SetupCrystal(crystalExistenceDuration, explosiveCrystalUnlocked, movingCrystalUnlocked, moveSpeed, FindClosestEnemy(currentCrystal.transform));
    }

    private void ReloadCrystalMag()
    {
        if (reloading)
        {
            return;
        }

        StartCoroutine(ReloadCrystalMag_Coroutine());
    }

    private IEnumerator ReloadCrystalMag_Coroutine()
    {
        reloading = true;
        EnterCooldown();    //װ����CD

        yield return new WaitForSeconds(reloadTime);

        Reload();
        reloading = false;
    }

    private void Reload()
    {
        if (!reloading)
        {
            return;
        }

        //������Ҫ��ӵĵ�ҩ����������ϻ��С��ȥ��ǰ��ҩ����
        int ammoToAdd = magSize - crystalMag.Count;

        // ��ӵ�ҩ����ϻ
        for (int i = 0; i < ammoToAdd; i++)
        {
            crystalMag.Add(crystalPrefab);
        }
    }

    private bool ShootCrystalGunIfAvailable()
    {
        //ˮ��ǹ����װ��״̬
        if (crystalGunUnlocked && !reloading)
        {
            //�����ӵ�
            if (crystalMag.Count > 0)
            {
                //ȡ����ϻ�е����һ��ˮ��
                GameObject crystalToSpawn = crystalMag[crystalMag.Count - 1];
                //ʵ����ˮ��
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                //�ӵ�ϻ���Ƴ���ʹ�õ�ˮ��
                crystalMag.Remove(crystalToSpawn);

                //������ˮ������
                newCrystal.GetComponent<CrystalSkillController>().
                    SetupCrystal(crystalExistenceDuration, explosiveCrystalUnlocked, movingCrystalUnlocked, moveSpeed, FindClosestEnemy(newCrystal.transform));

                //����������ڼ�ʱ��
                shootWindowTimer = shootWindow;

                //�����ϻΪ�գ���ʼ����װ��
                if (crystalMag.Count <= 0)
                {
                    ReloadCrystalMag();
                }
            }

            return true;    //�ɹ����
        }

        return false;   //ʧ��
    }

    public void DestroyCurrentCrystal_InCrystalMirageOnly()
    {
        //�����ǰˮ����Ϊ�գ�������
        if (currentCrystal != null)
        {
            Destroy(currentCrystal);
        }
    }

    public void CurrentCrystalSpecifyEnemy(Transform _enemy)
    {
        //�����ǰˮ����Ϊ�գ���ָ������ΪĿ��
        if (currentCrystal != null)
        {
            currentCrystal.GetComponent<CrystalSkillController>()?.SpecifyEnemyTarget(_enemy);
        }
    }

    public void EnterCooldown()
    {
        //�����ȴ��ʱ��<0��δ����ˮ��ǹ��������ȴͼ��
        if (cooldownTimer < 0 && !crystalGunUnlocked)
        {
            InGame_UI.instance.SetCrystalCooldownImage();
            cooldownTimer = cooldown;
        }
        else if (cooldownTimer < 0 && crystalGunUnlocked)   //�����ȴ��ʱ��С��0���ѽ���ˮ��ǹ��������ȴͼ��,����ȡˮ��ǹ��ȴʱ��
        {
            InGame_UI.instance.SetCrystalCooldownImage();
            cooldownTimer = GetCrystalCooldown();
        }

        skillLastUseTime = Time.time;   //��¼�������һ��ʹ��ʱ��
    }

    public float GetCrystalCooldown()
    {
        //���δ����ˮ��ǹ����ȴʱ��ΪĬ����ȴʱ��
        float crystalCooldown = cooldown;

        if (crystalGunUnlocked)
        {
            if (shootWindowTimer > 0)
            {
                //���������ڼ�ʱ������0����ȴʱ��Ϊ������
                crystalCooldown = shootCooldown;
            }

            if (reloading)
            {
                //�����������װ���ȴʱ��Ϊˮ��ǹ����װ��ʱ��
                crystalCooldown = reloadTime;
            }
        }

        return crystalCooldown; //����ˮ����ȴʱ��
    }

    public override bool SkillIsReadyToUse()
    {
        //����ѽ���ˮ��ǹ
        if (crystalGunUnlocked)
        {
            //�����������װ��״̬������׼���ÿ���ʹ��
            if (!reloading)
            {
                return true;
            }
            return false; //���򣬼���δ׼����
        }
        else
        {
            //�����ȴ��ʱ��С��0������׼���ÿ���ʹ��
            if (cooldownTimer < 0)
            {
                return true;
            }
            return false; //���򣬼���δ׼����
        }
    }

    //public Transform GetCurrentCrystalTransform()
    //{
    //    return currentCrystal?.transform;
    //}


    //public void CurrentCrystalChooseRandomEnemy(float _searchRadius)
    //{
    //    if (currentCrystal != null)
    //    {
    //        currentCrystal.GetComponent<CrystalSkillController>()?.CrystalChooseRandomEnemy(_searchRadius);
    //    }
    //}

    #region Unlock Crystal Skills
    private void UnlockCrystal()
    {
        if (crystalUnlocked)
        {
            return;
        }

        if (crystalUnlockButton.unlocked)
        {
            crystalUnlocked = true;
        }
    }

    private void UnlockCrystalMirage()
    {
        if (mirageBlinkUnlocked)
        {
            return;
        }

        if (mirageBlinkUnlockButton.unlocked)
        {
            mirageBlinkUnlocked = true;
        }
    }

    private void UnlockExplosiveCrystal()
    {
        if (explosiveCrystalUnlocked)
        {
            return;
        }

        if (explosiveCrystalUnlockButton.unlocked)
        {
            explosiveCrystalUnlocked = true;
        }
    }

    private void UnlockMovingCrystal()
    {
        if (movingCrystalUnlocked)
        {
            return;
        }

        if (movingCrystalUnlockButton.unlocked)
        {
            movingCrystalUnlocked = true;
        }
    }

    private void UnlockCrystalGun()
    {
        if (crystalGunUnlocked)
        {
            return;
        }

        if (crystalGunUnlockButton.unlocked)
        {
            crystalGunUnlocked = true;
        }
    }
    #endregion
}
