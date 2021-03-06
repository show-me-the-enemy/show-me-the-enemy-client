// battle만 테스트할때
//#define BATTLE_TEST
// 대신 project setting->player->other->ScriptingDefine

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameController : BaseElement, BaseElement.IBaseController
{
    public InGameApplication _app;

    #region lim
    public OverPopup overPopup;
    public BuildupManager buildupManager;
    public HudController hudController;
    public InGameModel gameModel;
    public MobGenerator mobGenerator;
    public List<Monster> monsters = new List<Monster>();

    public Dictionary<string, int> killMobCount = new Dictionary<string, int>();
    public Dictionary<string, int> addMobCount = new Dictionary<string, int>();
    [HideInInspector]
    public bool isWin = true;
    [HideInInspector]
    public string oppositeId = "";
    [HideInInspector]
    public string myId = "";
    [HideInInspector]
    public bool goDeath = false;
    bool oFinish = false;
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
        NotificationCenter.Instance.AddObserver(OnNotification, ENotiMessage.InGameBuildUpResponse);
        NotificationCenter.Instance.AddObserver(OnNotification, ENotiMessage.InGameFinishResponse);
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

        NotificationCenter.Instance.RemoveObserver(OnNotification, ENotiMessage.InGameStartResponse);
        NotificationCenter.Instance.RemoveObserver(OnNotification, ENotiMessage.InGameBuildUpResponse);
        NotificationCenter.Instance.RemoveObserver(OnNotification, ENotiMessage.InGameFinishResponse);
    }

    public void SetActive(bool flag)
    {

    }
    public void GoLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }
    private void ReceiveBUR(string sender, string type, string name, int count)
    {
        string msg = "";
        if (sender == myId)
        {
            switch (type)
            {
                case "monster":
                    msg = "Send the " + name + "to "+oppositeId;
                    break;
                case "weapon":
                case "accessory":
                    msg = "Purchase the "+name + ".";
                    break;
            }
            if (msg != "")
            {
                msg = "<color=cyan>" + msg + "</color>";
            }
        }
        else
        {
            switch (type)
            {
                case "monster":
                    msg = "Received the " + name + " from " +oppositeId;
                    addMobCount[name]+=count;
                    break;
                case "kill":
                    msg = oppositeId+" killed " + count + " " + name + "s.";
                    addMobCount[name] += (int)(count / 4);
                    break;
                case "weapon":
                case "accessory":
                    msg = oppositeId+" purchased the" + name + ".";
                    break;
            }
            if (msg != "")
            {
                msg = "<color=magenta>" + msg + "</color>";
            }

            if (type == "Finish")
            {
                buildupManager.AddRogText("sync with " + oppositeId);
                oFinish = true;
            }
        }
        if (msg != "")
        {
            buildupManager.AddRogText(msg);
        }
    }

    private void OnNotification(Notification noti)
    {
        switch (noti.msg)
        {
            case ENotiMessage.InGameStartResponse:
                InGameStatusResponse statusRes = (InGameStatusResponse)noti.data[EDataParamKey.InGameStatusResponseP];
                oppositeId = statusRes.secondUsername;
                break;
            case ENotiMessage.InGameBuildUpResponse: 
                InGameBuildUpResponse buildUpRes = (InGameBuildUpResponse)noti.data[EDataParamKey.InGameBuildUpResponseP];
                string sender = buildUpRes.sender;
                string type = buildUpRes.type;
                string name = buildUpRes.name;
                int count = buildUpRes.count;
                ReceiveBUR(sender, type, name, count);
                break;
            case ENotiMessage.InGameFinishResponse:
                goDeath = true;
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
            } else if(Input.GetKey(KeyCode.Escape))
            {
                // TODO: 방 끊기 코드 
                SceneManager.LoadScene("LobbyScene");
            }
        }

        public void Dispose()
        {
            _controller.myId = NetworkManager.Instance.UserName;
            if (_controller.oppositeId == "")
                _controller.oppositeId = NetworkManager.Instance.SeccondUserName;

            _controller.buildupManager.AddRogText("Connected with "+_controller.oppositeId);
            _controller.hudController.SetPlayerName(_controller.myId);
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
            AudioManager.Instance.PlayBGM("Battle" + Random.Range(1, 4), 0.7f);

            _controller.gameModel.AddRound();
            foreach (BaseElement.IBaseController ba in _controller._app.contollers)
            {
                if (ba != null) ba.Set();
            }
            foreach (Monster mob in _controller.monsters)
            {
                if (mob != null)
                    mob.Set();
            }

            MobGenerator mg = _controller.mobGenerator;
            float bbt = _controller.gameModel.GetBattleTime();
            mg.SetRoundTime(bbt);
            int bt = (int)bbt;

            string[] bisicMobs = {"Air","Bat", "BatSmall"};
            foreach (string name in mg.mobNames)
            {
                int genCount = 2 * bt + _controller.addMobCount[name];
                mg.SetMobNum(name, genCount);
            }
            mg.SetMobNum("Bread", _controller.addMobCount["Bread"]);

            foreach (string name in mg.mobNames)
            {
                _controller.addMobCount[name] = 0;
                _controller.killMobCount[name] = 0;
            }

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
                _controller.ChangeState(EInGameState.UPGRADE);
            }
            else if( _controller.goDeath)
            {
                _controller.ChangeState(EInGameState.DEATH);
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
#if BATTLE_TEST
#else
            if (_controller._currentState == EInGameState.UPGRADE)
            {
                foreach (string key in _controller.mobGenerator.mobNames)
                {
                    int count = _controller.killMobCount[key];
                    if(count==0) continue;
                    string type = "kill";
                    string name = key;
                    NetworkManager.Instance.SendBuildUpMsg(type, name, count);
                }
            }
#endif
        }

    }

    protected class StateHandlerUpgrade : IInGameStateHandler
    {
        private float _currentUpgradeTime = 0;
        private InGameController _controller;
        private bool imFinish = false;

        //빌드업 MonstersList
        private List<InGameBuildUpResponse> sendMsgs = new List<InGameBuildUpResponse>();

        public void Init(InGameController controller)
        {
            _controller = controller;
        }

        public void Set()
        {
            AudioManager.Instance.PlayBGM("Buildup");
            sendMsgs.Clear();
            _controller.buildupManager.gameObject.SetActive(true);
            _controller.buildupManager.updateItems();
            _currentUpgradeTime = 0;
            _controller.hudController.UpdateCoinBar(false);
            _controller.oFinish = false;
            imFinish = false;
        }

        public void OnNotification(Notification noti)
        {
            
        }

        public void AdvanceTime(float dt_sec)
        {
            _currentUpgradeTime += dt_sec;

            float percent = 1 - _currentUpgradeTime / _controller.gameModel.GetBuildupTime();
            float remainTime = _controller.gameModel.GetBuildupTime()-_currentUpgradeTime;
            _controller.hudController.SetTimeBar(percent, remainTime);
            _controller.buildupManager.AdvanceTime(dt_sec);

#if BATTLE_TEST
            if (_currentUpgradeTime > _controller.gameModel.GetBuildupTime())
            {
                _controller.ChangeState(EInGameState.BATTLE);
            }
#else
            if (_currentUpgradeTime > _controller.gameModel.GetBuildupTime())
            {
                if (_controller.oFinish && imFinish)
                {
                    _controller.ChangeState(EInGameState.BATTLE);
                }
                else if(!imFinish)
                {
                    imFinish = true;
                    NetworkManager.Instance.SendBuildUpMsg("Finish", "", 0);
                }
            }
#endif

            else if (_controller.goDeath)
            {
                _controller.ChangeState(EInGameState.DEATH);
            }
        }

        public void Dispose()
        {
            if (_controller.buildupManager.isPurchase)
                _controller.gameModel.AddBuyLevel();
            _controller.buildupManager.gameObject.SetActive(false);

            NotificationCenter.Instance.RemoveObserver(OnNotification, ENotiMessage.InGameBuildUpResponse);
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
        const float duration = 10;
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
            if (isWin)
            {
                AudioManager.Instance.PlayBGM("OverWin");
            }
            else
            {
                AudioManager.Instance.PlayBGM("OverLose");
            }
            int crystal = (isWin) ? (round+1) * 100 : 0;
#if BATTLE_TEST
#else
            NetworkManager.Instance.GameResult(round, crystal);
#endif
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