using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : BaseElement, BaseElement.IBaseController
{
    public InGameController _game;
    public Animator animator;
    private Rigidbody2D rb;
    public Transform fireRoot;
    public UiBarView hpBar;
    public HudController hudController;
    public WeaponManager weaponManager;
    public CoinGenerator coinGenerator;
    public InGameModel gameModel;
    public float speed;
    [HideInInspector]
    public float maxHp = 100f;
    private float hp;
    [HideInInspector]
    public float defensive = 1.0f;

    private List<IWeapon> weapons = new List<IWeapon>();

    private bool isImmotal = false;
    public float defaultImmotalTime = 0.1f;
    public float itemImmotalTime = 6.0f;


    public void Heal(int amount)
    {
        hp += amount;
        if(hp > maxHp)hp = maxHp;
    }
    public void GetAllCoins()
    {
        coinGenerator.MoveCoins(transform.position);
    }
    public void GetDamaged(float d)
    {
        if (isImmotal || hp < 0) return;
        AudioManager.Instance.PlaySFX("PDeath");
        animator.SetTrigger("Damaged");
        hp -= defensive*d;
        hpBar.setValue(hp / maxHp);
        if (hp < 0) Death();
        else StartCoroutine(SetImmotal(defaultImmotalTime));
    }
    void Death()
    {
        _game.isWin = false;
#if BATTLE_TEST
#else
        Debug.Log("Send Die");
        NetworkManager.Instance.PlayerDie();
#endif
        hp = -1f;
        isImmotal = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Death");
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
#if BATTLE_TEST
        _game.goDeath = true;
#endif
    }
    public void SetItemImmotal()
    {
        if (isImmotal)
        {
            StopCoroutine(SetImmotal(defaultImmotalTime));
            GetComponent<SpriteRenderer>().DOKill();
        }
        isImmotal = true;
        GetComponent<SpriteRenderer>().DOColor(new Color(1, 1, 0, 1), itemImmotalTime).SetEase(Ease.Flash, itemImmotalTime * 10).SetLink(gameObject).OnComplete(() =>
        {
            isImmotal = false;
        });
    }
    public void ToggleCheatImmotal()
    {
        const int imSec = 20 * 60;//20분
        isImmotal = !isImmotal;
        if(isImmotal)
            GetComponent<SpriteRenderer>().DOColor(new Color(1, 1, 0, 1), imSec).SetEase(Ease.Flash, imSec * 10).SetLink(gameObject);
        else
        {
            StopCoroutine(SetImmotal(defaultImmotalTime));
            GetComponent<SpriteRenderer>().DOKill();
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
    }
    IEnumerator SetImmotal(float sec)
    {
        isImmotal = true;
        yield return new WaitForSeconds(sec);
        isImmotal = false;
    }
    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Coin")
        {
            Coin c = col.GetComponent<Coin>();
            gameModel.SaveCoin(c.GetCoin());
        } 
        else if (col.tag == "Item")
        {
            GroundItem item = col.GetComponent<GroundItem>();
            item.UseItem(this);
        }
    }
    public void AddWeapon(IWeapon w)
    {
        weapons.Add(w);
    }
    public void Init()
    {
        IWeapon w = weaponManager.GetWeapon("Whip");
        w.AddLevel(); // 하면 무기에서 플레이어 참조해서 무기 추가해줌 쓰레기코드임.. ㅈㅅ
        AddWeapon(w);
        //hp = maxHp;
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

        foreach (IWeapon w in weapons)
        {
            string action = w.AtUpdate(dt_sec);
            if (action.Length > 0)
            {
                animator.SetTrigger(action);
            }
        }
    }
    public void Set()
    {
        animator.enabled = true;
        hp = maxHp;
        hpBar.setValue(hp / maxHp);
    }
    public void Dispose()
    {
        animator.enabled = false;
        rb.velocity = Vector2.zero; 
        StopCoroutine(SetImmotal(defaultImmotalTime));
    }

    public void SetActive(bool flag)
    {
        throw new System.NotImplementedException();
    }
}
