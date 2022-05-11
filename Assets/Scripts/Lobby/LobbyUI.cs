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
        _txtTopBarName.text = "USERNAME : "+NetworkManager.Instance.UserName;
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
