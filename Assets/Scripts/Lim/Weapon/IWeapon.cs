using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IWeapon : MonoBehaviour
{
    protected Transform player;
    protected Transform fireRoot;
    public int level=0;

    public Sprite icon;
    [HideInInspector]
    public string title;
    public int id;


    public abstract class ILevelInfo
    {
        public string discription;
    }
    [HideInInspector]
    public ILevelInfo[] levelInfos;

    public void Init(Transform _player, Transform _fireRoot)
    {
        player = _player;
        fireRoot = _fireRoot;
    }
    public string getDiscription()
    {
        if (level < levelInfos.Length)
        {
            return levelInfos[level].discription;
        }
        return "max level";
    }

    public abstract void LevelInfoSetup();
    public abstract int AddLevel();
    public abstract void Attack();
    public abstract string AtUpdate(float dt_sec);
}
