using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwordSkillController : MonoBehaviour
{
    private Animator anim;  //剑动画
    private Rigidbody2D rb; //剑刚体
    private CircleCollider2D cd;    //剑碰撞体
    private Player player;

    private bool canRotate = true;  //控制剑是否旋转
    private bool isReturning;   //返回玩家手中
    private float swordReturnSpeed; //返回速度
    private Vector2 launchSpeed; //发射速度，特效方向

    private float enemyFreezeDuration;  //冻结敌人时间
    private float enemyVulnerableDuration;  //敌人脆弱状态持续时间

    [Header("弹跳剑信息")]
    private bool isBouncingSword;   //当前是否为弹跳剑
    private int bounceAmount;   //弹跳次数
    private float bounceSpeed;  //弹跳速度
    private List<Transform> bounceTargets = new List<Transform>();  //记录弹跳目标
    private int bounceTargetIndex;  //当前弹跳索引

    [Header("穿刺剑信息")]
    private bool isPierceSword; //当前是否穿刺剑
    private int pierceAmount;   //剑穿刺次数

    [Header("旋转剑相关信息")]
    private bool isSpinSword;   //当前是否旋转剑
    private float maxTravelDistance;    //最大飞行距离
    private float spinDuration; //旋转时间
    private float spinTimer;    //旋转记时
    private bool wasStopped;    //旋转是否停止


    private float spinHitCooldown;  //旋转剑集中目标冷却时间
    private float spinHitTimer; //当前冷却计时器

    private bool spinTimerHasBeenSetToSpinDuration = false;     //是否设置了旋转计时器
    private float spinDirection;    //剑旋转方向


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
        player = PlayerManager.instance.player;
    }

    private void Update()
    {
        //剑旋转方向为当前移动方向
        if (canRotate)
        {
            transform.right = rb.velocity;
        }
        //回收剑，则
        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, swordReturnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 1.5)
            {
                player.CatchSword(); //接住后销毁剑
            }
        }

        BounceSwordLogic();

        SpinSwordLogic();

        DestroySwordIfTooFar(30);   //剑离太远则销毁
    }

    //停止剑移动并旋转
    private void StopAndSpin()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition; //停止位置

        if (!spinTimerHasBeenSetToSpinDuration)
        {
            spinTimer = spinDuration;
        }

        spinTimerHasBeenSetToSpinDuration = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //正在回来，则不触发任何逻辑
        if (isReturning)
        {
            return;
        }

        //如果碰撞到敌人
        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
        }


        //旋转剑击中敌人后停止移动并开始旋转
        if (isSpinSword)
        {
            StopAndSpin();
            return;
        }
        else
        {
            DamageAndFreezeAndVulnerateEnemy(collision);
        }


        SetupBounceSwordTargets(collision);

        SwordStuckInto(collision);
    }

    //碰撞到的敌人挂上冻结、脆弱状态
    private void DamageAndFreezeAndVulnerateEnemy(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            //对敌人造成伤害
            player.stats.DoDamge(enemy.GetComponent<CharacterStats>());

            //如果解锁冻结技能，冻结敌人
            if (SkillManager.instance.sword.timeStopUnlocked)
            {
                enemy.FreezeEnemyForTime(enemyFreezeDuration);

            }

            //如果解锁脆弱技能，使敌人进入脆弱状态
            if (SkillManager.instance.sword.vulnerabilityUnlocked)
            {
                //Debug.Log($"Enemy {enemy.gameObject.name} is vulnerable");
                enemy.stats.BecomeVulnerableForTime(enemyVulnerableDuration);
            }

            //summon charm effect
            Inventory.instance.UseCharmEffect_ConsiderCooldown(enemy.transform);
            //ItemData_Equipment equippedCharm = Inventory.instance.GetEquippedEquipmentByType(EquipmentType.Charm);

            //if (equippedCharm != null)
            //{
            //    equippedCharm.ExecuteItemEffect(enemy.transform);
            //}
        }

    }

    private void SwordStuckInto(Collider2D collision)
    {
        //穿刺剑，减少穿刺次数
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        //旋转剑，击中后停止运动进入旋转
        if (isSpinSword && collision.GetComponent<Enemy>() != null)
        {
            StopAndSpin();
            return;
        }

        canRotate = false;  //停止旋转
        cd.enabled = false; //禁用碰撞器

        rb.isKinematic = true;  //剑不受物理影响
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        //弹跳剑，且有目标，则跳出
        if (isBouncingSword && bounceTargets.Count > 0)
        {
            return;
        }

        anim.SetBool("Rotation", false);    //停止动画
        transform.parent = collision.transform; //

        //尘土特效
        ParticleSystem dustFX = GetComponentInChildren<ParticleSystem>();
        if (dustFX != null)
        {
            if (launchSpeed.x < 0)
            {
                dustFX.transform.localScale = new Vector3(-1, 1, 1);    //反转特效
            }

            dustFX.Play();  //播放特效
        }
    }

    private void BounceSwordLogic()
    {
        //剑是弹跳剑，且有目标列表
        if (isBouncingSword && bounceTargets.Count > 0)
        {
            //Debug.Log("开始弹跳");
            //剑的位置向目标位置飞去
            transform.position = Vector2.MoveTowards(transform.position, bounceTargets[bounceTargetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, bounceTargets[bounceTargetIndex].position) < 0.15f)
            {

                DamageAndFreezeAndVulnerateEnemy(bounceTargets[bounceTargetIndex].GetComponent<Collider2D>());
                //增加索引，减少弹跳次数
                bounceTargetIndex++;
                bounceAmount--;
                
                if (bounceAmount <= 0)
                {
                    isBouncingSword = false;
                    isReturning = true;
                }
                //目标索引超过列表长度，重置。
                if (bounceTargetIndex >= bounceTargets.Count)
                {
                    bounceTargetIndex = 0;
                }
            }
        }
    }

    //获取弹跳剑附近目标
    private void SetupBounceSwordTargets(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            //列表为空
            if (isBouncingSword && bounceTargets.Count <= 0)
            {
                //获取所有与剑位置重叠的碰撞器，
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    //若碰撞器附加敌人组件
                    if (hit.GetComponent<Enemy>() != null)
                    {
                        bounceTargets.Add(hit.transform);   //将敌人变换组件添加弹跳列表中
                    }
                }
            }

            //// 根据与玩家的距离对弹跳目标列表进行排序
            bounceTargets.Sort(new SortByDistanceToPlayer_BounceSwordTargets());
        }
    }

    private void SpinSwordLogic()
    {
        if (isSpinSword)
        {
            //剑达到最远飞行距离，且没有停止过
            if (Vector2.Distance(player.transform.position, transform.position) >= maxTravelDistance && !wasStopped)
            {
                StopAndSpin();  //停止旋转
            }
            //剑已经停止
            if (wasStopped)
            {
                //减少旋转计时器和旋转击中计时器
                spinTimer -= Time.deltaTime;
                spinHitTimer -= Time.deltaTime;

                //旋转剑移向敌人，如果进入旋转模式
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);

                //旋转计时器时间到
                if (spinTimer < 0)
                {
                    isReturning = true;  
                    isSpinSword = false;
                }

                //旋转击中计时器时间到
                if (spinHitTimer < 0)
                {   
                    //重置旋转击中计时器
                    spinHitTimer = spinHitCooldown;

                    //获取所有与剑重叠过的碰撞器，范围为1的圆
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)  //碰撞器附加了敌人组件
                        {
                            DamageAndFreezeAndVulnerateEnemy(hit);  //对敌人造成损伤并冻结
                        }
                    }
                }
            }
        }
    }

    public void SetupSword(Vector2 _launchSpeed, float _swordGravity, float _swordReturnSpeed, float _enemyFreezeDuration, float _enemyVulnerableDuration)
    {
        rb.velocity = _launchSpeed; //剑的初始速度

        rb.gravityScale = _swordGravity;//剑的重力影响系数

        swordReturnSpeed = _swordReturnSpeed; //剑返回时速度

        enemyFreezeDuration = _enemyFreezeDuration;//敌人被冻结的持续时间

        enemyVulnerableDuration = _enemyVulnerableDuration; //设置敌人被脆弱化的持续时间

        launchSpeed = _launchSpeed; //保存发射速度

        //剑不是穿刺剑，则开始旋转动画
        if (!isPierceSword)
        {
            anim.SetBool("Rotation", true);
        }

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);  //根据剑的速度设置旋转方向

    }

    public void SetupBounceSword(bool _isBounceSword, int _bounceAmount, float _bounceSpeed)
    {
        isBouncingSword = _isBounceSword;   //设置剑弹跳特性
        bounceAmount = _bounceAmount;   //设置弹跳次数
        bounceSpeed = _bounceSpeed; //设置弹跳速度
    }

    public void SetupPierceSword(bool _isPierceSword, int _pierceAmount)
    {
        isPierceSword = _isPierceSword; //设置穿刺特性
        pierceAmount = _pierceAmount;   //设置穿刺次数
    }

    public void SetupSpinSword(bool _isSpinSword, float _maxTravelDistance, float _spinDuration, float _spinHitCooldown)
    {
        isSpinSword = _isSpinSword; //设置剑是否具有旋转特性
        maxTravelDistance = _maxTravelDistance;     //设置剑的最大旅行距离
        spinDuration = _spinDuration;   //设置旋转持续时间
        spinHitCooldown = _spinHitCooldown; //设置旋转时击中敌人的冷却时间
    }

    public void ReturnSword()
    {
        //如果剑正在敌人之间弹跳，则不能返回
        if (bounceTargets.Count > 0)
        {
            return;
        }


        rb.constraints = RigidbodyConstraints2D.FreezeAll;  //冻结刚体，不受物理引擎影响，完全停止物体动态行为
        // rb.isKinematic = false; //通过脚本控制运动，物体本身不受物理影响

        transform.parent = null;    //将剑的父对象设置为null，使其不再跟随之前的对象
        isReturning = true; //剑的返回标志为真
    }

    private void DestroySwordIfTooFar(float _maxDistance)
    {
        //剑与玩家之间的距离大于设定的最大距离，则销毁
        if (Vector2.Distance(player.transform.position, transform.position) >= _maxDistance)
        {
            Destroy(gameObject);
        }
    }
}
