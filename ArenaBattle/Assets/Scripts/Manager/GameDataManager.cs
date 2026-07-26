using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using Generated.CsvData;
public class StatusBase
{
    public CharacterClass class_type;
    public Grade grade;
    public Attributes attribute;
    public Role role;
    public float str;
    public float vit;
    public float agi;
    public float dex;
    public float luk;
    public float atk;
    public float def;
    public float def_pen;
    public float hp;
    public float hit;
    public float avd;
    public float cri;
    public float cri_dmg;
    public float res;
    public float spd;

    public virtual void SetStatus(int index)
    {

    }
}
public class CharacterStatus : StatusBase
{
    public override void SetStatus(int index)
    {
        var data = CSVDataManager.GetTable<Dino_ClassTable>().GetData(index);

        class_type = data.type;
        grade = data.grade;
        attribute = data.attribute;
        role = data.role;
        str = data.str;
        vit = data.vit;
        agi = data.agi;
        dex = data.dex;
        luk = data.luk;
        atk = data.atk;
        def = data.def;
        def_pen = data.def_pen;
        hp = data.hp;
        hit = data.hit;
        avd = data.avd;
        cri = data.cri;
        cri_dmg = data.cri_dmg;
        res = data.res;
        spd = data.spd;
        //spd = 1;
    }
}
public class MonsterStatus : StatusBase
{
    public Monster_AI_Type type_ai;

    public int level;

    public override void SetStatus(int index)
    {
        var data = CSVDataManager.GetTable<MonsterTable>().GetData(index);

        class_type = data.monster_class;
        grade = data.grade;
        attribute = data.attribute;
        role = data.role;
        str = data.str;
        vit = data.vit;
        agi = data.agi;
        dex = data.dex;
        luk = data.luk;
        atk = data.atk;
        def = data.def;
        def_pen = data.def_pen;
        hp = data.hp;
        hit = data.hit;
        avd = data.avd;
        cri = data.cri;
        cri_dmg = data.cri_dmg;
        res = data.res;
        spd = data.spd;
    }

}

#region New Enum
public enum Action_Type
{
    None = 0,
    BuffDebuff,
    CC,
    Stack,
}
public enum Condition_Type
{

}
public enum Trigger_Timing
{
    None = 0,
    Hit = 1,
    Critical,
    Evade,
    Death,
    Revival,
    TurnStart,
    TurnEnd,
}
public enum Type_Target_Auto_AI
{
    None = 0,
    Anger,
    Confusion,
    Fascination,
}

public enum Cc_Group
{
    None,
    NoAction,
    Injury,
    AI,
    InTarget,
}
public enum Cc_Type
{

}
public enum Type_Action_Lock
{
    Possible = 0,
    Impossible,
}
public enum Type_Target_Lock
{
    Possible = 0,
    Impossible,
}
public enum Type_CC_Clear
{
    None,
    Possible,
}
public enum Type_Attack_Lock
{
    None = 0,
    Basic,
    Skill1,
    Skill2,
    Skills,
    All,
}
public enum Buff_Special_Status_Type
{
    None = 0,
    Reflect,
    Shield
}
public enum Buff_Stat_Type
{
    Str = 1,
    HP,
    Dex,
    Agi,
    Luk,

    Atk = 11,
    Def
}
public enum Buff_Type
{
    Buff = 1,
    DeBuff,
    Revival,
}
public enum Dmg_Add_Type
{
    None = 0,
    Str,
    HP,
    Dex,
    Agi,
    Luk
}
public enum Type_Calculation
{
    None = 0,
    Hit,
    Heal,
}
public enum Stack_Type
{
    Positive = 1,
    Negative,
}
public enum Link_Bone_Move
{
    Fix,
    Bone,
}
public enum Fx_Projectile_Type
{
    Common = 1,
    Projectile
}
public enum Type_Action
{
    None,
    Range = 2,
    ParabolaRange,

    Melee = 6,
    CenterMelee,
    AcionMoveMelee,
    ActionMoveCenterMelee,
}
public enum Trigger_Condition
{
    Confirmation = 0,
    Hit,
    Critical,
    Death,
}
//public enum Type_Action
//{

//}

public enum Type_Active
{
    Active = 1,
    Passive,
}

public enum Type_Range
{
    Melee = 1,
    Range
}

