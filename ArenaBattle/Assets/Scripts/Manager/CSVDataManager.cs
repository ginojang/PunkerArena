using Generated.CsvData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVDataManager : Singleton<CSVDataManager>
{
    static bool isInit = false;
    private static Dictionary<Type, object> _dicTable = new Dictionary<Type, object>();
    private static Dictionary<Type, object> _dicServerTable = new Dictionary<Type, object>();

    public static void InitTables(bool init = false)
    {
        if (init)
        {
            ClearTableDatas();
            isInit = false;
        }

        if (isInit)
            return;

        //InitAWSData();

        // SetTableData<DinoTable>("dino_basic");
        // SetTableData<ExpTable>("expTable");
        // SetTableData<SetOptionTable>("set_option");
        // SetTableData<StringTable>("string");
        // SetTableData<SkillTable>("skill");
        //
        SetTableData<PartsTable>("dino_part");
        // SetTableData<TalentTable>("talent");
        // SetTableData<randomOptionTable>("random_option");
        // SetTableData<PureDnaBonusTable>("pure_dna_bonus");
        // SetTableData<EffectTable>("effect");
        //
        // //�ű� ���̺�
        // //SetTableData<CameraTable>("camera");
        // // SetTableData<DinoMonsterTable>("dino_monster");
        // SetTableData<DinoStatusTable>("dino_status");
        // SetTableData<LimitValueTable>("limitvalue");
        // //SetTableData<MonsterAITable>("monsterai");
        // SetTableData<SkillLevelTable>("skilllevel");
        // SetTableData<TimeChangeTable>("timechange");
        // SetTableData<StageMonsterTable>("stagemonster");
        // SetTableData<EffectListTable>("effectlist");
        // SetTableData<BuffTable>("buff");
        // SetTableData<AniskillTable>("aniskill");
        // SetTableData<BurstTable>("burst");
        // SetTableData<BurstTypeTable>("burst_type");
        
        SetTableData<Anim_EventTable>("ani_event");
        SetTableData<ArgTable>("Arg");
        SetTableData<AttributeTable>("Dino_Attribute");
        SetTableData<Dino_ClassTable>("Dino_Class");
        SetTableData<MonsterTable>("Monster");
        SetTableData<Monster_GroupTable>("Monster_Group");
        SetTableData<Monster_PartTable>("Monster_Part");
        SetTableData<SkillTable>("Skill");
        SetTableData<StageTable>("Stage");
        SetTableData<TagTable>("Tag");
        SetTableData<AniTable>("Ani");
        SetTableData<StringTable>("String");
        SetTableData<Skill_StackTable>("Skill_Stack");
        SetTableData<Skill_BuffTable>("Skill_Buff");
        SetTableData<Skill_CcTable>("Skill_Cc");
        SetTableData<TriggerTable>("Trigger");
        SetTableData<ConditionTable>("Condition");
        SetTableData<EffectTable>("Effect");

        #region >>원정대<
        SetTableData<ConstBaseTable>("const_base");
        SetTableData<StringUiTable>("string_ui");
        SetTableData<HelpPopupBaseTable>("helppopup_base");
        #endregion

        isInit = true;

//        DinoTable dino = GetTable<DinoTable>();
//        string str = GetTable<StringTable>().GetLocalString(1);

    }

    public static void ClearTableDatas()
	{
        _dicTable.Clear();
    }

    public static void SetTableData<T>(string path) where T : new()
    {
        T t;

        List <Dictionary<string, string>> dataDic = new List<Dictionary<string, string>>();
        ReadCSV(path, ref dataDic);

        t = new T();
        IDefTable table = t as IDefTable;
        table.SetData(dataDic);
        _dicTable.Add(typeof(T), t);
    }

    public static void SetServerData<T>(string csvData) where T : new()
    {
        T t;

        List<Dictionary<string, string>> dataDic = new List<Dictionary<string, string>>();
        ReadCSV<T>(csvData, ref dataDic);

        t = new T();
        IDefTable table = t as IDefTable;
        table.SetData(dataDic);
        _dicTable.Add(typeof(T), t);
    }

    public static void SetTableData<T>(string path, CharacterClass type = CharacterClass.None) where T : new()
    {
        T t;

        List<Dictionary<string, string>> dataDic = new List<Dictionary<string, string>>();
        ReadCSV(path, ref dataDic);

        if (_dicTable.ContainsKey(typeof(T)))
        {
            t = GetTable<T>();
            IDefTable table = t as IDefTable;
            table.SetData(dataDic, type);
        }
        else
        {
            t = new T();
            IDefTable table = t as IDefTable;
            table.SetData(dataDic, type);
            _dicTable.Add(typeof(T), t);
        }
    }

    public static void SetTableData<T>(string path, CharacterTalent type = CharacterTalent.None) where T : new()
    {
        T t;

        List<Dictionary<string, string>> dataDic = new List<Dictionary<string, string>>();
        ReadCSV(path, ref dataDic);

        if (_dicTable.ContainsKey(typeof(T)))
        {
            t = GetTable<T>();
            IDefTable table = t as IDefTable;
            table.SetData(dataDic, type);
        }
        else
        {
            t = new T();
            IDefTable table = t as IDefTable;
            table.SetData(dataDic, type);
            _dicTable.Add(typeof(T), t);
        }
    }

    public T SetServerTable<T>() where T : new()
    {
        if (_dicTable.ContainsKey(typeof(T)))
            return default;

        T t = new T();
        _dicTable.Add(typeof(T), t);

        return t;
    }

    public static T GetTable<T>()
    {
        if (_dicTable.ContainsKey(typeof(T)) == false)
        {
            Debug.LogErrorFormat("���̺� �����Ͱ� �����ϴ�. ���̺�� : {0}", typeof(T).ToString());
            return default(T);
        }

        return (T)_dicTable[typeof(T)];
    }

    // �������� �о�� Txt ���Ϸ� ������ȭ ��
    public static bool ReadCSV<T>(string strData, ref List<Dictionary<string, string>> dataDic)
    {
        dataDic.Clear();
        dataDic = CSVReader.ReadData(strData);

        return dataDic != null;
    }

    public static bool ReadCSV(string strPath, ref List<Dictionary<string, string>> dataDic)
    {
        dataDic.Clear();
        dataDic = CSVReader.Read(strPath);

        return dataDic != null;
    }

    #region ������ ���� �Լ� ����
    public static Dictionary<int, Generated.CsvData.partsData> GetClassPartInfo(CharacterClass charClass)
    {
        PartsTable parts = GetTable<PartsTable>();
        Dictionary<int, Generated.CsvData.partsData> datas = parts.GetDataList(charClass);

        return datas;
    }

    public static Dictionary<int, Generated.CsvData.partsData> GetRandomPartItem(ItemType type, ref CharacterClass charclass, ref int index)
    {
        PartsTable parts = GetTable<PartsTable>();

        Dictionary<int, Generated.CsvData.partsData> datas = parts.GetRandomDataList(type, ref charclass, ref index);

        return datas;
    }
    #endregion ������ ���� �Լ� ����

    #region ĳ���� Ŭ���� ã��
    public static CharacterClass SearchClass(string str)
    {
        CharacterClass characterClass = CharacterClass.None;
        string[] strArray;
        string result;

        strArray = str.Split('_');
        result = strArray[1];

        for(int i = 0; i < (int)CharacterClass.limited; i++)
        {
            if(result == ((CharacterClass)i).ToString())
            {
                characterClass = (CharacterClass)i;
                break;
            }
        }

        return characterClass;
    }
    #endregion
    #region ���� ������ �̸� ã��
    public static List<string> GetMonsterPrefab()
    {
        List<string> monster = new List<string>();
        var data = CSVDataManager.GetTable<MonsterTable>().DicData;

        foreach(var stageInfo in data)
        {
            var monsterName = stageInfo.Value.res_prefab;
            monster.Add(monsterName);
        }

        return monster;
    }
    #endregion
    #region Effect
    //public static List<int> GetCommonEffect()
    //{
    //    List<int> commonEffect = new List<int>();
    //    var _dicData = GetTable<EffectTable>().DicData;

    //    foreach (var effect in _dicData)
    //    {
    //        var eventType = effect.Value.event_type;
    //        if (eventType == EffectEventType.Common)
    //            commonEffect.Add(effect.Key);
    //    }

    //    return commonEffect;
    //}
    //public static int GetEffectIndex(string name)
    //{
    //    int index = 0;
    //    var _dicData = GetTable<EffectTable>().DicData;

    //    foreach (var effect in _dicData)
    //    {
    //        string effectName = effect.Value.filename;
    //        if(name == effectName)
    //        {
    //            index = effect.Value.index;
    //            break;
    //        }
    //    }

    //    return index;
    //}
    #endregion

    #region StageTimeData
    public static StageMonsterTable.stageData GetStageTimeData(int index)
    {
        StageMonsterTable.stageData data;
        GetTable<StageMonsterTable>()._stageDicData.TryGetValue(index, out data);

        return data;
    }
    #endregion

    #region Monster

    public static int[] GetAllMonsterGroup(int stage)
    {
        int[] group = CSVDataManager.GetTable<StageTable>().GetData(stage).monsterGroupID;
        
        return group;
    }
    

    #endregion
}
