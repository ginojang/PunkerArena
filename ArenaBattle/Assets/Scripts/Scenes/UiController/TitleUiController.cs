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
                // Popup으로 실패 했음을 알려주고 확인시 타이틀로 다시....
                //GuiUtility.ShowNoticeOkPopUp("음성인식을 3회 실패해서 강제로 넘어갑니다.", OnClickGoBetia);

                Application.Quit();
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
