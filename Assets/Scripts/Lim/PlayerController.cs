using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public InGameController _game;
    public Animator animator;
    private Rigidbody2D rb;
    public float speed;
    public Transform fireRoot;
    public UiBarView hpBar;
    public float maxHp = 100f;
    private float hp;
    public int coinAmout = 0;
    public HudController hudController;

    public WeaponManager weaponManager;
    private List<IWeapon> weaphons = new List<IWeapon>();

    private bool isImmotal = false;
    public float immotalTime = 0.1f;

    


    public void Init()
    {
        weaphons.Add(weaponManager.GetWeapon("whip"));
        weaphons.Add(weaponManager.GetWeapon("dagger"));
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

        foreach(IWeapon w in weaphons)
        {
            string action = w.AtUpdate(dt_sec);
            if (action.Length>0)
                animator.SetTrigger(action);
            
        }
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
    public Vector2 GetDirection()
    {
        if (rb.velocity.magnitude < 0.1)
            return Vector2.right * transform.localScale.x;
        return rb.velocity.normalized;
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
    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Coin")
        {
            Coin c = col.GetComponent<Coin>();
            coinAmout += c.GetCoin();
            hudController.UpdateCoinText(coinAmout);
        }
    }


    public void Set()
    {
        animator.enabled = true;
        hp = maxHp;
        hpBar.setValue(1);
    }
    public void Dispose()
    {
        animator.enabled = false;
        rb.velocity = Vector2.zero;
        StopAllCoroutines();
    }
}
