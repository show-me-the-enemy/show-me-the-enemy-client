//battle만 테스트할때
//#define BATTLE_TEST

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InGameController : BaseElement, BaseElement.IBaseController
{
    private InGameApplication _app = new InGameApplication();

    #region lim
    public OverPopup overPopup;
    public BuildupManager buildupManager;
    public HudController hudController;
    public InGameModel gameModel;
    public MobGenerator mobGenerator;
    public List<Monster> monsters = new List<Monster>();
    [HideInInspector]
    public bool isWin = true;
    [HideInInspector]
    public string oppositeId = "";
    #endregion

    public void Init()
    {
        _app = app as InGameApplication;
        overPopup.gameObject.SetActive(false);
        buildupManager.gameObject.SetActive(false);
        gameModel.Init();
        buildupManager.Init();
        isWin = true;
        InitHandlers();
#if BATTLE_TEST
        ChangeState(EInGameState.BATTLE);
#else
        ChangeState(EInGameState.LOADING);
#endif
        NotificationCenter.Instance.AddObserver(OnNotification, ENotiMessage.InGameStartResponse);
        NotificationCenter.Instance.AddObserver(OnNotification, ENotiMessage.InGameFinishResponse);
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

        NotificationCenter.Instance.RemoveObserver(OnNotification, ENotiMessage.InGameFinishResponse);
        NotificationCenter.Instance.RemoveObserver(OnNotification, ENotiMessage.InGameStartResponse);
        NotificationCenter.Instance.RemoveObserver(OnNotification, ENotiMessage.InGameBuildUpResponse);
    }

    public void SetActive(bool flag)
    {

    }

    //이런식으로 받아서 사용하면 됌
    private void OnNotification(Notification noti)
    {
        switch (noti.msg)
        {
            case ENotiMessage.InGameBuildUpResponse:
                InGameBuildUpResponse buildUpRes = (InGameBuildUpResponse)noti.data[EDataParamKey.InGameBuildUpResponse];
                Debug.Log(buildUpRes);
                //buildUpRes.numItem
                //buildUpRes.numMonsters
                //buildUpRes.sender
                //buildUpRes.status
                break;
            case ENotiMessage.InGameFinishResponse:
                InGameStatusResponse statusRes = (InGameStatusResponse)noti.data[EDataParamKey.InGameBuildUpResponse];
                oppositeId = statusRes.secondUsername;
                //statusRes.firstUsername
                //statusRes.id
                //statusRes.secondUsername
                //statusRes.status
                //statusRes.statusCode
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
        _handlers.Add(EInGameState.DEATH, new StateHandlerDeath());
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
                Debug.LogError(enterHandler);
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

        public void OnNotification(Notification noti)
        {

        }

        public void AdvanceTime(float dt_sec)
        {
            if (NetworkManager.Instance.IsGameReady)
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
            _controller = controller;
            _currentPlayTime = 0;
            foreach (BaseElement.IBaseController ba in _controller._app.contollers)
            {
                if (ba != null) ba.Init();
            }
            foreach (Monster mob in _controller.monsters)
            {
                if (mob != null)
                    mob.Init();
            }
        }

        public void Set()
        {
            foreach (BaseElement.IBaseController ba in _controller._app.contollers)
            {
                if (ba != null) ba.Set();
            }
            foreach (Monster mob in _controller.monsters)
            {
                if (mob != null)
                    mob.Set();
            }
            Debug.Log("Start:"+_currentPlayTime);
            MobGenerator mg = _controller.mobGenerator;

            float bbt = _controller.gameModel.GetBattleTime();
            mg.SetRoundTime(bbt);
            int bt = (int)bbt;
            mg.SetMobNum("Air", 3*bt);
            mg.SetMobNum("Bat", 3*bt);
            mg.SetMobNum("BatSmall", 3*bt);

            _controller.hudController.UpdateCoinBar(true);
            roundStartTime = _currentPlayTime;
        }

        public void OnNotification(Notification noti)
        {

        }

        public void AdvanceTime(float dt_sec)
        {
            _currentPlayTime += dt_sec;

            foreach (BaseElement.IBaseController ba in _controller._app.contollers)
            {
                if (ba != null) ba.AdvanceTime(dt_sec);
            }
            foreach (Monster mob in _controller.monsters)
            {
                if (mob != null)
                    mob.AdvanceTime(dt_sec);
            }

            float prg_t = _currentPlayTime - roundStartTime;
            float percent = 1 - prg_t / _controller.gameModel.GetBattleTime();
            float remainTime = _controller.gameModel.GetBattleTime() - prg_t;
            _controller.hudController.SetTimeBar(percent, remainTime);
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
            foreach (Monster mob in _controller.monsters)
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

        //빌드업 MonstersList
        private List<int> _monsterList = new List<int>();
        //빌드업 ItemList
        private List<int> _itemList = new List<int>();

        public void Init(InGameController controller)
        {
            _controller = controller;
        }

        public void Set()
        {
            _monsterList.Clear();
            _itemList.Clear();
            //옵저버 패턴으로 OnAddBuildUp으로 오는 메세지 구독
            NotificationCenter.Instance.AddObserver(OnNotification, ENotiMessage.OnAddBuildUp);
            _controller.buildupManager.gameObject.SetActive(true);
            _controller.buildupManager.updateItems();
            _currentUpgradeTime = 0;
            _controller.hudController.UpdateCoinBar(false);
        }

        public void OnNotification(Notification noti)
        {
            if (noti.msg == ENotiMessage.OnAddBuildUp)
            {
                string type = (string)noti.data["purchase_type"];
                string name = (string)noti.data["purchase_name"];
                if(type == "monster")
                {

                }
                else if(type == "weapon")
                {

                }
                else if (type == "accessory")
                {

                }
            }
        }

        public void AdvanceTime(float dt_sec)
        {
            _currentUpgradeTime += dt_sec;

            float percent = 1 - _currentUpgradeTime / _controller.gameModel.GetBuildupTime();
            float remainTime = _controller.gameModel.GetBuildupTime()-_currentUpgradeTime;
            _controller.hudController.SetTimeBar(percent, remainTime);
            if (_currentUpgradeTime > _controller.gameModel.GetBuildupTime())
            {
#if BATTLE_TEST
#else
                //배틀스테이트 가기전에 item,monster List에 저장해둔것들 보냄
                foreach (var item in _itemList)
                {
                    NetworkManager.Instance.SendBuildUpMsg(0, item);
                }
                foreach(var monster in _monsterList)
                {
                    NetworkManager.Instance.SendBuildUpMsg(monster, 0);
                }
#endif
                _controller.ChangeState(EInGameState.BATTLE);
            }
        }

        public void Dispose()
        {
            //dispose할때 removeObserver해줘야함
            NotificationCenter.Instance.RemoveObserver(OnNotification, ENotiMessage.OnAddBuildUp);
            if (_controller.buildupManager.isPurchase)
                _controller.gameModel.AddBuyLevel();
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

        public void OnNotification(Notification noti)
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
        const float duration = 5;
        float progTime = 0;
        public void Init(InGameController controller)
        {
            _controller = controller;
        }

        public void Set()
        {
            progTime = 0;
            _controller.overPopup.gameObject.SetActive(true);
            int round = _controller.gameModel.round;
            bool isWin = _controller.isWin;
            int crystal = (isWin) ? round * 100 : 100;
            NetworkManager.Instance.GameResult(round, crystal);
            _controller.overPopup.Set(isWin, _controller.oppositeId, round, crystal);
        }

        public void OnNotification(Notification noti)
        {

        }

        public void AdvanceTime(float dt_sec)
        {
            progTime += dt_sec;
            if(progTime > duration)
                SceneManager.LoadScene("LobbyScene");
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
    void OnNotification(Notification noti);
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