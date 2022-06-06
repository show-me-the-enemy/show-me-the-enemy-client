using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform player;
    public Transform fireRoot;

    [SerializeField]
    IWeapon[] weapons;

    public void Init()
    {
        foreach (IWeapon w in weapons)
        {
            w.Init(player, fireRoot);
            w.LevelInfoSetup();
        }
    }

    public IWeapon GetWeapon(string key)
    {
        foreach (IWeapon weapon in weapons)
        {
            if (weapon.title == key)
            {
                return weapon;
            }
        }
        return null;
    }
    public IWeapon GetWeapon(int idx)
    {
        return weapons[idx];
    }
    public int GetNrWeapons() { return weapons.Length; }
}
