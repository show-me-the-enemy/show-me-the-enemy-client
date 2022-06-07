using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessoryManager : MonoBehaviour
{
    public PlayerController player;

    [SerializeField]
    IAccessory[] accessories;

    public void Init()
    {
        foreach (IAccessory a in accessories)
        {
            a.Init(player);
            a.LevelInfoSetup();
        }
    }

    public IAccessory GetAccessory(string key)
    {
        foreach (IAccessory a in accessories)
        {
            if (a.title == key)
            {
                Debug.Log(a.title);
                return a;
            }
        }
        return null;
    }
    public IAccessory GetAccessory(int idx)
    {
        return accessories[idx];
    }
    public int GetNrAccessories() { return accessories.Length; }
}
