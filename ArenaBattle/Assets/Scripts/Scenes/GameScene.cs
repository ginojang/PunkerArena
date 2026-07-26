using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : UnityScene
{
    protected override void Awake()
    {
        Debug.Log("################ GameScene Awake");
        base.Awake();
    }
    protected override void Start()
    {
        Debug.Log("################ GameScene Start");
        base.Start();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    protected override void OnClose()
    {
        base.OnClose();
    }
    protected override void OnPostLoad()
    {
        Debug.Log("################ GameScene2 OnPostLoad");
    }
}
