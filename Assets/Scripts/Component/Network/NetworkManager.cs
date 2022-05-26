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

    #region API context
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

    private bool _isIngameDie = false;
    public bool isIngameDie
    {
        get
        {
            return _isIngameDie;
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
        _isIngameDie = false;
        StartCoroutine(API_RoomCreate());
    }
    public void JoinRoom()
    {
        _isIngameDie = false;
        StartCoroutine(API_RoomJoin());
    }

    public void PlayerDie()
    {
        _isIngameDie = true;
        StartCoroutine(API_PlayerDie());
    }

    public void GameResult(int round)
    {
        UserGameResultRequest req = new UserGameResultRequest();
        req.username = _username;
        req.gameId = _currentRoomId;
        req.won = !_isIngameDie;
        if (req.won)
            req.crystal = 100;
        else
            req.crystal = 50;
        req.numRound = round;
        StartCoroutine(API_GameResult(req));
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

    #region Login Register API
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

    #region GameRoom API
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
                _isGameReady = false;
                _currentRoomId = res.id;
                ConnectSocket();
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
                _isGameReady = true;
                _currentRoomId = res.id;
                ConnectSocket();
                SceneManager.LoadScene("SampleScene");
            }
        }
    }
    #endregion

    #region GamePlay API
    IEnumerator API_PlayerDie()
    {
        string url = "http://ec2-3-37-203-23.ap-northeast-2.compute.amazonaws.com:8080/api/games/" + _currentRoomId;
        byte[] myData = System.Text.Encoding.UTF8.GetBytes("This is some test data");
        using (UnityWebRequest request = UnityWebRequest.Put(url, myData))
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
                string json = JsonUtility.ToJson(res);
                Debug.Log(json);
            }
        }
    }
    IEnumerator API_GameResult(UserGameResultRequest userRequest)
    {
        string url = "http://ec2-3-37-203-23.ap-northeast-2.compute.amazonaws.com:8080/api/users";
        string json = JsonUtility.ToJson(userRequest);
        Debug.LogError("b");
        using (UnityWebRequest request = UnityWebRequest.Put(url, json))
        {
            Debug.LogError("c");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + _accessToken);

            yield return request.SendWebRequest();

            Debug.LogError("d");
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
                Debug.LogError(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("c");
                UserGameResultResponse res = JsonUtility.FromJson<UserGameResultResponse>(request.downloadHandler.text);
                string resJson = JsonUtility.ToJson(res);
                Debug.Log(resJson);
                //DisconnectSocket();
            }
        }
    }
    #endregion

    #region WebSocket
    private WebSocket ws;
    private bool _isGameReady = false;
    private int _currentRoomId;
    public bool IsGameReady
    {
        get
        {
            return _isGameReady;
        }
    }
    private void ConnectSocket()
    {
        ws = new WebSocket("ws://ec2-3-37-203-23.ap-northeast-2.compute.amazonaws.com:8080/ws-gameplay/websocket");
        ws.OnMessage += ws_OnMessage;
        ws.OnOpen += ws_OnOpen;
        ws.OnError += ws_OnError;
        ws.Connect();

        StompMessageSerializer serializer = new StompMessageSerializer();

        var connect = new StompMessage("CONNECT");
        connect["accept-version"] = "1.2";
        connect["host"] = "";
        ws.Send(serializer.Serialize(connect));

        var sub = new StompMessage("SUBSCRIBE");
        sub["id"] = "sub-" + _currentRoomId.ToString();
        sub["destination"] = "/sub/games/"+ _currentRoomId.ToString();
        ws.Send(serializer.Serialize(sub));
        Console.ReadKey(true);
    }

    public void DisconnectSocket()
    {
        try
        {
            if (ws == null)
            {
                return;
            }
            if (ws.IsAlive)
            {
                ws.OnMessage -= ws_OnMessage;
                ws.OnOpen -= ws_OnOpen;
                ws.OnError -= ws_OnError;
                ws.Close();
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    private void ws_OnOpen(object sender, EventArgs e)
    {
        Debug.Log(DateTime.Now.ToString() + " ws_OnOpen says: " + e.ToString());
    }

    private void ws_OnMessage(object sender, MessageEventArgs e)
    {
        //Debug.Log("-----------------------------");
        StompMessageSerializer serializer = new StompMessageSerializer();

        var msg = serializer.Deserialize(e.Data);
        switch (msg.Command)
        {
            case "CONNECTED":
                Debug.Log(msg.Body);
                break;
            case "MESSAGE":
                if(msg.Headers["game-status"] =="start" || msg.Headers["game-status"] == "finish")
                {
                    _isGameReady = true;
                    InGameStatusResponse res = JsonUtility.FromJson<InGameStatusResponse>(msg.Body);
                    Hashtable sendData = new Hashtable();
                    sendData.Add(EDataParamKey.InGameStatusResponse, res);
                    NotificationCenter.Instance.PostNotification(ENotiMessage.InGameStatusResponse, sendData);
                }
                else
                {
                    InGameBuildUpResponse res = JsonUtility.FromJson<InGameBuildUpResponse>(msg.Body);
                    Hashtable sendData = new Hashtable();
                    sendData.Add(EDataParamKey.InGameBuildUpResponse, res);
                    NotificationCenter.Instance.PostNotification(ENotiMessage.InGameBuildUpResponse, sendData);
                }
                
                //foreach(var h in msg.Headers.Keys)
                //{
                //    Debug.Log("Key : " + h + ", Value : " + msg.Headers[h]);
                //}
                //Debug.Log(msg.Body);
                break;
            default:
                Debug.LogError("msg.Command 설정하시오");
                break;
        }

    }

    private void ws_OnError(object sender, ErrorEventArgs e)
    {
        Debug.Log(DateTime.Now.ToString() + " ws_OnError says: " + e.Message);
    }

    public void SendBuildUpMsg(int nMonsters, int nItem)
    {
        StompMessageSerializer serializer = new StompMessageSerializer();
        var request = new InGameBuildUpRequest() { id = _currentRoomId, sender = _username,numMonsters = nMonsters,numItem= nItem };
        var broad = new StompMessage("SEND", JsonUtility.ToJson(request));
        broad["content-type"] = "application/json";
        broad["destination"] = "/pub/build-up";
        ws.Send(serializer.Serialize(broad));
    }
    #endregion
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
[System.Serializable]
public class InGameBuildUpRequest
{
    public int id;
    public string sender;
    public int numMonsters;
    public int numItem;
}

[System.Serializable]
public class UserGameResultRequest
{
    public long gameId;
    public string username;
    public int numRound;
    public int crystal;
    public bool won;
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
[System.Serializable]
public class InGameBuildUpResponse
{
    public string sender;
    public string status;
    public int numMonsters;
    public int numItem;
}
[System.Serializable]
public class InGameStatusResponse
{
    public int statusCode;
    public int id;
    public string firstUsername;
    public string secondUsername;
    public string status;
}
[System.Serializable]
public class UserGameResultResponse
{
    public int statusCode;
    public string username;
    public int maxRound;
    public int crystal;
    public int numWins;
}
#endregion