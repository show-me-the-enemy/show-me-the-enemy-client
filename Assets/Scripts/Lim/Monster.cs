using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public Transform player;
    public float speed = 5;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    public void Init()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    public void AdvanceTime(float dt_sec)
    {
        Vector2 dir = (player.position - transform.position);
        dir = dir.normalized;
        dir *= speed;
        rb.velocity = dir;
    }
}
