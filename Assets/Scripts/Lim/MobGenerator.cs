using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobGenerator : MonoBehaviour
{
    public Transform player;
    public InGameApplication gameApp;
    float genDelaySec = 3.0f;
    int mobIdx = 0;
    public Monster[] MobPrefabs;

    float borderHalfWidth = 29.0f;
    float distRange = 20.0f;
    float borderHalfHeight = 17.6f;


    // Start is called before the first frame update
    public void Init()
    {
        StartCoroutine(genMonster());
    }

    // Update is called once per frame
    public void AdvanceTime(float dt_sec)
    {
        
    }

    IEnumerator genMonster()
    {
        while (true)
        {
            Monster mob = Instantiate(MobPrefabs[0], getRandomPosition(), Quaternion.identity);
            mob.player = player;
            mob.speed = 5;
            mob.Init();
            mob.transform.parent = gameObject.transform;
            gameApp.monsters[mobIdx++] =mob;
            yield return new WaitForSeconds(genDelaySec);
        }
    }
    public Vector2 getRandomPosition() {
        float y = Random.Range(-borderHalfHeight, borderHalfHeight);
        float x = Random.Range(borderHalfWidth, borderHalfWidth + distRange);
        if (Random.value < 0.5f) x = -x;
        return new Vector2(x, y);
    }
}
