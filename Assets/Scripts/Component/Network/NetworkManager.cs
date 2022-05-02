using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

    public void OnNotification(Notification noti)
    {
        if(noti.data[EDataParamKey.UserLoginRequest]!=null)
        {
            Debug.Log("On Noti UserLoginRequest"); 
            UserLoginRequest request = noti.data[EDataParamKey.UserLoginRequest] as UserLoginRequest;
            StartCoroutine(API_Login(request));
        }
        else if(noti.data[EDataParamKey.UserSignUpRequest] != null)
        {
            Debug.Log("On Noti UserSignUpRequest");
            UserSignUpRequest request = noti.data[EDataParamKey.UserSignUpRequest] as UserSignUpRequest;
            StartCoroutine(API_SignUp(request));
        }
    }

    IEnumerator API_SignUp(UserSignUpRequest userRequest)
    {
        string url = "http://ec2-3-37-203-23.ap-northeast-2.compute.amazonaws.com:8080/api/auth/register";
        string json = JsonUtility.ToJson(userRequest);
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
    IEnumerator API_Login(UserLoginRequest userRequest)
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
                Debug.Log(res.accessToken);
                Debug.Log(res.message);
                Debug.Log(res.refreshToken);
                Debug.Log(res.statusCode);

            }
        }
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
#endregion