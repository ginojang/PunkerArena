using System.Collections;
using System.Collections.Generic;
using Devil.Common;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

using Generated.CsvData;
public class DinoSkillTool : MonoBehaviour
{
    [SerializeField]
    private static GameObject dino = null;
    [SerializeField]
    private static GameObject startPoint = null;

    private static GameObject monster = null;

    private static GameObject monsterPoint = null;

    private Dictionary<string, GameObject> dinoBasePool = new Dictionary<string, GameObject>();

    private CharacterTalent curTalent = CharacterTalent.Carnivore;
    private CharacterClass curClass = CharacterClass.orange;
    private Dictionary<ItemType, selectedPartInfo> initData = new Dictionary<ItemType, selectedPartInfo>();
    private Dictionary<ItemType, string> selectedNames = new Dictionary<ItemType, string>();

    public eventData curSkillEventList;
    public eventData curMonsterSkillEventList;
    public bool bInit = false;
    public bool bInitAsset = false;

    public bool skillPlaying = false;
    public bool realPlay = false;
    public float PlayElapseTime = 0;
    public float aniLength = 0;
    public int curSkillIndex = 500;
    public int curMonsterSkillIndex = 401;
    public bool rangeAttack = false;
    private int hitEffect = 0;
    private GameObject projectile = null;
    private bool bPlayDino = true;

    public GameObject DINO
	{
		get { return dino; }
	}

    public GameObject Target
    {
        get { return monster; }
    }
    
    void Awake()
	{
        AssetManager.Instance.Initialize(OnAssetManagerIntializeComplete);
    }

    void Start()
    {
        Messenger.AddListener<int, GameObject>(Definition.Dino_Costume_End, OnCompleteDinoCostume);
        
        GameDataManager.Instance.Initialize();
        //EffectManager.Instance.Initialize();
        
        bInit = true;
    }

    public void Initialize(string initMonsterName)
	{
        LoadStartMonster(initMonsterName);
        LoadStartCharacter((CharacterTalent)1, (CharacterClass)1);
    }
    
	protected virtual void OnAssetManagerIntializeComplete()
    {
        UiAddressablePoolManager.Instance.Initialize();

        bInitAsset = true;
    }

    private void OnCompleteDinoCostume(int index, GameObject newDino)
    {
        dino = newDino;
        InitDinoInfo();
//        if(monster != null)
//            character.targetObj = monster.GetComponent<CharacterBase>();
    }

    private void InitDinoInfo()
    {
        CharacterBase character = dino.GetComponent<CharacterBase>();
        character.CharacterInfo.costumeInfo = new costumCharInfo(curTalent, curClass, dino.transform);
//        character.CharacterInfo.CharacterTalent = curTalent;
        character.InitializeCharacterAnimator();
        
        character.Get_AnimBase.CharType = CampType.Owner;
    }
    void Update()
    {
		if (skillPlaying && realPlay)
		{
			PlayElapseTime += Time.deltaTime;

			if (aniLength == 0)
			{
				PlayElapseTime = 0;
			}
			else
			{
				if (PlayElapseTime >= aniLength)
				{
					PlayElapseTime = aniLength;
					skillPlaying = false;
                    realPlay = false;
                }
			}

			//            Debug.Log("dinoTool PlayElapseTime  "+ PlayElapseTime);
		}

        if(rangeAttack)
		{
            UpdateRangeAttack();
		}
    }

    public void DeleteObject()
	{
        Destroy(dino);
        dino = null;
    }

    public void DeleteMonster()
    {
        Destroy(monster);
        monster = null;
    }

	private void OnDestroy()
	{
        Messenger.RemoveListener<int, GameObject>(Definition.Dino_Costume_End, OnCompleteDinoCostume);
        
        if (Application.isPlaying)
        {
            Destroy(dino);
            Destroy(GameObject.Find("AssetManager"));
            Destroy(GameObject.Find("__immortal__"));
        }
        else
        {
            DestroyImmediate(dino);
            DestroyImmediate(GameObject.Find("AssetManager"));
            DestroyImmediate(GameObject.Find("__immortal__"));
        }
    }

