using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class HPBar_UI : MonoBehaviour
{
    private Entity entity;   //���ø������Entity�������ʾ��ɫʵ��
    private RectTransform barTransform;     //���ڿ���UI��λ��
    private Slider slider;   //����ؼ���������ʾ�͸���Ѫ��

    private CharacterStats myStats;     //���ø������CharacterStats�������ʾ��ɫ��״̬���ݣ�������ֵ��

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();   
        barTransform = GetComponent<RectTransform>(); 
        slider = GetComponentInChildren<Slider>();
        myStats = GetComponentInParent<CharacterStats>();
    }

    //��UI����ʱ��ע�������
    private void OnEnable()
    {
        entity.onFlipped += FlipUI; //ע���ɫ��תʱ���õ��¼�
        myStats.onHealthChanged += UpdateHPUI;  //ע������ֵ�仯ʱ����UI
    }

    //��UI����ʱ������¼�������
    private void OnDisable()
    {
        if (entity != null)
        {
            entity.onFlipped -= FlipUI; //�����ɫ��תʱ���¼�����
        }

        if (myStats != null)
        {
            myStats.onHealthChanged -= UpdateHPUI; //�������ֵ�仯ʱ���¼�����
        }
    }


    private void Start()
    {
        UpdateHPUI();   //�ֶ�����һ��UI
    }

    //����Ѫ��UI������CharacterStats�е����ݸ���Ѫ�������ֵ�͵�ǰֵ
    private void UpdateHPUI()
    {
        slider.maxValue = myStats.getMaxHP(); //����Ѫ�������ֵ
        slider.value = myStats.currentHP; //����Ѫ���ĵ�ǰֵ
    }

    //��תUI
    private void FlipUI()
    {
        barTransform.Rotate(0, 180, 0); //ʹѪ��UI��ת180��
    }

}

