using System;
using System.Collections;
using System.Collections.Generic;
using Devil.Common;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Generated.CsvData;
//using Grpc.Core;
//using Grpc.Net.Client;
//using GrpcGreeter;
public partial class UiGame
{
    bool infoMove = false;
    private int clickCount = 0;
    private void Game_Enter()
    {
        infoMove = false;
        passButton.onClick.AddListener(PassButtonFunc);
        InfoOn();
    }
    private void Game_Update()
    {
        if(infoMove == true)
        {
            foreach (var item in characterInfo)
            {
                Transform ownerTrans = item.Key.InfoTransform;
                Vector3 position = Camera.main.WorldToScreenPoint(ownerTrans.position);
                item.Value.trans.position = position;
            }
        }
    }
    private void Game_Exit()
    {

    }
    private void InfoOn()
    {
        foreach(var item in characterInfo)
        {
            item.Value.trans.gameObject.SetActive(true);
        }

        infoMove = true;
    }

    public void InsertInfoIcon(CharacterBase _target, string _atlasName, string _name, Action_Type _type)
    {
        InfoUIData info;
        if (characterInfo.TryGetValue(_target, out info) == false)
            return;

        Image[] area = null;

        switch(_type)
        {
            case Action_Type.BuffDebuff:
                area = info.buffArea;
                break;
            case Action_Type.CC:
                area = info.buffArea;
                break;
            case Action_Type.Stack:
                area = info.stackArea;
                break;
        }

        ResourcePoolManager.Instance.AsyncGetData(_atlasName, objectType.atlasSprite, null, null, (sprite) =>
        {
            Sprite result = (Sprite)sprite;

            result.name = _name;
            area = info.buffArea;
            var empty = FindEmptyImage(area);

            var image = empty.GetComponent<Image>();
            image.sprite = result;
            image.gameObject.SetActive(true);

            SibilingIcon(area);
        }, _name);
    }
    public void RemoveInfoIcon(CharacterBase _target, string _name, Action_Type _type)
    {
        InfoUIData info;
        if (characterInfo.TryGetValue(_target, out info) == false)
            return;

        Image[] area = null;

        switch (_type)
        {
            case Action_Type.BuffDebuff:
                area = info.buffArea;
                break;
            case Action_Type.CC:
                area = info.buffArea;
                break;
            case Action_Type.Stack:
                area = info.stackArea;
                break;
        }

        for(int i = 0; i < area.Length; i++)
        {
            Sprite sprite = area[i].sprite;
            if(sprite.name == _name)
            {
                area[i].gameObject.SetActive(false);
                ResourcePoolManager.Instance.DestroyGameObject(sprite, false);
                SibilingIcon(area);
                break;
            }
        }

    }

    private Image FindEmptyImage(Image[] images)
    {
        Image result = null;
        
        for(int i = 0; i < images.Length; i++)
        {
            if (images[i].isActiveAndEnabled == false)
            {
                result = images[i];
                break;
            }
        }

        return result;
    }
    private void SibilingIcon(Image[] area)
    {
        for (int i = 0; i < area.Length; i++)
        {
            if (area[i].gameObject.activeSelf == false)
                area[i].transform.SetAsLastSibling();
        }
    }

    private void PassButtonFunc()
    {
        if (clickCount > 0)
        {
            clickCount = 0;
            SetSkillButtonInteractiveFalse();
            Messenger.Broadcast(Definition.GoToState, BattleManager.TRIGGER_FSM.SetRoundUI);
            return;
        }

        Messenger.Broadcast(Definition.SetPassNextTurn);
        clickCount++;
    }

    public void PassClick() // Temp
    {
        Messenger.Broadcast(Definition.SetPassNextTurn);
        SetSkillButtonInteractiveFalse();
        Messenger.Broadcast(Definition.GoToState, BattleManager.TRIGGER_FSM.SetRoundUI);
    }

    public void ResetPassClick()
    {
        clickCount = 0;
    }
}
