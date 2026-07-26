using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GUIScreenRatio : UIBehaviour
{
	[SerializeField]
	private float mDefaultRatio = 1.6f;
	[SerializeField]
	private Vector2 mCurScreenSize;

	[field: SerializeField]
	public CanvasScaler UICanvasScaler	{ get; set; }

	public void SetAspectRatio()
	{
		mCurScreenSize = new Vector2(Screen.width, Screen.height);
		var curRatio = mCurScreenSize.x / mCurScreenSize.y;

		if (curRatio < mDefaultRatio)
		{
			if (UICanvasScaler != null) UICanvasScaler.matchWidthOrHeight = 0f;
		}
		else
		{
			if (UICanvasScaler != null) UICanvasScaler.matchWidthOrHeight = 1f;
		}
	}

	public float GetAspectRatio()
	{
		var curRatio = mCurScreenSize.x / mCurScreenSize.y;
		var aspectRatio = 0f;
		if (curRatio < mDefaultRatio)
			aspectRatio = UICanvasScaler.referenceResolution.x / Screen.width;
		else
			aspectRatio = UICanvasScaler.referenceResolution.y / Screen.height;

		return aspectRatio;
	}

	protected override void OnRectTransformDimensionsChange()
	{
		if (mCurScreenSize.x == Screen.width && mCurScreenSize.y == Screen.height)
			return;

		SetAspectRatio();
	}


}
