using UnityEngine;
using UnityEngine.UI;

public class GuiSetText : MonoBehaviour
{
    [field: SerializeField]
    public Text Label { get; set; } = null;
    [field: SerializeField]
    public EStringTableType StringTableType { get; set; } = EStringTableType.None;
    [field: SerializeField]
    public int StringTableIndex { get; set; } = -1;

    private void Start()
    {
        // 처음 사용하는 언어에 따라서 기존것이 다를수 있으므로 스타트에서 필요에 의해서 재실행된다.
        //string data = Utility.GetString(StringTableType, StringTableIndex);

        //if (Label.text.Equals(data) == true)
        //    return;

        Set();
    }

    public void Set()
    {
        //if (Label == null)
        //    return;

        //Label.text = Utility.GetString(StringTableType, StringTableIndex);
    }
}
