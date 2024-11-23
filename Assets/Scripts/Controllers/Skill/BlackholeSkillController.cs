using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.Rendering;
using UnityEngine;

public class BlackholeSkillController : MonoBehaviour
{
    //用于存储快捷键的列表。必须在Prefab脚本中初始化，以便每次生成黑洞时重新创建，防止在按下快捷键时列表中的成员被清空。
    [SerializeField] private List<KeyCode> hotkeyList;
    [SerializeField] private GameObject hotkeyPrefab; 

    private float maxSize;  //黑洞大小  
    private bool canGrow = true;    //扩张开关
    private float growSpeed;    //扩张速度
    private bool canShrink; //缩小开关
    private float shrinkSpeed;  //缩小速度

    private int cloneAttackAmount;  //黑洞克隆攻击次数
    private float cloneAttackCooldown;  //攻击间隔
    private float cloneAttackTimer; //攻击间隔计时
    private bool canCloneAttack;    //控制攻击行为

    private bool canCreateHotkey = true;    //QTE快捷键提示

    private bool playerIsTransparent;   //玩家状态
    private bool canExitBlackHoleSkill; //退出黑洞技能状态

    private float QTEInputTimer;    //QTE输入计时器

    private List<Transform> enemyTargets = new List<Transform>();   //存储当前锁定的敌人列表
    private List<GameObject> createdHotkey = new List<GameObject>();    //快捷键对象列表（QTE列表）


    private void Update()
    {
        //每帧减少克隆攻击计时器，QTE输入计时器
        cloneAttackTimer -= Time.deltaTime;
        QTEInputTimer -= Time.deltaTime;

        //QTE输入计时器仍然有效
        if (QTEInputTimer >= 0)
        {
            //已经按下所有QTE按钮且数量与创建的热键数量相同
            if (enemyTargets.Count > 0 && enemyTargets.Count == createdHotkey.Count)
            {
                //提前释放克隆攻击，结束QTE输入窗口
                QTEInputTimer = Mathf.Infinity; //将QTE输入计时器设置为无限大
                ReadyToReleaseBlackholeCloneAttack();   //准备释放黑洞克隆攻击
                BlackholeCloneAttack();
            }
        }
        else if (QTEInputTimer < 0)  //QTE计时结束
        {
            //在QTE的敌人上释放克隆攻击
            if (enemyTargets.Count > 0)
            {
                ReadyToReleaseBlackholeCloneAttack();
                BlackholeCloneAttack();
            }
            else  //如果没有敌人被QTE，结束技能
            {
                EndCloneAttack();
            }

        }

        //黑洞扩张
        if (canGrow && !canShrink)
        {
            //通过插值函数Lerp逐渐增大黑洞的大小，直到达到maxSize
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            //与扩张相反
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
            {
                Destroy(gameObject);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //当敌人离开黑洞触发器时
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeEnemy(true);

            CreateHotkey(collision);

            //add hotkey
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeEnemy(false);
        }
    }

    public void SetupBlackholeSkill(float _maxSize, float _growSpeed, float _shrinkSpeed, int _cloneAttackAmount, float _cloneAttackCooldown, float _QTEInputWindow)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        cloneAttackAmount = _cloneAttackAmount;
        cloneAttackCooldown = _cloneAttackCooldown;
        QTEInputTimer = _QTEInputWindow;

        //player won't be transparent
        //if Replace Clone By Crystal is enabled in Clone Skill
        if (SkillManager.instance.clone.crystalMirageUnlocked)
        {
            playerIsTransparent = true;
        }
    }

    private void ReadyToReleaseBlackholeCloneAttack()
    {
        DestroyHotkeys();
        canCloneAttack = true;
        canCreateHotkey = false;  //can't add enemy to QTE list after releasing clone attack

        //make player transparent when releasing clone attack
        if (!playerIsTransparent)
        {
            PlayerManager.instance.player.fx.MakeEntityTransparent(true);
            playerIsTransparent = true;
        }
        //player will become visible again when exiting blackhole skill state
    }

    private void BlackholeCloneAttack()
    {
        if (cloneAttackTimer < 0 && canCloneAttack && cloneAttackAmount > 0 && enemyTargets.Count > 0)
        {
            cloneAttackTimer = cloneAttackCooldown;

            int randomIndex = Random.Range(0, enemyTargets.Count);

            Vector3 offset;
            //make clone spawn next to the enemy with a bit offset
            if (Random.Range(0, 100) > 50)
            {
                offset = new Vector3(1, 0);
            }
            else
            {
                offset = new Vector3(-1, 0);
            }

            //if Replace Clone By Crystal is enabled in Clone Skill
            //Create Crystal instead of Clone
            if (SkillManager.instance.clone.crystalMirageUnlocked)
            {
                SkillManager.instance.crystal.CreateCrystal();

                //ranomly select enemy inside the blackhole range
                SkillManager.instance.crystal.CurrentCrystalSpecifyEnemy(enemyTargets[randomIndex]);
            }
            else
            {
                SkillManager.instance.clone.CreateClone(enemyTargets[randomIndex].position + offset);
            }

            cloneAttackAmount--;

            if (cloneAttackAmount <= 0)
            {
                Invoke("EndCloneAttack", 0.5f);
            }
        }
    }

    private void EndCloneAttack()
    {
        DestroyHotkeys();
        canExitBlackHoleSkill = true;
        canShrink = true;
        canCloneAttack = false;
    }
    private void CreateHotkey(Collider2D collision)
    {
        //快捷键列表为空，无法为新的敌人生成快捷键
        if (hotkeyList.Count <= 0)
        {
            Debug.Log("No enough available hotkeys in list"); //打印调试信息，提示快捷键列表不足
            return; 
        }

        // 如果克隆攻击已经释放，则禁止为新的敌人生成快捷键
        if (!canCreateHotkey)
        {
            return; // 直接退出方法，不允许添加额外的 QTE 敌人
        }

        //创建一个新的快捷键对象，位置在敌人上方（偏移量为 +2 的位置）
        GameObject newHotkey = Instantiate(
            hotkeyPrefab, // 预制件
            collision.transform.position + new Vector3(0, 2), //位置设置为敌人当前位置的上方
            Quaternion.identity //停止旋转
        );

        // 将生成的快捷键对象加入已创建的快捷键列表
        createdHotkey.Add(newHotkey);

        // 获取新生成快捷键对象的控制脚本
        Blackhole_HotkeyController newHotkeyScript = newHotkey.GetComponent<Blackhole_HotkeyController>();

        // 从快捷键列表中随机选择一个按键作为快捷键
        KeyCode chosenKey = hotkeyList[Random.Range(0, hotkeyList.Count)];

        // 从可用快捷键列表中移除已选择的快捷键，避免重复使用
        hotkeyList.Remove(chosenKey);

        // 配置快捷键对象：
        // 1. 设置选定的快捷键
        // 2. 绑定到敌人的 Transform
        // 3. 将当前对象（黑洞）传递给快捷键控制脚本
        newHotkeyScript.SetupHotkey(chosenKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform)
    {
        enemyTargets.Add(_enemyTransform);
    }

    private void DestroyHotkeys()
    {
        if (createdHotkey.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < createdHotkey.Count; i++)
        {
            Destroy(createdHotkey[i]);
        }
    }

    public bool CloneAttackHasFinished()
    {
        if (canExitBlackHoleSkill)
        {
            return true;
        }

        return false;
    }
}
