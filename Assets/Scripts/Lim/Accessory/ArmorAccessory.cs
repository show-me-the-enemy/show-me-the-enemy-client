using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorAccessory : IAccessory
{
    public class ArmorLevelInfo : ILevelInfo
    {
        public float defensive = 1.0f;

        public ArmorLevelInfo(string discr,float defensive)
        {
            this.discription = discr;
            this.defensive = defensive;
        }
    }
    public override void AddLevel()
    {
        title = "Armor";
        level++;
        int idx = (level < levelInfos.Length) ? level - 1 : levelInfos.Length - 1;
        ArmorLevelInfo gli = (ArmorLevelInfo)levelInfos[idx];
        player.defensive = gli.defensive;
    }

    public override void LevelInfoSetup()
    {
        levelInfos = new ILevelInfo[7];
        levelInfos[0] = new ArmorLevelInfo("It increases your defense", 0.97f);
        levelInfos[1] = new ArmorLevelInfo("It increases your defense", 0.94f);
        levelInfos[2] = new ArmorLevelInfo("It increases your defense", 0.91f);
        levelInfos[3] = new ArmorLevelInfo("It increases your defense", 0.89f);
        levelInfos[4] = new ArmorLevelInfo("It increases your defense", 0.97f);
        levelInfos[5] = new ArmorLevelInfo("It increases your defense", 0.85f);
        levelInfos[6] = new ArmorLevelInfo("It increases your defense", 0.83f);
    }
}
