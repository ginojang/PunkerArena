using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using System;
using Generated.CsvData;
using Devil.Common;

public partial class BattleManager
{
    private void Load_Enter()
    {
        StartCoroutine(SetAction(TRIGGER_FSM.Load, SetLoadAction));
    }
    private void Load_Update()
    {

    }
    private void Load_Exit()
    {
        ResetAction();
        Messenger.Broadcast(Definition.CompleteState, Complete.BattleManager);
    }

    private void SetLoadAction()
    {
        stateAction.Add(InitailizeData);
        stateAction.Add(LoadCharacter);
        stateAction.Add(LoadMonster);
        stateAction.Add(LoadResource);
        stateAction.Add(InitializeFSM);
    }
    private void InitailizeData()
    {
        InvokeAction();
    }

    private void LoadCharacter()
    {
        StartCoroutine(LoadCharacterObject());
    }
    private IEnumerator LoadCharacterObject()
    {
        yield return new WaitUntil(() => GameDataManager.Instance.baseCharacter != null);

        UserData.CharacterSquadData data = GameDataManager.Instance.userData.serverSquadDic[modeType.Action][0];

        int count = 0;

        foreach (var chardata in data.serverSquadList)
        {
            bCostume = true;

            Messenger.AddListener<int, GameObject>(Definition.Dino_Costume_End, FinishLoadCostume);
            int nextPlayerCount = InGameData.Instance.AllyList.Count + 1;
            var characterObj = GameObject.Instantiate(GameDataManager.Instance.baseCharacter);

            charUID = chardata.Value.dinoID;
            CostumeManager.Instance.AddCostumeCharacter(characterObj, count, chardata.Value.charClass, chardata.Value.talent, chardata.Value.partslist);

            yield return new WaitUntil(() => !bCostume);


            count++;
            /*
                        userData.costumeInfo.model = characterObj;

                        Messenger.Broadcast(Definition.SetPosition, Camp.Ally, i, characterObj);

                        CostumeManager.instance_value.AddCostoumCharacter(userData.costumeInfo, userData.partslist);
            */
            yield return new WaitUntil(() => nextPlayerCount == InGameData.Instance.AllyList.Count);

        }

        InvokeAction();
    }
    private void FinishLoadCostume(int index, GameObject character)
    {
        Messenger.RemoveListener<int, GameObject>(Definition.Dino_Costume_End, FinishLoadCostume);
        character.gameObject.name = $"Character_{index}";
        CharacterBase charbase = character.GetComponent<CharacterBase>();

        charbase.InitializeFSM();
        charbase.InitializeCharacterAnimator();

        //var userData =  GameDataManager.Instance.userData.lastDatas.UserCharacterData[index];

        var userData = GameDataManager.Instance.userData.serverSquadDic[modeType.Action][0].serverSquadList[charUID];

        StartCoroutine(SetProfile(Camp.Ally, charbase, index));

        int statusIndex = (int)charbase.CharacterInfo.costumeInfo.CURCLASS;

        charbase.CharacterInfo.partslist = userData.partslist;
        charbase.CharacterInfo.dinoID = charUID;

        charbase.CharacterInfo.SetCharacterStatus(Camp.Ally, statusIndex);

        InGameData.Instance.AllyList.Add(charbase, new CharacterState());

        Messenger.Broadcast(Definition.SetPosition, Camp.Ally, index, character);

        bCostume = false;
    }
    private void LoadMonster()
    {
        StartCoroutine(LoadMonsterObject());
    }

    private bool monsterCostume = false;
    List<Generated.CsvData.MonsterData> monsterList = new List<Generated.CsvData.MonsterData>();

