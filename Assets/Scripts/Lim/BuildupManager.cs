using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildupManager : MonoBehaviour
{
    public InGameModel gameModel;
    public BuildupItem[] buildupItems;
    public PlayerController player;
    public Text logText;

    WeaponManager weaponManager;
    AccessoryManager accessoryManager;
    [HideInInspector]
    public bool isPurchase = false;
    string tempLog="";

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
            }
            else
            {
                notOverlap.Add(idx);
            }
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
            }
            else 
            { 
                notOverlap.Add(idx);
            }
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
        string type, name;
        int oriIdx = idx;
        if (idx < weapons.Count)
        {
            IWeapon w = weapons[idx];
            type = "weapon";
            name = w.title;
            if (!gameModel.Purchase(gameModel.GetBuildupItemPrice())|| w.levelInfos.Length-1 <= w.level) return;
            int lev = w.AddLevel();
            if (lev == 1) player.AddWeapon(w);
        }
        else
        {
            idx -= weapons.Count;
            IAccessory a = accessories[idx];
            type = "accessory";
            name = a.title;
            if (!gameModel.Purchase(gameModel.GetBuildupItemPrice())|| a.levelInfos.Length-1 <= a.level) return;
            a.AddLevel();
        }
        buildupItems[oriIdx].SetPurchaseCompleted();
        isPurchase = true;

#if BATTLE_TEST
#else
        NetworkManager.Instance.SendBuildUpMsg(type, name, 0);
#endif
    }

    public void AttackItemOnClick(int idx)
    {
        string type, name="";
        type = "monster";
        int count = 1;
        if (idx == 0)
        {
            if (!gameModel.Purchase(5000)) return;
            name = "Bread";
            Debug.Log("send bread gen");
        }
        else if(idx == 1)
        {
            if (!gameModel.Purchase(500)) return;
            name = "Air";
            count = 2;
            Debug.Log("send air gen");
        }
#if BATTLE_TEST
#else
        NetworkManager.Instance.SendBuildUpMsg(type, name, count);
#endif
    }
    
    public void AdvanceTime(float dt_sec)
    {
        if(tempLog != "")
        {
            logText.text = tempLog;
            tempLog = "";
        }
    }
    // 12 hours of painfull.12 hours of painfull.12 hours of painfull.
    // 12 hours of painfull.12 hours of painfull.12 hours of painfull.
    // 12 hours of painfull.12 hours of painfull.12 hours of painfull.
    // 12 hours of painfull.12 hours of painfull.12 hours of painfull.
    // 12 hours of painfull.12 hours of painfull.12 hours of painfull.
    // 12 hours of painfull.12 hours of painfull.12 hours of painfull.
    // 12 hours of painfull.12 hours of painfull.12 hours of painfull.
    string GetAdditionLog(string add)
    {
        if (tempLog != "") return add + "\n" + tempLog;
        else return add + "\n" + logText.text;
    }
    public void AddRogText(string log)
    {
        tempLog = GetAdditionLog(log);
    }
}