public enum Type_Target
{
    Target = 1,
    Self,
    Enemey,
    Ally,
}
public enum Monster_Type
{
    None,
    Normal,
    Dino
}
public enum Grade
{
    Normal,
    Rare,
    Legend
}
public enum Monster_AI_Type
{
    Normal,
    Attack,
    Defence,
}
public enum Role
{
    Tanker,
    Attacker,
    Supporter,
}
public enum Attributes
{
    Day = 1,
    Night,
    Dawn,
    Eclipse,
}

public enum CharacterClass
{
    None = 0,
    orange = 1,
    watermelon,
    durian,
    coconut,
    blueberry,
    melon,
    pineapple,
    banana,
    rambutan,
    dragonfruit,
    limited = 100,
}

public enum ValueType
{
    Int,
    Float,
    String
}
public enum ItemTypes
{
    Parts,
    Rune,
    Recipe,
    Material,
    Use
}
public enum Camp
{
    Ally,
    Enemy,
}
#endregion

#region Old Enum
public enum BuffType
{
    None = 0,
    TurnBuff,
    RealTimeBuff,
    Passive,
    TimeSystemBuff,
}
public enum BuffUseCondition
{
    None,
    Hit,
    UseSkill,
    Death,
    Heal,
    Critical,
    Evade,
    Miss
}
public enum BuffTargetType
{
    None,
    Self,
    Ally,
    Enemy,
    All,
    EnemyAll,
    AllyAll,
}
public enum BuffEffect
{
    None,
    HP,
    SP,
    MeleeDamage,
    RangeDamage,
    Defence,
    AttackRating,
    Evade,
    Critical,
    CriticalDamage,
    AttackSpeedGaugeDown,
    AttackSpeed,
    AttackPotion,
    Silence,
}
public enum BuffTimeType
{
    None,
    TurnCount,
    RealTime,
}

public enum ItemType
{
    body,
    headparts,
    eyes,
    mouth,
    back,
    tail,
    wing,
    pattern,
    Max,
}

// 캐릭터의 상성을 구분할 최상위 단계
public enum CharacterAttribute
{
    Attribute_Day = 1,
    Attribute_Night,
    Attribute_Dawn,
    Attribute_Eclipse,
}

// 캐럭터 등급(노말, 레어, 스페셜)
public enum CharacterRanking
{
    Normal,
    Rare,
    Legend,
}



// 캐릭터 속성(탱커, 딜러, 유틸)
public enum CharacterTalent
{
    None = 0,
    Carnivore,
    Herbivore,
    Omnivore,
    Max,
}

public enum CampType
{
    None,
    Owner,
    Enemy,
}

public enum CharacterType
{
    Character,
    Monster,
}

public enum MonsterGrade
{
    None,
    Normal,
    Boss
}

public enum CharacterArrangement
{
    Entry,
    Front,
    Middle,
    Back
}

public enum SkillTargetType
{
    None,
    Ally,
    Enemy
}
public enum SkillTargetAmount
{
    None,
    Single,
    Random,
    HighHP,
    LowHP,
    Multi,
}
public enum SkillDamageType
{
    None,
    Melee,
    Range,
    Heal,
    Buff
}
public enum SkillActionType
{
    None = 0,
    Move,
    Melee,
    Shoot,
    Stand,
}
public enum SkillLine
{
    None,
    Single,
    Row,
    Column,
    Around,
    Cross,
    All,
    Self,
}
public enum SkillSeletctType
{
    None,
    Targeting,
    NoneTargeting,
}
public enum SkillAddHit
{
    None,
    Use
}

public enum PartsOptionType
{
    None,

}
public enum DinoStatusOption
{
    None,
}
public enum DinoStatus
{
    Str,
    Vit,
    Dex,
    Agi,
    Luk
}
public enum TargetAIDistance
{
    None,
    Front,
    Middle,
    Back,
}
public enum TargetHP
{
    None,
    HighHP,
    LowHP,
}
// public enum TimeBuff
// {
//     None,
// }
public enum StageType
{
    Easy,
    Hard,
}
public enum AttachPoint
{
    None,
    Own,
    Slot,
}
#endregion
public class partsInfo
{
    public ItemType itemType;
    public string fileName; // 바디 파츠 파일명
    public string partName; // 바디 파츠 이름. 스트링 테이블 참조 인덱스
    //public int partSkill; // 스킬테이블 참조 인덱스
    //public PartsOptionType partOptionType;
    //public int partOptionValue; // 해당 스탯의 값
    public int skill01;
    public int skill02;

}
public class dinoStatusValue
{
    public DinoStatusOption statusOption;
    public float value;
}

