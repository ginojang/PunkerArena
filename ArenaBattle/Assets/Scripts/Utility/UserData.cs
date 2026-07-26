using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum modeType
{
	Action = 0,
	Story,
	Max,
}

public class UserData
{
	public long UID = 0;

	public string SID = "";
	
	// 서버로부터 내려받은 캐릭터들의 정보 적제
	// 캐릭터별 정보
	public class CharacterServerData
	{
		public string dinoID;
		public long GenSeq;
		public bool wingSlot;
		public int dinoPosition;
		public CharacterClass charClass;
		public int grade;
		public CharacterTalent talent;
		public costumCharInfo costumeInfo;
		public Dictionary<ItemType, string> partslist = new Dictionary<ItemType, string>();
		public int body_set_option_id;
		public int head_set_option_id;
		public int eyes_set_option_id;
		public int mouth_set_option_id;
		public int back_set_option_id;
		public int tail_set_option_id;
		public int wing_set_option_id;
		public int mating_count;
		public int pure_part_count;
		public int limited_part_type;
		public int limited_part_count;
		public int regist_no;
		public string characterName = "";
		public int level;
		public int exp;
		public int bonus_stat_level;
		public int bonus_str;
		public int bonus_vit;
		public int bonus_agi;
		public int bonus_dex;
		public int bonus_luk;
		public int body_skill_level;
		public int head_skill_level;
		public int eyes_skill_level;
		public int mouth_skill_level;
		public int back_skill_level;
		public int tail_skill_level;
		public int wing_skill_level; 
	}

	public class slotBuff
	{
		public int buffPosition;
		public int buffType;
		public int buffValue;
	}
	
	// 서버로부터 내려받은 캐릭터 정보 및 버프 정보 적재
	// 하나의 덱 구성
	public class CharacterSquadData
	{
		// 일반게임 디노의 리스트
		public Dictionary<string, CharacterServerData> serverSquadList = new Dictionary<string, CharacterServerData>();

		public List<slotBuff> buffList = new List<slotBuff>();
		
		
	}

	//public Dictionary<modeType, Dictionary<int, CharacterSquadData>> serverSquadList =
	//	new Dictionary<modeType, Dictionary<int, CharacterSquadData>>();

	// 덱 구성 디노의 정보
	public Dictionary<modeType, Dictionary<int, CharacterSquadData>> serverSquadDic =
		new Dictionary<modeType, Dictionary<int, CharacterSquadData>>();

	// 내가 가진 모든 디노의 정보
	public Dictionary<string, CharacterServerData> dinoServerList = new Dictionary<string, CharacterServerData>();
}