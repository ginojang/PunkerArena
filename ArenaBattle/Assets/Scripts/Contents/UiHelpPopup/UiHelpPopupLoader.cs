using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiHelpPopupLoader : MonoBehaviour
{
    [SerializeField]
    private string mIndex = string.Empty;

    public void Open()
    {
        string prefabName = CSVDataManager.GetTable<HelpPopupBaseTable>().GetPrefabName(mIndex);

        GuiMain.Instance.Open(prefabName);
    }
}
