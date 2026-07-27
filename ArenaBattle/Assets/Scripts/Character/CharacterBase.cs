using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using UnityEngine.EventSystems;
using Generated.CsvData;
using System.Reflection;


public class CharacterBaseInfo
{
    private Camp battleCamp;

    private StatusBase status = null;
    public costumCharInfo costumeInfo = null;
    public Dictionary<ItemType, string> partslist = new Dictionary<ItemType, string>();
    public List<Generated.CsvData.SkillData> characterSkillList = new List<SkillData>();
    public string dinoID = "";

    public void SetCharacterStatus(Camp camp ,int statusIndex)
    {
        battleCamp = camp;

        switch (battleCamp)
        {
            case Camp.Ally:
                status = new CharacterStatus();
                //var basicAttackData = CSVDataManager.GetTable<SkillTable>().GetData(Definition.Dino_BasicAttack);
                //characterSkillList.Add(basicAttackData);
                SetSkill(-1);
                break;
            case Camp.Enemy:
                status = new MonsterStatus();
                SetSkill(statusIndex);
                break;
        }

        status.SetStatus(statusIndex);
    }

    private void SetSkill(int monsterIndex)
    {
        if( costumeInfo == null )
            CSVDataManager.GetTable<SkillTable>().GetDataIndexList(ref characterSkillList, monsterIndex, CharacterTalent.None, partslist);
        else
            CSVDataManager.GetTable<SkillTable>().GetDataIndexList(ref characterSkillList, monsterIndex, Character_Talent, partslist);
    }

    public StatusBase CharacterStatus
    {
        get { return status; }
    }
    public CharacterTalent Character_Talent
    {
        get { return costumeInfo.CURTALENT; }
        set { costumeInfo.CURTALENT = value; }
    }

    public Camp BattleCamp
    {
        get { return battleCamp; }
    }

    public bool IsWing
    {
        get 
        {
            partslist.TryGetValue(ItemType.wing, out string wingValue);
            if (wingValue == null || wingValue == "" || wingValue == "none")
                return false;
            else
                return true;
        }
    }
}

public class CharacterProfile
{
    public Camp camp;
    public Texture2D snap;
    public Sprite battleTag;
    public Sprite attribute;
    public Grade grade;
}

public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) { }

    public AnimationClip this[string name]
    {
        get { return this.Find(x => x.Key.name.Equals(name)).Value; }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}

public class CharacterBase : MonoBehaviour
{
    protected class DamageInfo
    {
        public string HitEffect { set; get; }
        public GameObject HitParent { set; get; }
        public int HitCount { set;  get; }
        public float Damage { set; get; }
    }

    protected Dictionary<string, GameObject> effectTransDic;

    private StateMachine<CHARACTER_FSM> fsm;
    protected  object sub_Fsm = null;
    #region Enum
    public enum CHARACTER_FSM
    {
        None,
        BattleSetting,
        Ready,
        SetTarget,
        Shoot,
        Stand,
        Action,
        Move,
        End,
        Result,
    }
    #endregion
    public Dictionary<int, GameObject> effectListTool = new Dictionary<int, GameObject>();
    protected AnimBase characterAnimation;
    protected CharacterBaseInfo characterInfo = new CharacterBaseInfo();

    //protected Dictionary<BuffTimeType, List<BuffBase>> buffDic = new Dictionary<BuffTimeType, List<BuffBase>>();
    public Animator characterAnimator;
    [SerializeField]
    protected AnimatorOverrideController overrideAnimator = null;

    [SerializeField] protected GameObject infoPosition = null;
    private CharacterProfile profile = new CharacterProfile();

    protected DamageInfo damageInfo;
    protected bool setDamageInfo = false;

    protected Action rangeAction;

    public CharacterProfile Profile
    {
        get { return profile; }
    }

    #region Parameter
    public CharacterBaseInfo CharacterInfo
    {
        get { return characterInfo; }
		set { characterInfo = value; }
    }
    public AnimBase Get_AnimBase
    {
        get { return characterAnimation; }
        set { characterAnimation = value; }
    }
    public Transform InfoTransform
    {
        get { return infoPosition.transform; }
    }

    public GameObject InfoTransObject
    {
#if UNITY_EDITOR
        get { return infoPosition; }
#endif
        set { infoPosition = value; }
    }

    public AnimatorOverrideController OverrideAnimator
    {
        get { return overrideAnimator; }
    }
    #endregion

    #region Initialize
    protected virtual void InitializeData()
    {
        effectTransDic = new Dictionary<string, GameObject>();
    }
    public virtual void InitializeFSM()
    {
        if (fsm == null)
            fsm = StateMachine<CHARACTER_FSM>.Initialize(this, CHARACTER_FSM.None);
        else
            fsm.ChangeState(CHARACTER_FSM.None);
    }
    #endregion

