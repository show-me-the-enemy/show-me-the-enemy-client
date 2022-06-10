using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkUserModel
{
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
    public string type;
    public string name;
    public int count;
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
    public string type;
    public string name;
    public int count;
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


[System.Serializable]
public class TopTenUserRank
{
    public int rank;
    public string username;
    public int numWins;
    public int maxRound;
}

[System.Serializable]
public class TopTenUsersRankResponse
{
    public List<TopTenUserRank> Items;
}
#endregion