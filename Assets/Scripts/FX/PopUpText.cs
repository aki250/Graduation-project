using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;

public class PopUpText : MonoBehaviour
{
    private TextMeshPro myText;

    [SerializeField] private float appearingSpeed;  //出现速度
    [SerializeField] private float disappearingSpeed;   //消失速度
    [SerializeField] private float colorLosingSpeed;    //褪色速度

    [SerializeField] private float lifeTime;    //文本小时前持续时间
    private float textTimer;    //剩余时间

    private void Awake()
    {
        myText = GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        textTimer = lifeTime;   //初始化文本生存时间计时器
    }

    private void Update()
    {
        textTimer -= Time.deltaTime;

        //文本出现时，的动画，逐步向上移动，形成飘升效果
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + 1), appearingSpeed * Time.deltaTime);

        //<0则开始消失
        if (textTimer < 0)
        {
            //文本颜色alpha值逐渐减少，褪色效果
            float alpha = myText.color.a - colorLosingSpeed * Time.deltaTime;

            //更新文本颜色，RGB不变，只减少alpha透明度
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, alpha);

            // 当alpha小于50，文本开始向下移动，切换消失的动画
            if (myText.color.a < 50)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + 1), disappearingSpeed * Time.deltaTime);
            }

            if (myText.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }

    }
}
