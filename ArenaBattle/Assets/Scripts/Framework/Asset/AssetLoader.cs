using System.Collections.Generic;
using UnityEngine;

public class AssetLoader
{
    protected class AssetParam
    {
        public AssetParam(cbFinishLoad newCb, object newParam)
        {
            this.Cb = newCb;
            this.Param = newParam;
        }

        public cbFinishLoad Cb { get; set; }
        public object Param { get; set; }
    }

    public delegate void cbFinishLoad(AssetLoader ld, object p);

	/************************************************************************/
	// Variables

	public bool IsFailed { get; set; } = false;

    public bool IsCompressed { get; set; } = true;

	public bool IsLoadSucceed { get; set; } = false;

	public bool IsCallbackCalled { get; set; } = false;
	// public abstract bool isIdleState { get; }
	public UnityEngine.Object MainAsset { get; set; }
    public string AssetFullPath { get; set; }

    protected List<AssetParam> paramList;

    /************************************************************************/
    // Functions
    public virtual void SetEventFinishLoad(cbFinishLoad cb, object param = null)
    {
		IsCallbackCalled = false;
		IsLoadSucceed = false;

		if (null == paramList)
            paramList = new List<AssetParam>();

        if (null != cb)
            paramList.Add(new AssetParam(cb, param));
    }

    public virtual void CallEventFuncs()
    {
        if (null == paramList)
        {
            return;
        }

        foreach (AssetParam p in paramList)
        {
            p.Cb?.Invoke(this, p.Param);
        }

        paramList.Clear();

		IsCallbackCalled = true;
	}
}
