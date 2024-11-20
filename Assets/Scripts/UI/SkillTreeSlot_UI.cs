using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTreeSlot_UI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IGameProgressionSaveManager
{
    [SerializeField] private int skillPrice;    //���ܼ۸�

    //�������б����������������ض���Ϊ�İ󶨹�ϵ
    [SerializeField] private List<string> boundBehaveNameList;

    //Ӣ����������
    [Header("Ӣ��")]
    //��������Ӣ��
    [SerializeField] private string skillName;
    //��������Ӣ��
    [TextArea]
    [SerializeField] private string skillDescription;

    //������������
    [Header("����")]
    //������������
    [SerializeField] private string skillName_Chinese;
    //������������
    [TextArea]
    [SerializeField] private string skillDescription_Chinese;

    [Space]
    //���ܱ�������ɫ
    [SerializeField] private Color lockedSkillColor;

    //�����Ƿ����
    public bool unlocked;

    //������UI�У��ü��ܽ����Ľ���Ԫ��
    [SerializeField] private SkillTreeSlot_UI[] shouldBeUnlocked;

    //������UI�У��ü��������Ľ���Ԫ��
    [SerializeField] private SkillTreeSlot_UI[] shouldBeLocked;

    //����ͼ��
    private Image skillImage;

    //����UI��ʾ
    private UI ui;

                                              //���������Ըı�ʱ����������ᱻ���á�
    private void OnValidate()
    {
        gameObject.name = $"SkillTreeSlot_UI - {skillName}";
    }

    private void Awake()
    {
        //��ȡ��ǰ�����Image����������޸ļ���ͼ�����ʾЧ��
        skillImage = GetComponent<Image>();

        //��ȡ�������
        ui = GetComponentInParent<UI>();
    }


    private void Start()
    {
        //Ĭ�ϼ���ͼ�����ɫ��������ɫ
        skillImage.color = lockedSkillColor;

        //������ܽ���������ͼ��Ϊ��ɫ
        if (unlocked)
        {
            skillImage.color = Color.white;
        }
    }


    public void UnlockSkill()
    {
        //���ǰ�ü����Ƿ��ѽ���
        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            //�κ�һ��ǰ�ü���δ���������ӡ��ʾ������
            if (shouldBeUnlocked[i].unlocked == false)
            {
                Debug.Log("ǰ�ü�����δ������");
                return;
            }
        }

        //����Ƿ��л��⼼���ѽ���
        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            if (shouldBeLocked[i].unlocked == true)
            {
                Debug.Log("��������ѽ�������ǰ�����޷�����");
                return;
            }
        }

        //ȷ������û���ظ�����
        if (unlocked)
        {
            Debug.Log("���Ѿ��������ü��ܣ�");
            return;
        }

        //�����һ���֧�����ܷ���
        if (PlayerManager.instance.BuyIfAvailable(skillPrice) == false)
        {
            return; 
        }

        unlocked = true; //�����ܱ��Ϊ�ѽ���
        skillImage.color = Color.white; //����ͼ����ɫ
        Debug.Log($"�ɹ���������: {skillName}");
    }

    //�Զ��尴��
    private string AddBehaveKeybindNameToDescription(string _skillDescription)
    {
        //��������Ϊ�����б��滻���������е�ռλ��
        for (int i = 0; i < boundBehaveNameList.Count; i++)
        {
            // ������������а�����Ӧ��ռλ����BehaveName0��BehaveName1 �ȣ�
            if (_skillDescription.Contains($"BehaveName{i}"))
            {
                //��ȡ��Ӧ��Ϊ���Ƶİ����󶨣�ͨ����Ϊ���ƴ��ֵ��л�ȡ��
                string _keybindName = KeyBindManager.instance.keybindsDictionary[boundBehaveNameList[i]].ToString();

                //��׼�����������ƣ����磺��"Mouse0"ת��Ϊ"LMB"�ȣ�
                _keybindName = KeyBindManager.instance.UniformKeybindName(_keybindName);

                //�����������е�ռλ���滻Ϊ��Ӧ�İ���������
                _skillDescription = _skillDescription.Replace($"BehaveName{i}", _keybindName);
            }
        }

        //���ظ��º�ļ�������
        return _skillDescription;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //���㼼��ͼ��λ�õ�ƫ���������ڵ�����ʾ�����ʾλ��
        //����λ��ʱ���������ĸ�ƫ��ֵ�����ҡ��ϡ��¸��Ե�ƫ�ƣ�0.15f��
        Vector2 offset = ui.SetupToolTipPositionOffsetAccordingToUISlotPosition(transform, 0.15f, 0.15f, 0.15f, 0.15f);

        //���¼�����ʾ���λ�ã�������ʾ�ڼ���ͼ���Աߣ�ʹ��ƫ��������΢��
        ui.skillToolTip.transform.position = new Vector2(transform.position.x + offset.x, transform.position.y + offset.y);

        //��ȡ�����������滻���еİ���������
        string completedSkillDescription = AddBehaveKeybindNameToDescription(skillDescription);
        string completedSkillDescription_Chinese = AddBehaveKeybindNameToDescription(skillDescription_Chinese);

        //���ݵ�ǰ����������ʾ��Ӧ�ļ������ƺ�����
        if (LanguageManager.instance.localeID == 0)
        {
            //�����Ӣ�ģ���ʾӢ�����ƺ�����
            ui.skillToolTip.ShowToolTip(skillName, completedSkillDescription, skillPrice.ToString());
        }
        else if (LanguageManager.instance.localeID == 1)
        {
            //��������ģ���ʾ�������ƺ�����
            ui.skillToolTip.ShowToolTip(skillName_Chinese, completedSkillDescription_Chinese, skillPrice.ToString());
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //����뿪����ͼ�꣬���ؼ�����ʾ��
        ui.skillToolTip.HideToolTip();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //���������ͼ�꣬�����ü���
        UnlockSkill();
    }

    public void LoadData(GameData _data)
    {
        //�ӱ������Ϸ�����м��ؼ���������
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            //��Ϸ�����а����ü��ܵ���Ϣ������ظü��ܽ���״̬
            unlocked = value;
        }
    }

    public void SaveData(ref GameData _data)
    {
        //�����ܵĽ���״̬���浽��Ϸ������
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            //��Ϸ�������Ѿ����ڸü��ܣ����������״̬
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, unlocked);
        }
        else
        {
            //��Ϸ������û�иü��ܣ���������������״̬
            _data.skillTree.Add(skillName, unlocked);
        }
    }

}
