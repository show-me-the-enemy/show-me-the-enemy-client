using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MobGenerator : BaseApplication
{
    public Transform player;
    public InGameApplication gameApp;
    public CoinGenerator coinGenerator;
    Dictionary<string, Monster> mobPrefabs = new Dictionary<string, Monster>();
    public float genDelaySec = 3.0f;
    private float progTime = 0f;
    private int genMobCount = -1;

    const float borderHalfWidth = 29.0f;
    const float distRange = 20.0f;
    const float borderHalfHeight = 17.6f;


    void genMonster()
    {
        // 임시
        Monster mob = Instantiate(mobPrefabs.Values.ElementAt(Random.RandomRange(0, 3)), getRandomPosition(), Quaternion.identity);
        mob.player = player;
        mob.coinGenerator = coinGenerator;
        //mob.speed = 5;
        mob.Init();
        mob.transform.parent = gameObject.transform;
        gameApp.monsters.Add(mob);
        genDelaySec -= 0.01f;
    }
    public Vector2 getRandomPosition() {
        float px = player.position.x;
        float y = Random.Range(-borderHalfHeight, borderHalfHeight);
        float x = Random.Range(borderHalfWidth, borderHalfWidth + distRange);
        if (Random.value < 0.5f) x = -x;
        x += px;
        return new Vector2(x, y);
    }

    public override void Init()
    {
        mobPrefabs.Add("Air", Resources.Load<Monster>("Prefabs/Monsters/Air"));
        mobPrefabs.Add("Bat", Resources.Load<Monster>("Prefabs/Monsters/Bat"));
        mobPrefabs.Add("BatSmall", Resources.Load<Monster>("Prefabs/Monsters/BatSmall"));
    }

    public override void AdvanceTime(float dt_sec)
    {
        if (genMobCount < 0)
            return;
        progTime += dt_sec;
        int curIndex = (int)(progTime / genDelaySec);

        if(genMobCount < curIndex)
        {
            genMobCount++;
            genMonster();
        }
    }

    public override void Set()
    {
        genMobCount = 0;
    }

    public override void Dispose()
    {
        genMobCount = -1;
    }
}
