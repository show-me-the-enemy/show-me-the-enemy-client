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
    private Button _loginBtn;
    [SerializeField]
    private Button _signUpBtn;
    [SerializeField]
    private InputField _usernameInput;
    [SerializeField]
    private InputField _passwordInput;
    [SerializeField]
    private InputField _matchingPasswordInput;

    public void OnClick_Login()
    {
        Debug.Log("OnClick_Login");
        UserLoginRequest info = new UserLoginRequest();
        info.username = _usernameInput.text;
        info.password = _passwordInput.text;

        Hashtable sendData = new Hashtable();
        sendData.Add(EDataParamKey.UserLoginRequest, info);
        NotificationCenter.Instance.PostNotification(Notification.Instantiate(ENotiMessage.NetworkRequestLogin, sendData));
    }
    public void OnClick_SignUp()
    {
        Debug.Log("OnClick_Signup");
        UserSignUpRequest info = new UserSignUpRequest();
        info.username = _usernameInput.text;
        info.password = _passwordInput.text;
        info.matchingPassword = _matchingPasswordInput.text;

        Hashtable sendData = new Hashtable();
        sendData.Add(EDataParamKey.UserSignUpRequest, info);
        NotificationCenter.Instance.PostNotification(Notification.Instantiate(ENotiMessage.NetworkRequestSignUp, sendData));
    }
}
