using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerWeapon : IWeapon
{
    public GameObject daggerFire;
    float procTime = 0;
    public float interval = 1.1f;
    public float eachDamage = 11f;
    float fireOffset = 1f;
    public float fireSpeed = 10f;
    public float lifeTime = 5f;
    public int direction = 0;

    public class DaggerLevelInfo : ILevelInfo
    {
        public float eachDamage = 11f;
        public float fireSpeed = 1.5f;
        public float interval = 1.1f;
        public int direction = 0; // 0:forward, 1:2fire

        public DaggerLevelInfo(string discr, float eachDamage, float fireSpeed, float interval, int direction=0)
        {
            this.discription = discr;
            this.eachDamage = eachDamage;
            this.fireSpeed = fireSpeed;
            this.interval = interval;
            this.direction = direction;
        }
    }

    public override void Attack()
    {
        AudioManager.Instance.PlaySFX("Hit1");
        Vector2 pp = player.position;
        Vector2 pd = player.GetComponent<PlayerController>().GetDirection();
        pd *= fireOffset;
        pp += pd;
        if (direction == 0)
        {
            Vector3 firePos = new Vector3(pp.x, pp.y, 0);
            float angle = Mathf.Atan2(pd.y, pd.x) * Mathf.Rad2Deg;

            Fire d = Instantiate(daggerFire, firePos, Quaternion.identity).GetComponent<Fire>();
            Rigidbody2D frb = d.GetComponent<Rigidbody2D>();
            d.passCapacity = 1;
            d.transform.parent = fireRoot;
            frb.velocity = pd * fireSpeed;
            d.transform.localEulerAngles = Vector3.forward * (angle - 135f);
            d.damage = eachDamage;
        }
        else if (direction == 1)
        {
            Vector3 firePos = new Vector3(pp.x, pp.y + 0.5f, 0);
            float angle = Mathf.Atan2(pd.y, pd.x) * Mathf.Rad2Deg;

            Fire d = Instantiate(daggerFire, firePos, Quaternion.identity).GetComponent<Fire>();
            Rigidbody2D frb = d.GetComponent<Rigidbody2D>();
            d.passCapacity = 1;
            d.transform.parent = fireRoot;
            frb.velocity = pd * fireSpeed;
            d.transform.localEulerAngles = Vector3.forward * (angle - 135f);
            d.damage = eachDamage;

            firePos = new Vector3(pp.x, pp.y + 0.5f, 0);
            d = Instantiate(daggerFire, firePos, Quaternion.identity).GetComponent<Fire>();
            frb = d.GetComponent<Rigidbody2D>();
            d.passCapacity = 1;
            d.transform.parent = fireRoot;
            frb.velocity = pd * fireSpeed;
            d.transform.localEulerAngles = Vector3.forward * (angle - 135f);
            d.damage = eachDamage;
        }
    }

    public override string AtUpdate(float dt_sec)
    {
        procTime += dt_sec;
        if (procTime > interval)
        {
            procTime = 0;
            Attack();
            return "";
        }
        return "";
    }

    public override void LevelInfoSetup()
    {
        title = "Dagger";
        levelInfos = new DaggerLevelInfo[7];
        levelInfos[0] = new DaggerLevelInfo("Throw a knife that does not pass in the direction of movement.", 11f, 10f, 1.1f);
        levelInfos[1] = new DaggerLevelInfo("Increases damage to the knife.", 12f, 10f, 1.1f);
        levelInfos[2] = new DaggerLevelInfo("The speed of movement of the knife increases.", 12f, 11f, 1.1f);
        levelInfos[3] = new DaggerLevelInfo("My attack speed increases.", 12f, 11f, 1.0f);
        levelInfos[4] = new DaggerLevelInfo("We fire two at a time", 7f, 11f, 1.0f, 1);
        levelInfos[5] = new DaggerLevelInfo("We fire two at a time", 7f, 11f, 1.0f, 1);
        levelInfos[6] = new DaggerLevelInfo("We fire two at a time", 7f, 11f, 1.0f, 1);
    }

    public override int AddLevel()
    {
        level++;
        int idx = (level < levelInfos.Length) ? level - 1 : levelInfos.Length - 1;
        DaggerLevelInfo gli = (DaggerLevelInfo)levelInfos[idx];
        eachDamage = gli.eachDamage;
        fireSpeed = gli.fireSpeed;
        interval = gli.interval;
        direction = gli.direction;
        return level;
    }
}
