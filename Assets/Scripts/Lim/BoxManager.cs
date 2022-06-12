using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxManager : BaseElement, BaseElement.IBaseController
{
    public Transform player;
    public string[] itemNames;
    public float genDist = 30;

    GroundBox boxPrefab;
    List<GroundItem> itemPrefabs = new List<GroundItem>();
    float maxMoveX = 0;
    float minMoveX = 0;
    float lastGenMaxX = 0;
    float lastGenMinX = 0;

    float borderHalfWidth = 29.0f;
    float borderHalfHeight = 17.6f;
    const float distRange = 20.0f;

    public Vector2 GetRandomPosition(float playerX)
    {
        float y = Random.Range(-borderHalfHeight, borderHalfHeight);
        float x = Random.Range(borderHalfWidth, borderHalfWidth + distRange);
        if (Random.value < 0.5f) x = -x;
        x += playerX;
        return new Vector2(x, y);
    }

    public void Init()
    {
        Camera cam = Camera.main;
        Vector2 hwh = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        borderHalfHeight = -hwh.y;
        borderHalfWidth = -hwh.x;
        boxPrefab = Resources.Load<GroundBox>("Prefabs/Items/Box");
        foreach (string name in itemNames)
        {
            itemPrefabs.Add(Resources.Load<GroundItem>("Prefabs/Items/" + name));
        }
    }

    public void Set()
    {
    }
    public void GenBox(Vector2 playerPos)
    {
        GroundBox box = Instantiate(boxPrefab, GetRandomPosition(playerPos.x), Quaternion.identity);
        int randIdx = Random.Range(0, itemPrefabs.Count);
        box.itemPrefab = itemPrefabs[randIdx];
        box.transform.parent = gameObject.transform;
    }

    public void AdvanceTime(float dt_sec)
    {
        Vector2 playerPos = player.localPosition;
        if (maxMoveX < playerPos.x)
        {
            maxMoveX = playerPos.x;
            if (maxMoveX-lastGenMaxX>genDist)
            {
                lastGenMaxX = maxMoveX;
                GenBox(playerPos);
            }
        } 
        else if(minMoveX> playerPos.x)
        {
            minMoveX = playerPos.x;
            if (lastGenMinX-minMoveX > genDist)
            {
                lastGenMinX = minMoveX;
                GenBox(playerPos);
            }
        }
    }

    public void Dispose()
    {
    }

    public void SetActive(bool flag)
    {
    }
}
