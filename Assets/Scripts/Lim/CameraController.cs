using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


public class CameraController : BaseElement, BaseElement.IBaseController
{
    public Transform player;
    public Transform[] xFollowers;

    public void Init()
    {
        float wph = Screen.width / Screen.height;
        int halfX = Screen.width / 2;
        int halfY = Screen.height / 2;
        if (halfX % 2 == 1) halfX++;
        if (halfY % 2 == 1) halfY++;
        GetComponent<PixelPerfectCamera>().refResolutionX = halfX;
        GetComponent<PixelPerfectCamera>().refResolutionY = halfY;
    }

    public void AdvanceTime(float dt_sec)
    {
        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        foreach(Transform t in xFollowers)
            t.position = new Vector3(player.position.x, t.position.y, t.position.z);
    }

    public void Set()
    {
    }

    public void Dispose()
    {
    }

    public void SetActive(bool flag)
    {
        throw new System.NotImplementedException();
    }
}
