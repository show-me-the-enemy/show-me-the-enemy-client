using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilkAccessory : IAccessory
{
    float maxHP = 1.0f;
    public class MlikLevelInfo : ILevelInfo
    {
        public float maxHP = 100.0f;

        public MlikLevelInfo(string discr, float maxHP)
        {
            this.discription = discr;
            this.maxHP = maxHP;
        }
    }
    public override void AddLevel()
    {
        title = "Milk";
        level++;
        int idx = (level < levelInfos.Length) ? level - 1 : levelInfos.Length - 1;
        MlikLevelInfo gli = (MlikLevelInfo)levelInfos[idx];
        maxHP = gli.maxHP;
        player.defensive = maxHP;
    }

    public override void LevelInfoSetup()
    {
        levelInfos = new ILevelInfo[7];
        levelInfos[0] = new MlikLevelInfo("It increases your max hp", 110f);
        levelInfos[1] = new MlikLevelInfo("It increases your max hp", 120f);
        levelInfos[2] = new MlikLevelInfo("It increases your max hp", 130f);
        levelInfos[3] = new MlikLevelInfo("It increases your max hp", 140f);
        levelInfos[4] = new MlikLevelInfo("It increases your max hp", 150f);
        levelInfos[5] = new MlikLevelInfo("It increases your max hp", 160f);
        levelInfos[6] = new MlikLevelInfo("It increases your max hp", 170f);
    }
}