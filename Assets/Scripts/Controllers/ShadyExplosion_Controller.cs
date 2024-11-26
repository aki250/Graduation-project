using UnityEngine;

public class ShadyExplosion_Controller : MonoBehaviour
{
    private Animator anim;
    private CharacterStats shadyStats;
    private float growSpeed;
    private float maxSize;
    private float explosionRadius;

    private bool canGrow = true;

    private void Update()
    {
        if (canGrow)
        {
            //ʹ�ò�ֵ����������ը�ĳߴ�
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        //����ը�ӽ����ߴ�ʱ��ֹͣ������������ը����
        if (maxSize - transform.localScale.x < 0.5f)
        {
            canGrow = false; //ֹͣ����
            anim.SetTrigger("Explosion"); //������ը����
        }
    }

    //��ʼ����ը�Ĳ�����states,speed,size,radius
    public void SetupExplosion(CharacterStats _shadyStats, float _growSpeed, float _maxSize, float _explosionRadius)
    {
        anim = GetComponent<Animator>();

        shadyStats = _shadyStats;
        growSpeed = _growSpeed;
        maxSize = _maxSize;
        explosionRadius = _explosionRadius;
    }

    //����ը�˺��߼�
    private void Explosion()
    {
        //��ⱬը�뾶��������ײ��
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        //������⵽����ײ�壬����Ҿ�ը
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                shadyStats.DoDamge(hit.GetComponent<PlayerStats>());
            }

            if (hit.GetComponent<Enemy>() != null)
            {
                if (hit.gameObject == gameObject)
                {
                    continue;
                }

                shadyStats.DoDamge(hit.GetComponent<EnemyStats>());
            }
        }
    }

    public void ShadyChangeToDeathState()
    {
        shadyStats.GetComponent<Shady>()?.Die();
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