	public void LoadStartCharacter(CharacterTalent talent, CharacterClass charclass)
	{
        DeleteObject();

        Dictionary<ItemType, string> list = new Dictionary<ItemType, string>();

        string filename = GetCharacterFilename(talent, charclass);
        if (startPoint == null)
            startPoint = GameObject.Find("StartPoint");

        curTalent = talent;
        curClass = charclass;
        
        CostumeManager.Instance.LoadBaseCharacter(filename, curTalent, curClass, null, startPoint.transform, true);
    }
    
    public void LoadMonster(string prefabName)
    {
        DeleteMonster();

        string filename = $"Assets/Asset/Cha/monster/prefab/{prefabName}.prefab";
        if (monsterPoint == null)
            monsterPoint = GameObject.Find("MonsterPoint");

        AssetManager.Instance.LoadAssetAsync<GameObject>(filename, (AssetLoader ld, object p) =>
        {
            GameObject obj = ld.MainAsset as GameObject;
            monster = Instantiate(obj);
            
            monster.transform.SetParent(monsterPoint.transform);
            monster.transform.localPosition = Vector3.zero;
            monster.transform.localRotation = Quaternion.identity;
            CharacterBase character = monster.GetComponent<CharacterBase>();
            character.InitializeCharacterAnimator();
        
            character.Get_AnimBase.CharType = CampType.Enemy;
        });
    }

    public void LoadStartMonster(string filename)
    {
        LoadMonster(filename);
    }

    private void FinishCharacterLoad(object p)
    {
        List<object> charLoader = (List<object>)p;

        for (int i = 0; i < charLoader.Count; i++)
        {
            AssetLoader charobj = (AssetLoader)charLoader[i];
            GameObject charObj = charobj.MainAsset as GameObject;
        }
    }

    private string GetCharacterFilename(CharacterTalent talent, CharacterClass charclass)
    {
        string filename = "";
        string classname = charclass.ToString();
        string talentname = talent.ToString();
        if (talent == CharacterTalent.Carnivore)
        {
            talentname = "ca";
        }
        else if (talent == CharacterTalent.Omnivore)
        {
            talentname = "om";
        }
        else if (talent == CharacterTalent.Herbivore)
        {
            talentname = "he";
        }
        
        return filename = $"{Definition.baseFilePath}{classname}/prefab/{talentname}_{classname}_01.prefab";
    }

    public void StartAnimation(int skillIndex, eventData info, bool bDino = true)
    {
        hitEffect = 0;
        
        bPlayDino = bDino;
        if (bDino)
        {
            curSkillEventList = info;
            AnimationManager.Instance.Set_AniBool(dino.GetComponent<CharacterBase>(),
                $"{AnimBase.AnimationType.bAttack}", true);
            //dino.GetComponent<Character>().Get_AnimBase.Set_AttackAnimation(dino.GetComponent<Character>(), skillIndex);
        }
        else
        {
            curMonsterSkillEventList = info;
            AnimationManager.Instance.Set_AniBool(monster.GetComponent<CharacterBase>(),
                $"{AnimBase.AnimationType.bAttack}", true);
            //monster.GetComponent<Monster>().Get_AnimBase.Set_AttackAnimation(monster.GetComponent<Monster>(), skillIndex);
        }
    }

    public void StopAnimation()
    {
        skillPlaying = false;
        realPlay = false;
        PlayElapseTime = 0f;
        this.StopAllCoroutines();
        dino.GetComponent<CharacterBase>().Get_AnimBase.m_EventList.Clear();
        AnimationManager.Instance.Set_AniInteger(dino.GetComponent<CharacterBase>(), $"{AnimBase.AnimationType.iAttack}", 0);
        AnimationManager.Instance.Set_AniBool(dino.GetComponent<CharacterBase>(), $"{AnimBase.AnimationType.bAttack}", false);
        monster.GetComponent<CharacterBase>().Get_AnimBase.m_EventList.Clear();
        AnimationManager.Instance.Set_AniInteger(monster.GetComponent<CharacterBase>(), $"{AnimBase.AnimationType.iAttack}", 0);
        AnimationManager.Instance.Set_AniBool(monster.GetComponent<CharacterBase>(), $"{AnimBase.AnimationType.bAttack}", false);
    }

