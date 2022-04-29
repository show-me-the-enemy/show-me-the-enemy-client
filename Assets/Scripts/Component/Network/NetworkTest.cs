using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkTest : MonoBehaviour
{
    // Start is called before the first frame update

    private string token = null;

    private string login_id;
    private string login_password;

    private string _userName ="testchsbin";
    private string _password = "1234";
    private string _matchingPassword = "1234";

    public enum APIType
    {
        SignUp,
        Login,
        Logout
    }

    void Start()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            // 인터넷 연결이 안된경우
            ErrorCheck(-1000); // 인터넷 연결 에러
        }
        else
        {
            // 인터넷 연결이 된 경우
            StartCoroutine(APIExample(APIType.Login));
        }
    }

    IEnumerator APIExample(APIType _type)
    {
        switch (_type)
        {
            case APIType.Login:
                yield return StartCoroutine(API_Login());
                break;
            case APIType.SignUp:
                yield return StartCoroutine(API_SingUp());
                break;
        }
        yield return null;
    }

    #region API_Func
    /// <summary>
    /// API로 가입하는 함수
    /// </summary>
    IEnumerator API_SingUp()
    {
        UnityWebRequest request;
        using (request = UnityWebRequest.Post("http://ec2-3-37-203-23.ap-northeast-2.compute.amazonaws.com/api/auth/register:8080" + login_id + "&password=" + login_password, ""))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                SetToken(request.downloadHandler.text);
                if (request.responseCode != 200)
                    ErrorCheck(-(int)request.responseCode, "API_Login");
            }
        }
    }


    /// <summary>
    /// API로 로그인하여 토큰을 가져오는 함수
    /// 이때 가져온 토큰은 token 변수에 저장
    /// </summary>
    /// <returns>token = Gettoken</returns>
    IEnumerator API_Login()
    {
        UnityWebRequest request;
        using (request = UnityWebRequest.Post("http://___/login?email=" + login_id + "&password=" + login_password, ""))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                SetToken(request.downloadHandler.text);
                if (request.responseCode != 200)
                    ErrorCheck(-(int)request.responseCode, "API_Login");
            }
        }
    }

    /// <summary>
    /// API로 Logout을 하는 함수.
    /// 로그아웃시 가지고 있던 토큰값은 초기화됨.
    /// </summary>
    /// <returns>token = null</returns>
    IEnumerator API_Logout()
    {
        UnityWebRequest request;
        using (request = UnityWebRequest.Get("http://___/logout"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token);
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                SetToken(null);
                if (request.responseCode != 200)
                    ErrorCheck(-(int)request.responseCode, "API_Logout");
            }
        }
    }

    /// <summary>
    /// WWWForm을 이용한 Post API
    /// </summary>
    /// <returns></returns>
    public IEnumerator API_Post_Form()
    {
        UnityWebRequest request;

        WWWForm form = new WWWForm();
        form.AddField("userId", "JhonDo");
        form.AddField("type", "page");

        using (request = UnityWebRequest.Post("http://___", form))
        {
            request.SetRequestHeader("Id", "__");
            request.SetRequestHeader("authToken", token);
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                //Debug.Log(request.downloadHandler.text.JsonPrettyPrint());
                if (request.responseCode != 200)
                    ErrorCheck(-(int)request.responseCode, "API_Post_Form");
            }
        }
    }

    /// <summary>
    /// 파일 다운로드
    /// </summary>
    public IEnumerator API_File_Download_ForImageUpdate(int id, Image _img)
    {
        UnityWebRequest request;
        using (request = UnityWebRequest.Get("http://___/download?id=" + id))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token);

            string path = Path.Combine(Application.persistentDataPath, id + ".png");
            request.downloadHandler = new DownloadHandlerFile(path);

            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                // 이미지를 텍스처에 적용
                Texture2D tex = new Texture2D(64, 64, TextureFormat.DXT5, false);
                tex.LoadImage(request.downloadHandler.data);
                _img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

                if (request.responseCode != 200)
                    ErrorCheck(-(int)request.responseCode, "API_File_Download" + request.url);
            }
        }
    }

    int SetToken(string _input)
    {
        // 로그아웃시 토큰 초기화
        if (_input == null)
        {
            token = null;
            return 0;
        }

        // 로그인시 토큰 설정
        string[] temp = _input.Split('"');

        if (temp.Length != 5 || temp[1] != "token")
            ErrorCheck(-1001); // 토큰 형식 에러

        token = temp[3];
        return 0;
    }
    #endregion

    #region Occur Error
    int ErrorCheck(int _code)
    {
        if (_code > 0) return 0;
        else if (_code == -1000) Debug.LogError(_code + ", Internet Connect Error");
        else if (_code == -1001) Debug.LogError(_code + ", Occur token type Error");
        else if (_code == -1002) Debug.LogError(_code + ", Category type Error");
        else if (_code == -1003) Debug.LogError(_code + ", Item type Error");
        else Debug.LogError(_code + ", Undefined Error");
        return _code;
    }

    int ErrorCheck(int _code, string _funcName)
    {
        if (_code > 0) return 0;
        else if (_code == -400) Debug.LogError(_code + ", Invalid request in " + _funcName);
        else if (_code == -401) Debug.LogError(_code + ", Unauthorized in " + _funcName);
        else if (_code == -404) Debug.LogError(_code + ", not found in " + _funcName);
        else if (_code == -500) Debug.LogError(_code + ", Internal Server Error in " + _funcName);
        else Debug.LogError(_code + ", Undefined Error");
        return _code;
    }
    #endregion


    //void Start()
    //{
    //    StartCoroutine(UnityWebRequestGETTest()); 
    //    StartCoroutine(UnityWebRequestPOSTTEST());
    //}

    //IEnumerator UnityWebRequestGETTest()
    //{
    //    // GET 방식
    //    string apikey = "발급받은 API키를 넣는다.";
    //    string url = "https://api.neople.co.kr/df/servers?apikey=" + apikey;

    //    // UnityWebRequest에 내장되있는 GET 메소드를 사용한다.
    //    UnityWebRequest www = UnityWebRequest.Get(url);

    //    yield return www.SendWebRequest();  // 응답이 올때까지 대기한다.

    //    if (www.error == null)  // 에러가 나지 않으면 동작.
    //    {
    //        Debug.Log(www.downloadHandler.text);
    //    }
    //    else
    //    {
    //        Debug.Log("error");
    //    }
    //}

    //IEnumerator UnityWebRequestPOSTTEST()
    //{
    //    string url = "POST 통신을 사용할 서버 주소를 입력";
    //    WWWForm form = new WWWForm();
    //    string id = "아이디";
    //    string pw = "비밀번호";
    //    form.AddField("Username", id);
    //    form.AddField("Password", pw);
    //    UnityWebRequest www = UnityWebRequest.Post(url, form);  // 보낼 주소와 데이터 입력

    //    yield return www.SendWebRequest();  // 응답 대기

    //    if (www.error == null)
    //    {
    //        Debug.Log(www.downloadHandler.text);    // 데이터 출력
    //    }
    //    else
    //    {
    //        Debug.Log("error");
    //    }
    //}
}