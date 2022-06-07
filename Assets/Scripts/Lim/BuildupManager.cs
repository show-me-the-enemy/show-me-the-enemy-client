using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildupManager : MonoBehaviour
{
    public InGameModel gameModel;
    public BuildupItem[] buildupItems;
    public PlayerController player;
    WeaponManager weaponManager;
    AccessoryManager accessoryManager;
    public bool isPurchase = false;

    List<IWeapon> weapons = new List<IWeapon>();
    List<IAccessory> accessories = new List<IAccessory>();

    public void Init()
    {
        weaponManager = gameModel.weaponManager;
        accessoryManager = gameModel.accessoryManager;
    }

    public void updateItems()
    {
        weapons.Clear();
        accessories.Clear();
        isPurchase = false;

        int item_idx = 0;
        int max_weapon = weaponManager.GetNrWeapons();
        int max_accessory = accessoryManager.GetNrAccessories();

        int nr_weapon = Random.Range(0, 4);
        int nr_accessory = 3 - nr_weapon;
        List<int> notOverlap = new List<int>();
        for(int i = 0; i < nr_weapon; i++)
        {
            int idx = Random.Range(0, max_weapon);
            if(notOverlap.Contains(idx))
            {
                i--;
                continue;
            }
            notOverlap.Add(idx);
        }
        foreach(int weapon_idx in notOverlap) {
            IWeapon w = weaponManager.GetWeapon(weapon_idx);
            weapons.Add(w);
            buildupItems[item_idx++].SetItem(w.icon, gameModel.GetBuildupItemPrice(), w.title, w.getDiscription(), w.level == 0);
        }

        notOverlap.Clear();
        for (int i = 0; i < nr_accessory; i++)
        {
            int idx = Random.Range(0, max_accessory);
            if (notOverlap.Contains(idx))
            {
                i--;
                continue;
            }
            notOverlap.Add(idx);
        }
        foreach (int accessory_idx in notOverlap)
        {
            IAccessory a = accessoryManager.GetAccessory(accessory_idx);
            accessories.Add(a);
            buildupItems[item_idx++].SetItem(a.icon, gameModel.GetBuildupItemPrice(), a.title, a.getDiscription(), a.level == 0);
        }
    }
    public void ItemOnClick(int idx)
    {
        if (!gameModel.Purchase(gameModel.GetBuildupItemPrice())) return;
        buildupItems[idx].SetPurchaseCompleted();
        if (idx < weapons.Count)
        {
            int lev = weapons[idx].AddLevel();
            if (lev == 1) player.AddWeapon(weapons[idx]);
        }
        else
        {
            idx -= weapons.Count;
            accessories[idx].AddLevel();
        }
        isPurchase = true;

        //옵저버 패턴으로 내가 구매하면 정보 보냄
        Hashtable sendData = new Hashtable();
        sendData.Add(EDataParamKey.Integer, idx);
        NotificationCenter.Instance.PostNotification(ENotiMessage.OnAddBuildUp, sendData);
    }
}
