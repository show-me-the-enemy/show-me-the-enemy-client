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
    bool isAction = false;

    public override void Attack()
    {
        Vector3 firePos = new Vector3(player.position.x + player.localScale.x * fireOffset.x,
            player.position.y + fireOffset.y, player.position.z);

        Fire whip = Instantiate(whipFire, firePos, Quaternion.identity).GetComponent<Fire>();
        whip.transform.parent = fireRoot;
        whip.GetComponent<SpriteRenderer>().flipX = (player.localScale.x < 0);
        whip.damage = eachDamage;
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

}
