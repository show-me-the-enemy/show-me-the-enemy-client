using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoesAccessory : IAccessory
{
    float speed = 5.0f;
    public class ShoesLevelInfo : ILevelInfo
    {
        public float speed = 5.0f;

        public ShoesLevelInfo(string discr, float speed)
        {
            this.discription = discr;
            this.speed = speed;
        }
    }
    public override void AddLevel()
    {
        level++;
        int idx = (level < levelInfos.Length) ? level - 1 : levelInfos.Length - 1;
        ShoesLevelInfo gli = (ShoesLevelInfo)levelInfos[idx];
        speed = gli.speed;
        player.speed = speed;
    }

    public override void LevelInfoSetup()
    {
        title = "Shoes";
        levelInfos = new ILevelInfo[7];
        levelInfos[0] = new ShoesLevelInfo("It increases your speed", 5f);
        levelInfos[1] = new ShoesLevelInfo("It increases your speed", 5.5f);
        levelInfos[2] = new ShoesLevelInfo("It increases your speed", 6f);
        levelInfos[3] = new ShoesLevelInfo("It increases your speed", 6.5f);
        levelInfos[4] = new ShoesLevelInfo("It increases your speed", 7f);
        levelInfos[5] = new ShoesLevelInfo("It increases your speed", 7.5f);
        levelInfos[6] = new ShoesLevelInfo("It increases your speed", 8f);
    }
}