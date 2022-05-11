using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public InGameController _game;
    public Animator animator;
    private Rigidbody2D rb;
    public float speed;
    public GameObject whipFire;
    public Transform fireRoot;
    public float whipDelaySec = 1.0f;
    public UiBarView hpBar;
    public float maxHp = 100f;
    private float hp;


    private bool isImmotal = false;
    public float immotalTime = 0.1f;

    Vector2 fireOffset = new Vector3(2.85f, -0.293f, 0);


    public void Init()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void AdvanceTime(float dt_sec)
    {
        if (hp < 0) return;
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
        animator.SetFloat("MagVel", mag_vel);
    }

    IEnumerator WhipRoutine()
    {
        yield return new WaitForSeconds(whipDelaySec);
        while (true) 
        {
            animator.SetTrigger("Attack");
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
    public void GetDamaged(float d)
    {
        if (isImmotal || hp < 0) return;
        animator.SetTrigger("Damaged");
        hp -= d;
        hpBar.setValue(hp / maxHp);
        if (hp < 0) Death();
        else StartCoroutine(SetImmotal());
    }
    void Death()
    {
        hp = -1f;
        isImmotal = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Death");
        _game.GameOver();
    }
    public void DeathEvent()
    {
        animator.enabled=false;
    }
    IEnumerator SetImmotal()
    {
        isImmotal = true;
        yield return new WaitForSeconds(immotalTime);
        isImmotal = false;
    }


    public void Set()
    {
        animator.enabled = true;
        hp = maxHp;
        hpBar.setValue(1);
        StartCoroutine(WhipRoutine());
    }
    public void Dispose()
    {
        animator.enabled = false;
        rb.velocity = Vector2.zero;
        StopAllCoroutines();
    }
}
