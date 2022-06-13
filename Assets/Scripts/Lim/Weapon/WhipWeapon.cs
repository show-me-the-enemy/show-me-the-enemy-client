using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipWeapon : IWeapon
{
    public GameObject whipFire;
    float procTime = 0;
    public float interval = 1.0f;
    public float eachDamage = 11f;
    Vector2 fireOffset = new Vector3(2.85f, -0.293f, 0);
    int direction = 0;
    bool isAction = false;

    public class WhipLevelInfo : ILevelInfo
    {
        public float eachDamage = 11f;
        public float interval = 1.1f;
        public int direction = 0; // 0:forward, 1:and backward

        public WhipLevelInfo(string discr, float eachDamage, float interval, int direction = 0)
        {
            this.discription = discr;
            this.eachDamage = eachDamage;
            this.interval = interval;
            this.direction = direction;
        }
    }


    public override void Attack()
    {
        AudioManager.Instance.PlaySFX("Hit"+Random.Range(1,3));
        Vector3 firePos = new Vector3(player.position.x + player.localScale.x * fireOffset.x,
            player.position.y + fireOffset.y, player.position.z);

        Fire whip = Instantiate(whipFire, firePos, Quaternion.identity).GetComponent<Fire>();
        whip.transform.parent = fireRoot;
        whip.GetComponent<SpriteRenderer>().flipX = (player.localScale.x < 0);
        whip.damage = eachDamage;
        if(direction ==1)
        {
            firePos = new Vector3(player.position.x - player.localScale.x * fireOffset.x,
                player.position.y + fireOffset.y, player.position.z);

            whip = Instantiate(whipFire, firePos, Quaternion.identity).GetComponent<Fire>();
            whip.transform.parent = fireRoot;
            whip.GetComponent<SpriteRenderer>().flipX = !(player.localScale.x < 0);
            whip.damage = eachDamage;
        }
    }

    public override string AtUpdate(float dt_sec)
    {
        procTime += dt_sec;
        if(!isAction && procTime > interval - 0.2)
        {
            isAction = true;
            return "Attack";
        }
        else if(procTime > interval)
        {
            isAction = false;
            procTime = 0;
            Attack();
            return "";
        }
        return "";
    }

    public override void LevelInfoSetup()
    {
        title = "Whip";
        levelInfos = new WhipLevelInfo[7];
        levelInfos[0] = new WhipLevelInfo("Swing the whip forward at regular intervals.", 11f, 1.0f);
        levelInfos[1] = new WhipLevelInfo("Increase the attack power of the whip.", 12f, 1.0f);
        levelInfos[2] = new WhipLevelInfo("Increase the speed of the whip attack.", 12f, 0.9f);
        levelInfos[3] = new WhipLevelInfo("Increase the attack power of the whip.", 13f, 0.9f);
        levelInfos[5] = new WhipLevelInfo("Increase the speed of the whip attack.", 13f, 0.86f);
        levelInfos[6] = new WhipLevelInfo("Also swings whip in the back as well", 14f, 0.9f, 1);
        levelInfos[6] = new WhipLevelInfo("Increase the speed of the whip attack.", 14f, 0.86f, 1);
    }

    public override int AddLevel()
    {
        level++;
        int idx = (level < levelInfos.Length) ? level - 1 : levelInfos.Length - 1;
        WhipLevelInfo gli = (WhipLevelInfo)levelInfos[idx];
        eachDamage = gli.eachDamage;
        interval = gli.interval;
        direction = gli.direction;
        return level;
    }
}
