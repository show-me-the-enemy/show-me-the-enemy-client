using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameController : BaseElement, BaseElement.IBaseController
{
    private InGameApplication _app = new InGameApplication();
    public void Init()
    {
        _app = app as InGameApplication;
        InitHandlers();
        ChangeState(EInGameState.LOADING);
    }

    public void Set()
    {

    }

    public void AdvanceTime(float dt_sec)
    {
        if (_currentState != EInGameState.UNKNOWN)
        {
            GetStateHandler(_currentState).AdvanceTime(Time.deltaTime);
        }
    }

    public void Dispose()
    {
        if (_currentState != EInGameState.UNKNOWN)
        {
            GetStateHandler(_currentState).Dispose();
        }
    }

    public void SetActive(bool flag)
    {

    }


    #region State Handlers Base
    private Dictionary<EInGameState, IInGameStateHandler> _handlers = new Dictionary<EInGameState, IInGameStateHandler>();
    private EInGameState _currentState = EInGameState.UNKNOWN;

    private void InitHandlers()
    {
        _handlers.Clear();
        _handlers.Add(EInGameState.LOADING, new StateHandlerLoading());
        _handlers.Add(EInGameState.BATTLE, new StateHandlerBattle());
        _handlers.Add(EInGameState.UPGRADE, new StateHandlerUpgrade());
        _handlers.Add(EInGameState.PAUSE, new StateHandlerPause());

        foreach (EInGameState state in _handlers.Keys)
        {
            _handlers[state].Init();
        }
    }


    private void ChangeState(EInGameState nextState)
    {
        if (nextState != EInGameState.UNKNOWN && nextState != _currentState)
        {
            EInGameState prevState = _currentState;
            _currentState = nextState;
            IInGameStateHandler leaveHandler = GetStateHandler(prevState);
            if (leaveHandler != null)
            {
                leaveHandler.Dispose();
            }
            IInGameStateHandler enterHandler = GetStateHandler(_currentState);
            if (enterHandler != null)
            {
                enterHandler.Set();
            }
        }
    }

    private IInGameStateHandler GetStateHandler(EInGameState state)
    {
        if (_handlers.ContainsKey(state))
        {
            return _handlers[state];
        }
        return null;
    }
    #endregion

    #region State Handler Class
    protected class StateHandlerLoading : IInGameStateHandler
    {
        public void Init()
        {
        }

        public void Set()
        {
        }

        public void AdvanceTime(float dt_sec)
        {
        }

        public void Dispose()
        {
        }

    }

    protected class StateHandlerBattle : IInGameStateHandler
    {
        public void Init()
        {
        }

        public void Set()
        {
        }

        public void AdvanceTime(float dt_sec)
        {
        }

        public void Dispose()
        {
        }

    }

    protected class StateHandlerUpgrade : IInGameStateHandler
    {
        public void Init()
        {
        }

        public void Set()
        {
        }

        public void AdvanceTime(float dt_sec)
        {
        }

        public void Dispose()
        {
        }

    }

    protected class StateHandlerPause : IInGameStateHandler
    {
        public void Init()
        {
        }

        public void Set()
        {
        }

        public void AdvanceTime(float dt_sec)
        {
        }

        public void Dispose()
        {
        }

    }
    #endregion
}
public interface IInGameStateHandler
{
    void Init();
    void Set();
    void AdvanceTime(float dt_sec);
    void Dispose();
}

public enum EInGameState
{
    UNKNOWN,
    LOADING,
    BATTLE,
    UPGRADE,
    PAUSE,
}