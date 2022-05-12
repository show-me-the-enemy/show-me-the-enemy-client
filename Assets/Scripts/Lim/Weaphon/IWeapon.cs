using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IWeapon : MonoBehaviour
{
    protected Transform player;
    protected Transform fireRoot;
    public Sprite icon;

    public void Init(Transform _player, Transform _fireRoot)
    {
        player = _player;
        fireRoot = _fireRoot;
    }

    public abstract void Attack();
    public abstract string AtUpdate(float dt_sec);
}
