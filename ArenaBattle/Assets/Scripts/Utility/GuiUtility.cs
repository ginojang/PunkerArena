using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.U2D;
using Devil.Gui;

public static class GuiUtility
{
	private static Sprite sprite2x2Transparent;
	//private static UnityAction mirrorPopupCloseCallback;

	/// <summary>
	/// 이벤트 트리거를 게임오브젝트에 붙이고 이벤트 트리거 타입과 이벤트 발생시 받을 콜백 함수를 등록
	/// </summary>
	/// <param name="_game_object"></param>
	/// <param name="_event_id"></param>
	/// <param name="_callback"></param>
	public static void AddEventTrigger(GameObject _game_object, EventTriggerType _event_id, UnityAction<BaseEventData> _callback)
	{
		if (_game_object == null)
			return;

		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = _event_id;
		entry.callback.AddListener(_callback);

		var event_trigger = _game_object.GetComponent<EventTrigger>();
		if (event_trigger == null)
			event_trigger = _game_object.AddComponent<EventTrigger>();
		event_trigger.triggers.Add(entry);
	}

	public static void DestoryEventTrigger(GameObject _game_object)
	{
		if (_game_object == null)
			return;

		var event_trigger = _game_object.GetComponent<EventTrigger>();
		if (event_trigger == null)
			return;
		event_trigger.triggers.Clear();
		UnityEngine.Object.DestroyImmediate(event_trigger);
	}

	public static void ClearEventTrigger(GameObject _game_object)
	{
		if (_game_object == null)
			return;

		var event_trigger = _game_object.GetComponent<EventTrigger>();
		if (event_trigger == null)
			return;
		event_trigger.triggers.Clear();
	}

	public static void Initialize()
	{
		LoadSprite("Assets/Asset/ui/items/2x2Transparent.png", (s) => { sprite2x2Transparent = s; });
	}

	public static Sprite Get2x2TransparentSprite()
	{
		return sprite2x2Transparent;
	}

	public static void LoadAtlasedSprite(string atlasPath, string spriteName, Image _target, Action<bool> _callbackResult = null)
	{
		AssetManager.Instance.LoadAssetAsync<SpriteAtlas>(atlasPath, (AssetLoader _loader, object _param) =>
		{
			if (_loader.IsLoadSucceed == true)
			{
				var spriteAtlas = _loader.MainAsset as SpriteAtlas;
				var sprite = spriteAtlas.GetSprite(spriteName);
				_target.sprite = sprite;
			}

			if (_callbackResult != null && _callbackResult.Target != null)
				_callbackResult(_loader.IsLoadSucceed);
		});
	}

	public static void LoadSprite(string _path, Image _target, Action<bool> _callbackResult = null)
	{
		AssetManager.Instance.LoadAssetAsync<Sprite>(_path, (AssetLoader _loader, object _param) =>
		{
			if (_loader.IsLoadSucceed == true)
				_target.sprite = _loader.MainAsset as Sprite;

			if (_callbackResult != null && _callbackResult.Target != null)
				_callbackResult(_loader.IsLoadSucceed);
		});
	}

	public static void LoadSprite(string _path, Action<bool, Sprite> _callbackResult = null)
	{
		AssetManager.Instance.LoadAssetAsync<Sprite>(_path, (AssetLoader _loader, object _param) =>
		{
			Sprite sprite = null;
			if (_loader.IsLoadSucceed == true)
				sprite = _loader.MainAsset as Sprite;

			if (_callbackResult != null && _callbackResult.Target != null)
				_callbackResult.Invoke(_loader.IsLoadSucceed, sprite);
		});
	}

	public static void LoadSprite(string _path, Action<Sprite> _callbackResult = null)
	{
		AssetManager.Instance.LoadAssetAsync<Sprite>(_path, (AssetLoader _loader, object _param) =>
		{
			Sprite sprite = null;
			if (_loader.IsLoadSucceed == true)
				sprite = _loader.MainAsset as Sprite;

			if (_callbackResult != null && _callbackResult.Target != null)
				_callbackResult.Invoke(sprite);
		});
	}

	public static void AddAssetLoader(string addressableKey, List<AssetLoader> theList)
	{
		var loader = AssetManager.Instance.PreLoadAsset(addressableKey, null);
		if (loader != null)
		{
			theList.Add(loader);
		}
	}

