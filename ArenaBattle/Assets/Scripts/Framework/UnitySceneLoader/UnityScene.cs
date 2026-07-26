using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityTemplateProjects;
using UnityEngine;
//using UnityObject = UnityEngine.Object;

public abstract class UnityScene : MonoBehaviour
{
    public static UnityScene CurrentUnityScene { get; private set; }
    public static Camera CurrentUnityCamera { get; private set; }

    /************************************************************************/
    // Variables

    // protected GameObject main_stage
    // {
    //     get { return Main.MainObject; }
    // }

    private GameObject frameObject;

    public GameObject FrameObject
    {
        get
        {
            if (frameObject == null)
            {
                frameObject = GameObject.Find("__Frame__");
                if (!frameObject)
                {
                    frameObject = new GameObject("__Frame__") { layer = LayerMask.NameToLayer("Default") };
                }
            }
            return frameObject;
        }
    }


    public string[] bgmList = null;

    /************************************************************************/
    // Functions
    protected virtual void Awake()
    {
        // Main 객체 생성
        if (null != CurrentUnityScene)
        {
            Debug.Log("unityscene change is not completed yet...");
            return;
        }
        Main.NotifyStageAwake();
        UnitySceneLoader.NotifyStageAwake(gameObject);
        CurrentUnityScene = this;

        UnityEngine.Object[] camObjects = FindObjectsOfType(typeof(Camera));
        Camera mainCam = Camera.main;
    }

    protected virtual void Start()
    {
        // InputManager.Instance.InputUpdater.Reset();
        enabled = false;

        if(bgmList.Length > 0)
		{
            SoundManager.Instance.LoadBGM($"{bgmList[UnityEngine.Random.Range(0, bgmList.Length)]}");
            //SoundManager.Instance.LoadBGM($"Music/{bgmList[UnityEngine.Random.Range(0, bgmList.Length)]}");
        }
    }


    protected virtual void OnDestroy()
    {
        ResourcePoolManager.Instance.DestroyGameDatapool();
//        Debug.Assert(CurrentUnityScene != this);
    }


    internal static void CloseCurrent()
    {
        if (CurrentUnityScene == null)
            return;

        CurrentUnityScene.OnClose();
        CurrentUnityScene = null;
        CurrentUnityCamera = null;
    }


    protected virtual void OnClose()
    {
        GuiMain.Instance?.ClearUi();
        AssetManager.Instance.ReleaseAllAsset();
    }


    // unity scene 로드 코루틴 함수
    internal void PostLoadScene()
    {
        OnPostLoad();

        enabled = true;
    }

    // 씬 로드 이전의 준비 함수
    protected virtual void OnSetup(XmlNode config_root) { }
    protected virtual void OnPostLoad() { } //씬 로드 완료 이벤트
}
