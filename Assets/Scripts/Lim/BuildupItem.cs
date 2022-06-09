using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildupItem : MonoBehaviour
{
    public enum ItemState
    {
        NEW, UPGRADE, MAX
    } 
    public Image iconImg;
    public GameObject newText;
    public GameObject upgText;
    public GameObject purText;
    public GameObject maxText;
    public Text titleText;
    public Text discrText;
    public Text priceText;


    public void SetItem(Sprite img, int price, string title, string discr, ItemState itemState)
    {
        GetComponent<Button>().interactable = true;
        iconImg.sprite = img;
        priceText.text = price + "";
        titleText.text = title;
        discrText.text = discr;
        purText.SetActive(false);

        newText.SetActive(itemState == ItemState.NEW);
        upgText.SetActive(itemState == ItemState.UPGRADE);
        maxText.SetActive(itemState == ItemState.MAX);
        if(itemState == ItemState.MAX)
            GetComponent<Button>().interactable = false;
    }
    public void SetPurchaseCompleted()
    {
        GetComponent<Button>().interactable = false;
        purText.SetActive(true);
        newText.SetActive(false);
        upgText.SetActive(false);

    }
}
