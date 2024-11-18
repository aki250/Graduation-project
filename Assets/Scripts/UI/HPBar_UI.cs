using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class HPBar_UI : MonoBehaviour
{
    private Entity entity;   //引用父对象的Entity组件，表示角色实体
    private RectTransform barTransform;     //用于控制UI的位置
    private Slider slider;   //滑块控件，用于显示和更新血条

    private CharacterStats myStats;     //引用父对象的CharacterStats组件，表示角色的状态数据（如生命值）

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();   
        barTransform = GetComponent<RectTransform>(); 
        slider = GetComponentInChildren<Slider>();
        myStats = GetComponentInParent<CharacterStats>();
    }

    //在UI启用时，注册监听器
    private void OnEnable()
    {
        entity.onFlipped += FlipUI; //注册角色翻转时调用的事件
        myStats.onHealthChanged += UpdateHPUI;  //注册生命值变化时更新UI
    }

    //在UI禁用时，解除事件监听器
    private void OnDisable()
    {
        if (entity != null)
        {
            entity.onFlipped -= FlipUI; //解除角色翻转时的事件监听
        }

        if (myStats != null)
        {
            myStats.onHealthChanged -= UpdateHPUI; //解除生命值变化时的事件监听
        }
    }


    private void Start()
    {
        UpdateHPUI();   //手动更新一次UI
    }

    //更新血条UI，根据CharacterStats中的数据更新血条的最大值和当前值
    private void UpdateHPUI()
    {
        slider.maxValue = myStats.getMaxHP(); //更新血条的最大值
        slider.value = myStats.currentHP; //更新血条的当前值
    }

    //翻转UI
    private void FlipUI()
    {
        barTransform.Rotate(0, 180, 0); //使血条UI旋转180度
    }

}

