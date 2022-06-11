using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverPopup : MonoBehaviour
{
    public GameObject winImg;
    public GameObject loseImg;
    public Text titleT;
    public Text playerT;
    public Text roundT;
    public Text crystalT;

    public void Set(bool isWin, string opId, int round, int crystal)
    {
        winImg.SetActive(isWin);
        loseImg.SetActive(!isWin);
        titleT.text = isWin ? "You win!" : "You die";
        playerT.text = "opposite player: " + opId;
        roundT.text = "surviving round: " + round;
        crystalT.text = "acquired crystal: " + crystal;
    } 
}
