using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUiController : BaseUiController<GameUiController>
{
    protected override void OnCompletePreloadAsset()
    {
        GuiMain.Instance.Open<UiGame>();
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
    #region InGameState
    private void UIGameLoad()
    {
        StartCoroutine(DelayInstance());
    }
    private void UIGameReady()
    {
        UiGame.Instance.UIGameReady();
    }
    private void UIGameGame()
    {
        UiGame.Instance.UIGameGame();
    }
    private void UIGameResult()
    {
        UiGame.Instance.UIGameResult();
    }
    private IEnumerator DelayInstance()
    {
        yield return new WaitUntil(() => UiGame.Instance != null);

        UiGame.Instance.UIGameLoad();
        yield break;
    }
    #endregion

    private void SetSkillUI()
    {
        UiGame.Instance.SetSkillUI();
    }
    private void SetNextTurnUI(int index, CharacterProfile profile)
    {
        UiGame.Instance.SetNextTurnUI(index, profile);
    }
    
    private void SetCurrentTurnUI()
    {
        UiGame.Instance.SetCurrentTurnUI();
    }
    private void SetSkillButtonInteractiveFalse()
    {
        UiGame.Instance.SetSkillButtonInteractiveFalse();
    }
    private void SetSkillDescription()
    {
        UiGame.Instance.SetSkillDescription();
    }
    private void SkillDescriptionOff()
    {
        UiGame.Instance.SkillDescriptionOff();
    }
    private void SetRound()
    {
        UiGame.Instance.SetRound();
    }
    private void InsertInfoIcon(CharacterBase _target, string _atlas , string _name, Action_Type _type)
    {
        UiGame.Instance.InsertInfoIcon(_target, _atlas, _name, _type);
    }
    private void RemoveInfoIcon(CharacterBase _target, string _name, Action_Type _type)
    {
        UiGame.Instance.RemoveInfoIcon(_target, _name, _type);
    }

    private void ResetPassClick()
    {
        UiGame.Instance.ResetPassClick();
    }
    private void PassClick()
    {
        UiGame.Instance.PassClick();
    }

    private void AddBurst()
    {
        UiGame.Instance.AddBurst();
    }
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