    #region SetAnimator
    public virtual void InitializeCharacterAnimator()
    {
    }

    public void SetAnimatorOverride(AnimatorOverrideController data, AnimationClipOverrides clipOverrides)
    {
        overrideAnimator = data;
        characterAnimator.runtimeAnimatorController = overrideAnimator;

        overrideAnimator.ApplyOverrides(clipOverrides);
    }
    #endregion

    #region FSM
    protected virtual void ChangeFSMState(CHARACTER_FSM state)
    {
        fsm.ChangeState(state);
    }
    #endregion

    #region Battle
    public virtual void BattleEnter()
    {
        
    }
    public virtual void BattleExit()
    {

    }
    public virtual void Result(CampType type)
    {
 
    }
    #endregion

    #region Skill
    public virtual void SkillAction(SkillData data, int skillListIndex)
    {

    }
    public virtual void AddSkillCool(int skillListIndex)
    {

    }
    #endregion

    #region Set Attach Effect Position
    protected virtual void FindDinoAttachEffect()
    {
        if (effectTransDic == null)
            effectTransDic = new Dictionary<string, GameObject>();

        Transform boneTrans = null;
        string boneFirstName = "Bip001";

        boneTrans = Utility.FindBone(gameObject.transform, $"{boneFirstName} Head");
        GameObject head = boneTrans.Find("fx_head").gameObject;
        effectTransDic.Add(head.name, head);

        boneTrans = Utility.FindBone(gameObject.transform, $"{boneFirstName} Head");
        GameObject mouth = boneTrans.Find("fx_mouth").gameObject;
        effectTransDic.Add(mouth.name, mouth);

        boneTrans = Utility.FindBone(gameObject.transform, $"{boneFirstName} Back");
        GameObject back = boneTrans.Find("fx_back").gameObject;
        effectTransDic.Add(back.name, back);

        boneTrans = Utility.FindBone(gameObject.transform, $"{boneFirstName} Tail2");
        GameObject tail = boneTrans.Find("fx_tail").gameObject;
        effectTransDic.Add(tail.name, tail);

        boneTrans = Utility.FindBone(gameObject.transform, $"{boneFirstName} Spine");
        GameObject body = boneTrans.Find("fx_body").gameObject;
        effectTransDic.Add(body.name, body);

        boneTrans = Utility.FindBone(gameObject.transform, $"{boneFirstName}");
        GameObject hit = boneTrans.Find("fx_hit").gameObject;
        effectTransDic.Add(hit.name, hit);
    }
    protected virtual void FindMonsterAttachEffect()
    {
        if (effectTransDic == null)
            effectTransDic = new Dictionary<string, GameObject>();

        Transform boneTrans = null;
        string boneFirstName = "Bip001";

        boneTrans = Utility.FindBone(gameObject.transform, $"{boneFirstName} Spine");
        GameObject head = boneTrans.Find("fx_head").gameObject;
        effectTransDic.Add(head.name, head);

        boneTrans = Utility.FindBone(gameObject.transform, $"{boneFirstName} Spine");
        GameObject mouth = boneTrans.Find("fx_mouth").gameObject;
        effectTransDic.Add(mouth.name, mouth);

        boneTrans = Utility.FindBone(gameObject.transform, $"{boneFirstName} Spine");
        GameObject back = boneTrans.Find("fx_back").gameObject;
        effectTransDic.Add(back.name, back);

        boneTrans = Utility.FindBone(gameObject.transform, $"{boneFirstName} Spine");
        GameObject tail = boneTrans.Find("fx_tail").gameObject;
        effectTransDic.Add(tail.name, tail);

        boneTrans = Utility.FindBone(gameObject.transform, $"{boneFirstName} Pelvis");
        GameObject body = boneTrans.Find("fx_body").gameObject;
        effectTransDic.Add(body.name, body);

        boneTrans = Utility.FindBone(gameObject.transform, $"{boneFirstName}");
        GameObject hit = boneTrans.Find("fx_hit").gameObject;
        effectTransDic.Add(hit.name, hit);

        boneTrans = Utility.FindBone(gameObject.transform, $"{boneFirstName} Prop1");
        GameObject weapon = boneTrans.Find("fx_weapon").gameObject;
        effectTransDic.Add(weapon.name, weapon);
    }


    #endregion

