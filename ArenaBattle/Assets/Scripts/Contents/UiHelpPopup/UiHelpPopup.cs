using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiHelpPopup : UiBase<UiHelpPopup>
{
    [SerializeField]
    private Button mBindBtn_Prev = null;
    [SerializeField]
    private Button mBindBtn_Next = null;
    [SerializeField]
    private Button mBindBtn_Close = null;
    [SerializeField]
    private Text mBindText_Page = null;
    [SerializeField]
    private List<GameObject> mPageGameObjects = null;
    private GameObject mCurrentPage = null;
    private int mCurrentIndex = 0;

    // Start is called before the first frame update
    void Open()
    {
        mCurrentIndex = 0;
        Set();
    }

    #region >> CallBack <<
    public void Set()
    {
        if (mCurrentPage != null)
        {
            mCurrentPage.SetActive(false);
        }

        mCurrentPage = mPageGameObjects[mCurrentIndex];
        mCurrentPage.SetActive(true);
        // 현재 페이지 출력
        //mBindText_Page

        SetButtons();
    }

    private void SetButtons()
    {
        mBindBtn_Prev.gameObject.SetActive(mCurrentIndex > 0);
        mBindBtn_Next.gameObject.SetActive(mCurrentIndex < mPageGameObjects.Count - 1);
        mBindBtn_Close.gameObject.SetActive(mCurrentIndex == mPageGameObjects.Count - 1);
        mBindText_Page.text = (mCurrentIndex + 1).ToString();
    }

    public void OnClickMoveNext()
    {
        if (mCurrentIndex >= mPageGameObjects.Count - 1)
            return;

        mCurrentIndex++;

        Set();
    }

    public void OnClickMovePrev()
    {
        if (mCurrentIndex <= 0)
            return;

        mCurrentIndex--;

        Set();
    }

    public void OnClickClose()
    {
        mCurrentIndex = -1;

        Destroy(gameObject);
    }
    #endregion
}
