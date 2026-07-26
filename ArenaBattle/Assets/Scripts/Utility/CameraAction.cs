using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;

public enum actionCam
{
    Intro = 0,
    Intro_Owner,
    Intro_Enemy,
    Basic,
    Battle,
}
public class cameraInfo
{
    public float fov;
    public float minPlane;
    public float maxPlane;
    public float Depth;
    public int cullingMask;

    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}

public class CameraAction : MonoBehaviour
{
    Camera cam;
    cameraInfo initInfo = null;
    cameraInfo startInfo = null;
    cameraInfo destInfo = null;
    actionCam curActionCam = actionCam.Battle;
    [SerializeField] CinemachineCamera[] virtualCams;

    [SerializeField] GameObject lookBasic;
    // Start is called before the first frame update
    void Start()
    {
        cam = transform.GetComponent<Camera>();
        initInfo = new cameraInfo();
        startInfo = new cameraInfo();
        destInfo = new cameraInfo();

        lookBasic = GameObject.Find("LookBasic");
        
        InitGameCamera();

        SetActiveCam(actionCam.Battle);
        //        virtualCam = GameObject.Find("ActionCam").GetComponent<CinemachineVirtualCamera>();
        Messenger.AddListener(Definition.SetBasicTarget, OnSetActionBasic);
        Messenger.AddListener<CharacterBase>(Definition.SetLookAtTarget, OnSetActionCharacter);
        Messenger.AddListener<CharacterBase>(Definition.IntroOwnerCam, OnSetIntroOwnerCam);
        Messenger.AddListener<CharacterBase>(Definition.IntroEnemyCam, OnSetIntroEnemyCam);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
		{
            SetActionToTarget(destInfo);
		}
    }

	private void OnDestroy()
	{
        Messenger.RemoveListener(Definition.SetBasicTarget, OnSetActionBasic);
        Messenger.RemoveListener<CharacterBase>(Definition.SetLookAtTarget, OnSetActionCharacter);
        Messenger.RemoveListener<CharacterBase>(Definition.IntroOwnerCam, OnSetIntroOwnerCam);
        Messenger.RemoveListener<CharacterBase>(Definition.IntroEnemyCam, OnSetIntroEnemyCam);
    }

	void InitGameCamera()
	{
        initInfo.fov = cam.fieldOfView;
        initInfo.minPlane = cam.nearClipPlane;
        initInfo.maxPlane = cam.farClipPlane;
        initInfo.Depth = cam.depth;
        initInfo.cullingMask = cam.cullingMask;
        initInfo.position = cam.transform.position;
        initInfo.rotation = cam.transform.rotation;
        initInfo.scale = cam.transform.localScale;

        destInfo.fov = 30;
        destInfo.minPlane = cam.nearClipPlane;
        destInfo.maxPlane = cam.farClipPlane;
        destInfo.Depth = cam.depth;
        destInfo.cullingMask = cam.cullingMask;
        destInfo.position = cam.transform.position;
        destInfo.rotation = cam.transform.rotation;
        destInfo.scale = cam.transform.localScale;
    }

    private void OnSetActionBasic()
    {
        SetActiveCam(actionCam.Basic, null);
    }

    private void OnSetActionCharacter(CharacterBase charbase)
	{
        if (curActionCam != actionCam.Battle)
            SetActiveCam(actionCam.Battle, charbase);

        if (charbase == null)
		{
            virtualCams[(int)curActionCam].LookAt = null;
            ResetCameraInfo();
        }
        else
		{
            virtualCams[(int)curActionCam].LookAt = charbase.transform;
		}
    }

    private void OnSetIntroOwnerCam(CharacterBase charbase)
	{
        SetActiveCam(actionCam.Intro_Owner, charbase);
	}

    private void OnSetIntroEnemyCam(CharacterBase charbase)
    {
        SetActiveCam(actionCam.Intro_Enemy, charbase);
    }

    public void ResetCameraInfo()
	{
        cam.fieldOfView = initInfo.fov;
        cam.nearClipPlane = initInfo.minPlane;
        cam.farClipPlane = initInfo.maxPlane;
        cam.depth = initInfo.Depth;
        cam.cullingMask = initInfo.cullingMask;
        cam.transform.position = initInfo.position;
        cam.transform.rotation = initInfo.rotation;
        cam.transform.localScale = initInfo.scale;
	}

    public void SetActiveCam(actionCam camtype, CharacterBase target = null)
    {
        if (lookBasic == null)
            lookBasic = GameObject.Find("LookBasic");
        
        for (int i = 0; i < virtualCams.Length; i++)
        {
            if ((actionCam)i == camtype)
            {
                virtualCams[i].gameObject.SetActive(true);
            }
            else
            {
                virtualCams[i].gameObject.SetActive(false);
            }
        }

        curActionCam = camtype;

        virtualCams[(int)curActionCam].LookAt = target != null ? target.transform : lookBasic?.transform;
    }

    public void SetActionToTarget(cameraInfo info)
	{
        Tweener tween = cam.DOFieldOfView(info.fov, 1.0f);
        tween.onKill = EndAction;
    }

    public void ResetActionToTarget(cameraInfo info)
	{
        Tweener tween = cam.DOFieldOfView(info.fov, 1.0f);
        tween.onKill = () =>
        {
            ResetCameraInfo();
        };
    }

    private void EndAction()
	{
        ResetActionToTarget(initInfo);
	}
}
