using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwordSkillController : MonoBehaviour
{
    private Animator anim;  //������
    private Rigidbody2D rb; //������
    private CircleCollider2D cd;    //����ײ��
    private Player player;

    private bool canRotate = true;  //���ƽ��Ƿ���ת
    private bool isReturning;   //�����������
    private float swordReturnSpeed; //�����ٶ�
    private Vector2 launchSpeed; //�����ٶȣ���Ч����

    private float enemyFreezeDuration;  //�������ʱ��
    private float enemyVulnerableDuration;  //���˴���״̬����ʱ��

    [Header("��������Ϣ")]
    private bool isBouncingSword;   //��ǰ�Ƿ�Ϊ������
    private int bounceAmount;   //��������
    private float bounceSpeed;  //�����ٶ�
    private List<Transform> bounceTargets = new List<Transform>();  //��¼����Ŀ��
    private int bounceTargetIndex;  //��ǰ��������

    [Header("���̽���Ϣ")]
    private bool isPierceSword; //��ǰ�Ƿ񴩴̽�
    private int pierceAmount;   //�����̴���

    [Header("��ת�������Ϣ")]
    private bool isSpinSword;   //��ǰ�Ƿ���ת��
    private float maxTravelDistance;    //�����о���
    private float spinDuration; //��תʱ��
    private float spinTimer;    //��ת��ʱ
    private bool wasStopped;    //��ת�Ƿ�ֹͣ


    private float spinHitCooldown;  //��ת������Ŀ����ȴʱ��
    private float spinHitTimer; //��ǰ��ȴ��ʱ��

    private bool spinTimerHasBeenSetToSpinDuration = false;     //�Ƿ���������ת��ʱ��
    private float spinDirection;    //����ת����


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
        player = PlayerManager.instance.player;
    }

    private void Update()
    {
        //����ת����Ϊ��ǰ�ƶ�����
        if (canRotate)
        {
            transform.right = rb.velocity;
        }
        //���ս�����
        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, swordReturnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 1.5)
            {
                player.CatchSword(); //��ס�����ٽ�
            }
        }

        BounceSwordLogic();

        SpinSwordLogic();

        DestroySwordIfTooFar(30);   //����̫Զ������
    }

    //ֹͣ���ƶ�����ת
    private void StopAndSpin()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition; //ֹͣλ��

        if (!spinTimerHasBeenSetToSpinDuration)
        {
            spinTimer = spinDuration;
        }

        spinTimerHasBeenSetToSpinDuration = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //���ڻ������򲻴����κ��߼�
        if (isReturning)
        {
            return;
        }

        //�����ײ������
        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
        }


        //��ת�����е��˺�ֹͣ�ƶ�����ʼ��ת
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

    //��ײ���ĵ��˹��϶��ᡢ����״̬
    private void DamageAndFreezeAndVulnerateEnemy(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            //�Ե�������˺�
            player.stats.DoDamge(enemy.GetComponent<CharacterStats>());

            //����������Ἴ�ܣ��������
            if (SkillManager.instance.sword.timeStopUnlocked)
            {
                enemy.FreezeEnemyForTime(enemyFreezeDuration);

            }

            //��������������ܣ�ʹ���˽������״̬
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
        //���̽������ٴ��̴���
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        //��ת�������к�ֹͣ�˶�������ת
        if (isSpinSword && collision.GetComponent<Enemy>() != null)
        {
            StopAndSpin();
            return;
        }

        canRotate = false;  //ֹͣ��ת
        cd.enabled = false; //������ײ��

        rb.isKinematic = true;  //����������Ӱ��
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        //������������Ŀ�꣬������
        if (isBouncingSword && bounceTargets.Count > 0)
        {
            return;
        }

        anim.SetBool("Rotation", false);    //ֹͣ����
        transform.parent = collision.transform; //

        //������Ч
        ParticleSystem dustFX = GetComponentInChildren<ParticleSystem>();
        if (dustFX != null)
        {
            if (launchSpeed.x < 0)
            {
                dustFX.transform.localScale = new Vector3(-1, 1, 1);    //��ת��Ч
            }

            dustFX.Play();  //������Ч
        }
    }

    private void BounceSwordLogic()
    {
        //���ǵ�����������Ŀ���б�
        if (isBouncingSword && bounceTargets.Count > 0)
        {
            //Debug.Log("��ʼ����");
            //����λ����Ŀ��λ�÷�ȥ
            transform.position = Vector2.MoveTowards(transform.position, bounceTargets[bounceTargetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, bounceTargets[bounceTargetIndex].position) < 0.15f)
            {

                DamageAndFreezeAndVulnerateEnemy(bounceTargets[bounceTargetIndex].GetComponent<Collider2D>());
                //�������������ٵ�������
                bounceTargetIndex++;
                bounceAmount--;
                
                if (bounceAmount <= 0)
                {
                    isBouncingSword = false;
                    isReturning = true;
                }
                //Ŀ�����������б��ȣ����á�
                if (bounceTargetIndex >= bounceTargets.Count)
                {
                    bounceTargetIndex = 0;
                }
            }
        }
    }

    //��ȡ����������Ŀ��
    private void SetupBounceSwordTargets(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            //�б�Ϊ��
            if (isBouncingSword && bounceTargets.Count <= 0)
            {
                //��ȡ�����뽣λ���ص�����ײ����
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    //����ײ�����ӵ������
                    if (hit.GetComponent<Enemy>() != null)
                    {
                        bounceTargets.Add(hit.transform);   //�����˱任�����ӵ����б���
                    }
                }
            }

            //// ��������ҵľ���Ե���Ŀ���б��������
            bounceTargets.Sort(new SortByDistanceToPlayer_BounceSwordTargets());
        }
    }

    private void SpinSwordLogic()
    {
        if (isSpinSword)
        {
            //���ﵽ��Զ���о��룬��û��ֹͣ��
            if (Vector2.Distance(player.transform.position, transform.position) >= maxTravelDistance && !wasStopped)
            {
                StopAndSpin();  //ֹͣ��ת
            }
            //���Ѿ�ֹͣ
            if (wasStopped)
            {
                //������ת��ʱ������ת���м�ʱ��
                spinTimer -= Time.deltaTime;
                spinHitTimer -= Time.deltaTime;

                //��ת��������ˣ����������תģʽ
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);

                //��ת��ʱ��ʱ�䵽
                if (spinTimer < 0)
                {
                    isReturning = true;  
                    isSpinSword = false;
                }

                //��ת���м�ʱ��ʱ�䵽
                if (spinHitTimer < 0)
                {   
                    //������ת���м�ʱ��
                    spinHitTimer = spinHitCooldown;

                    //��ȡ�����뽣�ص�������ײ������ΧΪ1��Բ
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)  //��ײ�������˵������
                        {
                            DamageAndFreezeAndVulnerateEnemy(hit);  //�Ե���������˲�����
                        }
                    }
                }
            }
        }
    }

    public void SetupSword(Vector2 _launchSpeed, float _swordGravity, float _swordReturnSpeed, float _enemyFreezeDuration, float _enemyVulnerableDuration)
    {
        rb.velocity = _launchSpeed; //���ĳ�ʼ�ٶ�

        rb.gravityScale = _swordGravity;//��������Ӱ��ϵ��

        swordReturnSpeed = _swordReturnSpeed; //������ʱ�ٶ�

        enemyFreezeDuration = _enemyFreezeDuration;//���˱�����ĳ���ʱ��

        enemyVulnerableDuration = _enemyVulnerableDuration; //���õ��˱��������ĳ���ʱ��

        launchSpeed = _launchSpeed; //���淢���ٶ�

        //�����Ǵ��̽�����ʼ��ת����
        if (!isPierceSword)
        {
            anim.SetBool("Rotation", true);
        }

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);  //���ݽ����ٶ�������ת����

    }

    public void SetupBounceSword(bool _isBounceSword, int _bounceAmount, float _bounceSpeed)
    {
        isBouncingSword = _isBounceSword;   //���ý���������
        bounceAmount = _bounceAmount;   //���õ�������
        bounceSpeed = _bounceSpeed; //���õ����ٶ�
    }

    public void SetupPierceSword(bool _isPierceSword, int _pierceAmount)
    {
        isPierceSword = _isPierceSword; //���ô�������
        pierceAmount = _pierceAmount;   //���ô��̴���
    }

    public void SetupSpinSword(bool _isSpinSword, float _maxTravelDistance, float _spinDuration, float _spinHitCooldown)
    {
        isSpinSword = _isSpinSword; //���ý��Ƿ������ת����
        maxTravelDistance = _maxTravelDistance;     //���ý���������о���
        spinDuration = _spinDuration;   //������ת����ʱ��
        spinHitCooldown = _spinHitCooldown; //������תʱ���е��˵���ȴʱ��
    }

    public void ReturnSword()
    {
        //��������ڵ���֮�䵯�������ܷ���
        if (bounceTargets.Count > 0)
        {
            return;
        }


        rb.constraints = RigidbodyConstraints2D.FreezeAll;  //������壬������������Ӱ�죬��ȫֹͣ���嶯̬��Ϊ
        // rb.isKinematic = false; //ͨ���ű������˶������屾��������Ӱ��

        transform.parent = null;    //�����ĸ���������Ϊnull��ʹ�䲻�ٸ���֮ǰ�Ķ���
        isReturning = true; //���ķ��ر�־Ϊ��
    }

    private void DestroySwordIfTooFar(float _maxDistance)
    {
        //�������֮��ľ�������趨�������룬������
        if (Vector2.Distance(player.transform.position, transform.position) >= _maxDistance)
        {
            Destroy(gameObject);
        }
    }
}