    private IEnumerator LoadMonsterObject()
    {
        int currentStage = 1;

        Attributes start = CSVDataManager.GetTable<StageTable>().GetData(currentStage).time_start;
        InGameData.Instance.CurrentAttribute = start;

        int[] monsterGroupData = CSVDataManager.GetAllMonsterGroup(currentStage);

        monsterList.Clear();

        for (int i = 0; i < monsterGroupData.Length; i++)
        {
            if (monsterGroupData[i] == 0)
                continue;

            int[] groupData = CSVDataManager.GetTable<Monster_GroupTable>().GetData(monsterGroupData[i]).monsterID;
            for (int j = 0; j < groupData.Length; j++)
            {
                if (groupData[j] == 0)
                    continue;

                Generated.CsvData.MonsterData data = CSVDataManager.GetTable<MonsterTable>().GetData(groupData[j]);

                monsterList.Add(data);
            }
        }
        string prefix = "";
        for (int i = 0; i < monsterList.Count; i++)
        {
            string monstername = "";
            if (monsterList[i].type == Monster_Type.Dino)
            {
                if (monsterList[i].monster_talent == CharacterTalent.Carnivore)
                    prefix = "ca_";
                else if (monsterList[i].monster_talent == CharacterTalent.Omnivore)
                    prefix = "om_";
                else if (monsterList[i].monster_talent == CharacterTalent.Herbivore)
                    prefix = "he_";

                monstername = prefix + "orange_01";
            }
            else
            {
                monstername = monsterList[i].res_prefab;
            }

            GameObject monsterObj = null;

            ResourcePoolManager.Instance.AsyncGetData(monstername, objectType.character, null, null, (monster) =>
            {
                //yield return new WaitUntil(() => nextPlayerCount == InGameData.Instance.AllyList.Count);

                monsterObj = (GameObject)monster;

                if (monsterList[i].type == Monster_Type.Dino)
                {
                    monsterCostume = true;

                    Messenger.AddListener<int, GameObject>(Definition.Dino_Costume_End, FinishCostumeMonster);

                    Monster_PartTable partTbl = CSVDataManager.GetTable<Monster_PartTable>();

                    Monster_PartData data = partTbl.GetData(monsterList[i].monster_part_id);

                    Dictionary<ItemType, string> itemlist = new Dictionary<ItemType, string>();

                    itemlist.Add(ItemType.body, prefix + data.body);
                    itemlist.Add(ItemType.headparts, prefix + data.head);
                    itemlist.Add(ItemType.eyes, prefix + data.eyes);
                    itemlist.Add(ItemType.mouth, prefix + data.mouth);
                    itemlist.Add(ItemType.back, prefix + data.back);
                    itemlist.Add(ItemType.tail, prefix + data.tail);
                    if (data.wing != "none")
                        itemlist.Add(ItemType.wing, prefix + data.wing);

                    CostumeManager.Instance.AddCostumeMonster((GameObject)monsterObj, i, monsterList[i].monster_class,
                        monsterList[i].monster_talent, monsterList[i].idx, itemlist);
                }
                else
                {
                    LoadedNormalMonster(i, (GameObject)monsterObj);
                }
            });

            yield return new WaitUntil(() => monsterCostume == false);
        }

        yield return null;

        InvokeAction();
    }

    private void CreateMonsterTexture(GameObject monster, int index, int level, bool bDino)
    {
        GameObject renderroot = GameObject.Find($"MonsterRenderTextureRoot");
        GameObject normalSnapShot = renderroot.FindChildByName("Normal_SnapShot");
        GameObject bossSnapShot = renderroot.FindChildByName("Boss_SnapShot");
        GameObject dinoSnapShot = renderroot.FindChildByName("Dino_SnapShot");
        GameObject dinobossSnapShot = renderroot.FindChildByName("DinoBoss_SnapShot");

        Transform snapPos = null;
        Camera snapCam = null;
        Transform backParent = null;

        GameObject obj = null;

        if (level == 3) // 보스타입
        {
            if (bDino)
            {
                normalSnapShot.SetActive(false);
                bossSnapShot.SetActive(false);
                dinoSnapShot.SetActive(false);
                dinobossSnapShot.SetActive(true);
                obj = dinobossSnapShot;
            }
            else
            {
                normalSnapShot.SetActive(false);
                bossSnapShot.SetActive(true);
                dinoSnapShot.SetActive(false);
                dinobossSnapShot.SetActive(false);
                obj = bossSnapShot;
            }

            snapPos = obj.transform.Find("CharacterPos");
            snapCam = obj.transform.Find("Camera").GetComponent<Camera>();
        }
        else
        {
            if (bDino)
            {
                normalSnapShot.SetActive(false);
                bossSnapShot.SetActive(false);
                dinoSnapShot.SetActive(true);
                dinobossSnapShot.SetActive(false);
                obj = dinoSnapShot;
            }
            else
            {
                normalSnapShot.SetActive(false);
                bossSnapShot.SetActive(false);
                dinoSnapShot.SetActive(false);
                dinobossSnapShot.SetActive(true);
                obj = dinobossSnapShot;
            }

            snapPos = obj.transform.Find("CharacterPos");
            snapCam = obj.transform.Find("Camera").GetComponent<Camera>();
        }

        backParent = monster.transform.parent;

        monster.transform.SetParent(snapPos);
        monster.transform.localPosition = Vector3.zero;
        monster.transform.localRotation = Quaternion.identity;

        GameDataManager.Instance.monsterImage[index] = GameDataManager.Instance.CharacterSnapShot(snapCam, index);

        monster.transform.SetParent(backParent);
        monster.transform.localPosition = Vector3.zero;
        monster.transform.localRotation = Quaternion.identity;
    }

