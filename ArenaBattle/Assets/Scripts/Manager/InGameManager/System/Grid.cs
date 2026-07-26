using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class Grid : MonoBehaviour
{
    public Camp GridCamp { set; get; }

    private GameObject selectedable;
    private GameObject target;
    private GameObject seleted;
    private GameObject caster;
    private bool touchOn = false;
    private bool skillTarget = false;

    private WeakReference<CharacterBase> owner = null;

    public bool TouchOn
    {
        get { return touchOn; }
    }
    public bool SkillTarget
    {
        get { return skillTarget; }
    }



    private void Start()
    {

    }
    #region Initalize Grid
    public void SetPosition(CharacterBase _owner)
    {
        owner = new WeakReference<CharacterBase>(_owner);
        _owner.transform.SetParent(transform);

        _owner.gameObject.transform.localPosition = Vector3.up * -5;
        _owner.gameObject.transform.localRotation = Quaternion.identity;
    }
    public void InitializeGrid(Camp _camp)
    {
        GridCamp = _camp;

        target = gameObject.transform.Find("indicator_demage").gameObject;
        selectedable = gameObject.transform.Find("indicator_select_able").gameObject;
        seleted = gameObject.transform.Find("indicator_selected").gameObject;
        caster = gameObject.transform.Find("indicator_caster").gameObject;

        MessageAddListner();
    }
    #endregion

    public void CasterOn()
    {
        caster.SetActive(true);
    }

    public void CasterOff()
    {
        caster.SetActive(false);
    }
    private void GridOn(Camp _camp)
    {
        if(_camp != GridCamp)
        {
            GridOff();
            return;
        }

        var parent = transform.parent;

        if(owner != null)
        {
            selectedable.gameObject.SetActive(true);
            touchOn = true;
        }
        else
        {
            touchOn = false;
            seleted.gameObject.SetActive(true);
        }
    }
    private void GridOff()
    {
        touchOn = false;
        skillTarget = false;

        caster.gameObject.SetActive(false);
        seleted.gameObject.SetActive(false);
        selectedable.gameObject.SetActive(false);
        target.gameObject.SetActive(false);
    }


    public void GridTargetOn()
    {
        if(owner != null)
        {
            CharacterBase character = null;
            owner.TryGetTarget(out character);

            if (InGameData.Instance.GetCharacterTargetLock(character) == false)
                skillTarget = true;
            else
                skillTarget = false;
        }

        target.gameObject.SetActive(true);
    }

    private void GridTargetOff()
    {
        skillTarget = false;
        if(target.activeSelf == true)
            target.gameObject.SetActive(false);
    }
    private void MessageAddListner()
    {
        Messenger.AddListener(Definition.GridTargetOff, GridTargetOff);
        Messenger.AddListener(Definition.GridOff, GridOff);
        Messenger.AddListener<Camp>(Definition.GridTouchOn, GridOn);
    }
    private void MessageRemoveListner()
    {
        Messenger.RemoveListener(Definition.GridTargetOff, GridTargetOff);
        Messenger.RemoveListener(Definition.GridOff, GridOff);
        Messenger.RemoveListener<Camp>(Definition.GridTouchOn, GridOn);
    }

    private void OnDestroy()
    {
        MessageRemoveListner();
    }
}
