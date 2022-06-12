using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoginController : MonoBehaviour
{
    //원래 이부분은 view에 들어가야 마땅하나 login scene에 하도 큰 내용이 없어서
    //이 부분만 예외적으로 controller안에서 해결해두고
    //나중에 혹시 볼륨이 커지면 그때가서 분류
    [SerializeField]
    private GameObject _loginPopup;
    [SerializeField]
    private GameObject _signupPopup;

    [SerializeField]
    private InputField _usernameInputLogin;
    [SerializeField]
    private InputField _passwordInputLogin;
    [SerializeField]
    private Button _loginBtn;

    [SerializeField]
    private InputField _usernameInputSignUp;
    [SerializeField]
    private InputField _passwordInputSignUp;
    [SerializeField]
    private InputField _matchingPasswordInputSignUp;
    [SerializeField]
    private Button _signUpBtn;

    public void Start()
    {
        SetResol(0);
    }
    public void SetResol(int i)
    {
        switch (i)
        {
            case 0:
                Screen.SetResolution(1280, 720, false);
                break;
            case 1:
                Screen.SetResolution(1920, 1080, false);
                break;
        }
    }
    public void Awake()
    {
        AudioManager.Instance.PlayBGM("Lobby");
        FindObjectOfType<Slider>().onValueChanged.AddListener(SetVolume);
        FindObjectOfType<Dropdown>().onValueChanged.AddListener(SetResol);
    }

    public void OnClick_PopupActive(string input)
    {
        switch (input)
        {
            case "Login":
                _loginPopup.SetActive(true);
                break;
            case "Signup":
                _signupPopup.SetActive(true);
                break;
        }
    }
    public void OnClick_Login()
    {
        UserLoginRequest info = new UserLoginRequest();
        info.username = _usernameInputLogin.text;
        info.password = _passwordInputLogin.text;

        Hashtable sendData = new Hashtable();
        sendData.Add(EDataParamKey.UserLoginRequest, info);
        NotificationCenter.Instance.PostNotification(Notification.Instantiate(ENotiMessage.NetworkRequestLogin, sendData));
        _loginPopup.SetActive(false);
    }
    public void OnClick_SignUp()
    {
        UserSignUpRequest info = new UserSignUpRequest();
        info.username = _usernameInputSignUp.text;
        info.password = _passwordInputSignUp.text;
        info.matchingPassword = _matchingPasswordInputSignUp.text;

        Hashtable sendData = new Hashtable();
        sendData.Add(EDataParamKey.UserSignUpRequest, info);
        NotificationCenter.Instance.PostNotification(Notification.Instantiate(ENotiMessage.NetworkRequestSignUp, sendData));
        _signupPopup.SetActive(false);
    }

    public void OnClick_ClosePopup()
    {
        _loginPopup.SetActive(false);
        _signupPopup.SetActive(false);
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
    }

}
