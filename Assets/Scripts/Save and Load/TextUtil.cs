/********************************************************************
	created:	2024/11/24
	created:	24:11:2024   20:45
	file ext:	cs
	author:		WilhelmLiu
	call:       1367296274@qq.com
	purpose:	字符串处理工具类
*********************************************************************/
using System;
using System.Text;
using UnityEngine.Events;

public enum E_SymbolType
{
    Comma,  //逗号,，
    Semicolon,  //分号;；
    Colon,  //冒号:：
    VerBar, //竖杠|
	HorBar, //横杠-
    Underline,  //下划线_
    BlankSpace, //空格
    WellNum,    //井号＃
}

public class TextUtil
{
    private static StringBuilder resultStr = new StringBuilder("");

    #region 字符串拆分相关
    public static string[] SplitStr(string str, E_SymbolType type)
	{
		if(string.IsNullOrEmpty(str))
		{
			return new string[0];
		}
		string newStr = str;
		switch (type)
		{
			case E_SymbolType.Comma:
				while(newStr.IndexOf("，") != -1)
				{
					newStr.Replace("，", ",");
				}
				return newStr.Split(',');
			case E_SymbolType.Semicolon:
                while (newStr.IndexOf("；") != -1)
                {
                    newStr.Replace("；", ";");
                }
                return newStr.Split(';');
			case E_SymbolType.Colon:
                while (newStr.IndexOf("：") != -1)
                {
                    newStr.Replace("：", ":");
                }
                return newStr.Split(':');
			case E_SymbolType.VerBar:
                return newStr.Split('|');
            case E_SymbolType.HorBar:
                return newStr.Split('-');
			case E_SymbolType.Underline:
                return newStr.Split('_');
			case E_SymbolType.BlankSpace:
                return newStr.Split(' ');
            case E_SymbolType.WellNum:
                return newStr.Split('#');
            default:
				break;
		}
		return new string[0];
	}

    public static int[] SplitStrToIntArr(string str, E_SymbolType type)
	{
		string[] strs = SplitStr(str, type);
		if (strs.Length == 0)
		{
			return new int[0];
		}
		return Array.ConvertAll<string, int>(strs, str => int.Parse(str));
    }

    public static void SplitStrToIntArrTwice(string str, E_SymbolType typeOne, E_SymbolType typeTwo, UnityAction<int, int> callBack)
	{
        string[] strs = SplitStr(str, typeOne);
        if (strs.Length == 0)
        {
			return;
        }
        int[] ints;
        for (int i = 0; i < strs.Length; i++)
		{
            ints = SplitStrToIntArr(strs[i], typeTwo);
			if(ints.Length < 2)
			{
				continue;
			}
			callBack?.Invoke(ints[0], ints[1]);
        }
    }

    public static void SplitStrTwice(string str, E_SymbolType typeOne, E_SymbolType typeTwo, UnityAction<string, string> callBack)
	{
        string[] strs = SplitStr(str, typeOne);
        if (strs.Length == 0)
        {
            return;
        }
        string[] strs2;
        for (int i = 0; i < strs.Length; i++)
        {
            strs2 = SplitStr(strs[i], typeTwo);
            if (strs2.Length < 2)
            {
                continue;
            }
            callBack?.Invoke(strs2[0], strs2[1]);
        }
    }

    public static void SplitStrTwice(string str, E_SymbolType typeOne, E_SymbolType typeTwo, UnityAction<string, string, string> callBack)
    {
        string[] strs = SplitStr(str, typeOne);
        if (strs.Length == 0)
        {
            return;
        }
        string[] strs2;
        for (int i = 0; i < strs.Length; i++)
        {
            strs2 = SplitStr(strs[i], typeTwo);
            if (strs2.Length < 3)
            {
                continue;
            }
            callBack?.Invoke(strs2[0], strs2[1], strs2[2]);
        }
    }

    #endregion

}
