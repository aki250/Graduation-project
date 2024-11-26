using UnityEngine;

public class Arrow_Controller : MonoBehaviour
{
    [SerializeField] private string targetLayerName = "Player";
    //[SerializeField] private int damage; //�����˺�ֵ

    private Vector2 flySpeed;    //�����ٶ�
    private Rigidbody2D rb; //�������
    private CharacterStats archerStats; //�����ֵ����ԣ������˺�����


    [SerializeField] private bool canMove = true;    //���Ƿ��ƶ�

    [SerializeField] private bool flipped = false;  //��ת

    //���Ƿ�ס
    private bool isStuck = false;

    private void Awake()
    {
        //��ʼ���������
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (canMove)
        {
            //�������ٶ�
            rb.velocity = flySpeed;
            //�������ĳ���ʹ��ʼ��������з���
            transform.right = rb.velocity;
        }

        //�����������У�3��5����͸��������
        if (isStuck)
        {
            Invoke("BecomeTransparentAndDestroyArrow", Random.Range(3, 5));
        }

        // ���������ʱ�䳬��10��δ����Ŀ�꣬�Զ�����
        Invoke("BecomeTransparentAndDestroyArrow", 10f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("���� " + collision.gameObject.name + " ��ײ");

        //��������������
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
        {
            if (collision.GetComponent<CharacterStats>() != null)
            {
                //��Ŀ������˺�
                archerStats.DoDamge(collision.GetComponent<CharacterStats>());
                //����Ŀ��������
                StuckIntoCollidedObject(collision);
            }
        }
        //�����е���
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //���ڵ�����
            StuckIntoCollidedObject(collision);
        }
    }

    //��ʼ�����ķ����ٶȺ͹����ֵ�����
    public void SetupArrow(Vector2 _speed, CharacterStats _archerStats)
    {
        flySpeed = _speed;

        //�����������У���Ҫ��ת
        if (flySpeed.x < 0)
        {
            transform.Rotate(0, 180, 0);
        }

        archerStats = _archerStats;
    }

    private void StuckIntoCollidedObject(Collider2D collision)
    {
        //�رռ���β��Ч��
        GetComponentInChildren<ParticleSystem>()?.Stop();

        //��ֹ����ס���ζ�Ŀ������˺�
        GetComponent<CapsuleCollider2D>().enabled = false;

        //��������������
        canMove = false;
        rb.isKinematic = true; //����Ϊ��̬
        rb.constraints = RigidbodyConstraints2D.FreezeAll; //�������
        transform.parent = collision.transform; //��������Ϊ��ײ�����������

        isStuck = true;
    }

    private void BecomeTransparentAndDestroyArrow()
    {
        //��ȡ����SpriteRenderer���
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        //���ټ���͸����
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - (5 * Time.deltaTime));

        //����ȫ͸���������ٶ���
        if (sr.color.a <= 0)
        {
            Destroy(gameObject);
        }
    }

    //��ת���ķ���ʹ�乥�����˶�����ң�
    public void FlipArrow()
    {
        if (flipped)
        {
            return; //����Ѿ���ת�����ظ�����
        }

        //��ת�����ٶȺͼ��ķ���
        flySpeed.x *= -1;
        flySpeed.y *= -1;
        transform.Rotate(0, 180, 0);
        flipped = true;

        targetLayerName = "Enemy";  //�޸�Ŀ���Ϊ"Enemy"
    }
}
