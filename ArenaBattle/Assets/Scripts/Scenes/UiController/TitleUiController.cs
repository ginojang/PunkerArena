using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUiController : BaseUiController<TitleUiController>
{
    protected override void OnCompletePreloadAsset()
    {
        GuiMain.Instance.Open<UiTitle>();
        AddGuiInitListener<UiTitle>(Initialize);
    }

    private void Initialize(GuiObject target)
    {
        // TEST
        // 로그인 인증
        ClientNetworkContents.SendLogin(SystemInfo.deviceUniqueIdentifier, (response) =>
        {
            if (response == null)
            {
                // [OFFLINE BYPASS] 서버(EC2) 미접속 시 종료(Application.Quit) 대신 임시 계정으로 진행.
                // 원복 시 아래 두 줄을 지우고 Application.Quit(); 복구.
                GameDataManager.Instance.userData.UID = 0;
                GameDataManager.Instance.userData.SID = "offline";
            }
            else
            {
                GameDataManager.Instance.userData.UID = response.Uid;
                GameDataManager.Instance.userData.SID = response.Sid;
            }
        });
/*        
        ClientNetworkContents.SendNoticeList(1, 1, (response) =>
        {
            if (response == null)
            {
                // Popup으로 실패 했음을 알려주고 확인시 타이틀로 다시....
                //GuiUtility.ShowNoticeOkPopUp("음성인식을 3회 실패해서 강제로 넘어갑니다.", OnClickGoBetia);

                Application.Quit();
            }
        });*/
    }
    
    protected override void OnDestroyComponent()
    {
        base.OnDestroyComponent();
    }
}
