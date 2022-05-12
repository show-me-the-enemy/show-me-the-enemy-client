using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    public Text coinText;

    public void UpdateCoinText(int amount)
    {
       coinText.text = "Coin : "+amount;
    }
}
