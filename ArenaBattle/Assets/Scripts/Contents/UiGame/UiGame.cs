using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Devil.Gui;
using System;
using System.Linq;
using Devil.Common;
using UnityEngine.EventSystems;
using MonsterLove.StateMachine;
using DG.Tweening;

public partial class UiGame : UiBase<UiGame>
{
    private enum State
    {
        None,
        SetRound,
        SetTurn,
    }
    private struct SkillDescription
    {
        //Top
        public Text levelLabel;
        public Text namingLabel;
        public Text description;

        //Bottom
        public Text turnActive;
        public Text coolTime;
        public Text nextAction;
    }

    protected struct InfoUIData
    {
        public Transform trans;
        public Slider hp;
        public Image[] buffArea;
        public Image[] stackArea;
        public GameObject select;
    }

    protected struct BurstGaugeData
    {
        public Slider gauge;
        public Text gaugeText;
    }

    [SerializeField] private GameObject roundSlider = null;
    [SerializeField] private GameObject skillUI = null;
    [SerializeField] private GameObject descriptionSkill = null;
    [SerializeField] private GameObject roundObject = null;
    [SerializeField] private GameObject skillUIProfile = null;
    [SerializeField] private GameObject characterInfoParent = null;
    [SerializeField] private Button passButton = null;
    [SerializeField] private GameObject burstUI = null;
    
    private SkillDescription skillDescription;
    private BurstGaugeData burstData;

    private List<Action> setRound = new List<Action>();
    private List<Action> setTurn = new List<Action>();
    private List<Action> actionState = new List<Action>();


    private State currentState = State.None;
    private int actionIndex = 0;

    protected Dictionary<CharacterBase, InfoUIData> characterInfo = new Dictionary<CharacterBase, InfoUIData>();

    protected override void Awake()
    {
        base.Awake();
    }

    private void SetUIGameObject()
    {
        descriptionSkill.SetActive(false);

        GameObject top = descriptionSkill.transform.Find("Top").gameObject;
        GameObject location = top.transform.Find("Location").gameObject;
        GameObject label = top.transform.Find("Label").gameObject;
        GameObject tag = top.transform.Find("Tag").gameObject;
        GameObject skillDesc = top.transform.Find("SkillDescription").gameObject;

        GameObject bottom = descriptionSkill.transform.Find("Bottom").transform.Find("LayoutGroup").gameObject;
        GameObject turnActive = bottom.transform.Find("TurnActive").gameObject;
        GameObject coolTime = bottom.transform.Find("CoolTime").gameObject;
        GameObject nextAction = bottom.transform.Find("NextAction").gameObject;
        
        skillDescription = new SkillDescription();

        // Top Set
        skillDescription.namingLabel = label.transform.Find("NameLabel").gameObject.GetComponent<Text>();
        skillDescription.levelLabel = label.transform.Find("LevelLabel").gameObject.GetComponent<Text>();
        skillDescription.description = skillDesc.GetComponent<Text>();

        //Bottom Set
        skillDescription.turnActive = turnActive.transform.Find("NumberLabel").gameObject.GetComponent<Text>();
        skillDescription.coolTime = coolTime.transform.Find("NumberLabel").gameObject.GetComponent<Text>();
        skillDescription.nextAction = nextAction.transform.Find("NumberLabel").gameObject.GetComponent<Text>();

        Slider gauge = burstUI.GetComponent<Slider>();
        Text gaugetext = burstUI.GetComponentInChildren<Text>();

        burstData = new BurstGaugeData();
        burstData.gauge = gauge;
        burstData.gaugeText = gaugetext;
    }
    public enum E_GAME_FSM
    {
        Nothing,
        Load,
        Ready,
        Game,
        Result
    }

    private StateMachine<E_GAME_FSM> fsm = null;

