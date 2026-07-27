using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [UI 무력화 / CLI 오토배틀] 인게임 UI(UiGame)를 코드에서 무력화한 상태.
/// UiGame 프리팹은 남아있지만(GuiMain.Open&lt;UiGame&gt;) 여기서 그 메서드를 호출하지 않아 inert.
/// 단, 전투 FSM 진행에 물려 있던 브로드캐스트는 그대로 유지해야 전투가 멈추지 않는다:
///   - CompleteState(UIGame)  : InGame 페이즈(Load/Ready/Result) 진행
///   - BattleManagerInvokeAction : SetCurrentTurnUI/SetRound/AddBurst 완료 시 다음 액션
///   - PassClick : SetPassNextTurn + GoToState(SetRoundUI)
/// 나머지 시각 처리(스킬설명/아이콘/버튼 등)는 전부 no-op.
/// 원복: 각 메서드 본문을 UiGame.Instance.X() 호출로 되돌리면 됨.
/// </summary>
public class GameUiController : BaseUiController<GameUiController>
{
    protected override void OnCompletePreloadAsset()
    {
        GuiMain.Instance.Open<UiGame>(); // 프리팹은 유지(inert)
    }
    protected override void OnAwake()
    {
        base.OnAwake();
        UIMessengerAddListner();
    }
    private void Start()
    {
    }
    protected override void OnDestroyComponent()
    {
        base.OnDestroyComponent();
        UIMessengerRemoveListner();
    }

    #region InGameState (FSM 진행 브로드캐스트만)
    private void UIGameLoad()   { Messenger.Broadcast(Definition.CompleteState, Complete.UIGame); }
    private void UIGameReady()  { Messenger.Broadcast(Definition.CompleteState, Complete.UIGame); }
    private void UIGameGame()   { /* Game 페이즈: UI 없음 */ }
    private void UIGameResult() { Messenger.Broadcast(Definition.CompleteState, Complete.UIGame); }
    #endregion

    // ---- FSM 진행에 물린 것: 진행 브로드캐스트로 대체 ----
    private void SetCurrentTurnUI() { Messenger.Broadcast(Definition.BattleManagerInvokeAction); }
    private void SetRound()         { Messenger.Broadcast(Definition.BattleManagerInvokeAction); }
    private void AddBurst()         { Messenger.Broadcast(Definition.BattleManagerInvokeAction); }
    private void PassClick()
    {
        Messenger.Broadcast(Definition.SetPassNextTurn);
        Messenger.Broadcast(Definition.GoToState, BattleManager.TRIGGER_FSM.SetRoundUI);
    }

    // ---- 순수 시각 처리: no-op ----
    private void SetSkillUI() { }
    private void SetNextTurnUI(int index, CharacterProfile profile) { }
    private void SetSkillButtonInteractiveFalse() { }
    private void SetSkillDescription() { }
    private void SkillDescriptionOff() { }
    private void InsertInfoIcon(CharacterBase _target, string _atlas, string _name, Action_Type _type) { }
    private void RemoveInfoIcon(CharacterBase _target, string _name, Action_Type _type) { }
    private void ResetPassClick() { }

    private void UIMessengerAddListner()
    {
        #region State
        Messenger.AddListener(Definition.InGameLoad, UIGameLoad);
        Messenger.AddListener(Definition.InGameReady, UIGameReady);
        Messenger.AddListener(Definition.InGameGame, UIGameGame);
        Messenger.AddListener(Definition.InGameResult, UIGameResult);
        #endregion

        Messenger.AddListener<int, CharacterProfile>(Definition.SetNextTurnUI, SetNextTurnUI);
        Messenger.AddListener(Definition.SetCurrentTurnUI, SetCurrentTurnUI);
        Messenger.AddListener(Definition.SetSkillButtonInteractiveFalse, SetSkillButtonInteractiveFalse);
        Messenger.AddListener(Definition.SetSkillDescription, SetSkillDescription);
        Messenger.AddListener(Definition.SkillDescriptionOff, SkillDescriptionOff);
        Messenger.AddListener(Definition.SetRoundUI, SetRound);
        Messenger.AddListener<CharacterBase, string, string, Action_Type>(Definition.InsertInfoIcon, InsertInfoIcon);
        Messenger.AddListener<CharacterBase, string, Action_Type>(Definition.RemoveBuffIcon, RemoveInfoIcon);
        Messenger.AddListener(Definition.ResetPassClick, ResetPassClick);
        Messenger.AddListener(Definition.PassClick, PassClick);
        Messenger.AddListener(Definition.AddBurst, AddBurst);
    }
    private void UIMessengerRemoveListner()
    {
        #region State
        Messenger.RemoveListener(Definition.InGameLoad, UIGameLoad);
        Messenger.RemoveListener(Definition.InGameReady, UIGameReady);
        Messenger.RemoveListener(Definition.InGameGame, UIGameGame);
        Messenger.RemoveListener(Definition.InGameResult, UIGameResult);
        #endregion

        Messenger.RemoveListener<int, CharacterProfile>(Definition.SetNextTurnUI, SetNextTurnUI);
        Messenger.RemoveListener(Definition.SetCurrentTurnUI, SetCurrentTurnUI);
        Messenger.RemoveListener(Definition.SetSkillButtonInteractiveFalse, SetSkillButtonInteractiveFalse);
        Messenger.RemoveListener(Definition.SetSkillDescription, SetSkillDescription);
        Messenger.RemoveListener(Definition.SkillDescriptionOff, SkillDescriptionOff);
        Messenger.RemoveListener(Definition.SetRoundUI, SetRound);
        Messenger.RemoveListener<CharacterBase, string, string, Action_Type>(Definition.InsertInfoIcon, InsertInfoIcon);
        Messenger.RemoveListener<CharacterBase, string, Action_Type>(Definition.RemoveBuffIcon, RemoveInfoIcon);
        Messenger.RemoveListener(Definition.ResetPassClick, ResetPassClick);
        Messenger.RemoveListener(Definition.PassClick, PassClick);
        Messenger.RemoveListener(Definition.AddBurst, AddBurst);
    }
}
