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
            //使用插值方法逐渐增大爆炸的尺寸
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        //当爆炸接近最大尺寸时，停止生长并触发爆炸动画
        if (maxSize - transform.localScale.x < 0.5f)
        {
            canGrow = false; //停止生长
            anim.SetTrigger("Explosion"); //触发爆炸动画
        }
    }

    //初始化爆炸的参数。states,speed,size,radius
    public void SetupExplosion(CharacterStats _shadyStats, float _growSpeed, float _maxSize, float _explosionRadius)
    {
        anim = GetComponent<Animator>();

        shadyStats = _shadyStats;
        growSpeed = _growSpeed;
        maxSize = _maxSize;
        explosionRadius = _explosionRadius;
    }

    //处理爆炸伤害逻辑
    private void Explosion()
    {
        //检测爆炸半径内所有碰撞体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        //遍历检测到的碰撞体，有玩家就炸
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
