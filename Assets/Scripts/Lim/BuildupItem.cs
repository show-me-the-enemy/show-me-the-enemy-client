using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildupItem : MonoBehaviour
{
    public Image iconImg;
    public GameObject newText;
    public GameObject upgText;
    public Text titleText;
    public Text discrText;
    public Text priceText;


    public void SetItem(Sprite img, int price, string title, string discr, bool isNew)
    {
        iconImg.sprite = img;
        priceText.text = price + "";
        titleText.text = title;
        discrText.text = discr;
        newText.SetActive(isNew);
        upgText.SetActive(!isNew);
    }
}