    private void InitializeFSM()
    {
        if (fsm == null)
            fsm = StateMachine<E_GAME_FSM>.Initialize(this, E_GAME_FSM.Nothing);
        else
            fsm.ChangeState(E_GAME_FSM.Nothing);

    }

    #region InGameState
    public void UIGameLoad()
    {
        InitializeFSM();
        fsm.ChangeState(E_GAME_FSM.Load);
    }
    public void UIGameReady()
    {
        InitializeSetRound();
        InitializeStartTurn();
        fsm.ChangeState(E_GAME_FSM.Ready);
    }
    public void UIGameGame()
    {
        fsm.ChangeState(E_GAME_FSM.Game);
    }
    public void UIGameResult()
    {
        fsm.ChangeState(E_GAME_FSM.Result);
    }
    #endregion
   
    public void SetSkillButtonInteractiveFalse()
    {
        passButton.interactable = false;
        var buttons = skillUI.GetComponentsInChildren<SkillButton>();
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<Button>().interactable = false;
        }
    }
    public void SetSkillDescription()
    {
        var data = InGameData.Instance.CurrentSkillData;
        string name = CSVDataManager.GetTable<StringTable>().GetString(data.name_string);
        string level = "1";
        string desc = CSVDataManager.GetTable<StringTable>().GetString(data.desc_string); ;
        string turnactive = data.turn_control.ToString();
        string cooltime = data.turn_cool.ToString();
        string nextaction = data.turn_next.ToString();

        skillDescription.namingLabel.text = name;
        skillDescription.levelLabel.text = $"Lv.{level}";
        skillDescription.description.text = desc;
        skillDescription.turnActive.text = $"+{turnactive} Turn";
        skillDescription.coolTime.text = $"+{cooltime} Turn"; ;
        skillDescription.nextAction.text = $"+{nextaction} Turn"; ;

        descriptionSkill.SetActive(true);
    }
    public void SkillDescriptionOff()
    {
        descriptionSkill.SetActive(false);
    }
    public void SetNextTurnUI(int index, CharacterProfile profile)
    {
        //if(index > 9)
        //{
        //    var rectSlider = roundSlider.GetComponent<ScrollRect>();
        //    rectSlider.horizontalScrollbar.value = 0;
        //}

        Turn[] turn = roundObject.GetComponentsInChildren<Turn>();

        turn[index].SetTempProfile(profile);
    }
    
    #region Set Round
    public void SetRound()
    {
        StartCoroutine(StartActionState(State.SetRound));
    }
    private void InitializeSetRound()
    {
        setRound.Add(SetTurnSibling);
        setRound.Add(SetRoundUI);
    }
    private void SetTurnSibling()
    {
        GameObject moveObj = null;
        var current = roundObject.transform.Find("Current");
        var next = roundObject.transform.Find("Next");

        moveObj = current.GetComponentsInChildren<Turn>()[0].gameObject;
        moveObj.transform.SetParent(next.transform);
        moveObj.transform.SetAsLastSibling();

        moveObj = next.GetComponentsInChildren<Turn>()[0].gameObject;
        moveObj.transform.SetParent(current.transform);
        moveObj.transform.SetAsLastSibling();

        StartAction();
    }

    private void SetRoundUI()
    {
        CharacterProfile[] profile = InGameData.Instance.UIProfile;
        Turn[] turn = roundObject.GetComponentsInChildren<Turn>();

        int result = InGameData.Instance.CurrentTurn;
        for (int i = 0; i < profile.Length; i++)
        {
            turn[i].SetProfile(profile[i], result);
            result++;
        }

        var rectSlider = roundSlider.GetComponent<ScrollRect>();
        rectSlider.horizontalScrollbar.value = 1;

        StartAction();
    }
    #endregion
    
    #region Set Current Turn
    public void SetCurrentTurnUI()
    {
        StartCoroutine(StartActionState(State.SetTurn));
    }
    private void InitializeStartTurn()
    {
        setTurn.Add(SetProfile);
        setTurn.Add(SetSkillUI);
    }


    public void SetProfile()
    {
        if(InGameData.Instance.CurrentTurnCharacter != null)
        {
            CharacterProfile profile = InGameData.Instance.CurrentTurnCharacter.Profile;
            var portrait = skillUIProfile.transform.Find("Portrait").gameObject.GetComponent<RawImage>();
            var  battleTag = skillUIProfile.transform.Find("PositionTag").gameObject.GetComponent<Image>();
            var attribute = skillUIProfile.transform.Find("Attribute").gameObject.GetComponent<Image>();

            portrait.texture = profile.snap;
            battleTag.sprite = profile.battleTag;
            attribute.sprite = profile.attribute;
        }

        StartAction();
    }
    public void SetSkillUI()
    {
        CharacterBase charbase = InGameData.Instance.CurrentTurnCharacter;
        Camp camp = charbase.CharacterInfo.BattleCamp;
        CharacterState state = null;

        switch(camp)
        {
            case Camp.Ally:
                InGameData.Instance.AllyList.TryGetValue(charbase, out state);
                break;
            case Camp.Enemy:
                InGameData.Instance.EnemyList.TryGetValue(charbase, out state);
                break;
        }
        var attackLock = state.AttackLock;

        if (charbase != null)
        {
            List<Generated.CsvData.SkillData> characterSkill = charbase.CharacterInfo.characterSkillList;
            var buttons = skillUI.GetComponentsInChildren<SkillButton>();
            skillUI.gameObject.SetActive(true);

            if (charbase.Profile.camp == Camp.Ally)
            {
                for (int i = 0; i < characterSkill.Count; i++)
                {
                    if (attackLock.HasFlag((Attack_Lock)i + 1))
                    {
                        buttons[i].InitailizeSkillButton(characterSkill[i], false);
                        continue;
                    }
                    
                    buttons[i].InitailizeSkillButton(characterSkill[i], true);
                }
                passButton.interactable = true;
            }
            else
            {
                for(int i = 0; i < buttons.Length; i++)
                {
                    if (i >= characterSkill.Count)
                    {
                        buttons[i].InitailizeSkillButton(null, false);
                        continue;
                    }

                    buttons[i].InitailizeSkillButton(characterSkill[i], false);
                }
                passButton.interactable = false;
            }
        }

        
        StartAction();
    }
    #endregion

    #region Burst
    public void AddBurst()
    {
        float value = InGameData.Instance.BurstGauge;
        float result = value / 100;

        Slider gauge = burstData.gauge;

        Tweener tween = gauge.DOValue(result, 0.2f);
        tween.onKill = () =>
        {
            burstData.gaugeText.text = $"{(int)value}%";
            
            Messenger.Broadcast(Definition.BattleManagerInvokeAction);
            if(result >= 100)
            {
                // Effect
            }
        };
    }
    #endregion

    #region  Action
    private IEnumerator StartActionState(State state)
    {
        yield return new WaitUntil(() => currentState == State.None);

        currentState = state;
        StartAction();
    }
    private void StartAction()
    {
        if (currentState == State.None)
            return;

        List<Action> actionList = GetActionList();
        if(actionIndex >= actionList.Count)
        {
            switch(currentState)
            {
                case State.SetTurn:
                    Messenger.Broadcast(Definition.BattleManagerInvokeAction);
                    break;
                case State.SetRound:
                    Messenger.Broadcast(Definition.BattleManagerInvokeAction);
                    break;
            }

            actionIndex = 0;
            currentState = State.None;
            return;
        }

        Action action = actionList[actionIndex];
        actionIndex++;

        action.Invoke();
    }
    
    private List<Action> GetActionList()
    {
        List<Action> list = null;
        switch(currentState)
        {
            case State.None:
                break;
            case State.SetTurn:
                list = setTurn;
                break;
            case State.SetRound:
                list = setRound;
                break;
        }
        return list;
    }
    #endregion
}
