using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devil.Gui;
using Lobby;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UiTitle : UiBase<UiTitle>
{
    [SerializeField]
    private Button start_Btn = null;
    
    protected override void Start()
    {
        base.Start();
        start_Btn.onClick.AddListener(OnClickStartBtn);
    }

    private void OnClickStartBtn()
    {
        start_Btn.interactable = false;

        ClientNetworkContents.SendUsereData(GameDataManager.Instance.userData.UID, GameDataManager.Instance.userData.SID, (response) =>
        {
            // [OFFLINE] 서버 유저데이터 파싱(SetUserData) 삭제 — 서버/네트웍 재작성 예정. 바로 진입.
            EntryGame();
        });
    }
    
    private void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            
        }
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    private void EntryGame()
    {
        Main main = Main.MainObject.GetComponent<Main>();

        if (AssetManager.Instance.TotalDownloadBundleSize > 0)
            PlayState.Instance.ChangePlayState(PlayState.STATES.Patch);
        else
            PlayState.Instance.ChangePlayState(PlayState.STATES.Menu);
    }

    
}
