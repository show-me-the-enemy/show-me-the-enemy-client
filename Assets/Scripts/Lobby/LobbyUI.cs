using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField]
    private Text _txtTopBarName;
    public void Start()
    {
        NetworkManager.Instance.UpdateUserInfo();
        _txtTopBarName.text = "USERNAME : "+NetworkManager.Instance.UserName 
                            +" crystal : " + NetworkManager.Instance.Craystal
                            +" numWinds : "+NetworkManager.Instance.NumWins;
    }
    public void OnClick_CreateRoom()
    {
        NetworkManager.Instance.CreateRoom();
    }
    public void OnClick_JoinRoom()
    {
        NetworkManager.Instance.JoinRoom();
    }
}
