using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPanel_InGame_UI : MonoBehaviour
{
    public static SkillPanel_InGame_UI instance;

    public GameObject dashIcon;         //Dashͼ��
    public GameObject parryIcon;        //Parryͼ��
    public GameObject crystalIcon;      //Crystalͼ��
    public GameObject throwSwordIcon;   //Throw Swordͼ��
    public GameObject blackholeIcon;    //Blackhole����ͼ��

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        //���������ʱ�����ݼ��ܽ���״̬��ʾ����ͼ��
        ShowAllSkillIconsAccordingToUnlockState();
    }

    private void Start()
    {
        HideAllSkillIcons();        //�������м���ͼ�꣬ȷ��������ʱ�����������

        // ���ݼ��ܽ���״̬��ʾ����ͼ��
        ShowAllSkillIconsAccordingToUnlockState();

        // �������м���ͼ����ı���Ϣ
        UpdateAllSkillIconTexts();
    }

    // �������м���ͼ����ı�����
    public void UpdateAllSkillIconTexts()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            // ����ÿ�������壬���������Ӧ�ļ���ͼ���ı�
            transform?.GetChild(i)?.GetComponentInChildren<SkillIconText_InGame_UI>()?.UpdateSkillIconText();
        }
    }

    //�������м���ͼ��
    private void HideAllSkillIcons()
    {
        dashIcon.SetActive(false);    
        parryIcon.SetActive(false);     
        crystalIcon.SetActive(false);   
        throwSwordIcon.SetActive(false); 
        blackholeIcon.SetActive(false);  
    }

    //���ݼ��ܽ���״̬��ʾ��Ӧ�ļ���ͼ��
    public void ShowAllSkillIconsAccordingToUnlockState()
    {
        //Dash�ѽ�������ʾDashͼ�꣬��������
        if (SkillManager.instance.dash.dashUnlocked)
        {
            dashIcon.SetActive(true);
        }
        else
        {
            dashIcon.SetActive(false);
        }

        if (SkillManager.instance.parry.parryUnlocked)
        {
            parryIcon.SetActive(true);
        }
        else
        {
            parryIcon.SetActive(false);
        }

        if (SkillManager.instance.crystal.crystalUnlocked)
        {
            crystalIcon.SetActive(true);
        }
        else
        {
            crystalIcon.SetActive(false);
        }

        if (SkillManager.instance.sword.throwSwordSkillUnlocked)
        {
            throwSwordIcon.SetActive(true);
        }
        else
        {
            throwSwordIcon.SetActive(false);
        }

        if (SkillManager.instance.blackhole.blackholeUnlocked)
        {
            blackholeIcon.SetActive(true);
        }
        else
        {
            blackholeIcon.SetActive(false);
        }
    }
}
