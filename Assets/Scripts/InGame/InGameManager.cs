using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour, System.IDisposable
{
    [SerializeField]
    private InGameApplication _inGameApp;

    void Start()
    {
        _inGameApp.Init();
        _inGameApp.Set();
    }

    // Update is called once per frame
    void Update()
    {
        _inGameApp.AdvanceTime(Time.deltaTime);
    }


    public void Dispose()
    {
        _inGameApp.Dispose();
    }
}
