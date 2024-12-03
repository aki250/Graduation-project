using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterimageFX : MonoBehaviour
{
    private SpriteRenderer sr;  //用于显示图像的SpriteRenderer
    private float afterimageColorLosingSpeed;  //控制残影透明度减少的速度

    //设置残影的精灵图和透明度变化速度
    public void SetupAfterImage(Sprite _spriteImage, float _afterimageColorLosingSpeed)
    {
        sr = GetComponent<SpriteRenderer>();  //获取当前物体的 SpriteRenderer 组件
        sr.sprite = _spriteImage;  //设置残影图像为传入的精灵

        afterimageColorLosingSpeed = _afterimageColorLosingSpeed;  //设置透明度减少的速度
    }

    private void Update()
    {
        //透明度减少，逐渐使透明度变为 0
        float alpha = sr.color.a - afterimageColorLosingSpeed * Time.deltaTime;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);  //更新SpriteRenderer颜色（透明度变化）

        if (sr.color.a <= 0)
        {
            Destroy(gameObject);  //移除残影效果
        }
    }
}
