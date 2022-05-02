using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkUserModel
{
    private string _username;
    public string UserName
    {
        get
        {
            return _username;
        }
        set
        {
            _username = value;
        }
    }
    private string _password;
    public string Password
    {
        get
        {
            return _password;
        }
        set
        {
            _password = value;
        }
    }
    private string _accessToken = null;
    public string AccessToken
    {
        get
        {
            return _accessToken;
        }
        set
        {
            _accessToken = value;
        }
    }
    private string _refreshToken = null;
    public string RefreshToken
    {
        get
        {
            return _refreshToken;
        }
        set
        {
            _refreshToken = value;
        }
    }


}
