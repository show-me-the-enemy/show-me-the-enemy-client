using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    public UiBarView timeBar;
    public UiBarView coinBar;

    public Text coinText;
    public Text coinCntText;
    public InGameModel gameModel;

    public void SetTimeBar(float percent, float prgTime)
    {
        timeBar.setValue(percent, prgTime);
    }
    public void UpdateCoinBar(bool showProgress = false)
    {
        int coin = gameModel.coinAmout;
        coinText.text = "Coin: " + gameModel.coinAmout;

        if (!showProgress) 
        {
            coinCntText.text = "";
            coinBar.setValue(0);
        }
        else 
        {
            int itemPrice = gameModel.GetBuildupItemPrice();
            int purchaseCount = coin / itemPrice;
            int minCoin = coin - purchaseCount * itemPrice;
            float percent = (float)minCoin/itemPrice;
            coinCntText.text = purchaseCount+"";
            coinBar.setValue(percent);
        }
    }
}
