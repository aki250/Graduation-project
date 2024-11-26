using UnityEngine;

public class Arrow_Controller : MonoBehaviour
{
    [SerializeField] private string targetLayerName = "Player";
    //[SerializeField] private int damage; //箭的伤害值

    private Vector2 flySpeed;    //飞行速度
    private Rigidbody2D rb; //刚体组件
    private CharacterStats archerStats; //弓箭手的属性，用于伤害计算


    [SerializeField] private bool canMove = true;    //箭是否移动

    [SerializeField] private bool flipped = false;  //翻转

    //箭是否卡住
    private bool isStuck = false;

    private void Awake()
    {
        //初始化刚体组件
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (canMove)
        {
            //箭飞行速度
            rb.velocity = flySpeed;
            //调整箭的朝向，使其始终面向飞行方向
            transform.right = rb.velocity;
        }

        //箭卡在物体中，3到5秒后变透明并销毁
        if (isStuck)
        {
            Invoke("BecomeTransparentAndDestroyArrow", Random.Range(3, 5));
        }

        // 如果箭飞行时间超过10秒未击中目标，自动销毁
        Invoke("BecomeTransparentAndDestroyArrow", 10f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("箭与 " + collision.gameObject.name + " 碰撞");

        //如果箭击中了玩家
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
        {
            if (collision.GetComponent<CharacterStats>() != null)
            {
                //对目标造成伤害
                archerStats.DoDamge(collision.GetComponent<CharacterStats>());
                //卡在目标物体上
                StuckIntoCollidedObject(collision);
            }
        }
        //箭击中地面
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //卡在地面上
            StuckIntoCollidedObject(collision);
        }
    }

    //初始化箭的飞行速度和弓箭手的属性
    public void SetupArrow(Vector2 _speed, CharacterStats _archerStats)
    {
        flySpeed = _speed;

        //如果箭向左飞行，需要翻转
        if (flySpeed.x < 0)
        {
            transform.Rotate(0, 180, 0);
        }

        archerStats = _archerStats;
    }

    private void StuckIntoCollidedObject(Collider2D collision)
    {
        //关闭箭的尾迹效果
        GetComponentInChildren<ParticleSystem>()?.Stop();

        //防止箭卡住后多次对目标造成伤害
        GetComponent<CapsuleCollider2D>().enabled = false;

        //将箭卡在物体上
        canMove = false;
        rb.isKinematic = true; //刚体为静态
        rb.constraints = RigidbodyConstraints2D.FreezeAll; //冻结刚体
        transform.parent = collision.transform; //将箭设置为碰撞物体的子物体

        isStuck = true;
    }

    private void BecomeTransparentAndDestroyArrow()
    {
        //获取箭的SpriteRenderer组件
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        //减少箭的透明度
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - (5 * Time.deltaTime));

        //箭完全透明，则销毁对象
        if (sr.color.a <= 0)
        {
            Destroy(gameObject);
        }
    }

    //翻转箭的方向（使其攻击敌人而非玩家）
    public void FlipArrow()
    {
        if (flipped)
        {
            return; //如果已经翻转，则不重复操作
        }

        //翻转飞行速度和箭的方向
        flySpeed.x *= -1;
        flySpeed.y *= -1;
        transform.Rotate(0, 180, 0);
        flipped = true;

        targetLayerName = "Enemy";  //修改目标层为"Enemy"
    }
}