public enum LANG
{
    KOR,
    ENG,
    MAX,
}

public enum EffectTarget
{
    None,
    Caster,
    Target,
    Shoot,

}
public enum BurstMode
{
    none = 0,
    normal,
    full,
    mega,
    ultra,
}

public enum EStringTableType
{
    None,
    String,
    UI,
    Max
}

// 해당 값은 AniTable 인덱스랑 매칭된다.
public enum EDefaultAniType
{
    idle = 1,
    run = 2,
    attack = 3,
    hit = 4,
    //hit_critical = 5, 현재 없음
    //avoid = 6,
    death = 7,
    skill_run1 = 9,
    skill_run2 = 10,
    //stun = 101,
    //freeze = 102,
    //sleep = 103,
    //injuri = 104,
    //confusion = 105,
    //emotion1 = 201,
    //emotion2 = 202,
    //emotion3 = 203,
    //emotion4 = 204,
}

public class stringLangInfo
{
    public string kr;
    public string en;
}
/*
// 나중에는 애니메이션 이벤트로 이펙트는 싹 바꿀 예정
public class effectInfo
{
    public CharacterBase charBase = null;
    public float effectTime = 0f;
    public GameObject effectObject = null;

    public effectInfo(string effectname, CharacterType type, Transform parent)
	{
        effectObject = GameDataManager.Instance.GetSkillEffectObject(effectname, type, parent);
    }
}
*/

public class eventInfoTool
{
    public string invokeName;
    public float invokeTime;
    public int effectIndex;
    public int targetIndex;
}

public class eventInfoList
{
    public List<AnimEventData> infoList = new List<AnimEventData>();
}

public class selectedPartInfo
{
    public selectedPartInfo(CharacterTalent talent, CharacterClass charclass, string partname, int idx)
    {
        charTalent = talent;
        charClass = charclass;
        partItemName = partname;
        index = idx;
    }

    public CharacterTalent charTalent = CharacterTalent.Carnivore;
    public CharacterClass charClass = CharacterClass.orange;
    public string partItemName = "";
    public ItemType part = ItemType.body;
    public int index = -1;
}

public class GameDataManager : Singleton<GameDataManager>
{
    private bool isInitialize = false;

    public UserData userData { get; private set; } = new UserData();

    private GameDataManager() { }

    public GameObject baseCharacter = null;
    public GameObject baseMonster = null;
    // 랜덤 캐릭터로...
    public List<GameObject> baseCharacterList = new List<GameObject>();
    public List<GameObject> baseMonsterList = new List<GameObject>();

    public GameObject baseProjectile = null;

    //임시 데이터 적용
    public Dictionary<int, Generated.CsvData.SkillData> skillDatas = null;
    public List<CharacterBase> tempList = new List<CharacterBase>();

    private int resWidth;
    private int resHeight;
    string path;

    public Texture2D[] characterImage = new Texture2D[Definition.MAX_CHARACTER];
    public Texture2D[] monsterImage = new Texture2D[Definition.MAX_MONSTER];

    public SystemLanguage PlayerLanguage
    {
        get
        {
#if PRODUCTION
                //PRODUCTION은 무조건 Korean
                return SystemLanguage.Korean ;
#else
            return (SystemLanguage)PlayerPrefs.GetInt(Definition.STR_LOCALIZATION_KEY, (int)Application.systemLanguage);
#endif
        }
        set
        {
            PlayerPrefs.SetInt(Definition.STR_LOCALIZATION_KEY, (int)value);
            PlayerPrefs.Save();
        }
    }

    public void Initialize()
    {
        if (isInitialize == false)
        {
            isInitialize = true;
            

            LoadGameData();

            if(userData == null)
			{
                SaveGameData();
			}
        }

        //resWidth = 450;
        //resHeight = 600;
        resWidth = 128;
        resHeight = 128;
        path = Application.dataPath + "/ScreenShot/";
        Debug.Log(path);
    }

    public void LoadGameData()
    {
        if (ES3.KeyExists("userData"))
        {
            var bytes = ES3.Load<byte[]>("userData");
            var jsonString = System.Text.Encoding.UTF8.GetString(bytes);
            userData = JsonConvert.DeserializeObject<UserData>(jsonString);
        }

        PlayerLanguage = Application.systemLanguage;


        LoadMonster();
    }

