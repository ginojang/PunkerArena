using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Grpc.Core;
using System.Threading.Tasks;
using System;

public partial class ClientNetworkContents
{
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
