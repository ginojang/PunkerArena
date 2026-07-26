using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AddressableAssets;

public class GameDataBase : MonoBehaviour
{
    [SerializeField]
    public float slowTime;

    [System.Serializable]
    public class CharacterData
    {
        public CharacterClass characterClass;
        public CampType campType;
        public CharacterType characterType;
        public CharacterTalent characterTalent;

        public float hp;
        public float maxhp;
        public float basicAttack;
        public int characterSP;
        
        [Header("체크 XXXX")]
        public bool death = false;
        [Header("조절 필요 (소수점 가능)")]
        public float attackSpeed;
        [Header("게임 중 변경이 어렵습니다.")]
        public CharacterArrangement arrangePosition;
        [Header("게임 중 변경이 어렵습니다.")]
        public int slotPosition;
        [Header("캐릭터 스킬 인덱스 값")]
        public int[] skillIndex = new int[4];
    }
    [System.Serializable]
    public class UISystem
    {
        [Header("0~1 사이로 세팅")]
        public float costSpeed;
        [Header("0~10 사이로 세팅 ")]
        public int startCost;
        [Header("0 으로 세팅")]
        public int currentCost;
        [Header("0~10 스킬 선택 시간 세팅")]
        public float selectSkillTime;
    }
    [System.Serializable]
    public class BattleSystem
    {
        [Header("플레이어 무적")]
        public bool playerCharacterInvincibility = false;
        [Header("적군 무적")]
        public bool enemyCharacterInvincibility = false;
        [Header("공격 속도 업 Value (랜덤 값)")]
        public float minAttackSpeedUp;
        public float maxAttackSpeedUp;
    }

    static public GameDataBase Instance = null;

    [SerializeField]public List<CharacterData> playerCharacterStatus = new List<CharacterData>();
    [SerializeField]public List<CharacterData> enemyCharacterStatus = new List<CharacterData>();
    [SerializeField] public UISystem playerUISystem = new UISystem();
    [SerializeField] public UISystem enemyUISystem = new UISystem();
    [SerializeField] public BattleSystem battleSystem = new BattleSystem();


    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        gameObject.name = "GameDataBase";

        ImmortalGameObject.AttachObject(gameObject);
    }
}
