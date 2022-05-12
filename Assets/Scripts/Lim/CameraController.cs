﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


public class CameraController : MonoBehaviour
{
    public Transform player;
    public Transform[] xFollowers;

    public void Init()
    {
        float wph = Screen.width / Screen.height;
        GetComponent<PixelPerfectCamera>().refResolutionX = Screen.width / 2;
        GetComponent<PixelPerfectCamera>().refResolutionY = Screen.height / 2;
    }

    public void AdvanceTime(float dt_sec)
    {
        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        foreach(Transform t in xFollowers)
            t.position = new Vector3(player.position.x, t.position.y, t.position.z);
    }
}
