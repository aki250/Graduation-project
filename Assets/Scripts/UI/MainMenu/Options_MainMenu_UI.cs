using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Options_MainMenu_UI : MonoBehaviour
{
    //�������ò��������˵�
    public void SaveAndReturnToTitle()
    {
        //����Э�̱������ò��������˵�
        StartCoroutine(SaveSettingsAndReturnToTitle_Coroutine());
    }

    private IEnumerator SaveSettingsAndReturnToTitle_Coroutine()
    {
        //���ñ����������������
        SaveManager.instance.SaveSettings();

        //��0.1�룬ȷ�������ѱ���
        yield return new WaitForSeconds(0.1f);

        //�������˵�����
        SceneManager.LoadScene("MainMenu");
    }
}
