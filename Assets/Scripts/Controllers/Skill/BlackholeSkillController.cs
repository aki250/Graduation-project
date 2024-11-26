using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.Rendering;
using UnityEngine;

public class BlackholeSkillController : MonoBehaviour
{
    //���ڴ洢��ݼ����б�������Prefab�ű��г�ʼ�����Ա�ÿ�����ɺڶ�ʱ���´�������ֹ�ڰ��¿�ݼ�ʱ�б��еĳ�Ա����ա�
    [SerializeField] private List<KeyCode> hotkeyList;
    [SerializeField] private GameObject hotkeyPrefab; 

    private float maxSize;  //�ڶ���С  
    private bool canGrow = true;    //���ſ���
    private float growSpeed;    //�����ٶ�
    private bool canShrink; //��С����
    private float shrinkSpeed;  //��С�ٶ�

    private int cloneAttackAmount;  //�ڶ���¡��������
    private float cloneAttackCooldown;  //�������
    private float cloneAttackTimer; //���������ʱ
    private bool canCloneAttack;    //���ƹ�����Ϊ

    private bool canCreateHotkey = true;    //QTE��ݼ���ʾ

    private bool playerIsTransparent;   //���״̬
    private bool canExitBlackHoleSkill; //�˳��ڶ�����״̬

    private float QTEInputTimer;    //QTE�����ʱ��

    private List<Transform> enemyTargets = new List<Transform>();   //�洢��ǰ�����ĵ����б�
    private List<GameObject> createdHotkey = new List<GameObject>();    //��ݼ������б�QTE�б�


    private void Update()
    {
        //ÿ֡���ٿ�¡������ʱ����QTE�����ʱ��
        cloneAttackTimer -= Time.deltaTime;
        QTEInputTimer -= Time.deltaTime;

        //QTE�����ʱ����Ȼ��Ч
        if (QTEInputTimer >= 0)
        {
            //�Ѿ���������QTE��ť�������봴�����ȼ�������ͬ
            if (enemyTargets.Count > 0 && enemyTargets.Count == createdHotkey.Count)
            {
                //��ǰ�ͷſ�¡����������QTE���봰��
                QTEInputTimer = Mathf.Infinity; //��QTE�����ʱ������Ϊ���޴�
                ReadyToReleaseBlackholeCloneAttack();   //׼���ͷźڶ���¡����
                BlackholeCloneAttack();
            }
        }
        else if (QTEInputTimer < 0)  //QTE��ʱ����
        {
            //��QTE�ĵ������ͷſ�¡����
            if (enemyTargets.Count > 0)
            {
                ReadyToReleaseBlackholeCloneAttack();
                BlackholeCloneAttack();
            }
            else  //���û�е��˱�QTE����������
            {
                EndCloneAttack();
            }

        }

        //�ڶ�����
        if (canGrow && !canShrink)
        {
            //ͨ����ֵ����Lerp������ڶ��Ĵ�С��ֱ���ﵽmaxSize
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            //�������෴
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
            {
                Destroy(gameObject);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�������뿪�ڶ�������ʱ
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
            collision.GetComponent<Enemy>().FreezeEnemy(false); //�Ӵ����˶���״̬
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

        //������ܹ������п�¡���ܽ����˷�֧���ܣ�ˮ�������¡������һ���͸��
        if (SkillManager.instance.clone.crystalMirageUnlocked)
        {
            playerIsTransparent = true;
        }
    }

    private void ReadyToReleaseBlackholeCloneAttack()
    {
        DestroyHotkeys();
        canCloneAttack = true;
        canCreateHotkey = false; //��ֹ���ͷſ�¡��������ӵ��˵�QTE�б�

        //���������͸����
        if (!playerIsTransparent)
        {
            PlayerManager.instance.player.fx.MakeEntityTransparent(true);   
            playerIsTransparent = true;
        }
        //�˳��ڶ��������Զ��ָ�
    }

    private void BlackholeCloneAttack()
    {   //��������ʱ�����ڡ����˺ڶ���¡���������й������������������ĵ��ˣ�
        if (cloneAttackTimer < 0 && canCloneAttack && cloneAttackAmount > 0 && enemyTargets.Count > 0)
        {
            cloneAttackTimer = cloneAttackCooldown; //���ù�����ʱ��

            int randomIndex = Random.Range(0, enemyTargets.Count);  //���ѡ��Ŀ�����


            Vector3 offset;
            if (Random.Range(0, 100) > 50)
            {
                offset = new Vector3(1, 0); //�Ҳ�����
            }
            else
            {
                offset = new Vector3(-1, 0);    //��
            }

            //����ѧϰˮ����Ӱ
            if (SkillManager.instance.clone.crystalMirageUnlocked)
            {
                SkillManager.instance.crystal.CreateCrystal();

                //���ѡ��һ���ںڶ���Χ�ڵĵ���
                SkillManager.instance.crystal.CurrentCrystalSpecifyEnemy(enemyTargets[randomIndex]);
            }
            else
            {
                //�����ѡ��ĵ���λ�ü���ƫ������������¡
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
        //ɾ�ȼ����ؼ��ܣ����ڶ����ع���
        DestroyHotkeys();
        canExitBlackHoleSkill = true;
        canShrink = true;
        canCloneAttack = false;
    }
    private void CreateHotkey(Collider2D collision)
    {
        //��ݼ��б�Ϊ�գ��޷�Ϊ�µĵ������ɿ�ݼ�
        if (hotkeyList.Count <= 0)
        {
            Debug.Log("No enough available hotkeys in list"); //��ʾ��ݼ��б���
            return; 
        }

        //��¡�����Ѿ��ͷţ����ֹΪ�µĵ������ɿ�ݼ�
        if (!canCreateHotkey)
        {
            return; 
        }

        //����һ���µĿ�ݼ�����λ���ڵ����Ϸ���ƫ����Ϊ +2 ��λ�ã�
        GameObject newHotkey = Instantiate(
            hotkeyPrefab, //Ԥ�Ƽ�
            collision.transform.position + new Vector3(0, 2), //λ������Ϊ���˵�ǰλ�õ��Ϸ�
            Quaternion.identity //ֹͣ��ת
        );

        createdHotkey.Add(newHotkey);   //�����ɵĿ�ݼ���������Ѵ����Ŀ�ݼ��б�

        // ��ȡ�����ɿ�ݼ�����Ŀ��ƽű�
        Blackhole_HotkeyController newHotkeyScript = newHotkey.GetComponent<Blackhole_HotkeyController>();

        //�ӿ�ݼ��б������ѡ��һ��������Ϊ��ݼ�
        KeyCode chosenKey = hotkeyList[Random.Range(0, hotkeyList.Count)];

        //�ӿ��ÿ�ݼ��б����Ƴ���ѡ��Ŀ�ݼ��������ظ�ʹ��
        hotkeyList.Remove(chosenKey);

        // ���ÿ�ݼ�����
        // 1. ����ѡ���Ŀ�ݼ�
        // 2. �󶨵����˵� Transform
        // 3. ����ǰ���󣨺ڶ������ݸ���ݼ����ƽű�
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
