using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CloneSkillController : MonoBehaviour
{
    private SpriteRenderer sr;  //��ɫSpriteRenderer�����ڿ��ƿ�¡���Ӿ�����
    private Animator anim;  //��ɫAnimator�����ڿ��ƿ�¡�Ķ���

    private float cloneDuration;  //��¡�����ʱ��
    private float cloneTimer;  //��¡���ʱ��
    private float colorLosingSpeed;  //��¡�������ٶ�

    [SerializeField] private Transform attackCheck;  //��⹥������λ��
    [SerializeField] private float attackCheckRadius;  //������ⷶΧ�뾶
    private Transform closestEnemy;  //�������

    private bool canDuplicateClone;  //�Ƿ����ɿ�¡����
    private float duplicatePossibility;  //��¡�������ɵĿ�����

    private bool cloneFacingRight = true;  //��¡���Ƿ������Ҳ�
    private float cloneFacingDirection = 1;  //������1�ң�-1��

    private float cloneAttackDamageMultiplier;  //��¡�幥���˺�����

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>(); 
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;  //ÿ֡���ٿ�¡���ʱ����������ֵ

        if (cloneTimer < 0)
        {
            //��¡������ʼ������ɫ
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLosingSpeed));

            if (sr.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    // ���ÿ�¡����ز���
    public void SetupClone(float _cloneDuration, float _colorLosingSpeed, bool _canAttack, Transform _closestEnemy, bool _canDuplicateClone, float _duplicatePossibility, float _cloneAttackDamageMultiplier)
    {
        if (_canAttack)
        {
            //�����¡�幥������������ù�������
            anim.SetInteger("AttackNumber", Random.Range(1, 4));
        }

        // ���ÿ�¡�ĳ���ʱ�䡢��ʧ�ٶȡ����ˡ���¡�������ɵĿ����ԵȲ���
        cloneDuration = _cloneDuration;
        colorLosingSpeed = _colorLosingSpeed;
        cloneTimer = cloneDuration;  // ���ÿ�¡��ʱ��Ϊ��¡����ʱ��

        closestEnemy = _closestEnemy;  // ��������ĵ���

        // ʹ��¡��������ĵ���
        FaceClosestTarget();

        canDuplicateClone = _canDuplicateClone;  // �Ƿ�������ɸ���
        duplicatePossibility = _duplicatePossibility;  // �������ɸ���
        cloneAttackDamageMultiplier = _cloneAttackDamageMultiplier;  // ��¡�����˺�����
    }

    // ��������������ֹͣ��¡
    private void AnimationTrigger()
    {
        cloneTimer = -0.1f;  // ����������¡��ʱ
    }

    // ���������¼�
    private void AttackTrigger()
    {
        // ���ѧ���˼����Ļ����ܣ���¡����ʱҲ��Ӧ�û���Ч��
        if (SkillManager.instance.clone.aggressiveCloneCanApplyOnHitEffect)
        {
            Inventory.instance.ReleaseSwordArcane_ConsiderCooldown();  // �ͷŽ��İ������ܣ�����еĻ���
        }

        // ��⹥����Χ�ڵ�������ײ�壨���ˣ�
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
        {
            // �����ײ���ǵ���
            if (hit.GetComponent<Enemy>() != null)
            {
                Enemy enemy = hit.GetComponent<Enemy>();  // ��ȡ����

                // ��ȡ��ҵ����ԣ����ڼ����˺���
                PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

                // ��¡���˺�Ӧ�ñ���ұ�����˺�С
                if (playerStats != null)
                {
                    playerStats.CloneDoDamage(enemy.GetComponent<CharacterStats>(), cloneAttackDamageMultiplier, transform);  // ��¡�����˺�
                }

                // ���ѧ���˼������󣬿�¡����ʱҲ��Ӧ�û���Ч��
                if (SkillManager.instance.clone.aggressiveCloneCanApplyOnHitEffect)
                {
                    Inventory.instance.UseSwordEffect_ConsiderCooldown(enemy.transform);  // �������ļ���Ч��
                }

                // ����������ɸ���
                if (canDuplicateClone)
                {
                    // ��������Ƿ��ڵ��˸�������һ������
                    if (Random.Range(0, 100) < duplicatePossibility && SkillManager.instance.clone.currentDuplicateCloneAmount < SkillManager.instance.clone.maxDuplicateCloneAmount)
                    {
                        // �ڵ����Ա����λ�ô�������
                        SkillManager.instance.clone.CreateDuplicateClone(new Vector3(hit.transform.position.x + 1f * cloneFacingDirection, hit.transform.position.y));
                    }
                }
            }
        }
    }

    // ʹ��¡��������ĵ���
    private void FaceClosestTarget()
    {
        if (closestEnemy != null)
        {
            // �����¡�ڵ����ұߣ���ת��¡�泯����
            if (transform.position.x > closestEnemy.position.x)
            {
                CloneFlip();
            }
        }
    }

    // ��ת��¡���泯����
    private void CloneFlip()
    {
        transform.Rotate(0, 180, 0);  // ��ת����180��

        cloneFacingRight = !cloneFacingRight;  // �л��泯����
        cloneFacingDirection = -cloneFacingDirection;  // �ı��¡�泯�������ֵ��-1Ϊ��1Ϊ�ң�
    }
}