    public void SaveGameData()
    {
        var jsonString = JsonConvert.SerializeObject(userData);
        ES3.Save<byte[]>("userData", System.Text.Encoding.UTF8.GetBytes(jsonString));
    }

    public List<int> GetSkillIndexByClass(CharacterClass charclass)
    {
        skillDatas = new Dictionary<int, Generated.CsvData.SkillData>();
        skillDatas = CSVDataManager.GetTable<SkillTable>().DicData;
        List<int> skillList = new List<int>();

        foreach (var data in skillDatas)
        {
            // if (charclass == data.Value.skill_class|| data.Value.skill_class == CharacterClass.None)
            // {
            //     skillList.Add(data.Key);
            // }
        }

        return skillList;
    }

    // 한마리 캐릭터의 아이템 정보로 이미지 만들기
    public GameObject CharacterSnapShotByImage(Dictionary<ItemType, costumePartsInfo> itemlist, Transform parent, bool iswing, bool rotate = false)
    {
        GameObject Obj = null;

        ResourcePoolManager.Instance.AsyncGetData("UiCommonCharacter2D", objectType.gameobject, null, parent, (obj) =>
        {
            Obj = (GameObject) obj;

            foreach (var part in itemlist)
            {
                if(part.Key == ItemType.pattern)
                    continue;

                if (!iswing)
                {
                    Transform child = Obj.transform.Find("wing");
                    
                    child.gameObject.SetActive(false);
                }

                ResourcePoolManager.Instance.AsyncGetData("CharacterParts", objectType.atlasSprite, null, null, (sprite) =>
                {
                    Sprite charSprite = (Sprite) sprite;

                    Transform child = Obj.transform.Find(part.Key.ToString());
                    
                    child.GetComponent<Image>().sprite = charSprite;
                }, itemlist[part.Key].partName);
            }

            if (parent != null)
            {
                Obj.transform.parent = parent;
                Obj.transform.localPosition = Vector3.zero;
            }
        });

        return Obj;
    }
    
    public GameObject CharacterSnapShotByImage(Dictionary<ItemType, string> itemlist, Transform parent, bool iswing, bool rotate = false)
    {
        GameObject Obj = null;

        ResourcePoolManager.Instance.AsyncGetData("UiCommonCharacter2D", objectType.gameobject, null, parent, (obj) =>
        {
            Obj = (GameObject) obj;

            foreach (var part in itemlist)
            {
                if(part.Key == ItemType.pattern)
                    continue;

                if (!iswing)
                {
                    Transform child = Obj.transform.Find("wing");
                    
                    child.gameObject.SetActive(false);
                }

                ResourcePoolManager.Instance.AsyncGetData("CharacterParts", objectType.atlasSprite, null, null, (sprite) =>
                {
                    Sprite charSprite = (Sprite) sprite;

                    Transform child = Obj.transform.Find(part.Key.ToString());
                    
                    child.GetComponent<Image>().sprite = charSprite;
                }, itemlist[part.Key]);
            }

            if (parent != null)
            {
                Obj.transform.parent = parent;
                Obj.transform.localPosition = Vector3.zero;
            }
        });

        return Obj;
    }
    
    public Texture2D CharacterSnapShot(Camera curCam, int index)
    {
        RenderTexture backup;
        CameraClearFlags preClearFlags = curCam.clearFlags;
        Color preBackgroundColor = curCam.backgroundColor;
/*
        DirectoryInfo dir = new DirectoryInfo(path);
        if (!dir.Exists)
        {
            Directory.CreateDirectory(path);
        }
*/
        string name;
        name = path + ($"Character{index+1}") + ".png";

        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        backup = curCam.targetTexture;
        curCam.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        //Rect rec = new Rect(0, 0, screenShot.width, screenShot.height);

        curCam.clearFlags = CameraClearFlags.SolidColor;
        curCam.backgroundColor = Color.black;
        curCam.Render();
        Texture2D blackBackgroundCapture = CaptureView(rt, screenShot.width, screenShot.height);
        curCam.backgroundColor = Color.white;
        curCam.Render();
        Texture2D whiteBackgroundCapture = CaptureView(rt, screenShot.width, screenShot.height);

        for (int x = 0; x < whiteBackgroundCapture.width; ++x)
        {
            for (int y = 0; y < whiteBackgroundCapture.height; ++y)
            {
                Color black = blackBackgroundCapture.GetPixel(x, y);
                Color white = whiteBackgroundCapture.GetPixel(x, y);
                if (black != Color.clear)
                {
                    whiteBackgroundCapture.SetPixel(x, y, GetColor(black, white));
                }
            }
        }

        whiteBackgroundCapture.Apply();
        screenShot = whiteBackgroundCapture;
        UnityEngine.Object.DestroyImmediate(blackBackgroundCapture);
        curCam.backgroundColor = preBackgroundColor;
        curCam.clearFlags = preClearFlags;


        //byte[] bytes = screenShot.EncodeToPNG();
        //File.WriteAllBytes(name, bytes);

        curCam.targetTexture = backup;

        return screenShot;
    }