    bool bCostume = false;
    private string charUID = "";
    private void LoadedNormalMonster(int index, GameObject monster)
    {
        monster.transform.position = _loadPosition.transform.position;
        monster.transform.position = Vector3.zero;

        //monster.gameObject.name = $"{monster[index].res_prefab}";

        CharacterBase charbase = monster.GetComponent<CharacterBase>();
        charbase.InitializeFSM();
        charbase.InitializeCharacterAnimator();
        charbase.CharacterInfo.SetCharacterStatus(Camp.Enemy, monsterList[index].idx);

        CreateMonsterTexture(monster, index, (int)monsterList[index].boss_type, false);

        StartCoroutine(SetProfile(Camp.Enemy, charbase, index));

        InGameData.Instance.EnemyList.Add(charbase, new CharacterState());
        Messenger.Broadcast(Definition.SetPosition, Camp.Enemy, index, charbase.gameObject);
    }

    private void FinishCostumeMonster(int index, GameObject monster)
    {
        Messenger.RemoveListener<int, GameObject>(Definition.Dino_Costume_End, FinishCostumeMonster);

        monsterCostume = false;

        monster.transform.position = _loadPosition.transform.position;
        monster.transform.position = Vector3.zero;

        //monster.gameObject.name = $"{monster[index].res_prefab}";

        CharacterBase charbase = monster.GetComponent<CharacterBase>();
        charbase.InitializeFSM();
        charbase.InitializeCharacterAnimator();
        charbase.CharacterInfo.SetCharacterStatus(Camp.Enemy, monsterList[index].idx);

        CreateMonsterTexture(monster, index, (int)monsterList[index].boss_type, true);

        StartCoroutine(SetProfile(Camp.Enemy, charbase, index));

        InGameData.Instance.EnemyList.Add(charbase, new CharacterState());
        Messenger.Broadcast(Definition.SetPosition, Camp.Enemy, index, charbase.gameObject);
    }

    private IEnumerator SetProfile(Camp camp, CharacterBase character, int index)
    {
        CharacterProfile profile = character.Profile;
        Texture2D[] snapArray = null;
        int iconName = Convert.ToInt32('A');
        int tagNum = 0;
        switch (camp)
        {
            case Camp.Ally:
                tagNum = 1;
                snapArray = GameDataManager.Instance.characterImage;
                break;
            case Camp.Enemy:
                tagNum = 2;
                snapArray = GameDataManager.Instance.monsterImage;
                break;
        }

        iconName += index;
        char ascii = Convert.ToChar(iconName);
        string battleTagIcon = $"UI_Ingame_Tag_{ascii}{tagNum}";

        ResourcePoolManager.instance_value.AsyncGetData("Ingame", objectType.atlasSprite, null, null, (icon) =>
        {
            profile.battleTag = (Sprite)icon;
        }, battleTagIcon);

        yield return new WaitUntil(() => profile.battleTag != null);

        string attributeIcon = $"icon_Attribute_{character.CharacterInfo.CharacterStatus.attribute}";
        ResourcePoolManager.instance_value.AsyncGetData("Ingame", objectType.atlasSprite, null, null, (icon) =>
        {
            profile.attribute = (Sprite)icon;
        }, attributeIcon);

        yield return new WaitUntil(() => profile.attribute != null);

        profile.camp = camp;
        profile.snap = snapArray[index];
    }

    private void LoadResource()
    {
        StartCoroutine(LoadSkillEffectResource());
    }
    private IEnumerator LoadSkillEffectResource()
    {
        List<CharacterBase> list = new List<CharacterBase>();
        List<string> effectName = new List<string>();
        list.AddRange(InGameData.Instance.AllyList.Keys);
        list.AddRange(InGameData.Instance.EnemyList.Keys);
        bool loadComplete = false;

        for(int i = 0; i < list.Count; i++)
        {
            var skillList = list[i].CharacterInfo.characterSkillList;
            for(int j = 0; j < skillList.Count; j++)
            {
                effectName.Add(skillList[j].res_fx_run);
                effectName.Add(skillList[j].res_fx_attack);
                effectName.Add(skillList[j].res_fx_hit);
                effectName.Add(skillList[j].res_fx_projectile);
            }
        }

        for(int i = 0; i < effectName.Count; i++)
        {
            loadComplete = false;
            if (ResourcePoolManager.Instance.IsContainsData(effectName[i]) == true || effectName[i] == "none")
                continue;

            ResourcePoolManager.Instance.AsyncLoadData(effectName[i], objectType.gameobject, (effect)=> { if (effect != null) loadComplete = true; });
            yield return new WaitUntil(() => loadComplete == true);
        }

        InvokeAction();
    }
}
