using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapElement : MonoBehaviour
{
    [Header("地图元素")]
    public bool isMapElementThatCannotReuse;
    [Tooltip("每个地图元素ID都是唯一")]
    public int mapElementID;

    protected virtual void Start()
    {
        DestroySelfIfHasBeenUsed();
    }

    private void DestroySelfIfHasBeenUsed()
    {
        if (isMapElementThatCannotReuse)
        {
            foreach (var id in GameManager.instance.UsedMapElementIDList)
            {
                if (mapElementID == id)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