    private Color GetColor(Color black, Color white)
    {
        float alpha = GetAlpha(black.r, white.r);
        return new Color(
            black.r / alpha,
            black.g / alpha,
            black.b / alpha,
            alpha);
    }

    private float GetAlpha(float black, float white)
    {
        return 1 + black - white;
    }

    private Texture2D CaptureView(RenderTexture rt, int width, int height)
    {
        Texture2D captureView = new Texture2D((int)width, (int)height, TextureFormat.ARGB32, false);
        RenderTexture.active = rt;
        captureView.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        return captureView;
    }

    #region InGame
    public void GameSceneMessengerAddListner()
    {
        Messenger.AddListener(Definition.LoadData, GameOjbectLoad);
    }
    public void GameSceneMessengerRemoveListner()
    {
        Messenger.RemoveListener(Definition.LoadData, GameOjbectLoad);
    }

    private void GameOjbectLoad()
    {
        if(tempList == null)
            tempList = new List<CharacterBase>();
        else
            tempList.Clear();

        LoadCharacter();
#if false
        LoadMonster();
#endif
    }
    public void LoadCharacter()
    {
        if (baseCharacter != null)
            return;
#if false
        List<object> param = new List<object>();
        List<AssetLoader> baseCharList = new List<AssetLoader>();

        for (int i = 1; i < 11; i++)
        {
            // 캐릭터 로드, 코스튬 및 리스트에 추가 등.
            AssetLoader fishLoader = AssetManager.Instance.PreLoadAsset($"Assets/Asset/Character/Character{i}.prefab", null);
            baseCharList.Add(fishLoader);
            param.Add(fishLoader);
        }
        AssetManager.Instance.LoadAssetAsyncForPreloadAsset(baseCharList, FinishCharacterLoad, param);
#else
        Messenger.AddListener<int, GameObject>(Definition.Dino_Costume_End, FinishLoadBaseChar);
        CostumeManager.Instance.LoadBaseCharacter($"{Definition.baseCharBodyName}", CharacterTalent.Carnivore, CharacterClass.orange);
#endif
    }

    private void FinishLoadBaseChar(int index, GameObject basechar)
	{
        Messenger.RemoveListener<int, GameObject>(Definition.Dino_Costume_End, FinishLoadBaseChar);
    }

    private void LoadAssetBaseChar(AssetLoader ld, object p)
    {
        if (false == ld.IsLoadSucceed)
            return;

        baseCharacter = ld.MainAsset as GameObject;
        baseMonster = ld.MainAsset as GameObject;
    }

