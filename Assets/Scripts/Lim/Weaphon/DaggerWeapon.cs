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

    public override void Attack()
    {
        Vector2 pp = player.position;
        Vector2 pd = player.GetComponent<PlayerController>().GetDirection();
        pd *= fireOffset;
        pp += pd;

        Vector3 firePos = new Vector3(pp.x,pp.y, 0);
        float angle = Mathf.Atan2(pd.y,pd.x)*Mathf.Rad2Deg;

        Debug.Log(angle);
        Fire d = Instantiate(daggerFire, firePos, Quaternion.identity).GetComponent<Fire>();
        Rigidbody2D frb = d.GetComponent<Rigidbody2D>();
        d.passCapacity = 1;
        d.transform.parent = fireRoot;
        frb.velocity = pd * fireSpeed;
        d.transform.localEulerAngles = Vector3.forward * (angle-135f);
        d.damage = eachDamage;
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
}