	#region RectTransform
	public enum Corner
	{
		LeftTop,
		RightTop,
		LeftBottom,
		RightBottom,
	}
	/*
	public static bool Overlaps(this RectTransform a, RectTransform b)
	{
		return a.WorldRect().Overlaps(b.WorldRect());
	}
	public static bool Overlaps(this RectTransform a, RectTransform b, bool allowInverse)
	{
		return a.WorldRect().Overlaps(b.WorldRect(), allowInverse);
	}

	public static Rect WorldRect(this RectTransform rectTransform)
	{
		Vector2 sizeDelta = rectTransform.sizeDelta;
		float rectTransformWidth = sizeDelta.x * rectTransform.lossyScale.x;
		float rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

		Vector3 position = rectTransform.position;
		return new Rect(position.x - rectTransformWidth / 2f, position.y - rectTransformHeight / 2f, rectTransformWidth, rectTransformHeight);
	}

	public static bool rectOverlaps(this RectTransform rectTrans1, RectTransform rectTrans2)
	{
		//Rect rect1 = new Rect(rectTrans1.localPosition.x, rectTrans1.localPosition.y, rectTrans1.rect.width, rectTrans1.rect.height);
		//Rect rect2 = new Rect(rectTrans2.localPosition.x, rectTrans2.localPosition.y, rectTrans2.rect.width, rectTrans2.rect.height);
		Rect rect1 = new Rect(rectTrans1.position.x, rectTrans1.position.y, rectTrans1.rect.width, rectTrans1.rect.height);
		Rect rect2 = new Rect(rectTrans2.position.x, rectTrans2.position.y, rectTrans2.rect.width, rectTrans2.rect.height);

		return rect1.Overlaps(rect2);
	}

	public static Rect RectTransformToScreenSpace(RectTransform transform)
	{
		Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
		float x = transform.position.x + transform.anchoredPosition.x;
		float y = Screen.height - transform.position.y - transform.anchoredPosition.y;

		return new Rect(x, y, size.x, size.y);
	}

	public static Bounds GetRectTransformBounds(RectTransform transform)
	{
		Vector3[] WorldCorners = new Vector3[4];
		transform.GetWorldCorners(WorldCorners);
		Bounds bounds = new Bounds(WorldCorners[0], Vector3.zero);
		for (int i = 1; i < 4; ++i)
		{
			bounds.Encapsulate(WorldCorners[i]);
		}
		return bounds;
	}*/

	public static Rect GetScreenCoordinates(RectTransform uiElement)
	{
		var worldCorners = new Vector3[4];
		uiElement.GetWorldCorners(worldCorners);
		var result = new Rect(
					  worldCorners[0].x,
					  worldCorners[0].y,
					  worldCorners[2].x - worldCorners[0].x,
					  worldCorners[2].y - worldCorners[0].y);
		return result;
	}

