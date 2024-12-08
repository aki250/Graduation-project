using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    protected Player player;

    public float cooldown;  //������ȴʱ��
    protected float cooldownTimer;  //��ȴ��ʱ��
    public float skillLastUseTime { get; protected set; } = 0;  //�ϴ�ʹ�ü��ܵ�ʱ��

    // �ڼ�������ʱ���г�ʼ��
    protected virtual void Start()
    {
        player = PlayerManager.instance.player;  //��ȡ���ʵ��

        CheckUnlockFromSave();  //�ӱ����м�鼼���Ƿ����
    }

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;  //��ʱ������ʱ
    }

    //��鼼���Ƿ��ѽ���
    protected virtual void CheckUnlockFromSave()
    {

    }

    //��鼼���Ƿ����ʹ��
    public virtual bool SkillIsReadyToUse()
    {
        if (cooldownTimer < 0)  //�����ȴ��ʱ��С��0����ʾ������׼����
        {
            return true;
        }
        else
        {
            //�����������õ�����Ӧ����ʾ�ı�
            if (LanguageManager.instance.localeID == 0)
            {
                player.fx.CreatePopUpText("Skill is in cooldown");  
            }
            else if (LanguageManager.instance.localeID == 1)
            {
                player.fx.CreatePopUpText("������ȴ�У�"); 
            }
            return false;
        }
    }

    //������ܿ���ʹ�ã���ִ�м���
    public virtual bool UseSkillIfAvailable()
    {
        if (cooldownTimer < 0)  //������ܴ�����ȴ״̬
        {
            UseSkill();
            cooldownTimer = cooldown;  //������ȴ��ʱ��
            return true;
        }

        //��������������ʾ�ı�
        if (LanguageManager.instance.localeID == 0)
        {
            player.fx.CreatePopUpText("Skill is in cooldown");  //Ӣ��
        }
        else if (LanguageManager.instance.localeID == 1)
        {
            player.fx.CreatePopUpText("������ȴ�У�");  //����
        }
        return false;
    }

    public virtual void UseSkill()
    {
    }

    //Ѱ������ĵ���
    protected virtual Transform FindClosestEnemy(Transform _searchCenter)
    {
        Transform closestEnemy = null;

        //�������뾶�ڲ������е���
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_searchCenter.position, 12);

        float closestDistanceToEnemy = Mathf.Infinity;  //������˾���

        //Ѱ������ĵ���
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)  //ȷ��Ŀ���ǵ���
            {
                float currentDistanceToEnemy = Vector2.Distance(_searchCenter.position, hit.transform.position);

                //�����ǰ���˵ľ�������������
                if (currentDistanceToEnemy < closestDistanceToEnemy)
                {
                    closestDistanceToEnemy = currentDistanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }

        return closestEnemy;
    }

    //�������뾶�����ѡ��һ������
    protected virtual Transform ChooseRandomEnemy(Transform _searchCenter, float _targetSearchRadius)
    {
        Transform targetEnemy = null;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _targetSearchRadius);

        //���Ұ뾶�ڵĵ���
        List<Transform> enemies = new List<Transform>();

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)  //ȷ��Ŀ���ǵ���
            {
                enemies.Add(hit.transform);
            }
        }

        //����ҵ����ˣ������ѡ��һ����ΪĿ��
        if (enemies.Count > 0)
        {
            targetEnemy = enemies[Random.Range(0, enemies.Count)];
        }

        return targetEnemy;
    }
}