    private void LoadMonster()
    {
        if (baseMonster != null)
            return;

        List<object> param = new List<object>();
        List<AssetLoader> baseMonList = new List<AssetLoader>();
        List<string> allMonster = CSVDataManager.GetMonsterPrefab();
        for(int i = 0; i < allMonster.Count; i++)
        {
            AssetLoader fishLoader = AssetManager.Instance.PreLoadAsset($"{allMonster[i]}", null);
            baseMonList.Add(fishLoader);
            param.Add(fishLoader);
        }
        AssetManager.Instance.LoadAssetAsyncForPreloadAsset(baseMonList, FinishMonsterLoad, param);
    }
    public GameObject GetMonsterObject(string monster)
    {
        GameObject obj = null;
        for(int i = 0; i < baseMonsterList.Count; i++)
        {
            if(baseMonsterList[i].name == monster)
            {
                obj = baseMonsterList[i];
                break;
            }
        }

        return obj;
    }
    private void FinishResourceLoad(AssetLoader Id, object p)
    {
        baseProjectile = Id.MainAsset as GameObject;
    }
    private void FinishCharacterLoad(object p)
    {
//        baseCharacter = Id.MainAsset as GameObject;

        List<object> charLoader = (List<object>)p;

        for (int i = 0; i < charLoader.Count; i++)
        {
            AssetLoader charobj = (AssetLoader)charLoader[i];
            GameObject charObj = charobj.MainAsset as GameObject;

            baseCharacterList.Add(charObj);
        }
    }
    private void FinishMonsterLoad(object p)
    {
        List<object> charLoader = (List<object>)p;

        for (int i = 0; i < charLoader.Count; i++)
        {
            AssetLoader charobj = (AssetLoader)charLoader[i];
            GameObject charObj = charobj.MainAsset as GameObject;

            baseMonsterList.Add(charObj);
        }
    }
#endregion
    //public CharacterSkillData.CharSkillData GetSkillDataByID(int id)
    //{
    //    int count = 0;
    //    foreach (var data in skillDatas.datas)
    //    {
    //        if (id == data.ID)
    //        {
    //            return skillDatas.datas[count];
    //        }

    //        count++;
    //    }

    //    return null;
    //}

    //public void LoadSkillEffectDatas()
    //{
    //    foreach (var skill in skillDatas.datas)
    //    {
    //        if (skill.subSkillInfo.startSkillEffectName != "")
    //        {
    //            if (!skillEffectSourceList.ContainsKey(skill.subSkillInfo.startSkillEffectName))
    //            {
    //                AssetManager.Instance.LoadAssetAsync<GameObject>($"Assets/Asset/SkillEffect/{skill.subSkillInfo.startSkillEffectName}", (AssetLoader ld, object p) =>
    //                {
    //                    skillEffectSourceList.Add(skill.subSkillInfo.startSkillEffectName, ld.MainAsset as GameObject);
    //                });
    //            }
    //        }
    //        if (skill.subSkillInfo.sourceSkillEffectName != "")
    //        {
    //            if (!skillEffectSourceList.ContainsKey(skill.subSkillInfo.sourceSkillEffectName))
    //            {
    //                AssetManager.Instance.LoadAssetAsync<GameObject>($"Assets/Asset/SkillEffect/{skill.subSkillInfo.sourceSkillEffectName}", (AssetLoader ld, object p) =>
    //                {
    //                    skillEffectSourceList.Add(skill.subSkillInfo.sourceSkillEffectName, ld.MainAsset as GameObject);
    //                });
    //            }
    //        }
    //        if (skill.subSkillInfo.destSkillEffectName != "")
    //        {
    //            if (!skillEffectSourceList.ContainsKey(skill.subSkillInfo.destSkillEffectName))
    //            {
    //                AssetManager.Instance.LoadAssetAsync<GameObject>($"Assets/Asset/SkillEffect/{skill.subSkillInfo.destSkillEffectName}", (AssetLoader ld, object p) =>
    //                {
    //                    skillEffectSourceList.Add(skill.subSkillInfo.destSkillEffectName, ld.MainAsset as GameObject);
    //                });
    //            }
    //        }
    //    }
    //}

    //public GameObject GetSkillEffectObject(string effectName, CharacterType charType = CharacterType.Owner, Transform parent = null)
    //{
    //    GameObject effect = null;

    //    if (skillEffectSourceList.ContainsKey(effectName))
    //    {
    //        effect = GameObject.Instantiate(skillEffectSourceList[effectName]);
    //        if (charType == CharacterType.Enemy)
    //        {
    //            Vector3 scale = effect.GetComponent<RectTransform>().localScale;
    //            scale.x *= -1;
    //            effect.GetComponent<RectTransform>().localScale = scale;
    //        }

    //        effect.transform.SetParent(parent);
    //        effect.GetComponent<RectTransform>().localPosition = Vector3.zero;
    //    }

    //    return effect;
    //}

    //public void ResetCharacter()
    //{
    //    for (int i = 0; i < playerList.Count; i++)
    //    {
    //        playerList[i].CharacterAction.ChangeActionFSMState(ACTION_FSM.None);
    //    }
    //    for (int i = 0; i < enemyList.Count; i++)
    //    {
    //        enemyList[i].CharacterAction.ChangeActionFSMState(ACTION_FSM.None);
    //    }

    //    BattleManager.Instance.ChangeBattleState(Battle_FSM.None);
    //}
}
