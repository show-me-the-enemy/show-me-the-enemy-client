using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameController : BaseElement, BaseElement.IBaseController
{
    private InGameApplication _app = new InGameApplication();
    #region lim
    public GameObject overPanel;
    public GameObject buildupPanel;
    private float roundStartTime;
    public float roundLength = 30;
    private float buildupStartTime;
    public float buildupLength = 10;
    public UiBarView topBar;

    public void GameOver()
    {
        StartCoroutine(PopOverRoutine());
        ChangeState(EInGameState.PAUSE);
    }
    IEnumerator PopOverRoutine()
    {
        yield return new WaitForSeconds(1f);
        overPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("LobbyScene");
    }
    #endregion

    public void Init()
    {
        _app = app as InGameApplication;
        overPanel.SetActive(false);
        buildupPanel.SetActive(false);
        InitHandlers();
        ChangeState(EInGameState.LOADING);
        NotificationCenter.Instance.AddObserver(OnNotification, ENotiMessage.InGameStatusResponse);
        NotificationCenter.Instance.AddObserver(OnNotification, ENotiMessage.InGameBuildUpResponse);
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

    //이런식으로 받아서 사용하면 됌
    private void OnNotification(Notification noti)
    {
        switch(noti.msg)
        {
            case ENotiMessage.InGameBuildUpResponse:
                
                InGameBuildUpResponse buildUpRes = (InGameBuildUpResponse)noti.data[EDataParamKey.InGameBuildUpResponse];
                Debug.Log(buildUpRes);
                //buildUpRes.numItem
                //buildUpRes.numMonsters
                //buildUpRes.sender
                //buildUpRes.status
                break;
            case ENotiMessage.InGameStatusResponse:
                InGameStatusResponse statusRes = (InGameStatusResponse)noti.data[EDataParamKey.InGameStatusResponse];
                Debug.Log(statusRes);
                //statusRes.firstUsername
                //statusRes.id
                //statusRes.secondUsername
                //statusRes.status
                //statusRes.statusCode
                break;
        }
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
        private InGameController _controller;
        public void Init(InGameController controller)
        {
            _controller = controller;
            //매치서버 접속
        }

        public void Set()
        {
            _controller._app.View.LoadingPopup.SetActive(true);
            //내가 준비되면 상대방에게도 신호보냄
        }

        public void AdvanceTime(float dt_sec)
        {
            if(NetworkManager.Instance.IsGameReady)
            {
                _controller.ChangeState(EInGameState.BATTLE); //배틀 스테이지로 넘어감
            }
        }

        public void Dispose()
        {
            _controller._app.View.LoadingPopup.SetActive(false);
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
            _controller._app.coinGenerator.Init();
            _controller._app.weaponManager.Init();
            foreach (Monster mob in _controller._app.monsters)
            {
                if(mob != null )
                    mob.Init(); 
            }
        }

        public void Set()
        {
            _controller._app.playerController.Set();
            _controller._app.mobGenerator.Set();
            foreach (Monster mob in _controller._app.monsters)
            {
                if (mob != null)
                    mob.Set();
            }
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

            float prg_t = _currentPlayTime - _controller.roundStartTime;
            _controller.topBar.setValue(1 - prg_t / _controller.roundLength);
            if(prg_t > _controller.roundLength)
            {
                _controller.roundStartTime = _currentPlayTime;
                _controller.ChangeState(EInGameState.UPGRADE);
            }
        }
        public void Dispose()
        {
            _controller._app.playerController.Dispose();
            _controller._app.mobGenerator.Dispose();
            foreach (Monster mob in _controller._app.monsters)
            {
                if (mob != null)
                    mob.Dispose();
            }
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
            _controller.buildupPanel.SetActive(true);
            _currentUpgradeTime = 0;
        }

        public void AdvanceTime(float dt_sec)
        {
            _currentUpgradeTime += dt_sec;

            float prg_t = _currentUpgradeTime - _controller.buildupStartTime;
            _controller.topBar.setValue(1 - prg_t / _controller.buildupLength);
            if (prg_t > _controller.buildupLength)
            {
                _controller.buildupStartTime = _currentUpgradeTime;
                _controller.ChangeState(EInGameState.BATTLE);
            }
        }

        public void Dispose()
        {
            _controller.buildupPanel.SetActive(false);
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