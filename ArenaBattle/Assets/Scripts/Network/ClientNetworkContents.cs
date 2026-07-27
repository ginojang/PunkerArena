using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Grpc.Core;
using System.Threading.Tasks;
using System;

public partial class ClientNetworkContents
{
    // [OFFLINE] 서버(EC2/S3)가 죽어있는 개발 단계 플래그. true면 RPC를 보내지 않고 콜백을
    // 즉시 default로 호출해, 부팅/메뉴가 죽은 서버 응답(수십 초 connect 타임아웃)을 기다리며
    // 멈추는 것을 막는다. 서버 복구 시 false로 (그때는 각 XxxAsync에 deadline도 추가 권장).
    public static bool OfflineMode = true;

    // OfflineMode면 콜백을 즉시 default로 호출하고 true 반환(호출측에서 곧바로 return).
    static bool OfflineSkip<T>(System.Action<T> callbackRecv)
    {
        if (!OfflineMode) return false;
        UnityEngine.Debug.Log("[NET] OfflineMode: skip RPC, callback(default)");
        callbackRecv?.Invoke(default);
        return true;
    }

    static void CheckError(RpcException e)
    {
        Debug.LogError($"Server packet error found  :  {e.Status.Detail}");
    }
    
    async static Task<TResponse> SendPacket<TResponse>(AsyncUnaryCall<TResponse> call)
    {
        try
        {
            return await call.ResponseAsync;
        }
        catch (RpcException e)
        {
            CheckError(e);
            
            var arrErrDetail = e.Status.Detail.Split('|');
            if (arrErrDetail != null)
            {
                if (arrErrDetail.Length > 0)
                {
                    if (int.TryParse(arrErrDetail[0], out int errorCode))
                    {
                        Debug.Log($"[NetworkManager] ErrorCode: {errorCode}");
                        // error 처리
                        // {
                        // }
                    }
                }
                if (arrErrDetail.Length > 1)
                {
                    // local, stage 서버에서 상세 에러 출력
                    Debug.Log($"[NetworkManager] Error Detail: {arrErrDetail[1]}");
                }
            }

            return default(TResponse);
        }
    }
}
