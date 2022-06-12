using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GroundBox : MonoBehaviour
{
    public SpriteRenderer sr;
    public GroundItem itemPrefab;
    bool isCrash = false;

    public void Open()
    {
        if (isCrash) return;
        
        isCrash = true;
        sr.DOFade(0, 0.2f).OnComplete(() =>
        {
            if (itemPrefab != null)
            {
                GroundItem item = Instantiate(itemPrefab, transform.position, Quaternion.identity);
                item.transform.parent = transform.parent;
            }
            else
            {
                Debug.Log("box doesn't have item");
            }
            Destroy(gameObject);
        });
    }
}
