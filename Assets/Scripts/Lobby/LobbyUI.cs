using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour,System.IDisposable
{
    [SerializeField]
    private Text _txtTopBarName;
    [SerializeField]
    private Text _txtRanking;
    public void Start()
    {
        NotificationCenter.Instance.AddObserver(OnUpdatePlayerData, ENotiMessage.UpdatePlayerDate);
        NotificationCenter.Instance.AddObserver(OnUpdateRanking, ENotiMessage.TopTenUsersRankResponse);
        NetworkManager.Instance.GetTopTenRaking();
        NetworkManager.Instance.UpdateUserInfo();
        AudioManager.Instance.PlayBGM("Main");
    }

    public void OnUpdatePlayerData(Notification noti)
    {
        _txtTopBarName.text = "USERNAME : " + NetworkManager.Instance.UserName
                            + " crystal : " + NetworkManager.Instance.Craystal
                            + " numWinds : " + NetworkManager.Instance.NumWins;
    }

    public void OnUpdateRanking(Notification noti)
    {
        string tempTxt = "";
        TopTenUsersRankResponse request = noti.data[EDataParamKey.TopTenUsersRankResponse] as TopTenUsersRankResponse;
        foreach (var info in request.Items)
        {
            tempTxt+= info.rank + ". "+info.username+"\n < numWins: "+info.numWins + " , maxRound: " + info.maxRound + " >\n\n";
        }
        _txtRanking.text = tempTxt;
    }

    public void OnClick_CreateRoom()
    {
        NetworkManager.Instance.CreateRoom();
    }
    public void OnClick_JoinRoom()
    {
        NetworkManager.Instance.JoinRoom();
    }
    public void OnClick_DeletaAllGame()
    {
        NetworkManager.Instance.DeleteAllGame();
    }

    public void Dispose()
    {
        NotificationCenter.Instance.RemoveObserver(OnUpdatePlayerData, ENotiMessage.UpdatePlayerDate);
        NotificationCenter.Instance.RemoveObserver(OnUpdateRanking, ENotiMessage.TopTenUsersRankResponse);
    }
}
