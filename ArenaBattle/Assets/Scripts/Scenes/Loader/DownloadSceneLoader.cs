using System.Collections;
using UnityEngine;

/// <summary>
/// [Addressables 제거] 오프라인/로컬 빌드는 내려받을 번들이 없다.
/// 기존 어드레스블 번들 다운로드/사이즈체크/캐시정리 로직을 전부 걷어내고 즉시 완료 처리한다.
/// </summary>
public class DownloadSceneLoader : UnitySceneLoader
{
    System.Action completeDownload = null;
    bool patchComplete = false;

    protected override void OnDownloadResources(System.Action onCompleteDownloadResource)
    {
        Messenger.AddListener(Definition.LOADING_UI_STARTBUTTON_CLICK, OnStartBtnClick);
        Messenger.AddListener(Definition.LOADING_UI_STARTDOWNLOADBUTTON_CLICK, OnStartDownLoadBtnClick);
        Messenger.AddListener(Definition.LOADING_UI_CANCELDOWNLOADBUTTON_CLICK, OnCancelDownLoadBtnClick);

        // 다운로드할 번들 없음 → 즉시 진행
        StartCoroutine(DownloadResourcesAsync(onCompleteDownloadResource));
    }

    private void OnStartDownLoadBtnClick()
    {
        Messenger.RemoveListener(Definition.LOADING_UI_STARTDOWNLOADBUTTON_CLICK, OnStartDownLoadBtnClick);
        StartCoroutine(DownloadResourcesAsync(completeDownload));
    }

    private void OnCancelDownLoadBtnClick()
    {
        Messenger.RemoveListener(Definition.LOADING_UI_STARTBUTTON_CLICK, OnStartBtnClick);
        Messenger.RemoveListener(Definition.LOADING_UI_STARTDOWNLOADBUTTON_CLICK, OnStartDownLoadBtnClick);
        Messenger.RemoveListener(Definition.LOADING_UI_CANCELDOWNLOADBUTTON_CLICK, OnCancelDownLoadBtnClick);

        Application.Quit();
    }

    private IEnumerator DownloadResourcesAsync(System.Action onCompleteDownloadResource)
    {
        patchComplete = false;
        StartCoroutine(StartDownloadPatch());
        yield return new WaitUntil(() => patchComplete == true);
        onCompleteDownloadResource();
    }

    protected override void OnUpdateLoadingProgress(AssetLoader loader, object p)
    {
    }

    private void OnStartBtnClick()
    {
        // 비정상 종료 이어하기 부분
        base.DoComplete();
        PlayState.Instance.ChangePlayState(PlayState.STATES.Menu);
    }

    private IEnumerator StartDownloadPatch()
    {
        // 내려받을 번들 없음: 즉시 완료 통보
        Messenger.Broadcast(Definition.LOADING_UI_PATCH_COMPLETE);
        AssetManager.Instance.TotalDownloadBundleSize = 0;
        patchComplete = true;
        yield break;
    }
}
