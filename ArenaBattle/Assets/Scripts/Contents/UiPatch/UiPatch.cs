using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
//using PiratePanic;

public class UiPatch : UiBase<UiPatch>
{
    [SerializeField]
    GameObject root;
    [SerializeField]
    GameObject patchPopupObj = null;
    [SerializeField]
    Button confirmBtn;
    [SerializeField]
    Button cancelBtn;
    [SerializeField]
    Button downloadBtn;
    [SerializeField]
    Text patchText;
    [SerializeField]
    Button startBtn;
    [SerializeField]
    Text patchDescText;
    [SerializeField]
    GameObject sliderObj;
    [SerializeField]
    Slider slider;
    [SerializeField]
    Text sliderText;

    protected override void Awake()
	{
		base.Awake();

        Messenger.AddListener<float, string>(Definition.LOADING_UI_PATCH_COUNT, OnPatchCount);
        Messenger.AddListener(Definition.LOADING_UI_PATCH_COMPLETE, OnPatchComplete);
    }

	// Start is called before the first frame update
	protected override void Start()
    {
        base.Start();
        if (AssetManager.Instance.TotalDownloadBundleSize > 0)
        {
            SetPatchInfo(AssetManager.Instance.TotalDownloadBundleSize);
        }
        else
		{
            patchPopupObj.SetActive(false);
            startBtn.gameObject.SetActive(true);
        }

        startBtn.onClick.AddListener(OnClickStartBtn);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnPatchCount(float curvalue, string patchCount)
	{
        slider.value = curvalue;
        sliderText.text = $"������Ʈ �� : {patchCount}% ";
    }

    private void OnPatchComplete()
	{
        slider.value = 1;
        sliderText.text = $"������Ʈ �Ϸ� ";
        startBtn.interactable = true;
    }

    public void SetActiveObject(bool bView)
    {
        root.SetActive(bView);
    }

    private void OnClickStartBtn()
	{
        Messenger.Broadcast(Definition.LOADING_UI_STARTBUTTON_CLICK);
    }

    private void OnClickDownloadBtn()
    {
        Messenger.Broadcast(Definition.LOADING_UI_STARTDOWNLOADBUTTON_CLICK);

        patchPopupObj.SetActive(false);
        sliderObj.SetActive(true);
        slider.value = 0;
        sliderText.text = $"������Ʈ �� : 0%";
    }

    private void OnClickCancelBtn()
    {
        Messenger.Broadcast(Definition.LOADING_UI_CANCELDOWNLOADBUTTON_CLICK);
    }

    private void SetPatchInfo(long patch)
    {
        //long enableSpace = 1;
        patchPopupObj.SetActive(true);
//        sliderObj.SetActive(false);
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
//        enableSpace = (long)DiskUtils.CheckAvailableSpace();
#elif UNITY_ANDROID || UNITY_IOS
//            enableSpace = (long)DiskUtils.CheckAvailableSpace();            
#endif
/*        enableSpace *= 1000000;
        if (enableSpace < patch) // ��ġ������ ������
        {
            patchDescText.text = $"�뷮�� �����Ͽ� �ٿ�ε� �� �� �����ϴ�\n�뷮 Ȯ�� �� �ٽ� �õ� ���ּ���";
            downloadBtn.onClick.AddListener(OnClickDownloadBtn);
            cancelBtn.onClick.AddListener(OnClickCancelBtn);
            confirmBtn.gameObject.SetActive(false);
            startBtn.gameObject.SetActive(false);
        }
        else
*/        {
            patchDescText.text = "�ð��� �ټ� �ɸ� �� ������\nWifi ȯ���� �����մϴ�.";
            downloadBtn.onClick.AddListener(OnClickDownloadBtn);
            cancelBtn.onClick.AddListener(OnClickCancelBtn);
            confirmBtn.gameObject.SetActive(false);
            startBtn.interactable = false;
        }

        float mb = (patch / 1000000.0f);
        mb = Mathf.Ceil(mb);
        string str = ((int)mb).ToString();

        str = $"�ٿ�ε� �뷮 : {str} MB";

        patchText.text = str;
    }

	protected override void OnDestroy()
	{
		base.OnDestroy();

        Messenger.RemoveListener<float, string>(Definition.LOADING_UI_PATCH_COUNT, OnPatchCount);
        Messenger.RemoveListener(Definition.LOADING_UI_PATCH_COMPLETE, OnPatchComplete);
    }
}
