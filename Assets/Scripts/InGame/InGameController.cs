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
        ChangeState(EInGameState.BATTLE);
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
            _handlers[state].Init(this);
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
        private bool _isConnectReady = false;
        private InGameController _controller;
        public void Init(InGameController controller)
        {
            _controller = controller;
            //매치서버 접속
        }

        public void Set()
        {
            //내가 준비되면 상대방에게도 신호보냄
        }

        public void AdvanceTime(float dt_sec)
        {
            if(_isConnectReady /*&&상대방 준비완료*/)
            {
                _controller.ChangeState(EInGameState.BATTLE); //배틀 스테이지로 넘어감
            }
        }

        public void Dispose()
        {
        }

    }
    protected class StateHandlerBattle : IInGameStateHandler
    {
        private float _currentPlayTime;
        private InGameController _controller;
        public void Init(InGameController controller)
        {
            _controller = controller;
            _currentPlayTime = 0;

            _controller._app.playerController.Init();
            _controller._app.cameraController.Init();
            _controller._app.tileScroller.Init();
            _controller._app.mobGenerator.Init();
            foreach (Monster mob in _controller._app.monsters)
            {
                if(mob != null )
                    mob.Init(); 
            }

        }

        public void Set()
        {

        }

        public void AdvanceTime(float dt_sec)
        {
            _currentPlayTime += dt_sec;

            _controller._app.playerController.AdvanceTime(dt_sec);
            _controller._app.cameraController.AdvanceTime(dt_sec);
            _controller._app.tileScroller.AdvanceTime(dt_sec);
            _controller._app.mobGenerator.AdvanceTime(dt_sec);
            foreach (Monster mob in _controller._app.monsters)
            {
                if(mob!= null )
                    mob.AdvanceTime(dt_sec);
            }

            /*if (_currentPlayTime >= 일정 시간이 지나면)
            {
                상대방에게도 신호를 보내고
                동기를 맞춘 후 upgrade state로 변경
                _controller.ChangeState(EInGameState.UPGRADE); //업그레이드 스테이트로 변경
            }*/

        }
        public void Dispose()
        {
        }

    }

    protected class StateHandlerUpgrade : IInGameStateHandler
    {
        private float _currentUpgradeTime = 0;
        private InGameController _controller;
        public void Init(InGameController controller)
        {
            _controller = controller;
        }

        public void Set()
        {
            _currentUpgradeTime = 0;
        }

        public void AdvanceTime(float dt_sec)
        {
            _currentUpgradeTime += dt_sec;
            /*if (_currentPlayTime >= 일정 시간이 지나면)
            {
                상대방에게도 신호를 보내고
                동기를 맞춘 후 battle state로 변경
                _controller.ChangeState(EInGameState.BATTLE); //업그레이드 스테이트로 변경
            }*/
        }

        public void Dispose()
        {
        }

    }

    protected class StateHandlerPause : IInGameStateHandler //아마 연결이 끊기면 오게되는 state???
    {
        public void Init(InGameController controller)
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
    void Init(InGameController controller);
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