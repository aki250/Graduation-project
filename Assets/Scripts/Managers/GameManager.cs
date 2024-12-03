using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IGameProgressionSaveManager
{
    public static GameManager instance;

    private Player player;
    [SerializeField] private Checkpoint[] checkpoints;  //�洢��Ϸ������Checkpoint����
    public string lastActivatedCheckpointID { get; set; }

    [Header("�������")]
    [SerializeField] private GameObject deathBodyPrefab;
    public int droppedCurrencyAmount;
    [SerializeField] private Vector2 deathPosition; //��¼���һ������Ҽ���ļ���� ID��ͨ��������Ϸ�浵����ȱ��档

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

        //��ȡ�����е�����Checkpoint
        checkpoints = FindObjectsOfType<Checkpoint>();

        player = PlayerManager.instance.player;

        //��ʼ����ʹ�õ�ͼԪ��ID���б�
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

    //Ѱ���������
    private Checkpoint FindClosestActivatedCheckpoint()
    {
                            //��ʼ����̾���Ϊ����󣬱�ʾ��ʼ�Ҳ�����Ч����
        float closestDistance = Mathf.Infinity;
        //��ʼ��������Ѽ������Ϊnull
        Checkpoint closestActivatedCheckpoint = null;

        //�������м���
        foreach (var checkpoint in checkpoints)
        {
            //���㵱ǰ���������֮��ľ���
            float distanceToCheckpoint = Vector2.Distance(player.transform.position, checkpoint.transform.position);

            //�����ǰ���������Ҹ����������ѱ�����
            if (distanceToCheckpoint < closestDistance && checkpoint.activated == true)
            {
                //������̾���
                closestDistance = distanceToCheckpoint;
                //����������Ѽ������
                closestActivatedCheckpoint = checkpoint;
            }
        }

        //����������Ѽ������
        return closestActivatedCheckpoint;
    }

    //����������
    private void LoadDroppedCurrency(GameData _data)
    {
        //�����������ʱ�����������
        droppedCurrencyAmount = _data.droppedCurrencyAmount;
        deathPosition = _data.deathPosition;    //�����������λ��

        //���䣬��������λ���������壬������ͬ�����������
        if (droppedCurrencyAmount > 0)
        {
            GameObject deathBody = Instantiate(deathBodyPrefab, deathPosition, Quaternion.identity);
            deathBody.GetComponent<DroppedCurrencyController>().droppedCurrency = droppedCurrencyAmount;
        }

        droppedCurrencyAmount = 0;  //////////////////////////��ֹ�����δ����ʱ��������ѡ�������Ϸʱ��������������������
        ////////////////////////                                ��յ���Ļ�����������Ϊ���û�е����κλ���
    }

    //���ؼ���
    private void LoadCheckpoints(GameData _data)
    {
        //������Ϸ���� ���� �ֵ�
        foreach (var search in _data.checkpointsDictionary)
        {
            //��������
            foreach (var checkpoint in checkpoints)
            {
                //��ǰ�����ID�������е�ƥ�䣬�������
                if (checkpoint.checkpointID == search.Key && search.Value == true)
                {
                    checkpoint.ActivateCheckpoint();
                }
            }
        }
    }

    //����Ϸ�����м����ϴμ���ļ���ID
    private void LoadLastActivatedCheckpoint(GameData _data)
    {
        lastActivatedCheckpointID = _data.lastActivatedCheckpointID;
    }

    private void SpawnPlayerAtClosestActivatedCheckpoint(GameData _data)
    {
        //��Ϸ������û���������ļ���ID
        if (_data.closestActivatedCheckpointID == null)
        {
            return;
        }

        foreach (var checkpoint in checkpoints)
        {
            //��ǰ�����ID���������ļ���IDƥ��
            if (_data.closestActivatedCheckpointID == checkpoint.checkpointID)
            {
                //����ҵ�λ������Ϊ�ü����λ��
                player.transform.position = checkpoint.transform.position;
            }
        }
    }

    private void SpawnPlayerAtLastActivatedCheckpoint(GameData _data)
    {
        if (_data.lastActivatedCheckpointID == null)
        {
            //��Ϸ������û����󼤻�ļ���ID��
            return;
        }

        foreach (var checkpoint in checkpoints)
        {
            //��ǰ�����ID���ϴμ���ļ���IDƥ��
            if (_data.lastActivatedCheckpointID == checkpoint.checkpointID)
            {
                //����ҵ�λ������Ϊ�ü����λ��
                player.transform.position = checkpoint.transform.position;
            }
        }
    }

    //�ӱ������Ϸ�����м�����ʰȡ�ĵ�ͼ��ƷID�б�
    private void LoadPickedUpItemInMapIDList(GameData _data)
    {
        //��鱣����������Ƿ������ʹ�õĵ�ͼԪ��ID�б�
        if (_data.UsedMapElementIDList != null)
        {
            foreach (var serach in _data.UsedMapElementIDList)
            {
                //ÿ��ID��ӵ���ǰ����ʹ�õ�ͼԪ��ID�б���
                UsedMapElementIDList.Add(serach);
            }
        }
    }


    public void LoadData(GameData _data)
    {
        //�����Ѷ�ʧ�Ļ�������
        LoadDroppedCurrency(_data);

        //��ʰȡ����Ʒ��Ϣ����ItemObject�Զ�����
        LoadPickedUpItemInMapIDList(_data);

        //�������б���Ϊ�Ѽ���ļ���
        LoadCheckpoints(_data);

        //������󼤻�ļ��� ID
        LoadLastActivatedCheckpoint(_data);

        //��ҽ����������ļ��㴦����
        SpawnPlayerAtLastActivatedCheckpoint(_data);
    }
    public void SaveData(ref GameData _data)
    {
        //�����������λ�úͶ�ʧ�Ļ�������
        _data.droppedCurrencyAmount = droppedCurrencyAmount;
        _data.deathPosition = player.transform.position;

        //����������ļ���ID
        _data.checkpointsDictionary.Clear();

        //�����������ļ��㲢������ID
        _data.closestActivatedCheckpointID = FindClosestActivatedCheckpoint()?.checkpointID;

        // �������м��㲢�����伤��״̬
        foreach (Checkpoint checkpoint in checkpoints)
        {
            _data.checkpointsDictionary.Add(checkpoint.checkpointID, checkpoint.activated);
        }

        //������󼤻�ļ���ID
        _data.lastActivatedCheckpointID = lastActivatedCheckpointID;

        //��ղ�������ʹ�õĵ�ͼԪ��ID�б�
        _data.UsedMapElementIDList.Clear();
        foreach (var itemID in UsedMapElementIDList)
        {
            _data.UsedMapElementIDList.Add(itemID);
        }
    }

}
