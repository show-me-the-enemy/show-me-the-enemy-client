using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Transform[] xFollowers;

    public void Init()
    {
    }

    public void AdvanceTime(float dt_sec)
    {
        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        foreach(Transform t in xFollowers)
            t.position = new Vector3(player.position.x, t.position.y, t.position.z);
    }
}