    public void EndAction(CharacterBase actor)
    {
        actor.Get_AnimBase.m_EventList.Clear();
        AnimationManager.Instance.Set_AniInteger(actor, $"{AnimBase.AnimationType.iAttack}", 0);
        AnimationManager.Instance.Set_AniBool(actor, $"{AnimBase.AnimationType.bAttack}", false);
    }
    
    #region EVENTFUNC

    IEnumerator OnDamage(object[] param)
    {
        yield return new WaitForSeconds((float)param[0]);
        CharacterBase actor = (CharacterBase)param[1];

        OnDamage();
    }
    IEnumerator RangeAttack(object[] param)
    {
        yield return new WaitForSeconds((float)param[0]);
        CharacterBase actor = (CharacterBase)param[1];

        StartCoroutine(OnRangeAttack());
        //actor.RangeAttack();
    }
    IEnumerator ChangeState(object[] param)
    {
        yield return new WaitForSeconds((float)param[0]);
        CharacterBase actor = (CharacterBase)param[1];

        AnimationManager.Instance.Set_AniInteger(actor, $"{AnimBase.AnimationType.iAttack}", 0);
        AnimationManager.Instance.Set_AniBool(actor, $"{AnimBase.AnimationType.bAttack}", false);
        
        //skillPlaying = false;
        //realPlay = false;

        StartCoroutine(ActionEnd(0.3f, actor));
    }

    IEnumerator AnimEventClearAttack(object[] param)
    {
        yield return new WaitForSeconds((float)param[0]);

        CharacterBase actor = (CharacterBase)param[1];
    }
    IEnumerator AttachEffect(object[] param)
    {
        yield return new WaitForSeconds((float)param[0]);
        CharacterBase actor = (CharacterBase)param[1];

        OnAttachEffect((int)param[2], (EffectTarget)param[3]);
    }
    IEnumerator OnHeal(object[] param)
    {
        yield return new WaitForSeconds((float)param[0]);

        OnHeal();
    }
    IEnumerator AttachTargetEffect(object[] param)
    {
        yield return new WaitForSeconds((float)param[0]);
        //List<CharacterBase> actor = BattleManager.Instance.TargetList;

        //for(int i = 0; i < actor.Count; i++)
        {
            OnAttachEffect((int)param[2], (EffectTarget)param[3]);
        }
    }
    IEnumerator ChangeStateHit(object[] param)
    {
        yield return new WaitForSeconds((float)param[0]);
        CharacterBase actor = (CharacterBase)param[1];

//        AnimationManager.Instance.Set_AniInteger(actor, AnimBase.AnimationType.tHit, 0);
        AnimationManager.Instance.Set_AniBool(actor, $"{AnimBase.AnimationType.bHit}", false);

        StartCoroutine(ActionEnd(1.0f,actor));
    }

    IEnumerator ActionEnd(float ftime, CharacterBase actor)
	{
        yield return new WaitForSeconds(ftime);

        foreach(var data in actor.GetComponent<CharacterBase>().effectListTool)
            //EffectManager.Instance.RemoveEffect( data.Key, data.Value);

        actor.GetComponent<CharacterBase>().effectListTool.Clear();

        actor.Get_AnimBase.m_EventList.Clear();
        
//        skillPlaying = false;
//        realPlay = false;
    }

    #endregion

    #region Event Action Func

    public void OnDamage()
    {
        CharacterBase target = null;
        if (bPlayDino)
            target = monster.GetComponent<CharacterBase>();
        else
            target = dino.GetComponent<CharacterBase>();
        
        target.Get_AnimBase.Set_Hit(target);
        if (hitEffect == 0)
            hitEffect = 1;
        OnAttachEffect(hitEffect, EffectTarget.Target);
        
        StartCoroutine(ActionEnd(1.0f, target));
    }
    
