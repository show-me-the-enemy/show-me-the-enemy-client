using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GroundItem : MonoBehaviour
{
    // todo: inheritance
    public int state = 0;
    bool isUsed = false;

    public void UseItem(PlayerController player)
    {
        if (isUsed) return;
        isUsed = true;
        switch (state)
        {
            case 0:
                player.Heal(20);
                break;
            case 1:
                player.GetAllCoins();
                break;
            case 2:
                player.SetItemImmotal();
                break;
        }
        transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
