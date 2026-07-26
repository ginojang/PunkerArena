using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devil.Gui;
using Lobby;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UiTitle : UiBase<UiTitle>
{
    [SerializeField]
    private Button start_Btn = null;
    
    protected override void Start()
    {
        base.Start();
        start_Btn.onClick.AddListener(OnClickStartBtn);
    }

    private void OnClickStartBtn()
    {
        start_Btn.interactable = false;

        ClientNetworkContents.SendUsereData(GameDataManager.Instance.userData.UID, GameDataManager.Instance.userData.SID, (response) =>
        {
            if (response == null)
            {
                // Popup으로 실패 했음을 알려주고 확인시 타이틀로 다시....
                //GuiUtility.ShowNoticeOkPopUp("음성인식을 3회 실패해서 강제로 넘어갑니다.", OnClickGoBetia);

                start_Btn.interactable = true;
                PlayState.Instance.ChangePlayState(PlayState.STATES.Title);
            }
            else
            {
                SetUserData(response);
                EntryGame();
            }
        });
    }
    
    private void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            
        }
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    private void SetUserData(GetUserDataResponse response)
    {
        // 본게임
        //int squadCount = response.Account.Squad.ListSquadMode[0].ListSquadDetail.Count;
        // fruttidino_story
        //int storyCount = response.Account.Squad.ListSquadMode[1].ListSquadDetail.Count;

        // 파츠 정보먼저 읽어서 리스트화 해놓고 캐릭터 정보 세팅할때 찾아쓴다.
        int detailCount = response.Account.Squad.ListSquadMode.Count;

        for (int ii = 0; ii < detailCount; ii++)
        {
            if (!GameDataManager.Instance.userData.serverSquadDic.ContainsKey(
                    (modeType) response.Account.Squad.ListSquadMode[ii].ModeType))
            {
                Dictionary<int, UserData.CharacterSquadData> squadList =
                    new Dictionary<int, UserData.CharacterSquadData>();
                GameDataManager.Instance.userData.serverSquadDic.Add((modeType)ii, squadList);
            }
            
            int squadTotal = response.Account.Squad.ListSquadMode[ii].ListSquadDetail.Count;
            for (int i = 0; i < squadTotal; i++)
            {
                int dinoCount = response.Account.Squad.ListSquadMode[ii].ListSquadDetail[i].ListDino.Count;
                int buffCount = response.Account.Squad.ListSquadMode[ii].ListSquadDetail[i].ListBuff.Count;

                UserData.CharacterSquadData squaddata = new UserData.CharacterSquadData();

                Dictionary<int, UserData.CharacterServerData>
                    datalist = new Dictionary<int, UserData.CharacterServerData>();

                for (int j = 0; j < dinoCount; j++)
                {
                    UserData.CharacterServerData data = new UserData.CharacterServerData();
                    data.dinoID = response.Account.Squad.ListSquadMode[0].ListSquadDetail[i].ListDino[j].DinoId;
                    data.dinoPosition = response.Account.Squad.ListSquadMode[0].ListSquadDetail[i].ListDino[j].DinoPosition;
                    squaddata.serverSquadList.Add(data.dinoID, data);
                }
            
                for (int j = 0; j < buffCount; j++)
                {
                    UserData.slotBuff buff = new UserData.slotBuff();
                    buff.buffType = response.Account.Squad.ListSquadMode[0].ListSquadDetail[i].ListBuff[j].BuffType;
                    buff.buffValue = response.Account.Squad.ListSquadMode[0].ListSquadDetail[i].ListBuff[j].BuffValue;
                    squaddata.buffList.Add(buff);
                }
            
                GameDataManager.Instance.userData.serverSquadDic[(modeType)ii].Add(i, squaddata);
            }
        }

        detailCount = response.Dino.ListDinoDetail.Count;

        for (int i = 0; i < detailCount; i++)
        {
            UserData.CharacterServerData dino = new UserData.CharacterServerData();

            dino.dinoID = response.Dino.ListDinoDetail[i].GenId;
            dino.GenSeq = response.Dino.ListDinoDetail[i].GenSeq;
            dino.charClass = (CharacterClass) response.Dino.ListDinoDetail[i].DinoType;
            dino.grade = response.Dino.ListDinoDetail[i].Grade;
            dino.talent = (CharacterTalent) response.Dino.ListDinoDetail[i].Talent;
            if (response.Dino.ListDinoDetail[i].WingSlot == 1)
                dino.wingSlot = true;
            else
                dino.wingSlot = false;

            dino.partslist.Add(ItemType.body,
                Utility.GetFileNameByTalent(dino.talent, response.Dino.ListDinoDetail[i].Body));
            dino.partslist.Add(ItemType.headparts,
                Utility.GetFileNameByTalent(dino.talent, response.Dino.ListDinoDetail[i].Head));
            dino.partslist.Add(ItemType.eyes,
                Utility.GetFileNameByTalent(dino.talent, response.Dino.ListDinoDetail[i].Eyes));
            dino.partslist.Add(ItemType.mouth,
                Utility.GetFileNameByTalent(dino.talent, response.Dino.ListDinoDetail[i].Mouth));
            dino.partslist.Add(ItemType.back,
                Utility.GetFileNameByTalent(dino.talent, response.Dino.ListDinoDetail[i].Back));
            dino.partslist.Add(ItemType.tail,
                Utility.GetFileNameByTalent(dino.talent, response.Dino.ListDinoDetail[i].Tail));
            dino.partslist.Add(ItemType.wing,
                Utility.GetFileNameByTalent(dino.talent, response.Dino.ListDinoDetail[i].Wing));

            dino.body_set_option_id = response.Dino.ListDinoDetail[i].BodySetOptionId;
            dino.head_set_option_id = response.Dino.ListDinoDetail[i].HeadSetOptionId;
            dino.eyes_set_option_id = response.Dino.ListDinoDetail[i].EyesSetOptionId;
            dino.mouth_set_option_id = response.Dino.ListDinoDetail[i].MouthSetOptionId;
            dino.back_set_option_id = response.Dino.ListDinoDetail[i].BackSetOptionId;
            dino.tail_set_option_id = response.Dino.ListDinoDetail[i].TailSetOptionId;
            dino.wing_set_option_id = response.Dino.ListDinoDetail[i].WingSetOptionId;

            dino.mating_count = response.Dino.ListDinoDetail[i].MatingCount;
            dino.pure_part_count = response.Dino.ListDinoDetail[i].PurePartCount;
            dino.limited_part_type = response.Dino.ListDinoDetail[i].LimitedPartType;
            dino.limited_part_count = response.Dino.ListDinoDetail[i].LimitedPartCount;
            dino.regist_no = response.Dino.ListDinoDetail[i].RegistNo;

            dino.characterName = response.Dino.ListDinoDetail[i].DinoName;
            dino.exp = response.Dino.ListDinoDetail[i].Exp;
            dino.bonus_stat_level = response.Dino.ListDinoDetail[i].BonusStatLevel;
            dino.bonus_str = response.Dino.ListDinoDetail[i].BonusStr;
            dino.bonus_vit = response.Dino.ListDinoDetail[i].BonusVit;
            dino.bonus_agi = response.Dino.ListDinoDetail[i].BonusAgi;
            dino.bonus_dex = response.Dino.ListDinoDetail[i].BonusDex;
            dino.bonus_luk = response.Dino.ListDinoDetail[i].BonusLuk;
            dino.body_skill_level = response.Dino.ListDinoDetail[i].BodySkillLevel;
            dino.head_skill_level = response.Dino.ListDinoDetail[i].HeadSkillLevel;
            dino.eyes_skill_level = response.Dino.ListDinoDetail[i].EyesSkillLevel;
            dino.mouth_skill_level = response.Dino.ListDinoDetail[i].MouthSkillLevel;
            dino.back_skill_level = response.Dino.ListDinoDetail[i].BackSkillLevel;
            dino.tail_skill_level = response.Dino.ListDinoDetail[i].TailSkillLevel;
            dino.wing_skill_level = response.Dino.ListDinoDetail[i].WingSkillLevel;
            
            GameDataManager.Instance.userData.dinoServerList.Add(dino.dinoID, dino);
        }

        for (int ii = 0; ii < (int)modeType.Max; ii++)
        {
            for (int i = 0; i < detailCount; i++)
            {
                int squadCount = response.Account.Squad.ListSquadMode[ii].ListSquadDetail.Count;
                
                for (int j = 0; j < squadCount; j++)
                {
                    int count = GameDataManager.Instance.userData.serverSquadDic[(modeType)ii].Count;

                    for (int k = 0; k < count; k++)
                    {
                        Dictionary<int,UserData.CharacterSquadData> squadlist =  GameDataManager.Instance.userData.serverSquadDic[(modeType) ii];
                        if (squadlist[j].serverSquadList.ContainsKey(response.Dino.ListDinoDetail[i].GenId))
                        {
                            UserData.CharacterServerData dino = squadlist[j]
                                .serverSquadList[response.Dino.ListDinoDetail[i].GenId];

                            dino.dinoID = response.Dino.ListDinoDetail[i].GenId;
                            dino.GenSeq = response.Dino.ListDinoDetail[i].GenSeq;
                            dino.charClass = (CharacterClass) response.Dino.ListDinoDetail[i].DinoType;
                            dino.grade = response.Dino.ListDinoDetail[i].Grade;
                            dino.talent = (CharacterTalent) response.Dino.ListDinoDetail[i].Talent;
                            if (response.Dino.ListDinoDetail[i].WingSlot == 1)
                                dino.wingSlot = true;
                            else
                                dino.wingSlot = false;
                            
                            
                            dino.partslist.Add(ItemType.body,
                                Utility.GetFileNameByTalent(dino.talent, response.Dino.ListDinoDetail[i].Body));
                            dino.partslist.Add(ItemType.headparts,
                                Utility.GetFileNameByTalent(dino.talent, response.Dino.ListDinoDetail[i].Head));
                            dino.partslist.Add(ItemType.eyes,
                                Utility.GetFileNameByTalent(dino.talent, response.Dino.ListDinoDetail[i].Eyes));
                            dino.partslist.Add(ItemType.mouth,
                                Utility.GetFileNameByTalent(dino.talent, response.Dino.ListDinoDetail[i].Mouth));
                            dino.partslist.Add(ItemType.back,
                                Utility.GetFileNameByTalent(dino.talent, response.Dino.ListDinoDetail[i].Back));
                            dino.partslist.Add(ItemType.tail,
                                Utility.GetFileNameByTalent(dino.talent, response.Dino.ListDinoDetail[i].Tail));
                            dino.partslist.Add(ItemType.wing,
                                Utility.GetFileNameByTalent(dino.talent, response.Dino.ListDinoDetail[i].Wing));

                            dino.body_set_option_id = response.Dino.ListDinoDetail[i].BodySetOptionId;
                            dino.head_set_option_id = response.Dino.ListDinoDetail[i].HeadSetOptionId;
                            dino.eyes_set_option_id = response.Dino.ListDinoDetail[i].EyesSetOptionId;
                            dino.mouth_set_option_id = response.Dino.ListDinoDetail[i].MouthSetOptionId;
                            dino.back_set_option_id = response.Dino.ListDinoDetail[i].BackSetOptionId;
                            dino.tail_set_option_id = response.Dino.ListDinoDetail[i].TailSetOptionId;
                            dino.wing_set_option_id = response.Dino.ListDinoDetail[i].WingSetOptionId;

                            dino.mating_count = response.Dino.ListDinoDetail[i].MatingCount;
                            dino.pure_part_count = response.Dino.ListDinoDetail[i].PurePartCount;
                            dino.limited_part_type = response.Dino.ListDinoDetail[i].LimitedPartType;
                            dino.limited_part_count = response.Dino.ListDinoDetail[i].LimitedPartCount;
                            dino.regist_no = response.Dino.ListDinoDetail[i].RegistNo;

                            dino.characterName = response.Dino.ListDinoDetail[i].DinoName;
                            dino.exp = response.Dino.ListDinoDetail[i].Exp;
                            dino.bonus_stat_level = response.Dino.ListDinoDetail[i].BonusStatLevel;
                            dino.bonus_str = response.Dino.ListDinoDetail[i].BonusStr;
                            dino.bonus_vit = response.Dino.ListDinoDetail[i].BonusVit;
                            dino.bonus_agi = response.Dino.ListDinoDetail[i].BonusAgi;
                            dino.bonus_dex = response.Dino.ListDinoDetail[i].BonusDex;
                            dino.bonus_luk = response.Dino.ListDinoDetail[i].BonusLuk;
                            dino.body_skill_level = response.Dino.ListDinoDetail[i].BodySkillLevel;
                            dino.head_skill_level = response.Dino.ListDinoDetail[i].HeadSkillLevel;
                            dino.eyes_skill_level = response.Dino.ListDinoDetail[i].EyesSkillLevel;
                            dino.mouth_skill_level = response.Dino.ListDinoDetail[i].MouthSkillLevel;
                            dino.back_skill_level = response.Dino.ListDinoDetail[i].BackSkillLevel;
                            dino.tail_skill_level = response.Dino.ListDinoDetail[i].TailSkillLevel;
                            dino.wing_skill_level = response.Dino.ListDinoDetail[i].WingSkillLevel;

                            break;
                        }
                    }
                }
            }
        }
    }
    
    private void EntryGame()
    {
        Main main = Main.MainObject.GetComponent<Main>();

        if (AssetManager.Instance.TotalDownloadBundleSize > 0)
            PlayState.Instance.ChangePlayState(PlayState.STATES.Patch);
        else
            PlayState.Instance.ChangePlayState(PlayState.STATES.Menu);
    }

    
}
