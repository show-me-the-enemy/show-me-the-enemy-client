using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarlicWeapon : IWeapon
{
    float procTime = 0;
    public float interval = 1.1f;
    public float eachDamage = 11f;
    public float size = 1.5f;
    List<Monster> monsters = new List<Monster>();

    public class GarlicLevelInfo : ILevelInfo
    {
        public float eachDamage=11f;
        public float size = 1.5f;
        public GarlicLevelInfo(string discr, float eachDamage, float size)
        {
            this.discription = discr;
            this.eachDamage = eachDamage;
            this.size = size;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Monster")
        {
            monsters.Add(collision.GetComponent<Monster>());
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Monster")
        {
            monsters.Remove(collision.GetComponent<Monster>());
        }
    }

    public override void Attack()
    {
        foreach(Monster m in monsters)
        {
            m.GetDamaged(eachDamage);
        }
    }
    public override void LevelInfoSetup()
    {
        title = "Garlic";
        levelInfos = new GarlicLevelInfo[7];
        levelInfos[0] = new GarlicLevelInfo("The bad smell of garlic continuously damages the surroundings.", 11f, 1.5f);
        levelInfos[1] = new GarlicLevelInfo("The radius of the garlic gets a little bigger", 11f, 1.6f);
        levelInfos[2] = new GarlicLevelInfo("Garlic's attack power gets bigger", 12f, 1.6f);
        levelInfos[3] = new GarlicLevelInfo("The radius of the garlic gets a little bigger", 12f, 1.7f);
        levelInfos[4] = new GarlicLevelInfo("Garlic's attack power gets bigger", 13f, 1.7f);
        levelInfos[5] = new GarlicLevelInfo("The radius of the garlic gets a little bigger", 13f, 1.8f);
        levelInfos[6] = new GarlicLevelInfo("Garlic's attack power gets bigger", 13f, 1.8f);
    }
    public override int AddLevel()
    {
        level++;
        int idx = (level < levelInfos.Length) ? level - 1 : levelInfos.Length - 1;
        GarlicLevelInfo gli = (GarlicLevelInfo)levelInfos[idx];
        size = gli.size;
        eachDamage = gli.eachDamage;
        return level;
    }
    public override string AtUpdate(float dt_sec)
    {
        Vector2 pp = player.localPosition;
        transform.localPosition = pp;
        transform.localScale = new Vector3(size, size, 1);

        procTime += dt_sec;
        if (procTime > interval)
        {
            procTime = 0;
            Attack();
            return "";
        }
        return "";
    }
}
