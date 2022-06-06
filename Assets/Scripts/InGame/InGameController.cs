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
    public BuildupManager buildupManager;
    public HudController hudController;
    public InGameModel gameModel;

    public void GameOver()
    {
        StartCoroutine(PopOverRoutine());
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
        buildupManager.gameObject.SetActive(false);
        gameModel.Init();
        buildupManager.Init();
        InitHandlers();
#if UNITY_EDITOR
        ChangeState(EInGameState.BATTLE);

#else
        ChangeState(EInGameState.LOADING);
#endif
        NotificationCenter.Instance.AddObserver(OnNotification, ENotiMessage.InGameStatusResponse);
        NotificationCenter.Instance.AddObserver(OnNotification, ENotiMessage.InGameBuildUpResponse);
        NotificationCenter.Instance.AddObserver(OnNotification, ENotiMessage.InGameFinished);
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
            case ENotiMessage.InGameFinished:
                ChangeState(EInGameState.DEATH);
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
        _handlers.Add(EInGameState.DEATH, new StateHandlerDeath());

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
        // 전체 플레이시간
        private float _currentPlayTime;
        private float roundStartTime=0;
        private InGameController _controller;
        public void Init(InGameController controller)
        {
            Debug.Log("battle init");
            _controller = controller;
            _currentPlayTime = 0;
            foreach(BaseElement.IBaseController ba in _controller._app.contollers)
            {
                if (ba != null) ba.Init();
            }
            foreach (Monster mob in _controller._app.monsters)
            {
                if(mob != null )
                    mob.Init(); 
            }
        }

        public void Set()
        {
            foreach (BaseElement.IBaseController ba in _controller._app.contollers)
            {
                if (ba != null) ba.Set();
            }
            foreach (Monster mob in _controller._app.monsters)
            {
                if (mob != null)
                    mob.Set();
            }

            _controller.hudController.UpdateCoinBar(true);
            roundStartTime = _currentPlayTime;
        }

        public void AdvanceTime(float dt_sec)
        {
            _currentPlayTime += dt_sec;

            foreach (BaseElement.IBaseController ba in _controller._app.contollers)
            {
                if (ba != null) ba.AdvanceTime(dt_sec);
            }
            foreach (Monster mob in _controller._app.monsters)
            {
                if(mob!= null )
                    mob.AdvanceTime(dt_sec);
            }

            float prg_t = _currentPlayTime - roundStartTime;
            float percent = 1 - prg_t / _controller.gameModel.GetBattleTime();
            _controller.hudController.SetTimeBar(percent, prg_t);
            if(prg_t > _controller.gameModel.GetBattleTime())
            {
                _controller.gameModel.AddRound();
                _controller.ChangeState(EInGameState.UPGRADE);
            }
        }
        public void Dispose()
        {

            foreach (BaseElement.IBaseController ba in _controller._app.contollers)
            {
                if (ba != null) ba.Dispose();
            }
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
            _controller.buildupManager.gameObject.SetActive(true);
            _controller.buildupManager.updateItems();
            _currentUpgradeTime = 0;
            _controller.hudController.UpdateCoinBar(false);
        }

        public void AdvanceTime(float dt_sec)
        {
            _currentUpgradeTime += dt_sec;

            float percent = 1 - _currentUpgradeTime / _controller.gameModel.GetBuildupTime();

            _controller.hudController.SetTimeBar(percent, _currentUpgradeTime);
            if (_currentUpgradeTime > _controller.gameModel.GetBuildupTime())
            {
                _controller.ChangeState(EInGameState.BATTLE);
            }
        }

        public void Dispose()
        {
            _controller.buildupManager.gameObject.SetActive(false);
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
    protected class StateHandlerDeath : IInGameStateHandler 
    {
        InGameController _controller;
        public void Init(InGameController controller)
        {
            _controller = controller;
        }

        public void Set()
        {
            //NetworkManager.Instance.PlayerDie();
            _controller.GameOver();
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
    DEATH,
}