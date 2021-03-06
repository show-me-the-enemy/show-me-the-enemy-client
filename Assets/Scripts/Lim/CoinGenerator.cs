using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    Dictionary<string, Coin> coinPrefabs = new Dictionary<string, Coin>();
    public void Start()
    {
        coinPrefabs.Add("blue", Resources.Load<Coin>("Prefabs/Items/BlueCoin"));
        coinPrefabs.Add("gold", Resources.Load<Coin>("Prefabs/Items/GoldCoin"));
        coinPrefabs.Add("green", Resources.Load<Coin>("Prefabs/Items/GreenCoin"));
        coinPrefabs.Add("purple", Resources.Load<Coin>("Prefabs/Items/PurpleCoin"));
        coinPrefabs.Add("red", Resources.Load<Coin>("Prefabs/Items/RedCoin"));
    }
    public void GenCoin(Vector3 pos, int amount)
    {
        string cName = "blue";
        if (amount < 150) cName = "blue";
        else if (amount < 350) cName = "gold";
        else if (amount < 550) cName = "green";
        else if (amount < 750) cName = "purple";
        else cName = "gold";
        Coin coin = Instantiate(coinPrefabs[cName], pos, Quaternion.identity);
        coin.amount = amount;
        coin.transform.parent = transform;
    }
    public void MoveCoins(Vector2 playerPos)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Coin c = transform.GetChild(i).GetComponent<Coin>();
            c.GoTo(playerPos);
        }
    }
}
