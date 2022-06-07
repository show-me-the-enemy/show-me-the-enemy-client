using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildupItem : MonoBehaviour
{
    public Image iconImg;
    public GameObject newText;
    public GameObject upgText;
    public GameObject purText;
    public Text titleText;
    public Text discrText;
    public Text priceText;


    public void SetItem(Sprite img, int price, string title, string discr, bool isNew)
    {
        GetComponent<Button>().interactable = true;
        iconImg.sprite = img;
        priceText.text = price + "";
        titleText.text = title;
        discrText.text = discr;
        newText.SetActive(isNew);
        upgText.SetActive(!isNew);
        purText.SetActive(false);
    }
    public void SetPurchaseCompleted()
    {
        GetComponent<Button>().interactable = false;
        purText.SetActive(true);
        newText.SetActive(false);
        upgText.SetActive(false);
    }
}
