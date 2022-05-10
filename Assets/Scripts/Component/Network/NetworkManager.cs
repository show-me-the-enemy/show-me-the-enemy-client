using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using WebSocketSharp;
using StompHelper;
public class NetworkManager : MonoBehaviour
{
    #region Singelton
    private static NetworkManager _instance;
    public static NetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NetworkManager>();
                if (FindObjectsOfType<NetworkManager>().Length > 1)
                {
                    Debug.LogError("[Singleton] Something went really wrong " +
                        " - there should never be more than 1 singleton!" +
                        " Reopening the scene might fix it.");
                    return _instance;
                }

                if (_instance == null)
                {
                    GameObject go = new GameObject("Default NetworkManager");
                    _instance = go.AddComponent<NetworkManager>();
                }
            }

            return _instance;
        }
    }
    #endregion

    #region context
    private string _username;
    public string UserName
    {
        get
        {
            return _username;
        }
    }
    private string _password;
    public string Password
    {
        get
        {
            return _password;
        }
    }
    private string _accessToken;
    public string AccessToken
    {
        get
        {
            return _accessToken;
        }
    }
    private string _refreshToken;
    public string RefreshToken
    {
        get
        {
            return _refreshToken;
        }
    }
    #endregion

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        NotificationCenter.Instance.AddObserver(OnNotification, ENotiMessage.NetworkRequestLogin);
        NotificationCenter.Instance.AddObserver(OnNotification, ENotiMessage.NetworkRequestSignUp);
    
    }

    public void CreateRoom()
    {
        StartCoroutine(API_RoomCreate());
    }
    public void JoinRoom()
    {
        StartCoroutine(API_RoomJoin());
    }

    public void OnNotification(Notification noti)
    {
        if(noti.data[EDataParamKey.UserLoginRequest]!=null)
        {
            UserLoginRequest request = noti.data[EDataParamKey.UserLoginRequest] as UserLoginRequest;
            StartCoroutine(API_Login(request, (callback) =>
            {
                SceneManager.LoadScene("LobbyScene");
            }));
        }
        else if(noti.data[EDataParamKey.UserSignUpRequest] != null)
        {
            UserSignUpRequest request = noti.data[EDataParamKey.UserSignUpRequest] as UserSignUpRequest;
            StartCoroutine(API_SignUp(request));
        }
    }

    #region Login Register
    IEnumerator API_SignUp(UserSignUpRequest userRequest)
    {
        string url = "http://ec2-3-37-203-23.ap-northeast-2.compute.amazonaws.com:8080/api/auth/register";
        string json = JsonUtility.ToJson(userRequest);
        Debug.Log(json);
        using (UnityWebRequest request = UnityWebRequest.Post(url, json))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
                Debug.LogError(request.downloadHandler.text);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
            }
        }

    }
    IEnumerator API_Login(UserLoginRequest userRequest, Action<UnityWebRequest> callback)
    {
        string url = "http://ec2-3-37-203-23.ap-northeast-2.compute.amazonaws.com:8080/api/auth/login";
        WWWForm form = new WWWForm();
        form.AddField("username", userRequest.username);
        form.AddField("password", userRequest.password);
        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
                Debug.LogError(request.downloadHandler.text);
            }
            else
            {
                UserLoginResponse res = JsonUtility.FromJson<UserLoginResponse>(request.downloadHandler.text);
                _refreshToken = res.refreshToken;
                _accessToken = res.accessToken;
                _username = userRequest.username;
                _password = userRequest.password;
                string json = JsonUtility.ToJson(res);
                Debug.Log(json);
                callback(request);
            }
        }
    }
    #endregion

    #region GameRoom
    IEnumerator API_RoomCreate()
    {
        string url = "http://ec2-3-37-203-23.ap-northeast-2.compute.amazonaws.com:8080/api/games/start/"+_username;
        WWWForm form = new WWWForm();
        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer "+_accessToken);

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
                Debug.LogError(request.downloadHandler.text);
            }
            else
            {
                GameRoomResponse res = JsonUtility.FromJson<GameRoomResponse>(request.downloadHandler.text);
                string json = JsonUtility.ToJson(res);
                Debug.Log(json);
                ConnectSocket(res.id);
                SceneManager.LoadScene("SampleScene");
            }
        }
    }
    IEnumerator API_RoomJoin()
    {
        string url = "http://ec2-3-37-203-23.ap-northeast-2.compute.amazonaws.com:8080/api/games/connect/random/" + _username;
        WWWForm form = new WWWForm();
        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer " + _accessToken);

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
                Debug.LogError(request.downloadHandler.text);
            }
            else
            {
                GameRoomResponse res = JsonUtility.FromJson<GameRoomResponse>(request.downloadHandler.text);
                Debug.Log(res.id); Debug.Log(res.statusCode); Debug.Log(res.firstUsername); Debug.Log(res.secondUsername); Debug.Log(res.status);
                ConnectSocket(res.id);
                SceneManager.LoadScene("SampleScene");
            }
        }
    }
    #endregion

    private void ConnectSocket(int id)
    {
        var ws = new WebSocket("ws://ec2-3-37-203-23.ap-northeast-2.compute.amazonaws.com:8080/ws-gameplay/websocket");
        ws.OnMessage += ws_OnMessage;
        ws.OnOpen += ws_OnOpen;
        ws.OnError += ws_OnError;
        ws.Connect();

        StompMessageSerializer serializer = new StompMessageSerializer();
        var sub = new StompMessage("SUBSCRIBE");
        sub["id"] = id.ToString();
        sub["destination"] = "/sub/games/"+id.ToString();
        ws.Send(serializer.Serialize(sub));
        Console.ReadKey(true);

        var sub2 = new StompMessage("SUBSCRIBE");
        sub2["destination"] = "/sub/games/" + id.ToString();
        ws.Send(serializer.Serialize(sub2));
        Console.ReadKey(true);
    }

    private void ws_OnOpen(object sender, EventArgs e)
    {
        Debug.Log(DateTime.Now.ToString() + " ws_OnOpen says: " + e.ToString());
        
    }

    private void ws_OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log("-----------------------------");
        Debug.Log(DateTime.Now.ToString() + " ws_OnMessage says: " + e.Data);

    }

    private void ws_OnError(object sender, ErrorEventArgs e)
    {
        Debug.Log(DateTime.Now.ToString() + " ws_OnError says: " + e.Message);
    }
}


//나중에 다른곳으로 따로 빼야할듯
#region network Request class
[System.Serializable]
public class UserLoginRequest
{
    public string username;
    public string password;
}

[System.Serializable]
public class UserSignUpRequest
{
    public string username;
    public string password;
    public string matchingPassword;
}
#endregion

#region network Response class
[System.Serializable]
public class UserLoginResponse
{
    public int statusCode;
    public string message;
    public string accessToken;
    public string refreshToken;
}
[System.Serializable]
public class GameRoomResponse
{
    public int statusCode;
    public int id;
    public string firstUsername;
    public string secondUsername;
    public string status;
}
#endregion