using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public float damage = 11f;
    public int passCapacity = 100;
    public float lifeTime = 10000;

    public void Start()
    {
        StartCoroutine(DestroyRoutine());
    }
    public IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(lifeTime);
        AutoDestory();
    }

    public void AutoDestory()
    {
        Destroy(gameObject);
    }

    public void OnTriggerStay2D(Collider2D col)
    {
        Debug.Log(col);
        if (col.tag == "Monster")
        {
            Monster mob = col.GetComponent<Monster>();
            mob.GetDamaged(damage);
            passCapacity--;
            if (passCapacity <= 0)
                Destroy(gameObject);
        }
        if (col.tag == "Box")
        {
            GroundBox box = col.GetComponent<GroundBox>();
            box.Open();
        }
    }
}
