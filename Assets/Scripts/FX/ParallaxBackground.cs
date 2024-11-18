using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;  //主摄像机的引用

    [SerializeField] private float parallaxEffect;  //控制背景的滚动速度

    private float xPosition;  // 背景的x坐标
    private float length;     // 背景的宽度，用于计算视差效果的平移

    private void Start()
    {
        // 查找并赋值主摄像机
        cam = GameObject.Find("Main Camera");

        // 获取背景的初始x坐标
        xPosition = transform.position.x;

        // 获取背景图像的宽度，这里使用了SpriteRenderer来取得背景的尺寸
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        // 根据摄像机的x坐标来计算视差效果的偏移量
        float BGPositionOffset = cam.transform.position.x * (1 - parallaxEffect);

        // 计算背景需要移动的距离，依据摄像机的x坐标和视差因子来调整
        float distanceToMove = cam.transform.position.x * parallaxEffect;

        // 更新背景的位置，将其按视差效果进行移动
        transform.position = new Vector3(xPosition + distanceToMove, transform.position.y);

        // 实现背景的无限循环效果（使背景图像从右侧或左侧重新出现）
        if (BGPositionOffset > xPosition + length)
        {
            // 当背景的右边界超出摄像机位置时，将背景重新定位到右侧
            xPosition += length;
        }
        else if (BGPositionOffset < xPosition - length)
        {
            // 当背景的左边界超出摄像机位置时，将背景重新定位到左侧
            xPosition -= length;
        }
    }
}
