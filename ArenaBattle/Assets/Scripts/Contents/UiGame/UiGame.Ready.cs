using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public partial class UiGame
{
    private void Ready_Enter()
    {
        StartCoroutine(LoadInfo());
    }
    private void Ready_Update()
    {

    }

    private void Ready_Exit()
    {

    }
   
    private IEnumerator LoadInfo()
    {
        List<CharacterBase> player = new List<CharacterBase>();
        player.AddRange(InGameData.Instance.AllyList.Keys);
        for (int i = 0; i < player.Count; i++)
        {
            bool loadComplete = false;
            ResourcePoolManager.Instance.AsyncGetData("CharacterInfoSlider", objectType.gameobject, null, null, (obj) =>
            {
                InfoUIData data = new InfoUIData();
                GameObject result = (GameObject)obj;
                GameObject infoArea = result.transform.Find("InfoArea").gameObject;
                GameObject buffArea = result.transform.Find("Top").gameObject;
                GameObject stackArea = result.transform.Find("Bottom").gameObject;
                GameObject select = result.transform.Find("Select").gameObject;

                data.trans = result.transform;
                data.select = select;
                data.hp = infoArea.GetComponentInChildren<Slider>();
                data.buffArea = buffArea.GetComponentsInChildren<Image>();
                for (int i = 0; i < data.buffArea.Length; i++)
                    data.buffArea[i].gameObject.SetActive(false);

                data.stackArea = stackArea.GetComponentsInChildren<Image>();
                for (int i = 0; i < data.stackArea.Length; i++)
                    data.stackArea[i].gameObject.SetActive(false);

                var tag = infoArea.transform.Find("PositionTag").gameObject.GetComponentInChildren<Image>();
                tag.sprite = player[i].Profile.battleTag;

                var attribute = infoArea.transform.Find("Attribute").gameObject;
                var characterAttri = player[i].CharacterInfo.CharacterStatus.attribute;
                attribute.transform.Find(characterAttri.ToString()).gameObject.SetActive(true);

                result.transform.SetParent(characterInfoParent.transform);
                result.transform.localPosition = Vector3.zero;
                result.SetActive(false);

                characterInfo.Add(player[i], data);
                loadComplete = true;
            }, "");

            yield return new WaitUntil(() => loadComplete == true);
        }

        List<CharacterBase> enemy = new List<CharacterBase>();
        enemy.AddRange(InGameData.Instance.EnemyList.Keys);
        for (int i = 0; i < enemy.Count; i++)
        {
            bool loadComplete = false;
            ResourcePoolManager.Instance.AsyncGetData("CharacterInfoSlider", objectType.gameobject, null, null, (obj) =>
            {
                InfoUIData data = new InfoUIData();
                GameObject result = (GameObject)obj;
                GameObject infoArea = result.transform.Find("InfoArea").gameObject;
                GameObject buffArea = result.transform.Find("Top").gameObject;
                GameObject stackArea = result.transform.Find("Bottom").gameObject;
                GameObject select = result.transform.Find("Select").gameObject;

                data.trans = result.transform;
                data.select = select;
                data.hp = infoArea.GetComponentInChildren<Slider>();
                data.buffArea = buffArea.GetComponentsInChildren<Image>();
                for(int i = 0; i < data.buffArea.Length; i++)
                    data.buffArea[i].gameObject.SetActive(false);

                data.stackArea = stackArea.GetComponentsInChildren<Image>();
                for (int i = 0; i < data.stackArea.Length; i++)
                    data.stackArea[i].gameObject.SetActive(false);

                var tag = infoArea.transform.Find("PositionTag").gameObject.GetComponentInChildren<Image>();
                tag.sprite = enemy[i].Profile.battleTag;

                var attribute = infoArea.transform.Find("Attribute").gameObject;
                var characterAttri = enemy[i].CharacterInfo.CharacterStatus.attribute;
                attribute.transform.Find(characterAttri.ToString()).gameObject.SetActive(true);

                result.transform.SetParent(characterInfoParent.transform);
                result.transform.localPosition = Vector3.zero;
                result.SetActive(false);

                characterInfo.Add(enemy[i], data);
                loadComplete = true;
            }, "");

            yield return new WaitUntil(() => loadComplete == true);
        }

        Messenger.Broadcast(Definition.CompleteState, Complete.UIGame);
        yield break;
    }
}
