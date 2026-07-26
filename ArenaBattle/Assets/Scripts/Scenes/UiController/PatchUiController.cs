using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchUiController : BaseUiController<PatchUiController>
{
    protected override void OnCompletePreloadAsset()
    {
        GuiMain.Instance.Open<UiPatch>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
    }
}
