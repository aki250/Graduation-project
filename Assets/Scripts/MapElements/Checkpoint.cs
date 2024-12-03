using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class Checkpoint : MonoBehaviour
{
    private Animator anim;

    public string checkpointID;  //����Ψһ��ʶ��

    public bool activated;  //��Ǽ����Ƿ񼤻�

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ��ײ������Player���򼤻�ü��㲢������Ϸ״̬
        if (collision.GetComponent<Player>() != null)
        {
            ActivateCheckpoint();
            SaveManager.instance.SaveGame();  //������Ϸ״̬
        }
    }

    public void ActivateCheckpoint()
    {
                            //������Ϸ�������е���󼤻�ļ���ID
        GameManager.instance.lastActivatedCheckpointID = checkpointID;

        //���ż��㼤����Ч
        AudioManager.instance.PlaySFX(5, transform);

        if (activated)
        {
            return;
        }

        anim.SetBool("Active", true);
        activated = true;  //����״̬����Ϊ�Ѽ���
    }

    [ContextMenu("���ɼ���ID")]
    private void GenerateCheckpointID()
    {
        //ʹ��GUID����Ψһ�ļ���ID��ȷ��ÿ�����㶼�в�ͬ�ı�ʶ��
        checkpointID = System.Guid.NewGuid().ToString();
    }
}
