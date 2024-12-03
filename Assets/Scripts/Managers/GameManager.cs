using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IGameProgressionSaveManager
{
    public static GameManager instance;

    private Player player;
    [SerializeField] private Checkpoint[] checkpoints;  //存储游戏中所有Checkpoint对象
    public string lastActivatedCheckpointID { get; set; }

    [Header("掉落货币")]
    [SerializeField] private GameObject deathBodyPrefab;
    public int droppedCurrencyAmount;
    [SerializeField] private Vector2 deathPosition; //记录最后一个被玩家激活的检查点的 ID，通常用于游戏存档或进度保存。

    public List<int> UsedMapElementIDList { get; set; } 

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

        //获取场景中的所有Checkpoint
        checkpoints = FindObjectsOfType<Checkpoint>();

        player = PlayerManager.instance.player;

        //初始化已使用地图元素ID的列表
        UsedMapElementIDList = new List<int>();
    }


    public void RestartScene()
    {
        Scene scene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(scene.name);
    }

    public void PauseGame(bool _pause)
    {
        if (_pause)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    //寻找最近检查点
    private Checkpoint FindClosestActivatedCheckpoint()
    {
                            //初始化最短距离为无穷大，表示初始找不到有效检查点
        float closestDistance = Mathf.Infinity;
        //初始化最靠近的已激活检查点为null
        Checkpoint closestActivatedCheckpoint = null;

        //遍历所有检查点
        foreach (var checkpoint in checkpoints)
        {
            //计算当前检查点与玩家之间的距离
            float distanceToCheckpoint = Vector2.Distance(player.transform.position, checkpoint.transform.position);

            //如果当前检查点距离玩家更近，并且已被激活
            if (distanceToCheckpoint < closestDistance && checkpoint.activated == true)
            {
                //更新最短距离
                closestDistance = distanceToCheckpoint;
                //更新最近的已激活检查点
                closestActivatedCheckpoint = checkpoint;
            }
        }

        //返回最靠近的已激活检查点
        return closestActivatedCheckpoint;
    }

    //死亡掉落金币
    private void LoadDroppedCurrency(GameData _data)
    {
        //加载玩家死亡时掉落货币数量
        droppedCurrencyAmount = _data.droppedCurrencyAmount;
        deathPosition = _data.deathPosition;    //加载玩家死亡位置

        //掉落，则在死亡位置生成遗体，设置相同掉落货币数量
        if (droppedCurrencyAmount > 0)
        {
            GameObject deathBody = Instantiate(deathBodyPrefab, deathPosition, Quaternion.identity);
            deathBody.GetComponent<DroppedCurrencyController>().droppedCurrency = droppedCurrencyAmount;
        }

        droppedCurrencyAmount = 0;  //////////////////////////防止在玩家未死亡时（例如在选择继续游戏时）误生成死亡遗体物体
        ////////////////////////                                清空掉落的货币数量，因为玩家没有掉落任何货币
    }

    //加载检查点
    private void LoadCheckpoints(GameData _data)
    {
        //遍历游戏数据 检查点 字典
        foreach (var search in _data.checkpointsDictionary)
        {
            //遍历检查点
            foreach (var checkpoint in checkpoints)
            {
                //当前检查点的ID与数据中的匹配，激活检查点
                if (checkpoint.checkpointID == search.Key && search.Value == true)
                {
                    checkpoint.ActivateCheckpoint();
                }
            }
        }
    }

    //从游戏数据中加载上次激活的检查点ID
    private void LoadLastActivatedCheckpoint(GameData _data)
    {
        lastActivatedCheckpointID = _data.lastActivatedCheckpointID;
    }

    private void SpawnPlayerAtClosestActivatedCheckpoint(GameData _data)
    {
        //游戏数据中没有最近激活的检查点ID
        if (_data.closestActivatedCheckpointID == null)
        {
            return;
        }

        foreach (var checkpoint in checkpoints)
        {
            //当前检查点的ID与最近激活的检查点ID匹配
            if (_data.closestActivatedCheckpointID == checkpoint.checkpointID)
            {
                //将玩家的位置设置为该检查点的位置
                player.transform.position = checkpoint.transform.position;
            }
        }
    }

    private void SpawnPlayerAtLastActivatedCheckpoint(GameData _data)
    {
        if (_data.lastActivatedCheckpointID == null)
        {
            //游戏数据中没有最后激活的检查点ID，
            return;
        }

        foreach (var checkpoint in checkpoints)
        {
            //当前检查点的ID与上次激活的检查点ID匹配
            if (_data.lastActivatedCheckpointID == checkpoint.checkpointID)
            {
                //将玩家的位置设置为该检查点的位置
                player.transform.position = checkpoint.transform.position;
            }
        }
    }

    //从保存的游戏数据中加载已拾取的地图物品ID列表
    private void LoadPickedUpItemInMapIDList(GameData _data)
    {
        //检查保存的数据中是否包含已使用的地图元素ID列表
        if (_data.UsedMapElementIDList != null)
        {
            foreach (var serach in _data.UsedMapElementIDList)
            {
                //每个ID添加到当前的已使用地图元素ID列表中
                UsedMapElementIDList.Add(serach);
            }
        }
    }


    public void LoadData(GameData _data)
    {
        //加载已丢失的货币数据
        LoadDroppedCurrency(_data);

        //已拾取的物品信息将由ItemObject自动销毁
        LoadPickedUpItemInMapIDList(_data);

        //激活所有保存为已激活的检查点
        LoadCheckpoints(_data);

        //加载最后激活的检查点 ID
        LoadLastActivatedCheckpoint(_data);

        //玩家将在最近激活的检查点处重生
        SpawnPlayerAtLastActivatedCheckpoint(_data);
    }
    public void SaveData(ref GameData _data)
    {
        //保存玩家死亡位置和丢失的货币数量
        _data.droppedCurrencyAmount = droppedCurrencyAmount;
        _data.deathPosition = player.transform.position;

        //清空最近激活的检查点ID
        _data.checkpointsDictionary.Clear();

        //查找最近激活的检查点并保存其ID
        _data.closestActivatedCheckpointID = FindClosestActivatedCheckpoint()?.checkpointID;

        // 遍历所有检查点并保存其激活状态
        foreach (Checkpoint checkpoint in checkpoints)
        {
            _data.checkpointsDictionary.Add(checkpoint.checkpointID, checkpoint.activated);
        }

        //保存最后激活的检查点ID
        _data.lastActivatedCheckpointID = lastActivatedCheckpointID;

        //清空并保存已使用的地图元素ID列表
        _data.UsedMapElementIDList.Clear();
        foreach (var itemID in UsedMapElementIDList)
        {
            _data.UsedMapElementIDList.Add(itemID);
        }
    }

}
