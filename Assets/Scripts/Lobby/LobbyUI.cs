using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    public void OnClick_CreateRoom()
    {
        NetworkManager.Instance.CreateRoom();
    }
    public void OnClick_JoinRoom()
    {
        NetworkManager.Instance.JoinRoom();
    }
}
