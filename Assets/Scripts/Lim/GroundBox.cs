using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GroundBox : MonoBehaviour
{
    public SpriteRenderer sr;
    [HideInInspector]
    public GroundItem itemPrefab;
    bool isCrash = false;

    public void Open()
    {
        if (isCrash) return;
        Debug.Log("Box crash");
        isCrash = true;
        sr.DOFade(0, 1).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
