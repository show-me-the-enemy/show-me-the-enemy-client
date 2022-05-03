using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    private Rigidbody2D rb;
    public float speed;


    public void Init()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void AdvanceTime(float dt_sec)
    {
        rb.velocity = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            rb.velocity += Vector2.left;

        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            rb.velocity += Vector2.right;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            rb.velocity += Vector2.up;

        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            rb.velocity += Vector2.down;

        rb.velocity = rb.velocity.normalized;
        rb.velocity *= speed;

        Vector3 ls3 = transform.localScale;
        transform.localScale = new Vector3((ls3.x) * ((ls3.x * rb.velocity.x < 0) ? -1 : 1), ls3.y, ls3.z);
        float mag_vel = rb.velocity.magnitude;
        animator.SetFloat("mag_vel", mag_vel);
    }
}
