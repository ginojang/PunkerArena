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

    // [OFFLINE] 서버 가챠 RPC/파싱 삭제 — 서버/네트웍 재작성 예정.
    public static void SendDevDinoGacha(int count, System.Action<string> callbackRecv = null)
    {
        callbackRecv?.Invoke(null);
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
