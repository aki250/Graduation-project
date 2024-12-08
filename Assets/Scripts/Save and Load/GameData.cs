using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

[System.Serializable]
public class GameData
{
    //当前玩家的货币数量
    public int currecny;

    //技能树数据，存储技能名与解锁状态
    public SerializableDictionary<string, bool> skillTree;

    //玩家背包物品，存储物品ID与堆叠数量
    public SerializableDictionary<string, int> inventory;

    //玩家装备ID列表
    public List<string> equippedEquipmentIDs;

    //存储过的检查点ID和激活状态
    public SerializableDictionary<string, bool> checkpointsDictionary;

    //最近激活检查点ID
    public string closestActivatedCheckpointID;

    //最后激活检查点ID
    public string lastActivatedCheckpointID;

    [Header("掉落货币数量")]
    //玩家死亡时掉落货币数量
    public int droppedCurrencyAmount;

    //玩家死亡位置
    public Vector2 deathPosition;

    [Header("地图元素")]
    //已使用的地图元素ID列表（已探索的区域，已触发的事件等）
    public List<int> UsedMapElementIDList;

    public bool isNew;

    //构造函数，初始化所有变量默认值
    public GameData()
    {
        //游戏进度相关
        this.currecny = 0;  //初始货币为0
        this.droppedCurrencyAmount = 0;  //死亡时掉落的货币数量默认为0
        this.deathPosition = Vector2.zero;  //玩家死亡时的位置默认为(0,0)

        //初始化已使用地图元素的ID列表
        UsedMapElementIDList = new List<int>();

        //初始化技能树
        skillTree = new SerializableDictionary<string, bool>();

        //初始化背包物品
        inventory = new SerializableDictionary<string, int>();

        //初始化已装备物品ID列表
        equippedEquipmentIDs = new List<string>();

        //初始化检查点字典
        checkpointsDictionary = new SerializableDictionary<string, bool>();

        //初始化最近和最后激活的检查点ID为空
        closestActivatedCheckpointID = string.Empty;
        lastActivatedCheckpointID = string.Empty;
    }
}
