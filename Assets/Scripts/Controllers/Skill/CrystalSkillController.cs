using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalSkillController : MonoBehaviour
{   
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private float crystalExistenceTimer; //ˮ������ʱ���ʱ��

    private bool canExplode; //ˮ���Ƿ���Ա�ը
    private bool canMove; //ˮ���Ƿ�����ƶ�
    private float moveSpeed; //ˮ������

    private bool canGrow; //ˮ���Ƿ��������
    private float growSpeed = 5; //ˮ���������ٶ�

    private Transform targetEnemy; //��ǰĿ�����

    private CrystalSkill crystalSkill; //ˮ������ʵ��

    private void Start()
    {
        crystalSkill = SkillManager.instance.crystal; //��ȡ��ǰ���ܹ�������,ˮ������ʵ��
    }

    private void Update()
    {
        crystalExistenceTimer -= Time.deltaTime; //����ˮ������ʱ��

        //���û��Ŀ����ˣ���ˮ���޷��ƶ�
        if (targetEnemy == null)
        {
            canMove = false; //�ر�ˮ���ƶ�
        }

        //���ˮ���Ĵ���ʱ���ѽ��������Դ�����ը
        if (crystalExistenceTimer < 0)
        {
            EndCrystal_ExplodeIfAvailable(); //ִ�б�ը��Ϊ��������ܣ�
        }

        // ���ˮ�������ƶ�������Ŀ�����
        if (canMove)
        {
            // ˮ�������˷����ƶ�
            transform.position = Vector2.MoveTowards(transform.position, targetEnemy.transform.position, moveSpeed * Time.deltaTime);

            // ���ˮ���ӽ����ˣ���ֹͣ�ƶ���������ը
            if (Vector2.Distance(transform.position, targetEnemy.transform.position) < 1)
            {
                canMove = false; // ֹͣˮ�����ƶ�
                EndCrystal_ExplodeIfAvailable(); // ִ�б�ը��Ϊ��������ܣ�
            }
        }

        // ���ˮ��������������������ˮ���Ĵ�С
        if (canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(2, 2), growSpeed * Time.deltaTime); // ʹ�ò�ֵƽ������
        }
    }


    public void SetupCrystal(float _crystalExistenceDuration, bool _canExplode, bool _canMove, float _moveSpeed, Transform _targetEnemy)
    {
        crystalExistenceTimer = _crystalExistenceDuration;  //ˮ������ʱ��
        canExplode = _canExplode;   //ˮ���ܷ�ը
        canMove = _canMove; //ˮ���ƶ�
        moveSpeed = _moveSpeed; //ˮ������
        targetEnemy = _targetEnemy; //ˮ������Ŀ��
    }

    //ˮ��Ч��
    public void EndCrystal_ExplodeIfAvailable()
    {
        if (canExplode)
        {
            //�ܱ���ը
            canGrow = true;
            anim.SetTrigger("Explosion");

            //û�н��� ˮ��ǹ  �������CD
            if (!crystalSkill.crystalGunUnlocked)   
            {
                crystalSkill.EnterCooldown();
            }
        }
        else
        {
            crystalSelfDestroy();
        }
    }

    //ˮ���Ա�
    public void crystalSelfDestroy()
    {
        Destroy(gameObject);

        if (!crystalSkill.crystalGunUnlocked)
        {
            crystalSkill.EnterCooldown();
        }
    }

    private void Explosion()
    {
        //ʹ������ϵͳ��ȡ������ˮ���ص���2D��ײ��������ķ�Χ����ײ���뾶
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        //����������ײ��
        foreach (var hit in colliders)
        {
            //�����ײ�������˵������
            if (hit.GetComponent<Enemy>() != null)
            {
                Enemy enemy = hit.GetComponent<Enemy>();

                PlayerManager.instance.player.stats.DoMagicDamage(enemy.GetComponent<CharacterStats>(), transform);

                //ʹ�õ�ǰװ���Ļ���Ч��
               //Inventory.instance.UseCharmEffect_ConsiderCooldown(enemy.transform);
            }
        }
    }


    public void SpecifyEnemyTarget(Transform _enemy)
    {
        targetEnemy = _enemy;
    }

    //public void CrystalChooseRandomEnemy(float _searchRadius)
    //{
    //    Transform originalTargetEnemy = targetEnemy;

    //    targetEnemy = SkillManager.instance.crystal.ChooseRandomEnemy(transform, _searchRadius);

    //    if (targetEnemy == null)
    //    {
    //        Debug.Log("No enemy is chosen" +
    //            "\n will choose original closest enemy");
    //        targetEnemy = originalTargetEnemy;
    //    }
    //}

}