    #region Animation
    public virtual void DummyAnimEvent()
    {

    }
    public virtual void OnDamage()
    {
        List<CharacterBase> targetList = InGameData.Instance.TargetList;
        int count = InGameData.Instance.CurrentSkillData.multi_hit_count;
        float term = InGameData.Instance.CurrentSkillData.multi_hit_term;
      
        StartCoroutine(StartOnHit(targetList, count, term));

    }
    private IEnumerator StartOnHit(List<CharacterBase> _targetList, int _count, float _term)
    {
        for(int i = 0; i < _targetList.Count; i++)
        {
            CharacterBase target = _targetList[i];
            for(int j = 0; j < _count; j++)
            {
                target.OnHit();
                yield return new WaitForSeconds(_term);
            }
        }

        yield break;
    }

    public virtual void OnDamage(int hitCount, float delay)
    {

    }
    public virtual void RangeAttack()
    {
        rangeAction.Invoke();
    }
    public virtual void MultiRangeAttack()
    {
        
    }
    public virtual void ChangeState()
    {

    }
    public virtual void Death()
    {
        
    }
    public virtual void OnHeal()
    {
       
    }

    public virtual void OnHit()
    {
        string name = damageInfo.HitEffect;
        Transform parent = damageInfo.HitParent.transform;

        EffectManager.Instance.PlayDoOnceEffect(name, parent);
        Get_AnimBase.Set_Hit(this);

        damageInfo.HitCount--;
        if (damageInfo.HitCount <= 0)
            ChangeNextSubState();
    }

    #endregion

    #region Effect
    public GameObject GetEffectParent(string parent, Link_Bone_Move move)
    {
        GameObject attachPoint = null;

        if (move == Link_Bone_Move.Fix)
            return gameObject;


        if (effectTransDic.TryGetValue(parent, out attachPoint) == false)
            attachPoint = gameObject;

        return attachPoint;
    }
    #endregion

    #region Buff
    #endregion


    public void StartSubAction<T>(T value) where T : struct, IConvertible, IComparable
    {
        sub_Fsm = StateMachine<T>.Initialize(this, value);
    }
    public void DeastroyFSM()
    {
        sub_Fsm = null;
    }
    public bool ChangeNextSubState(object[] arg = null)
	{
        bool res = true;
        if (sub_Fsm == null) return false; // [FIX] 죽은 캐릭터(DeastroyFSM)의 OnHit 등에서 sub_Fsm null → NRE 방지

        //MethodInfo info = sub_Fsm.GetType().GetMethod("ChangeNextState", new Type[] { typeof(string[])});

        MethodInfo info = null;
        
        if (arg == null)
        {
            info = sub_Fsm.GetType().GetMethod("ChangeNextState");
        }
        else
        {
            info = sub_Fsm.GetType().GetMethod("ChangeSubNextState", new Type[] { typeof(string[])});
        }
        
        
        if (info != null)
        {
            res = (bool)info.Invoke(sub_Fsm, arg);
        }

        return res;
    }


    protected virtual void OnDestroy()
    {
        Destroy(GetComponent<StateMachineRunner>());
    }

    public CharacterClass GetCharacterClass()
	{
        CharacterClass charclass = CharacterClass.None;

        string bodyname = characterInfo.partslist[ItemType.body];

        if(bodyname.Contains("orange"))
		{
            charclass = CharacterClass.orange;
		}
        else if (bodyname.Contains("watermelon"))
        {
            charclass = CharacterClass.watermelon;
        }
        if (bodyname.Contains("durian"))
        {
            charclass = CharacterClass.durian;
        }
        if (bodyname.Contains("coconut"))
        {
            charclass = CharacterClass.coconut;
        }
        if (bodyname.Contains("buleberry"))
        {
            charclass = CharacterClass.blueberry;
        }
        if (bodyname.Contains("melon"))
        {
            charclass = CharacterClass.melon;
        }
        if (bodyname.Contains("pineapple"))
        {
            charclass = CharacterClass.pineapple;
        }
        if (bodyname.Contains("banana"))
        {
            charclass = CharacterClass.banana;
        }
        if (bodyname.Contains("rambutan"))
        {
            charclass = CharacterClass.rambutan;
        }
        if (bodyname.Contains("dragonfruit"))
        {
            charclass = CharacterClass.dragonfruit;
        }

        return charclass;
    }

    public void ChangeMouthObject(bool bAttack)
	{
        if(bAttack)
		{
            CharacterInfo.costumeInfo.Mouth?.SetActive(false);
            CharacterInfo.costumeInfo.AHMouth?.SetActive(true);
        }
        else
		{
            CharacterInfo.costumeInfo.Mouth?.SetActive(true);
            CharacterInfo.costumeInfo.AHMouth?.SetActive(false);
        }
	}
    protected void DebugLog(string str)
    {
        string log = string.Format($"CharacterName : {gameObject.name} / State : {str}");
        Debug.LogWarning(log);
    }

    #region Messenger
    protected virtual void MessengerAddListner()
    {
        
    }
    protected virtual void MessengerRemoveListner()
    {
    }
    #endregion



}
