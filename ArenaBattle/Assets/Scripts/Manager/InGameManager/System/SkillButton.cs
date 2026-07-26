using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    [SerializeField] private GameObject icon;
    [SerializeField] private GameObject cooltime;

    private Generated.CsvData.SkillData skillData = null;

    public void InitailizeSkillButton(Generated.CsvData.SkillData data = null, bool interactable = false)
    {
        if(interactable == false)
            cooltime.SetActive(true);
        else
            cooltime.SetActive(false);

        if (data == null)
            return;

        skillData = data;
        var button = gameObject.GetComponent<Button>();
        button.interactable = interactable;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(PressSkillButton);
    }

    private void PressSkillButton()
    {
        InGameData.Instance.CurrentSkillData = skillData;

        Messenger.Broadcast(Definition.ResetPassClick);
        Messenger.Broadcast(Definition.SetSkillDescription);
        Messenger.Broadcast(Definition.SetNextTurnUIOff, InGameData.Instance.CurrentTurnCharacter.Profile);
        Messenger.Broadcast(Definition.SkillSelect);
        Messenger.Broadcast(Definition.SetCasterNextTurn);
    }
}
