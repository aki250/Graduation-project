using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Options_MainMenu_UI : MonoBehaviour
{
    //保存设置并返回主菜单
    public void SaveAndReturnToTitle()
    {
        //启动协程保存设置并返回主菜单
        StartCoroutine(SaveSettingsAndReturnToTitle_Coroutine());
    }

    private IEnumerator SaveSettingsAndReturnToTitle_Coroutine()
    {
        //调用保存管理器保存设置
        SaveManager.instance.SaveSettings();

        //等0.1秒，确保设置已保存
        yield return new WaitForSeconds(0.1f);

        //加载主菜单场景
        SceneManager.LoadScene("MainMenu");
    }
}
