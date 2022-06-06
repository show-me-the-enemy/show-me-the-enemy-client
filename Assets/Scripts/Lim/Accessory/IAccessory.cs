using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAccessory : MonoBehaviour
{
    protected PlayerController player;
    public int level = 0;

    public Sprite icon;
    [HideInInspector]
    public string title;


    public abstract class ILevelInfo
    {
        public string discription;
    }
    [HideInInspector]
    public ILevelInfo[] levelInfos;

    public void Init(PlayerController _player)
    {
        player = _player;
    }
    public bool IsMaxLevel()
    {
        return levelInfos.Length < level;
    }
    public string getDiscription()
    {
        if(level < levelInfos.Length)
        {
            return levelInfos[level].discription;
        }
        return "max level";
    }

    public abstract void LevelInfoSetup();
    public abstract void AddLevel();
}
