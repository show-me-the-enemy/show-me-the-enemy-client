using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MobGenerator : BaseElement, BaseElement.IBaseController
{
    public Transform player;
    public InGameController gameCtrl;
    public CoinGenerator coinGenerator;
    public List<string> mobNames = new List<string>();
    Dictionary<string, Monster> mobPrefabs = new Dictionary<string, Monster>();
    Dictionary<string, int> mobMaxCount = new Dictionary<string, int>();
    Dictionary<string, int> mobGenCount = new Dictionary<string, int>();

    private float progTime = 0f;
    private float roundtTime = 0f;

    float borderHalfWidth = 29.0f;
    float borderHalfHeight = 17.6f;
    const float distRange = 20.0f;


    void genMonster(string mobName)
    {
        // 임시
        Monster mob = Instantiate(mobPrefabs[mobName], getRandomPosition(), Quaternion.identity);
        mob.name = mobName;
        mob.gameCtrl = gameCtrl;
        mob.player = player;
        mob.coinGenerator = coinGenerator;
        //mob.speed = 5;
        mob.Init();
        mob.transform.parent = gameObject.transform;
        gameCtrl.monsters.Add(mob);
    }
    public Vector2 getRandomPosition() {
        float px = player.position.x;
        float y = Random.Range(-borderHalfHeight, borderHalfHeight);
        float x = Random.Range(borderHalfWidth, borderHalfWidth + distRange);
        if (Random.value < 0.5f) x = -x;
        x += px;
        return new Vector2(x, y);
    }

    public void Init()
    {
        Camera cam = Camera.main;
        Vector2 hwh = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        borderHalfHeight = -hwh.y;
        borderHalfWidth = -hwh.x;
        foreach(string mn in mobNames)
        {
            mobPrefabs.Add(mn, Resources.Load<Monster>("Prefabs/Monsters/"+mn));
            mobGenCount.Add(mn, 0);
            mobMaxCount.Add(mn, 0);
            gameCtrl.killMobCount.Add(mn, 0);
            gameCtrl.addMobCount.Add(mn, 0);
        }
    }
    public void AdvanceTime(float dt_sec)
    {
        progTime += dt_sec;
        foreach (string key in mobNames)
        {
            int gc = mobGenCount[key];
            int mc = mobMaxCount[key];
            //Debug.Log(mc);
            int targetCount = Mathf.CeilToInt(mc * progTime / roundtTime);
            if(gc < targetCount)
            {
                mobGenCount[key]++;
                genMonster(key);
            }
        }
    }

    public void Set()
    {
        foreach(string key in mobNames)
        {
            mobGenCount[key] = 0;
        }
        progTime = 0f;
    }
    public void SetRoundTime(float rt)
    {
        roundtTime = rt;
    }
    public void SetMobNum(string name, int count)
    {
        mobMaxCount[name] = count;
    }

    public void Dispose()
    {
    }

    public void SetActive(bool flag)
    {
        throw new System.NotImplementedException();
    }
}
