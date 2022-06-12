using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Coin : MonoBehaviour
{
    int anim_i = 0;
    public Sprite[] sprites = new Sprite[3];
    public SpriteRenderer sr;
    public int amount = 100;

    IEnumerator animRoutine()
    {
        while(true)
        {
            anim_i = (anim_i + 1) % 3;
            sr.sprite = sprites[anim_i];
            if(anim_i == 0)
                yield return new WaitForSeconds(0.3f);
            else
                yield return new WaitForSeconds(0.15f);
        }
    }
    public void Start()
    {
        StartCoroutine(animRoutine());
    }
    public void GoTo(Vector2 playerPos)
    {
        Vector2 pos = transform.position;
        float dist = (pos - playerPos).sqrMagnitude;
        transform.DOMove(playerPos, 0.5f);
    }
    public int GetCoin()
    {
        AudioManager.Instance.PlaySFX("Coin" + Random.Range(1, 4));
        StopCoroutine(animRoutine());
        StartCoroutine(DestroyRoutine());
        return amount;
    }
    IEnumerator DestroyRoutine()
    {
        const float ds = 0.3f;
        while(transform.localScale.x > 0)
        {
            transform.localScale = new Vector3((float)transform.localScale.x-ds, (float)transform.localScale.y-ds, (float)transform.localScale.z);
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(gameObject);
    }
}