	public static bool TestIntersectionOBBToAABB(this RectTransform obb, RectTransform aabb, Camera camera)
	{
		/*
		Rect the = GetScreenCoordinates(obb);
		Vector2 theScreenPoint = RectTransformUtility.WorldToScreenPoint(camera, the.min);
		if (RectTransformUtility.RectangleContainsScreenPoint(aabb, theScreenPoint, camera) == true)
		{
			return true;
		}*/
		var worldCorners = new Vector3[4];
		obb.GetWorldCorners(worldCorners);
		foreach (var the in worldCorners)
		{
			Vector2 theScreenPoint = RectTransformUtility.WorldToScreenPoint(camera, the);
			if (RectTransformUtility.RectangleContainsScreenPoint(aabb, theScreenPoint, camera) == true)
			{
				return true;
			}
		}

		// Need to calculate once again in the reverse way for a perfect detection such as dia

		//OnDrawGizmos
		/*아 돌아버리
		Rect rtOBB = GetScreenCoordinates(obb);
		Rect rtAABB = GetScreenCoordinates(aabb);
		return rtOBB.Overlaps(rtAABB);
		*/
		/*
		for (int i = 0; i < 4; i++)
		{
			Vector2 the = RectTransformUtility.WorldToScreenPoint(camera, obb.anchoredPosition);
			if (RectTransformUtility.RectangleContainsScreenPoint(aabb, the, camera) == true)
			{
				return true;
			}
		}
		*/

		return false;

		//return obb.rect.Overlaps(aabb.rect);
		//Bounds bounds = GetRectTransformBounds(obb);
		//Rect screenRect = new Rect(bounds.min, bounds.size);
		/*
		if (RectTransformUtility.RectangleContainsScreenPoint(aabb, screenRect.min) == true)
		{
			return true;
		}

		if (RectTransformUtility.RectangleContainsScreenPoint(aabb, screenRect.max) == true)
		{
			return true;
		}
		*/
		/*
		Rect screenSpaceRect = RectTransformToScreenSpace(obb);
		if (RectTransformUtility.RectangleContainsScreenPoint(aabb, screenSpaceRect.min) == true)
		{
			return true;
		}

		if (RectTransformUtility.RectangleContainsScreenPoint(aabb, screenSpaceRect.min) == true)
		{
			return true;
		}
		*/
		/*
		if (RectTransformUtility.RectangleContainsScreenPoint(bagBoundingBox, itemRoot.transform.position) == true)

		// Extract world corners of obb
		Vector3[] v = new Vector3[4];
		obb.GetWorldCorners(v);

		for (var i = 0; i < 4; i++)
		{
			if (RectTransformUtility.RectangleContainsScreenPoint(aabb, screenPosition) == true)
			{
				// Test fail, which means overlapping or collided.
				return true;
			}
		}

		return false;*/
	}

	/// <summary>
	/// Collision/Overlap test using bounds
	/// </summary>
	/// <param name="a">"Source RectTransform"</param>
	/// <param name="b">"Target RectTransform"</param>
	/// <returns></returns>
	public static bool Intersects(this RectTransform a, RectTransform b)
	{
		return a.GetWorldBounds().Intersects(b.GetWorldBounds());
	}

	public static Bounds GetWorldBounds(this RectTransform rectTransform)
	{
		var corners = new Vector3[4];
		rectTransform.GetWorldCorners(corners);

		var center = corners.Aggregate(Vector3.zero, (current, corner) => current + corner) / corners.Length;
		var size = new Vector3(
			corners.Max(corner => corner.x) - corners.Min(corner => corner.x),
			corners.Max(corner => corner.y) - corners.Min(corner => corner.y),
			1);
		return new Bounds(center, size);
	}

	public static Vector2 GetRelativePosition(this RectTransform rectTransform, Corner corner)
	{
		var parentBounds = rectTransform.parent.GetComponent<RectTransform>().GetWorldBounds();
		var bounds = rectTransform.GetWorldBounds();

		var pos = Vector2.zero;

		switch (corner)
		{
			case Corner.LeftBottom:
			case Corner.RightBottom:
				pos.y = bounds.min.y - parentBounds.min.y;
				break;
			case Corner.LeftTop:
			case Corner.RightTop:
				pos.y = parentBounds.max.y - bounds.max.y;
				break;
			default:
				throw new ArgumentOutOfRangeException("corner", corner, null);
		}

		switch (corner)
		{
			case Corner.LeftTop:
			case Corner.LeftBottom:
				pos.x = bounds.min.x - parentBounds.min.x;
				break;
			case Corner.RightTop:
			case Corner.RightBottom:
				pos.x = parentBounds.max.x - bounds.max.x;
				break;
			default:
				throw new ArgumentOutOfRangeException("corner", corner, null);
		}

		return pos;
	}

	public static void SetRelativePosition(this RectTransform rectTransform, Vector2 pos, Corner corner)
	{
		var parentBounds = rectTransform.parent.GetComponent<RectTransform>().GetWorldBounds();
		var bounds = rectTransform.GetWorldBounds();
		var anchoredPosition = rectTransform.position;

		switch (corner)
		{
			case Corner.LeftBottom:
			case Corner.RightBottom:
				anchoredPosition.y += parentBounds.min.y - bounds.min.y + pos.y;
				break;
			case Corner.LeftTop:
			case Corner.RightTop:
				anchoredPosition.y += parentBounds.max.y - bounds.max.y - pos.y;
				break;
			default:
				throw new ArgumentOutOfRangeException("corner", corner, null);
		}

		switch (corner)
		{
			case Corner.LeftTop:
			case Corner.LeftBottom:
				anchoredPosition.x += parentBounds.min.x - bounds.min.x + pos.x;
				break;
			case Corner.RightTop:
			case Corner.RightBottom:
				anchoredPosition.x += parentBounds.max.x - bounds.max.x - pos.x;
				break;
			default:
				throw new ArgumentOutOfRangeException("corner", corner, null);
		}

		rectTransform.position = anchoredPosition;
	}

