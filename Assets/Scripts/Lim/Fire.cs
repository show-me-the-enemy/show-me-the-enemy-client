using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public float damage = 11f;

    public void AutoDestory()
    {
        Destroy(gameObject);
    }

    public void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Monster")
        {
            Monster mob = col.GetComponent<Monster>();
            mob.GetDamaged(damage);
        }
    }
}
