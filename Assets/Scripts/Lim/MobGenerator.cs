using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MobGenerator : MonoBehaviour
{
    public Transform player;
    public InGameApplication gameApp;
    public float genDelaySec = 3.0f;
    Dictionary<string, Monster> mobPrefabs = new Dictionary<string, Monster>();

    float borderHalfWidth = 29.0f;
    float distRange = 20.0f;
    float borderHalfHeight = 17.6f;


    // Start is called before the first frame update
    public void Init()
    {
        mobPrefabs.Add("Air", Resources.Load<Monster>("Prefabs/Monsters/Air"));
        mobPrefabs.Add("Bat", Resources.Load<Monster>("Prefabs/Monsters/Bat"));
        mobPrefabs.Add("BatSmall", Resources.Load<Monster>("Prefabs/Monsters/BatSmall"));
        Set();
    }

    // Update is called once per frame
    public void AdvanceTime(float dt_sec)
    {
        
    }

    IEnumerator genMonster()
    {
        while (true)
        {
            // 임시
            Monster mob = Instantiate(mobPrefabs.Values.ElementAt(Random.RandomRange(0, 3)), getRandomPosition(), Quaternion.identity);
            mob.player = player;
            mob.coinGenerator = gameApp.coinGenerator;
            //mob.speed = 5;
            mob.Init();
            mob.transform.parent = gameObject.transform;
            gameApp.monsters.Add(mob);
            genDelaySec -= 0.01f;
            yield return new WaitForSeconds(genDelaySec);
        }
    }
    public Vector2 getRandomPosition() {
        float px = player.position.x;
        float y = Random.Range(-borderHalfHeight, borderHalfHeight);
        float x = Random.Range(borderHalfWidth, borderHalfWidth + distRange);
        if (Random.value < 0.5f) x = -x;
        x += px;
        return new Vector2(x, y);
    }
    public void Set()
    {
        StopAllCoroutines();
        StartCoroutine(genMonster());
    }
    public void Dispose()
    {
        StopAllCoroutines();
    }
}
