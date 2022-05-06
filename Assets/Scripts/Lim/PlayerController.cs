using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    private Rigidbody2D rb;
    public float speed;
    public GameObject whipFire;
    public Transform fireRoot;
    public float whipDelaySec = 1.0f;

    Vector2 fireOffset = new Vector3(2.849f, -0.293f, 0);


    public void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(WhipRoutine());
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

    IEnumerator WhipRoutine()
    {
        yield return new WaitForSeconds(whipDelaySec);
        while (true) 
        {
            animator.SetTrigger("attack");
            yield return new WaitForSeconds(whipDelaySec);
        }
    }
    public void WhipFire()
    {
        Vector3 firePos = new Vector3(transform.position.x + transform.localScale.x * fireOffset.x,
            transform.position.y + fireOffset.y, transform.position.z);

        GameObject whip = Instantiate(whipFire, firePos, Quaternion.identity);
        whip.transform.parent = fireRoot;
        whip.GetComponent<SpriteRenderer>().flipX = (transform.localScale.x < 0);
    }
}
