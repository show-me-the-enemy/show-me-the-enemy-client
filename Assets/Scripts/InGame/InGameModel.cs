using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameModel : MonoBehaviour
{
    public HudController hudController;
    public WeaponManager weaponManager;
    public AccessoryManager accessoryManager;

    [HideInInspector]
    public int round=0;
    [HideInInspector]
    public int buyLevel;
    [HideInInspector]
    public int coinAmout = 0;

    [System.Serializable]
    public class RoundInfo
    {
        public float battleTime = 10;
        public float buildupTime = 5;
    }
    public RoundInfo[] roundInfos;

    public void AddRound() { round++; }
    public void AddBuyLevel() { buyLevel++; }
    public float GetBattleTime()
    {
        int roundIdx = (round < roundInfos.Length - 1) ? round : roundInfos.Length - 1;
        return roundInfos[roundIdx].battleTime;
    }
    public float GetBuildupTime()
    {
        int roundIdx = (round < roundInfos.Length - 1) ? round : roundInfos.Length - 1;
        return roundInfos[roundIdx].buildupTime;
    }
    public int GetBuildupItemPrice()
    {
        return 1000 + 200 * buyLevel;
    }

    public bool Purchase(int price)
    {
        if (coinAmout < price) return false;

        coinAmout -= price;
        hudController.UpdateCoinBar(false);
        return true;
    }
    public void SaveCoin(int coins)
    {
        coinAmout += coins;
        hudController.UpdateCoinBar(true);
    }
    public void Init()
    {
        weaponManager.Init();
        accessoryManager.Init();
    }
}
