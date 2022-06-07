using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameApplication : BaseApplication    
{
    [SerializeField]
    private InGameModel _model;
    [SerializeField]
    private InGameView _view;
    [SerializeField]
    private InGameController _controller;

    public List<BaseElement> contollers;
    public List<Monster> monsters = new List<Monster>();

    public InGameModel Model
    {
        get { return _model; }
    }

    public InGameView View
    {
        get { return _view; }
    }
    public InGameController Controller
    {
        get { return _controller; }
    }

    public override void Init()
    {
        _controller.Init();
    }

    public override void Set()
    {
        _controller.Set();
    }

    public override void AdvanceTime(float dt_sec)
    {
        _controller.AdvanceTime(dt_sec);
    }

    public override void Dispose()
    {
        _controller.Dispose();
    }
}