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
        for (int i = 0; i < nr_weapon; i++)
        {
            int idx = Random.Range(0, max_weapon);
            if (notOverlap.Contains(idx))
            {
                i--;
                continue;
            }
            notOverlap.Add(idx);
        }
        foreach (int weapon_idx in notOverlap) {
            IWeapon w = weaponManager.GetWeapon(weapon_idx);
            weapons.Add(w);
            BuildupItem.ItemState itemState = (w.level == 0) ? BuildupItem.ItemState.NEW :
                ((w.level == w.levelInfos.Length) ? BuildupItem.ItemState.MAX : BuildupItem.ItemState.UPGRADE);
            buildupItems[item_idx++].SetItem(w.icon, gameModel.GetBuildupItemPrice(), w.title + ", Lv" + w.level + ".", w.getDiscription(), itemState);
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
            BuildupItem.ItemState itemState = (a.level == 0) ? BuildupItem.ItemState.NEW :
                ((a.level == a.levelInfos.Length) ? BuildupItem.ItemState.MAX : BuildupItem.ItemState.UPGRADE);
            buildupItems[item_idx++].SetItem(a.icon, gameModel.GetBuildupItemPrice(), a.title + ", Lv" + a.level + ".", a.getDiscription(), itemState);
        }
    }
    public void ItemOnClick(int idx)
    {
        if (!gameModel.Purchase(gameModel.GetBuildupItemPrice())) return;

        Hashtable sendData = new Hashtable();

        buildupItems[idx].SetPurchaseCompleted();
        if (idx < weapons.Count)
        {
            IWeapon w = weapons[idx];
            sendData.Add("purchase_type", "weapon");
            sendData.Add("purchase_name", w.title);
            int lev = w.AddLevel();
            if (lev == 1) player.AddWeapon(w);
        }
        else
        {
            idx -= weapons.Count;
            IAccessory a = accessories[idx];
            sendData.Add("purchase_type", "accessory");
            sendData.Add("purchase_name", a.title);
            accessories[idx].AddLevel();
        }
        isPurchase = true;

        
        NotificationCenter.Instance.PostNotification(ENotiMessage.OnAddBuildUp, sendData);
    }

    public void AttackItemOnClick(int idx)
    {
        Hashtable sendData = new Hashtable();
        sendData.Add("purchase_type", "monster");
        if (idx == 0)
        {
            if (!gameModel.Purchase(5000)) return;
            sendData.Add("purchase_name", "Bread");
            Debug.Log("send bread gen");
        }
        else if(idx == 1)
        {
            if (!gameModel.Purchase(500)) return;
            sendData.Add("purchase_name", "Air");
            Debug.Log("send air gen");
        }
        NotificationCenter.Instance.PostNotification(ENotiMessage.OnAddBuildUp, sendData);
    }
}
