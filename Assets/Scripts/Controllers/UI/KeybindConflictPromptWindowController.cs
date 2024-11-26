using System.Collections.Generic;
using UnityEngine;

                                            //控制按键冲突,提示窗口行为和逻辑。

public class KeybindConflictPromptWindowController : MonoBehaviour
{
    // 挂载的提示冲突的GameObject数组，每个元素代表一个冲突显示单元
    [SerializeField] private GameObject[] keybindConflicts;

    //设置按键冲突提示窗口的内容。根据给定的按键（KeyCode），查找冲突的行为名称并更新提示内容。
    public void SetupKeybindConflictPromptWindow(KeyCode _keyCode)
    {
        //对应按键冲突名称
        List<string> _behaveNames = new List<string>();

        // 遍历按键绑定管理器中的按键绑定字典，查找所有与指定按键冲突的行为
        foreach (var search in KeyBindManager.instance.keybindsDictionary)
        {
            if (search.Value == _keyCode)
            {
                // 将行为名称添加到列表中
                _behaveNames.Add(search.Key);
            }
        }

        //如果冲突的行为数量大于或等于2
        if (_behaveNames.Count >= 2)
        {
            // 计数器，用于限制最多显示2个冲突信息
            int k = 0;

            // 遍历冲突提示单元数组，并设置对应的提示内容
            for (int i = 0; i < keybindConflicts.Length; i++)
            {
                // 获取单元的控制器并设置行为名称和按键
                keybindConflicts[i].GetComponent<KeybindConflictController>()?.SetupKeybindConflict(_behaveNames[k], _keyCode.ToString());
                k++; // 更新计数器

                // 如果已设置2个冲突信息，停止后续操作
                if (k == 2)
                {
                    return;
                }
            }
        }
    }

    public void ClosePromptWindow()
    {
        // 销毁当前GameObject，移除窗口
        Destroy(gameObject);
    }
}
