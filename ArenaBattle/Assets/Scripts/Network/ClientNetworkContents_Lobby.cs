using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
using Common;
using Lobby;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public partial class ClientNetworkContents
{
    public async static void SendLogin(string ID, Action<SigninResponse> callbackRecv = null)
    {
        if (OfflineSkip(callbackRecv)) return;
        SigninService.SigninServiceClient _client1 =
            new SigninService.SigninServiceClient(NetworkManager.Instance.channel);

        using var call = _client1.SigninAsync(new SigninRequest{DeviceId = ID});
        
        var response = await SendPacket<SigninResponse>(call);

        if(callbackRecv != null)
            callbackRecv(response);
    }
    
    // 서버에 내 키릭터 리스트 요청
    public async static void SendUsereData(long uid, string sid, System.Action<GetUserDataResponse> callbackRecv = null)
    {
        if (OfflineSkip(callbackRecv)) return;
        GetUserDataService.GetUserDataServiceClient _client =
            new GetUserDataService.GetUserDataServiceClient(NetworkManager.Instance.channel);

        using var call = _client.GetUserDataAsync(new GetUserDataRequest{ Uid = uid, Sid = sid });
        
        var response = await SendPacket<GetUserDataResponse>(call);

        if(callbackRecv != null)
            callbackRecv(response);
    }

    // 캐릭터 교체 된 내용 서버에 전송하여 저장
    public async static void SendSetCharacter(UserData.CharacterServerData data, Action<DevSetDinoResponse> callbackRecv = null)
    {
        DevSetDinoService.DevSetDinoServiceClient _client =
            new DevSetDinoService.DevSetDinoServiceClient(NetworkManager.Instance.channel);

        var dino = new DinoInfo();
        //foreach (var item in data.partslist)
        {
            dino.ListDinoDetail.Add(new DinoDetailInfo()
            {
                GenId = data.dinoID,
                DinoType = (int)data.charClass,
                Grade = data.grade,
                Talent = (int)data.talent,
                WingSlot = data.wingSlot ? 1 : 0,
                Body = data.partslist[ItemType.body],
                Head = data.partslist[ItemType.headparts],
                Eyes = data.partslist[ItemType.eyes],
                Mouth = data.partslist[ItemType.mouth],
                Back = data.partslist[ItemType.back],
                Tail = data.partslist[ItemType.tail],
                Wing = data.partslist[ItemType.wing],
            });
        }
        
        using var call = _client.DevSetDinoAsync(new DevSetDinoRequest{Uid = GameDataManager.Instance.userData.UID, Sid = GameDataManager.Instance.userData.SID, Dino = dino});
        
        var response = await SendPacket<DevSetDinoResponse>(call);

        if(callbackRecv != null)
            callbackRecv(response);
    }

    public async static void SendDevDinoGacha(int count, System.Action<string> callbackRecv = null)
    {
        DevDinoGachaService.DevDinoGachaServiceClient _client =
            new DevDinoGachaService.DevDinoGachaServiceClient(NetworkManager.Instance.channel);

        using var call = _client.DevDinoGachaAsync(new DevDinoGachaRequest { Uid = GameDataManager.Instance.userData.UID, Sid = GameDataManager.Instance.userData.SID, Count = count });

        string testid = "";
        
        var response = await SendPacket<DevDinoGachaResponse>(call);

        for (int i = 0; i < response.Dino.ListDinoDetail.Count; i++)
        {
            UserData.CharacterServerData dino = new UserData.CharacterServerData();

            testid = response.Dino.ListDinoDetail[i].GenId;
            
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

        if (callbackRecv != null)
            callbackRecv(testid);
    }
    
    public async static void SendNoticeList(int store, int channel, System.Action<NoticeResponse> callbackRecv = null)
    {
        if (OfflineSkip(callbackRecv)) return;
        NoticeService.NoticeServiceClient _client = new NoticeService.NoticeServiceClient(NetworkManager.Instance.channel);
        
        using var call = _client.GetNoticeListAsync(new NoticeRequest { StoreType = 1, LanguageType = (int)SystemLanguage.Korean }); //

        var response = await SendPacket<NoticeResponse>(call);

        if(callbackRecv != null)
            callbackRecv(response);
    }
}
