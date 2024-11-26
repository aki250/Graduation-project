using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class CrystalSkill : Skill
{
    [Space]
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalExistenceDuration;    //水晶存在时间
    private GameObject currentCrystal;  //激活水晶对象
    [SerializeField] private SkillTreeSlot_UI crystalUnlockButton;  //解锁水晶技能按钮
    public bool crystalUnlocked { get; private set; }   //水晶是否解锁

    [Header("残影一闪解锁")]  //在传送到水晶位置时，在原始位置生成克隆体
    [SerializeField] private SkillTreeSlot_UI mirageBlinkUnlockButton; 
    public bool mirageBlinkUnlocked { get; private set; }   

    [Header("水晶爆炸解锁")]
    [SerializeField] private SkillTreeSlot_UI explosiveCrystalUnlockButton;
    public bool explosiveCrystalUnlocked { get; private set; }

    [Header("水晶移动解锁")]
    [SerializeField] private SkillTreeSlot_UI movingCrystalUnlockButton;
    public bool movingCrystalUnlocked { get; private set; }
    [SerializeField] private float moveSpeed;

    [Header("水晶枪解锁")]
    [SerializeField] private SkillTreeSlot_UI crystalGunUnlockButton;
    public bool crystalGunUnlocked { get; private set; }
    [SerializeField] private int magSize;   //弹匣大小
    [SerializeField] private float shootCooldown;  //射击频率
    [SerializeField] private float reloadTime;  //射击时间
    [SerializeField] private float shootWindow; //射击窗口
    private float shootWindowTimer; //水晶枪弹匣，存储水晶对象
    [SerializeField] private List<GameObject> crystalMag = new List<GameObject>();
    private bool reloading = false; //正在重新装填子弹


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

            //英语
            if (LanguageManager.instance.localeID == 0)
            {
                player.fx.CreatePopUpText("Skill is in cooldown");

            }
            //中文
            else if (LanguageManager.instance.localeID == 1)
            {
                player.fx.CreatePopUpText("技能冷却中！");
            }

            return false;
        }
    }

    //水晶技能选择
    public override void UseSkill()
    {
        base.UseSkill();

        //如果水晶枪技能已解锁，禁用所有单个水晶功能
        if (crystalGunUnlocked)
        {
            //如果水晶枪可用并成功射击
            if (ShootCrystalGunIfAvailable())
            {
                //更新技能面板中的水晶UI图标
                EnterCooldown();
                return;
            }

            //英文
            if (LanguageManager.instance.localeID == 0)
            {
                player.fx.CreatePopUpText("Skill is in cooldown");

            }
            //中文
            else if (LanguageManager.instance.localeID == 1)
            {
                player.fx.CreatePopUpText("技能冷却中！");
            }

            return;
        }

        //当前没有水晶，则创建
        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            //如果水晶可以移动向敌人，那么传送功能将被禁用
            if (movingCrystalUnlocked)
            {
                return;
            }

            //若此残影一闪技能解锁
            if (mirageBlinkUnlocked)
            {
                //玩家位置生成克隆体
                SkillManager.instance.clone.CreateClone(player.transform.position);
                //Destroy(currentCrystal);
                currentCrystal.GetComponent<CrystalSkillController>()?.crystalSelfDestroy();
            }

            //玩家与水晶位置互换
            Vector2 playerPosition = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPosition;

            currentCrystal.GetComponent<CrystalSkillController>()?.EndCrystal_ExplodeIfAvailable();

            EnterCooldown();
        }
    }

    public void CreateCrystal()
    {
        //创建水晶实例在玩家位置
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity); ;
        //获取水晶技能控制脚本
        CrystalSkillController currentCrystalScript = currentCrystal.GetComponent<CrystalSkillController>();
        //获取                                         持续时间               是否爆炸                是否移动            移速          寻找最近敌人
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
        EnterCooldown();    //装填，则进CD

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

        //计算需要添加的弹药数量，即弹匣大小减去当前弹药数量
        int ammoToAdd = magSize - crystalMag.Count;

        // 添加弹药到弹匣
        for (int i = 0; i < ammoToAdd; i++)
        {
            crystalMag.Add(crystalPrefab);
        }
    }

    private bool ShootCrystalGunIfAvailable()
    {
        //水晶枪不在装填状态
        if (crystalGunUnlocked && !reloading)
        {
            //且有子弹
            if (crystalMag.Count > 0)
            {
                //取出弹匣中的最后一个水晶
                GameObject crystalToSpawn = crystalMag[crystalMag.Count - 1];
                //实例化水晶
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                //从弹匣中移除已使用的水晶
                crystalMag.Remove(crystalToSpawn);

                //设置新水晶属性
                newCrystal.GetComponent<CrystalSkillController>().
                    SetupCrystal(crystalExistenceDuration, explosiveCrystalUnlocked, movingCrystalUnlocked, moveSpeed, FindClosestEnemy(newCrystal.transform));

                //重置射击窗口计时器
                shootWindowTimer = shootWindow;

                //如果弹匣为空，则开始重新装填
                if (crystalMag.Count <= 0)
                {
                    ReloadCrystalMag();
                }
            }

            return true;    //成功射击
        }

        return false;   //失败
    }

    public void DestroyCurrentCrystal_InCrystalMirageOnly()
    {
        //如果当前水晶不为空，则销毁
        if (currentCrystal != null)
        {
            Destroy(currentCrystal);
        }
    }

    public void CurrentCrystalSpecifyEnemy(Transform _enemy)
    {
        //如果当前水晶不为空，则指定敌人为目标
        if (currentCrystal != null)
        {
            currentCrystal.GetComponent<CrystalSkillController>()?.SpecifyEnemyTarget(_enemy);
        }
    }

    public void EnterCooldown()
    {
        //如果冷却计时器<0且未解锁水晶枪，设置冷却图像
        if (cooldownTimer < 0 && !crystalGunUnlocked)
        {
            InGame_UI.instance.SetCrystalCooldownImage();
            cooldownTimer = cooldown;
        }
        else if (cooldownTimer < 0 && crystalGunUnlocked)   //如果冷却计时器小于0且已解锁水晶枪，设置冷却图像,并获取水晶枪冷却时间
        {
            InGame_UI.instance.SetCrystalCooldownImage();
            cooldownTimer = GetCrystalCooldown();
        }

        skillLastUseTime = Time.time;   //记录技能最后一次使用时间
    }

    public float GetCrystalCooldown()
    {
        //如果未解锁水晶枪，冷却时间为默认冷却时间
        float crystalCooldown = cooldown;

        if (crystalGunUnlocked)
        {
            if (shootWindowTimer > 0)
            {
                //如果射击窗口计时器大于0，冷却时间为射击间隔
                crystalCooldown = shootCooldown;
            }

            if (reloading)
            {
                //如果正在重新装填，冷却时间为水晶枪重新装填时间
                crystalCooldown = reloadTime;
            }
        }

        return crystalCooldown; //返回水晶冷却时间
    }

    public override bool SkillIsReadyToUse()
    {
        //如果已解锁水晶枪
        if (crystalGunUnlocked)
        {
            //如果不在重新装填状态，技能准备好可以使用
            if (!reloading)
            {
                return true;
            }
            return false; //否则，技能未准备好
        }
        else
        {
            //如果冷却计时器小于0，技能准备好可以使用
            if (cooldownTimer < 0)
            {
                return true;
            }
            return false; //否则，技能未准备好
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
