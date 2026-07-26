using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using Grpc.Core;
using Lobby;

/*using Grpc;
using Grpc.Net;
using Grpc.Net.Client;
using System.Net.Http;
using Grpc.Net.Client.Web;
using System;
using Grpc.Core;*/

public enum NetworkType
{
    Lobby,
    Ingame,
    
}
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance_value;

    public static NetworkManager Instance
    {
        get
        {
            if (instance_value == null)
            {
                GameObject go = new GameObject("NetworkManager");
                instance_value = go.AddComponent<NetworkManager>();
                ImmortalGameObject.AttachObject(go);
            }

            return instance_value;
        }
    }

    private NetworkType sendType;
    private Channel _channel;
    private string _server = "ec2-13-212-207-205.ap-southeast-1.compute.amazonaws.com:6565";

    public Channel channel
    {
        get { return _channel; }
    }
    private void OnDisable()
    {
        if(_channel != null)
            _channel.ShutdownAsync().Wait();
    }

    private void OnEnable()
    {
        if(_channel != null)
            _channel.ShutdownAsync().Start();
    }

    public void Initialize()
	{
        _channel = new Channel(_server, ChannelCredentials.Insecure);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TestCallback(NoticeResponse response)
    {
        
    }
}