	public static Vector2 GetSizeDelata(this GameObject gameObject)
	{
		if (gameObject == null)
		{
			Debug.LogWarning("GuiUtility::GetSizeDelta::if (gameObject == null)");
			return Vector2.zero;
		}

		RectTransform the = gameObject.GetComponent<RectTransform>();
		if (the == null)
		{
			Debug.LogWarning("GuiUtility::GetSizeDelta::if (the == null)");
			return Vector2.zero;
		}

		return new Vector2(the.rect.width, the.rect.height);
	}

	public static void SetPivot(this GameObject gameObject, Corner corner)
	{
		if (gameObject == null)
		{
			Debug.LogWarning("GuiUtility::SetPivot::if (gameObject == null)");
			return;
		}

		RectTransform the = gameObject.GetComponent<RectTransform>();
		if (the == null)
		{
			Debug.LogWarning("GuiUtility::SetPivot::if (the == null)");
			return;
		}

		switch (corner)
		{
			case Corner.LeftTop:
				the.pivot = Vector2.up;
				break;
			case Corner.RightTop:
				the.pivot = Vector2.one;
				break;
			case Corner.LeftBottom:
				the.pivot = Vector2.zero;
				break;
			case Corner.RightBottom:
				the.pivot = Vector2.right;
				break;
			default:
				throw new ArgumentOutOfRangeException("corner", corner, null);
		}
	}
	#endregion

	#region Color
	public static void SetColorAlpha(this Image image, int alphaValue)
	{
		image.color = new Color(image.color.r, image.color.g, image.color.b, (float)alphaValue / 255);
	}

	public static void SetColorAlpha(this Text text, float alpha)
	{
		text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
	}
	#endregion

	
	#region Event
	///Returns 'true' if we touched or hovering on Unity UI element.
	public static bool IsPointerOverUIElement()
	{		
		return IsPointerOverUIElement(GetEventSystemRaycastResults());

		//EventSystem.current.IsPointerOverGameObject()
		/*
		bool the = EventSystem.current.IsPointerOverGameObject();
		return the;*/
	}

	///Returns 'true' if we touched or hovering on Unity UI element.
	public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
	{
		for (int index = 0; index < eventSystemRaysastResults.Count; index++)
		{
			RaycastResult curRaysastResult = eventSystemRaysastResults[index];
			if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
				return true;
		}
		return false;
	}

	///Gets all event systen raycast results of current mouse or touch position.
	static List<RaycastResult> GetEventSystemRaycastResults()
	{
		PointerEventData eventData = new PointerEventData(EventSystem.current);
		eventData.position = Input.mousePosition;
		List<RaycastResult> raysastResults = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, raysastResults);
		return raysastResults;
	}

	public static void AddListener(this LiteButton liteButton, UnityAction callback)
	{
		if (callback == null)
		{
			return;
		}

		// This tricky jolly stuff makes me crazy all the time dam it.
		// The following onClick UnityEvents can add a listener everytime dialog is invoked.
		liteButton.RemoveAllListeners();

		liteButton.onClick.AddListener(() => callback());
	}
	#endregion

	#region Text
	public static int GetLineCount(this Text text)
	{
		Canvas.ForceUpdateCanvases();
		TextGenerator the = text.cachedTextGenerator;
		return the.lineCount;
	}
	#endregion

	#region bool <-> int
	public static int ToInt(this bool flag)
	{
		return (flag == true) ? 1 : 0;
	}

	public static bool ToBool(this int value)
	{
		return (value == 0) ? false : true;
	}
	#endregion


	/// <summary>
	/// RichText TAG string remove.
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	public static string StripHtml(string input)
	{
		return Regex.Replace(input, "<.*?>", string.Empty);
	}

	public static string GetPath(this Transform current)
	{
		if (current.parent == null)
		{
			return current.name;
		}

		return current.parent.GetPath() + "/" + current.name;
	}
}
