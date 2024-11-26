using System.Collections.Generic;
using UnityEngine;

                                            //���ư�����ͻ,��ʾ������Ϊ���߼���

public class KeybindConflictPromptWindowController : MonoBehaviour
{
    // ���ص���ʾ��ͻ��GameObject���飬ÿ��Ԫ�ش���һ����ͻ��ʾ��Ԫ
    [SerializeField] private GameObject[] keybindConflicts;

    //���ð�����ͻ��ʾ���ڵ����ݡ����ݸ����İ�����KeyCode�������ҳ�ͻ����Ϊ���Ʋ�������ʾ���ݡ�
    public void SetupKeybindConflictPromptWindow(KeyCode _keyCode)
    {
        //��Ӧ������ͻ����
        List<string> _behaveNames = new List<string>();

        // ���������󶨹������еİ������ֵ䣬����������ָ��������ͻ����Ϊ
        foreach (var search in KeyBindManager.instance.keybindsDictionary)
        {
            if (search.Value == _keyCode)
            {
                // ����Ϊ������ӵ��б���
                _behaveNames.Add(search.Key);
            }
        }

        //�����ͻ����Ϊ�������ڻ����2
        if (_behaveNames.Count >= 2)
        {
            // ���������������������ʾ2����ͻ��Ϣ
            int k = 0;

            // ������ͻ��ʾ��Ԫ���飬�����ö�Ӧ����ʾ����
            for (int i = 0; i < keybindConflicts.Length; i++)
            {
                // ��ȡ��Ԫ�Ŀ�������������Ϊ���ƺͰ���
                keybindConflicts[i].GetComponent<KeybindConflictController>()?.SetupKeybindConflict(_behaveNames[k], _keyCode.ToString());
                k++; // ���¼�����

                // ���������2����ͻ��Ϣ��ֹͣ��������
                if (k == 2)
                {
                    return;
                }
            }
        }
    }

    public void ClosePromptWindow()
    {
        // ���ٵ�ǰGameObject���Ƴ�����
        Destroy(gameObject);
    }
}
