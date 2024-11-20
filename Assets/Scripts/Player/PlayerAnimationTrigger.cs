using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimationTrigger : MonoBehaviour
{
    //��ȡ����Player���
    private Player player => GetComponentInParent<Player>();

    //�»���ײ�������ڴ����»�����ʱ����ײ���
    [SerializeField] private CircleCollider2D downStrikeCollider;

    //����������������Player����е� AnimationTrigger ����
    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    //���������������ڴ����������������˵���ײ
    private void AttackTrigger()
    {
        //�ͷŽ��İ���Ч��������Ƿ���������ͷţ�������ȴʱ�䣩
        Inventory.instance.ReleaseSwordArcane_ConsiderCooldown();

        //��ȡ��ҹ�����Χ�ڵ�������ײ��
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        //�������м�⵽����ײ��������Ƿ��ǵ���
        foreach (var hit in colliders)
        {
            //����ǵ��ˣ�ִ�й����߼�
            if (hit.GetComponent<Enemy>() != null)
            {
                //��ȡ���˵�ͳ�����ݲ�����˺�
                EnemyStats _target = hit.GetComponent<EnemyStats>();
                player.stats.DoDamge(_target);

                //ʹ�ý���Ч����������ȴʱ��
                Inventory.instance.UseSwordEffect_ConsiderCooldown(_target.transform);
            }
        }
    }

    //���з��乥�������������ڷ�����й����������˻���
    private void AirLaunchAttackTrigger()
    {
        //�ͷŽ��İ���Ч��������Ƿ���������ͷ�
        Inventory.instance.ReleaseSwordArcane_ConsiderCooldown();

        //��ȡ��ҹ�����Χ�ڵ�������ײ���������ǵ��ˣ�
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        //�������м�⵽����ײ��������Ƿ��ǵ���
        foreach (var hit in colliders)
        {
            //����ǵ��ˣ�ִ�й����߼�
            if (hit.GetComponent<Enemy>() != null)
            {
                //��ȡ���˲���ʱ�޸�������˶���ʹ�䱻����
                Enemy _enemy = hit.GetComponent<Enemy>();
                Vector2 originalKnockbackMovement = _enemy.knockbackMovement;
                _enemy.knockbackMovement = new Vector2(0, 17); //���õ��˱�����

                //��ȡ���˵�ͳ�����ݲ�����˺�
                EnemyStats _target = hit.GetComponent<EnemyStats>();
                player.stats.DoDamge(_target);

                //������Ļ����Ч
                player.fx.ScreenShake(player.fx.shakeDirection_light);

                //�ָ����˵Ļ����˶�״̬
                _enemy.knockbackMovement = originalKnockbackMovement;

                //ʹ�ý���Ч��
                Inventory.instance.UseSwordEffect_ConsiderCooldown(_target.transform);
            }
        }
    }

    //���»���������ײ��
    private void DownStrikeColliderOpenTrigger()
    {
        downStrikeCollider.gameObject.SetActive(true);
    }

    //�ر��»���������ײ��
    private void DownStrikeColliderCloseTrigger()
    {
        downStrikeCollider.gameObject.SetActive(false);
    }

    //���з�����Ծ��������������ҵĿ��з�����Ծ����
    private void AirLaunchJumpTrigger()
    {
        player.AirLaunchJumpTrigger();
    }

    //�»���������������ҵ��»�����
    private void DownStrikeTrigger()
    {
        player.DownStrikeTrigger();
    }

    //ֹͣ�»��������������������ֹͣ�»������ķ���
    private void DownStrikeAnimStopTrigger()
    {
        player.DownStrikeAnimStopTrigger();
    }

    //Ͷ����������������SkillManager����Ͷ�����ļ��ܷ���
    private void ThrowSword()
    {
        SkillManager.instance.sword.CreateSword();
    }
}
