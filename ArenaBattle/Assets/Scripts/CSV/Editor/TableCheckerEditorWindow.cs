using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Generated.CsvData;
using System.Reflection;
using UnityEngine.U2D;

public class TableCheckerEditorWindow : EditorWindow
{
    [MenuItem("Tools/Table/Csv 유효성 체크")]
    static void Open()
    {
        EditorWindow.GetWindow(typeof(TableCheckerEditorWindow));
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("유효성 체크 가능 테이블");
        GUI.color = Color.yellow;
        EditorGUILayout.LabelField("중간에 실패시 검사를 중단하게 되어잇다.", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("[스테이지] StageTable->MonsterGroup->Monster");
        EditorGUILayout.LabelField("[디노] DinoClass->DinoTable");

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        if (GUILayout.Button("Set") == false)
            return;

        ClearLog();

        CSVDataManager.InitTables(true);

        // 대충 느낌같은 느낌으로 루트가 될 테이블 기준으로 나머지 테이블에 대한 파생을 한다.
        if (CheckStageTable() == false)
            return;

        if (CheckDinoTable() == false)
            return;

        if (CheckHelpPopupTable() == false)
            return;
    }

    public void ClearLog() //you can copy/paste this code to the bottom of your script
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

    bool IsContainAddressableList(string _name, string _spriteName)
    {
        // 아틀라스가 없어서 임시로 스프라이트는 항상 성공하게 한다.
        if (_spriteName.Length > 0)
            return true;

        if (_name.ToLower() == "none")
            return true;

        var asset = AssetManifest.Instance != null ? AssetManifest.Instance.Get(_name) : null;
        if (asset == null)
            return false;

        if (asset is UnityEngine.U2D.SpriteAtlas atlas)
            return atlas.GetSprite(_spriteName) != null;

        return true;
    }

    bool IsContainAddressableList_DinoParts(string _talentName, string _partsName)
    {
        if (_partsName.ToLower() == "none")
            return true;

        string mainAssetName = _talentName + _partsName;
        string findPartsName = _talentName + _partsName;
        mainAssetName = mainAssetName.Remove(mainAssetName.LastIndexOf("_"));

        var asset = AssetManifest.Instance != null ? AssetManifest.Instance.Get(mainAssetName) : null;
        GameObject findObject = asset as GameObject;
        if (findObject == null)
            return false;

        return findObject.transform.Find(findPartsName) != null;
    }

    #region >> StageTable <<
    bool CheckStageTable()
    {
        Debug.LogError(">>>>>>>>>>>>>>>> 스테이지 테이블 검사 <<<<<<<<<<<<<<<<<");

        StageTable table = CSVDataManager.GetTable<StageTable>();

        foreach( var element in table.DicData)
        {
            StageData data = element.Value;

            // name_string
            if (CheckStringTable(data.name_string) == false)
            {
                Debug.LogError($"[스테이지 테이블] idx : {data.idx}, name_string : {data.name_string}");
                return false;
            }

            // desc_string
            if (CheckStringTable(data.desc_string) == false)
            {
                Debug.LogError($"[스테이지 테이블] idx : {data.idx}, desc_string : {data.desc_string}");
                return false;
            }

            foreach (var subElement in data.monsterGroupID)
            {
                if (CheckMonsterGroup(data.idx, subElement) == true)
                    continue;

                return false;
            }

            // reward_item1_id
            if (CeckItemTable(data.reward_item1_id) == false)
            {
                Debug.LogError($"[스테이지 테이블] idx : {data.idx}, reward_item1_id : {data.reward_item1_id}");
                return false;
            }

            // reward_item2_id
            if (CeckItemTable(data.reward_item2_id) == false)
            {
                Debug.LogError($"[스테이지 테이블] idx : {data.idx}, reward_item2_id : {data.reward_item2_id}");
                return false;
            }

            // reward_item3_id
            if (CeckItemTable(data.reward_item3_id) == false)
            {
                Debug.LogError($"[스테이지 테이블] idx : {data.idx}, reward_item3_id : {data.reward_item3_id}");
                return false;
            }

            //res_prefab
            if (IsContainAddressableList(data.res_prefab, "") == false)
            {
                Debug.LogError($"[스테이지 테이블] idx : {data.idx}, res_prefab : {data.res_prefab}");
                return false;
            }

        }

        return true;
    }

    bool CheckMonsterGroup(int _stageTID, int _monsterGroupTID)
    {
        if (_monsterGroupTID <= 0)
            return true;

        Monster_GroupData data = CSVDataManager.GetTable<Monster_GroupTable>().GetData(_monsterGroupTID);

        if( data == null )
        {
            Debug.LogError($"[MonsterGroup] idx : {_stageTID}, Monster_Group : {_monsterGroupTID}");
            return false;
        }

        foreach( var element in data.monsterID )
        {
            if (element <= 0)
                continue;

            if (CheckMonsterTable(element) == true)
                continue;

            return false;
        }

        return true;
    }

    #endregion

    #region >> 몬스터 테이블 관련 <<
    bool CheckMonsterTable(int _mponsterTID)
    {
        MonsterData tableData = CSVDataManager.GetTable<MonsterTable>().GetData(_mponsterTID);

        // name_string
        if (CheckStringTable(tableData.name_string) == false)
        {
            Debug.LogError($"[몬스터테이블] idx : {tableData.idx}, name_string : {tableData.name_string}");
            return false;
        }

        // desc_string
        if (CheckStringTable(tableData.desc_string) == false)
        {
            Debug.LogError($"[몬스터테이블] idx : {tableData.idx}, desc_string : {tableData.desc_string}");
            return false;
        }

        // skill_id_basic_attack
        if (CheckSkillTable(tableData.type, 0, tableData.skill_id_basic_attack) == false)
        {
            Debug.LogError($"[몬스터테이블] idx : {tableData.idx}, skill_id_basic_attack : {tableData.skill_id_basic_attack}");
            return false;
        }

        // skill_id_active1
        if (CheckSkillTable(tableData.type, 0, tableData.skill_id_active1) == false)
        {
            Debug.LogError($"[몬스터테이블] idx : {tableData.idx}, skill_id_active1 : {tableData.skill_id_active1}");
            return false;
        }

        // skill_id_active2
        if (CheckSkillTable(tableData.type, 0, tableData.skill_id_active2) == false)
        {
            Debug.LogError($"[몬스터테이블] idx : {tableData.idx}, skill_id_active2 : {tableData.skill_id_active2}");
            return false;
        }

        // skill_id_passive1
        if (CheckSkillTable(tableData.type, 0, tableData.skill_id_passive1) == false)
        {
            Debug.LogError($"[몬스터테이블] idx : {tableData.idx}, skill_id_passive1 : {tableData.skill_id_passive1}");
            return false;
        }

        // skill_id_passive2
        if (CheckSkillTable(tableData.type, 0, tableData.skill_id_passive2) == false)
        {
            Debug.LogError($"[몬스터테이블] idx : {tableData.idx}, skill_id_passive2 : {tableData.skill_id_passive2}");
            return false;
        }

        if( tableData.type == Monster_Type.None )
        {
            if (CheckNormalMonster(tableData) == false)
                return false;
        }

        if (tableData.type == Monster_Type.Dino)
        {
            if (CheckDinoMonster(tableData) == false)
                return false;
        }

        // res_icon 검사
        if (IsContainAddressableList(tableData.res_atlas, tableData.res_icon) == false)
        {
            Debug.LogError($"[몬스터테이블] idx : {tableData.idx}, res_atlas : {tableData.res_atlas}, res_icon : {tableData.res_icon}");
            return false;
        }

        return true;
    }

    bool CheckNormalMonster(MonsterData _data)
    {
        // res_prefab
        if (IsContainAddressableList(_data.res_prefab, "") == false)
        {
            Debug.LogError($"[몬스터테이블] 어드레스블에 등록이 안되어 있습니다. res_prefab : {_data.res_prefab}");
            return false;
        }

        return true;
    }

    bool CheckDinoMonster(MonsterData _data)
    {
        Monster_PartData tableData = CSVDataManager.GetTable<Monster_PartTable>().GetData(_data.monster_part_id);

        if( tableData == null )
        {
            Debug.LogError($"[Monster_PartData] monster_part_id : {_data.monster_part_id}");
            return false;
        }

        string talent = string.Empty;

        switch (_data.monster_talent)
        {
            case CharacterTalent.Carnivore:
                talent = "ca_";
                break;
            case CharacterTalent.Herbivore:
                talent = "he_";
                break;
            case CharacterTalent.Omnivore:
                talent = "om_";
                break;
        }

        // 바디는 먼가 규약을 하기기 힘들다. 그리고 기본적으로 있으므로 바디는 체크하지 않는다.
        // tableData.body

        if (IsContainAddressableList_DinoParts(talent, tableData.head) == false)
        {
            string prefabName = talent + tableData.head;
            prefabName = prefabName.Remove(prefabName.LastIndexOf("_"));
            Debug.LogError($"[파츠테이블] 해당 프리팹에 파츠가 없습니다. 프리팹 : {prefabName}  파츠 : {tableData.head}");
            return false;
        }

        if (IsContainAddressableList_DinoParts(talent, tableData.eyes) == false)
        {
            string prefabName = talent + tableData.eyes;
            prefabName = prefabName.Remove(prefabName.LastIndexOf("_"));
            Debug.LogError($"[파츠테이블] 해당 프리팹에 파츠가 없습니다. 프리팹 : {prefabName}  파츠 : {tableData.eyes}");
            return false;
        }

        if (IsContainAddressableList_DinoParts(talent, tableData.mouth) == false)
        {
            string prefabName = talent + tableData.mouth;
            prefabName = prefabName.Remove(prefabName.LastIndexOf("_"));
            Debug.LogError($"[파츠테이블] 해당 프리팹에 파츠가 없습니다. 프리팹 : {prefabName}  파츠 : {tableData.mouth}");
            return false;
        }

        if (IsContainAddressableList_DinoParts(talent, tableData.back) == false)
        {
            string prefabName = talent + tableData.back;
            prefabName = prefabName.Remove(prefabName.LastIndexOf("_"));
            Debug.LogError($"[파츠테이블] 해당 프리팹에 파츠가 없습니다. 프리팹 : {prefabName}  파츠 : {tableData.back}");
            return false;
        }

        if (IsContainAddressableList_DinoParts(talent, tableData.tail) == false)
        {
            string prefabName = talent + tableData.tail;
            prefabName = prefabName.Remove(prefabName.LastIndexOf("_"));
            Debug.LogError($"[파츠테이블] 해당 프리팹에 파츠가 없습니다. 프리팹 : {prefabName}  파츠 : {tableData.tail}");
            return false;
        }

        if (IsContainAddressableList_DinoParts(talent, tableData.wing) == false)
        {
            string prefabName = talent + tableData.wing;
            prefabName = prefabName.Remove(prefabName.LastIndexOf("_"));
            Debug.LogError($"[파츠테이블] 해당 프리팹에 파츠가 없습니다. 프리팹 : {prefabName}  파츠 : {tableData.wing}");
            return false;
        }

        if (IsContainAddressableList_DinoParts(talent, tableData.belly) == false)
        {
            string prefabName = talent + tableData.belly;
            prefabName = prefabName.Remove(prefabName.LastIndexOf("_"));
            Debug.LogError($"[파츠테이블] 해당 프리팹에 파츠가 없습니다. 프리팹 : {prefabName}  파츠 : {tableData.belly}");
            return false;
        }

        // color

        return true;
    }
    #endregion

    #region >> 디노 테이블 관련 <<
    bool CheckDinoTable()
    {
        Debug.LogError(">>>>>>>>>>>>>>>> 디노 테이블 검사 <<<<<<<<<<<<<<<<<");

        Dino_ClassTable table = CSVDataManager.GetTable<Dino_ClassTable>();

        foreach ( var element in table.Dic )
        {
            Dino_ClassData data = element.Value;

            // 이름 검사
            if (CheckStringTable(data.name_string) == false)
                return false;

            if (CheckDinoPartTable(data.talent, data.type) == false)
                return false;

            // res_icon 검사
            if (IsContainAddressableList(data.res_atlas, data.res_icon) == false)
            {
                Debug.LogError($"[몬스터테이블] idx : {data.idx}, res_atlas : {data.res_atlas}, res_icon : {data.res_icon}");
                return false;
            }
        }

        return true;
    }

    bool CheckDinoPartTable(CharacterTalent _talent, CharacterClass _class)
    {
        Dictionary<int, Generated.CsvData.partsData> table = CSVDataManager.GetTable<PartsTable>().GetDataList(_class);

        if (table == null)
        {
            Debug.LogError($"[디노 파츠테이블] CharacterClass : {_class}");
            return false;
        }

        string talent = string.Empty;

        switch (_talent)
        {
            case CharacterTalent.Carnivore:
                talent = "ca_";
                break;
            case CharacterTalent.Herbivore:
                talent = "he_";
                break;
            case CharacterTalent.Omnivore:
                talent = "om_";
                break;
        }

        foreach (var element in table)
        {
            foreach (var subElement in element.Value.partList)
            {
                partsInfo data = subElement.Value;

                if (data.partName.ToLower().Contains("none") == true)
                    continue;

                if (IsContainAddressableList_DinoParts(talent, data.fileName) == false)
                {
                    string prefabName = talent + data.fileName;
                    prefabName = prefabName.Remove(prefabName.LastIndexOf("_"));
                    Debug.LogError($"[파츠테이블] 해당 프리팹에 파츠가 없습니다. 프리팹 : {prefabName}  파츠 : {data.fileName}");
                    return false;
                }

                if (data.skill01 > 0)
                {
                    if (CheckSkillTable(Monster_Type.None, element.Value.index, data.skill01) == false)
                        return false;
                }

                if (data.skill02 > 0)
                {
                    if (CheckSkillTable(Monster_Type.None, element.Value.index, data.skill02) == false)
                        return false;
                }
            }
        }

        return true;
    }
    #endregion

    #region >> Skill <<
    bool CheckSkillTable(Monster_Type monsterType, int _partTableIndex, int _skillTableIndex)
    {
        if (_skillTableIndex <= 0)
            return true;

        SkillData findData = CSVDataManager.GetTable<SkillTable>().GetData(_skillTableIndex);

        if (findData == null)
        {
            if (_partTableIndex > 0)
                Debug.LogError($"스킬 테이블에 없습니다. 파츠테이블 ID : {_partTableIndex}, 스킬테이블 ID : {_skillTableIndex} ");
            else
                Debug.LogError($"스킬 테이블에 없습니다. 스킬테이블 ID : {_skillTableIndex} ");

            return false;
        }

        // 스킬 이름 검사
        if (CheckStringTable(findData.name_string) == false)
        {
            Debug.LogError($"[스킬 테이블] idx : {_skillTableIndex}, name_string : {findData.name_string}");
            return false;
        }

        // 스킬 설명 검사
        if (CheckStringTable(findData.desc_string) == false)
        {
            Debug.LogError($"[스킬 테이블] idx : {_skillTableIndex}, desc_string : {findData.desc_string}");
            return false;
        }

        // level_up_item1_id
        if (CeckItemTable(findData.level_up_item1_id) == false)
        {
            Debug.LogError($"[스킬 테이블] idx : {_skillTableIndex}, level_up_item1_id : {findData.level_up_item1_id}");
            return false;
        }

        // level_up_item2_id
        if (CeckItemTable(findData.level_up_item1_id) == false)
        {
            Debug.LogError($"[스킬 테이블] idx : {_skillTableIndex}, level_up_item2_id : {findData.level_up_item2_id}");
            return false;
        }

        // buff_id
        //if (CheckBuffTable(findData.buff_id) == false)
        //{
        //    Debug.LogError($"[스킬 테이블] idx : {_skillTableIndex}, buff_id : {findData.buff_id}");
        //    return false;
        //}

        //// cc_id
        //if (CheckCCTable(findData.cc_id) == false)
        //{
        //    Debug.LogError($"[스킬 테이블] idx : {_skillTableIndex}, cc_id : {findData.cc_id}");
        //    return false;
        //}

        //// stack_id
        //if (CheckStackTable(findData.stack_id) == false)
        //{
        //    Debug.LogError($"[스킬 테이블] idx : {_skillTableIndex}, cc_id : {findData.stack_id}");
        //    return false;
        //}

        // res_fx_run
        if (IsContainAddressableList(findData.res_fx_run, "") == false)
        {
            Debug.LogError($"[스킬 테이블] idx : {_skillTableIndex}, res_fx_attack : {findData.res_fx_run}");
            return false;
        }

        // res_fx_attack 검사
        if (IsContainAddressableList(findData.res_fx_attack, "") == false)
        {
            Debug.LogError($"[스킬 테이블] idx : {_skillTableIndex}, res_fx_attack : {findData.res_fx_attack}");
            return false;
        }

        // res_fx_hit 검사
        if (IsContainAddressableList(findData.res_fx_hit, "") == false)
        {
            Debug.LogError($"[스킬 테이블] idx : {_skillTableIndex}, res_fx_hit : {findData.res_fx_hit}");
            return false;
        }

        // res_fx_projectile 검사
        if (IsContainAddressableList(findData.res_fx_projectile, "") == false)
        {
            Debug.LogError($"[스킬 테이블] idx : {_skillTableIndex}, res_fx_projectile : {findData.res_fx_projectile}");
            return false;
        }

        // res_ani_attack 검사
        if (CheckAniTable(monsterType, findData.res_ani_attack) == false)
        {
            Debug.LogError($"[스킬 테이블] idx : {_skillTableIndex}, res_ani_attack : {findData.res_ani_attack}");
            return false;
        }

        // res_icon 검사
        if( IsContainAddressableList(findData.res_atlas, findData.res_icon) == false)
        {
            Debug.LogError($"[스킬 테이블] idx : {_skillTableIndex}, res_atlas : {findData.res_atlas}, res_icon : {findData.res_icon}");
            return false;
        }

        return true;
    }
    #endregion

    #region >> Buff <<
    bool CheckBuffTable(int _tID)
    {
        if (_tID <= 0)
            return true;

        var findData = CSVDataManager.GetTable<Skill_BuffTable>().GetData(_tID);

        if (findData == null)
        {
            Debug.LogError($"[BuffTable] idx: {_tID}");
            return false;
        }

        if (CheckStringTable(findData.name_string) == false)
        {
            Debug.LogError($"[BuffTable 테이블] idx : {_tID}, name_string : {findData.name_string}");
            return false;
        }

        // res_fx_buff
        if (IsContainAddressableList(findData.res_fx_buff, "") == false)
        {
            Debug.LogError($"[BuffTable 테이블] idx : {_tID}, res_fx_buff : {findData.res_fx_buff}");
            return false;
        }

        // buff_icon
        if (IsContainAddressableList(findData.res_atlas, findData.res_icon) == false)
        {
            Debug.LogError($"[BuffTable 테이블] idx : {_tID}, res_atlas : {findData.res_atlas}, res_icon : {findData.res_icon}");
            return false;
        }

        return true;
    }
    #endregion

    #region >> CC <<
    bool CheckCCTable(int _tID)
    {
        if (_tID <= 0)
            return true;

        var findData = CSVDataManager.GetTable<Skill_CcTable>().GetData(_tID);

        if (findData == null)
        {
            Debug.LogError($"[CC Table] idx: {_tID}");
            return false;
        }

        if (CheckStringTable(findData.name_string) == false)
        {
            Debug.LogError($"[CC 테이블] idx : {_tID}, name_string : {findData.name_string}");
            return false;
        }

        // 일반 몬스터는 테이블에 없으므로 문제가 발생한다.
        // 먼가 해결책이 필요할듯
        if (CheckAniTable(Monster_Type.Dino, findData.res_ani_cc) == false)
        {
            Debug.LogError($"[CC 테이블] idx : {_tID}, name_string : {findData.name_string}");
            return false;
        }

        // res_fx_buff
        if (IsContainAddressableList(findData.res_fx_cc, "") == false)
        {
            Debug.LogError($"[CC 테이블] idx : {_tID}, res_fx_cc : {findData.res_fx_cc}");
            return false;
        }

        // buff_icon
        if (IsContainAddressableList(findData.res_atlas, findData.res_icon) == false)
        {
            Debug.LogError($"[CC 테이블] idx : {_tID}, res_atlas : {findData.res_atlas}, res_icon : {findData.res_icon}");
            return false;
        }

        return true;
    }
    #endregion

    #region >> Stack <<
    bool CheckStackTable(int _tID)
    {
        if (_tID <= 0)
            return true;

        var findData = CSVDataManager.GetTable<Skill_StackTable>().GetData(_tID);

        if (findData == null)
        {
            Debug.LogError($"[Stack Table] idx: {_tID}");
            return false;
        }

        if (CheckStringTable(findData.name_string) == false)
        {
            Debug.LogError($"[Stack 테이블] idx : {_tID}, name_string : {findData.name_string}");
            return false;
        }

        //if (CheckBuffTable(findData.activation_buff_id) == false)
        //{
        //    Debug.LogError($"[Stack 테이블] idx : {_tID}, activation_buff_id : {findData.activation_buff_id}");
        //    return false;
        //}

        //if (CheckCCTable(findData.activation_cc_id) == false)
        //{
        //    Debug.LogError($"[Stack 테이블] idx : {_tID}, activation_buff_id : {findData.activation_cc_id}");
        //    return false;
        //}

        // res_fx_buff
        if (IsContainAddressableList(findData.res_fx_activation_stack, "") == false)
        {
            Debug.LogError($"[Stack 테이블] idx : {_tID}, res_fx_activation_stack : {findData.res_fx_activation_stack}");
            return false;
        }

        // buff_icon
        if (IsContainAddressableList(findData.res_atlas, findData.res_icon) == false)
        {
            Debug.LogError($"[Stack 테이블] idx : {_tID}, res_atlas : {findData.res_atlas}, res_icon : {findData.res_icon}");
            return false;
        }

        return true;
    }
    #endregion

    #region >> Ani <<
    bool CheckAniTable(Monster_Type _monsterType, int _aniTableIndex)
    {
        if (_aniTableIndex <= 0)
            return true;

        // 일반 몬스터는 애니테이블을 사용하지 않는다.
        if (_monsterType == Monster_Type.Normal)
            return true;

        aniTableData findData = null;
        CharacterTalent[] talentList = { CharacterTalent.Carnivore, CharacterTalent.Herbivore, CharacterTalent.Omnivore };
        bool[] isWing = { false, true };

        foreach(var element in talentList)
        {
            foreach( var subElement in isWing)
            {
                findData = CSVDataManager.GetTable<AniTable>().GetData(_aniTableIndex, element, subElement);

                if (findData == null)
                {
                    Debug.LogError($"AniTable에 없습니다. ID : {_aniTableIndex}, Talent : {element}, Wing : {subElement}");
                    return false;
                }

                if (IsContainAddressableList(findData.res_ani, "") == false)
                {
                    Debug.LogError($"[애니테이블] 어드레스블에 등록이 안되어 있습니다. 파일 : {findData.res_ani}");
                    return false;
                }
            }
        }

        return true;
    }
    #endregion

    #region >> String <<
    bool CheckStringTable(string _stringTableIndex)
    {
        if (_stringTableIndex.ToLower().Contains("none") == true)
            return true;

        var findData = CSVDataManager.GetTable<StringTable>().GetData(_stringTableIndex);

        if (findData == null)
        {
            Debug.LogError($"[StringTable] idx: {_stringTableIndex}");
            return false;
        }

        return true;
    }
    #endregion

    #region >> ITEM <<
    bool CeckItemTable(int _tID)
    {
        if (_tID <= 0)
            return true;

        if( CSVDataManager.GetTable<ItemTable>() == null )
        {
            Debug.LogError($"[ItemTable] 아이템 테이블 로드 실패");
            return false;
        }

        ItemData data = CSVDataManager.GetTable<ItemTable>().GetData(_tID);

        if (data == null)
        {
            Debug.LogError($"[ItemTable] idx : {_tID}");
            return false;
        }

        // name_string
        if (CheckStringTable(data.name_string) == false)
        {
            Debug.LogError($"[ItemTable] idx : {data.idx}, name_string : {data.name_string}");
            return false;
        }

        // desc_string
        if (CheckStringTable(data.desc_string) == false)
        {
            Debug.LogError($"[ItemTable] idx : {data.idx}, desc_string : {data.desc_string}");
            return false;
        }

        // res_icon 검사
        if (IsContainAddressableList(data.res_atlas, data.res_icon) == false)
        {
            Debug.LogError($"[ItemTable] idx : {data.idx}, res_atlas : {data.res_atlas}, res_icon : {data.res_icon}");
            return false;
        }

        return true;
    }
    #endregion

    #region >> 헬프 팝업 관련 <<
    bool CheckHelpPopupTable()
    {
        Debug.LogError(">>>>>>>>>>>>>>>> 헬프 팝업 테이블 검사 <<<<<<<<<<<<<<<<<");

        HelpPopupBaseTable table = CSVDataManager.GetTable<HelpPopupBaseTable>();

        foreach (var element in table.DicData)
        {
            helpPopupBaseData data = element.Value;

            //res_prefab
            if (IsContainAddressableList(data.mPrefabName, "") == false)
            {
                Debug.LogError($"[스테이지 테이블] idx : {data.mIdx}, prefabname : {data.mPrefabName}");
                return false;
            }
        }

        return true;
    }
    #endregion
}
