using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class NoteButton : Button
{
    [SerializeField]
    ButtonDownEvent _onDown = new ButtonDownEvent();
    protected NoteButton() { }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        _onDown.Invoke();
    }
    public ButtonDownEvent onDown
    {
        get { return _onDown; }
        set { _onDown = value; }
    }
    [Serializable]
    public class ButtonDownEvent : UnityEvent { }
}
