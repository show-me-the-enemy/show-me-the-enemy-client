using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public Transform player;
    public float speed = 5;
    public float hp = 10f;
    public Animator animator;

    private bool isImmotal = false;
    public float immotalTime = 0.1f;

    public bool attackable = true;
    public float attackTime = 0.5f;
    public float damage = 10f;

    private Rigidbody2D rb;
    public CoinGenerator coinGenerator;
    public int coinAmount = 100;

    // Start is called before the first frame update
    public void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    public void AdvanceTime(float dt_sec)
    {
        if (hp < 0) return;
        Vector2 dir = (player.position - transform.position);
        float dist = dir.magnitude;
        dir /= dist; //mag
        dir *= speed;
        if (dist < 0.7f) rb.velocity = Vector2.zero;
        else rb.velocity = dir;
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            PlayerController p = col.GetComponent<PlayerController>();
            AttackPlayer(p);
        }
    }
    public void AutoDestory()
    {
        Destroy(gameObject);
    }
    public void GetDamaged(float d)
    {
        if (isImmotal) return;
        animator.SetTrigger("Damaged");
        hp -= d;
        if (hp < 0) Death();
        else StartCoroutine(SetImmotal());
    }
    void Death()
    {
        isImmotal = true;
        attackable = false;
        coinGenerator.genCoin(transform.position, coinAmount);
        hp = -1;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Death");
    }
    public void AttackPlayer(PlayerController p)
    {
        if (attackable == false) return;
        animator.SetTrigger("Attack");
        StartCoroutine(AttackDelay());
        p.GetDamaged(damage);
    }
    IEnumerator SetImmotal()
    {
        isImmotal = true;
        yield return new WaitForSeconds(immotalTime);
        isImmotal = false;
    }
    IEnumerator AttackDelay()
    {
        attackable = false;
        yield return new WaitForSeconds(attackTime);
        attackable = true;
    }

    public void Set()
    {
        animator.SetBool("Walk", true);
    }
    public void Dispose()
    {
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Death");
    }
}
