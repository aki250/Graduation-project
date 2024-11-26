using System.Collections;
using System.Collections.Generic;
using UnityEngine;


                                            //���Ƽ��� �׻�  ��Ϊ�߼��������ƶ������к��˺�����

public class Skill_ThunderStrikeController : MonoBehaviour
{
    //����Ŀ��״̬����
    [SerializeField] private CharacterStats targetStats;

    //�׻����ƶ�
    [SerializeField] private float thunderMoveSpeed;

    //�����˺�ֵ
    private int damage;

    private Animator anim;

    //�Ƿ��Ѿ����������߼�
    private bool triggered;

    //��ʼ���ű�����������
    private void Awake()
    {
        // ��ȡ�Ӷ����еĶ���������
        anim = GetComponentInChildren<Animator>();
    }


    private void Start()
    {
    }

    private void Update()
    {
        //Ŀ�겻����
        if (targetStats == null)    
        {
            return;
        }

        //�����˾��˳�
        if (triggered)
        {
            return;
        }

        // ʹ������Ŀ��λ���ƶ�
        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, thunderMoveSpeed * Time.deltaTime);

        // ���ü��ܵĳ���ʹ������Ŀ��
        transform.right = transform.position - targetStats.transform.position;

        // ��⼼���Ƿ�ӽ�Ŀ��
        if (Vector2.Distance(transform.position, targetStats.transform.position) < 0.2f)
        {
            triggered = true; // ��Ǽ����Ѵ��������߼�

            // ����������λ�úͷ���
            anim.transform.localPosition = new Vector3(0, 0.5f);
            anim.transform.localRotation = Quaternion.identity;

            // ���ü��ܵ�������ת��������ʾ�ߴ�
            transform.rotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);

            // �����ܹ��ص�Ŀ������ϣ�ȷ��������Ŀ��ͬ���ƶ�
            transform.parent = targetStats.transform;

            // �ӳٴ����˺��������߼�
            Invoke("DamageAndSelfDestroy", 0.25f);

            // �������ж���
            anim.SetTrigger("Hit");
        }
    }

    //���ü��ܵĳ�ʼ������
    public void Setup(int _damage, CharacterStats _targetStats)
    {
        damage = _damage; //�����˺�ֵ
        targetStats = _targetStats; //Ŀ��״̬����
    }

    //��Ŀ������˺������ټ��ܶ���
    private void DamageAndSelfDestroy()
    {
        //Ŀ��������쳣״̬
        targetStats.ApplyShockAilment(true);

        //��Ŀ������˺�
        targetStats.TakeDamage(damage, transform, targetStats.transform, false);

        //�ӳ����ټ��ܶ�����ȷ�����ж����������
        //Destroy(gameObject, 0.4f);
        if (targetStats == null)
        {
            Destroy(gameObject); // Ŀ�궪ʧʱ���ټ���
            return;
        }

    }
}