    public void OnDamage(CharacterBase character)
    {
        character.Get_AnimBase.Set_Hit(character);
    }
    
    public void OnHeal()
    {
    }
    public void HealDamageOn(CharacterBase target, int value)
    {
        
    }
    public void OnAttachEffect(int arrayIndex, EffectTarget target)
    {
        int skillindex = 0;
        Transform parent = null;
        if (bPlayDino)
        {
            skillindex = curSkillIndex;
        }
        else
        {
            skillindex = curMonsterSkillIndex;
        }

        SkillData info = CSVDataManager.GetTable<SkillTable>().GetData(skillindex);

        int index = 0;

        if (rangeAttack)
        {
            // index = info.skill_shoot_effect;
            hitEffect = CSVDataManager.GetTable<EffectListTable>().GetData(index).Length;
            
            if (bPlayDino)
                parent = monster.transform;
            else
                parent = dino.transform;
        }
        else
        {
            switch (target)
            {
                case EffectTarget.Caster:
                {
                    // index = info.skill_caster_effect;
                    if (bPlayDino)
                        parent = dino.transform;
                    else
                        parent = monster.transform;
                }
                    break;
                case EffectTarget.Target:
                {
                    // index = info.skill_target_effect;

                    if (bPlayDino)
                        parent = monster.transform;
                    else
                        parent = dino.transform;
                }
                    break;
                case EffectTarget.Shoot:
                {
                    // index = info.skill_shoot_effect;
                }
                    break;
            }
        }

        if (index > 0)
        {
            /*AttachPoint point = ResourcePoolManager.Instance.GetSkillAttachPoint(index, arrayIndex);
            if (point == AttachPoint.Slot)
            {
                if (bPlayDino)
                    parent = monster.transform;
                else
                    parent = dino.transform;
            }

            ResourcePoolManager.Instance.GetGameData(index, arrayIndex, parent);*/
        }
    }

    IEnumerator OnRangeAttack()
    {
        int skillindex = 0;
        if (bPlayDino)
            skillindex = curSkillIndex;
        else
            skillindex = curMonsterSkillIndex;
        
        SkillData info = CSVDataManager.GetTable<SkillTable>().GetData(skillindex);

        // int index = info.skill_shoot_effect;
        int index = 0;
        hitEffect = CSVDataManager.GetTable<EffectListTable>().GetData(index).Length;
        int[] effectlist = CSVDataManager.GetTable<EffectListTable>().GetData(index);
        //Generated.CsvData.effectData effectData = CSVDataManager.GetTable<EffectTable>().GetData(effectlist[0]);

        projectile = null;

        Transform obj;

        if (bPlayDino)
            obj = dino.transform;
        else
            obj = monster.transform;
/*        
        StartCoroutine(ResourcePoolManager.Instance.AsyncGetGameData("", objectType.character, obj, (effect) =>
        {
            projectIndex = effectlist[0];
            projectile = (GameObject)(effect);
            rangeAttack = true;
        }));
*/        
        //GameObject obj = StartCoroutine(ResourcePoolManager.Instance.AsyncGetGameData(effectlist[0], dino.transform));
        
        yield return new WaitUntil(() => projectile != null);
    }

    private void UpdateRangeAttack()
    {
        Vector3 forwardPosition;
        Transform lookobj = null;
        Transform obj = null;

        if (bPlayDino)
        {
            forwardPosition = monster.transform.position;
            lookobj = monster.transform;
            obj = dino.transform;
        }
        else
        {
            forwardPosition = dino.transform.position;
            lookobj = dino.transform;
            obj = monster.transform;
        }

        projectile.transform.LookAt(lookobj);
        projectile.transform.Translate(Vector3.forward * 10 * Time.deltaTime);

        var distance = Vector3.Distance(projectile.transform.position, forwardPosition);
        if (distance <= 0.2f)
        {
            //EffectManager.Instance.RemoveEffect(projectIndex, projectile);

            OnDamage();

            EndAction(obj.GetComponent<CharacterBase>());
            
            rangeAttack = false;
        }
    }

    #endregion
}
