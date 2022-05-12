using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform player;
    public Transform fireRoot;

    [SerializeField]
    List<string> weaponKeys = new List<string>();
    [SerializeField]
    List<IWeapon> weapons = new List<IWeapon>();

    public void Init()
    {
        foreach(IWeapon w in weapons)
        {
            w.Init(player, fireRoot);
        }
    }

    public IWeapon GetWeapon(string key)
    {
        int index = weaponKeys.IndexOf(key);
        return weapons[index];
    }
}
