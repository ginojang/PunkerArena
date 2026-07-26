using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class GuiAnimation : MonoBehaviour
{
	// Animation Type, 계속 추가...!
	public enum ANIMATION_TYPE
	{
		POPUP,
		FADE_IN,
		FADE_IN_OUTQUAD,
		FADE_OUT,
		FADE_OUT_SHORT,
		FADE_OUT_SHORT_CONTINUE,
		MOVE_OUTQUAD,
		MOVE_INQUAD_SHORT,
	}
	[SerializeField] private ANIMATION_TYPE animationType = ANIMATION_TYPE.POPUP;

	// 시작과 동시에 Animation Play
	[SerializeField] private bool autoPlay = true;

	// Animation이 Play 될 동안, 해당 객체의 최상위에 마우스 터치 인식을 막아줄 Blocker 사용 여부
	[SerializeField] private bool useTouchBlocker = false;
	private GameObject touchBlocker = null;

	[Header("[Move Options]")]
	[SerializeField] private Vector2 moveGoalPosition = Vector2.zero;
	public Vector2 MoveGoalPosition
	{
		set { moveGoalPosition = value; }
	}

	public UnityAction OnComplete { get; set; } = null;

	#region Animation's time variable
	public float TimePopupFade { get; set; } = 0.3f;
	public float TimePopupScale { get; set; } = 0.4f;
	public float TimeFadeIn { get; set; } = 0.6f;
	public float TimeFadeInOutQuad { get; set; } = 0.4f;
	public float TimeFadeOut { get; set; } = 0.6f;
	public float TimeFadeOutShort { get; set; } = 0.15f;
	public float TimeMoveOutQuad { get; set; } = 0.3f;
	public float TimeMoveInQuadShort { get; set; } = 0.15f;
	#endregion

	private CanvasGroup canvasGroup;
	private RectTransform rectTransform;
	private Vector2 initPosition;

	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		rectTransform = GetComponent<RectTransform>();
		initPosition = rectTransform.anchoredPosition;

		if (useTouchBlocker == true)
		{
			// Make Blocker
			touchBlocker = new GameObject("_TouchBlocker_");
			touchBlocker.layer = LayerMask.NameToLayer("UI");
				
			// FullScreen Mode
			var rt = touchBlocker.AddComponent<RectTransform>();
			rt.sizeDelta = Vector2.zero;
			rt.anchorMin = Vector2.zero;
			rt.anchorMax = Vector2.one;

			// Image Raycast 이용
			var image = touchBlocker.AddComponent<Image>();
			image.SetColorAlpha(0);

			touchBlocker.transform.SetParent(transform, false);
			touchBlocker.SetActive(false);
		}
	}

	private void Start()
	{
		if (autoPlay == true)
		{
			PlayAnimation(OnComplete);
		}
	}

	/// <summary>
	/// 현재 셋팅된 AnimationType의 연출을 Play
	/// </summary>
	public void PlayAnimation(UnityAction onComplete = null)
	{
		OnComplete = onComplete;

		var method = GetType().GetMethod("Animation_" + animationType.ToString(), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
		method?.Invoke(this, null);
	}

	/// <summary>
	/// 요청한 AnimationType값으로 연출 Play
	/// </summary>
	/// <param name="type"></param>
	public void PlayAnimation(ANIMATION_TYPE type, UnityAction onComplete = null)
	{
		animationType = type;
		PlayAnimation(onComplete);
	}

	/// <summary>
	/// Play 중인 Animation을 멈춤
	/// </summary>
	public void StopAnimation()
	{
		KillAnimation();
		touchBlocker?.SetActive(false);
		OnComplete = null;
	}

	/// <summary>
	/// DOKill로 모든 Tween Kill 처리
	/// </summary>
	private void KillAnimation()
	{
		transform.DOKill();
		canvasGroup.DOKill();
		rectTransform.DOKill();
	}

	/// <summary>
	/// Animation_[TYPE] 전, 호출되어 Animation 관련 기능 초기화
	/// </summary>
	private void ResetAnimation()
	{
		KillAnimation();
		touchBlocker?.SetActive(true);
	}

	/// <summary>
	/// Animation_[TYPE] 완료 후, 호출되어 Animation 관련 기능 마무리
	/// </summary>
	private void CompleteAnimation()
	{
		touchBlocker?.SetActive(false);
		OnComplete?.Invoke();
	}

	/// <summary>
	/// Popup 연출
	/// </summary>
	private void Animation_POPUP()
	{
		ResetAnimation();

		canvasGroup.alpha = 0f;
		canvasGroup.DOFade(1f, TimePopupFade);

		transform.localScale = Vector3.zero;
		transform.DOScale(1f, TimePopupScale).SetEase(Ease.OutBack).OnComplete(CompleteAnimation);
	}

	/// <summary>
	/// FadeIn
	/// </summary>
	private void Animation_FADE_IN()
	{
		ResetAnimation();

		canvasGroup.alpha = 0f;
		canvasGroup.DOFade(1f, TimeFadeIn).SetEase(Ease.Linear).OnComplete(CompleteAnimation);
	}

	/// <summary>
	/// FadeIn OutQuad
	/// </summary>
	private void Animation_FADE_IN_OUTQUAD()
	{
		ResetAnimation();

		canvasGroup.alpha = 0f;
		canvasGroup.DOFade(1f, TimeFadeInOutQuad).SetEase(Ease.OutQuad).OnComplete(CompleteAnimation);
	}

	/// <summary>
	/// FadeOut
	/// </summary>
	private void Animation_FADE_OUT()
	{
		ResetAnimation();

		canvasGroup.alpha = 1f;
		canvasGroup.DOFade(0f, TimeFadeOut).SetEase(Ease.Linear).OnComplete(CompleteAnimation);
	}

	/// <summary>
	/// FadeOut Short
	/// </summary>
	private void Animation_FADE_OUT_SHORT()
	{
		ResetAnimation();

		canvasGroup.alpha = 1f;
		canvasGroup.DOFade(0f, TimeFadeOutShort).SetEase(Ease.Linear).OnComplete(CompleteAnimation);
	}

	private void Animation_FADE_OUT_SHORT_CONTINUE()
	{
		ResetAnimation();

		canvasGroup.DOFade(0f, TimeFadeOutShort).SetEase(Ease.Linear).OnComplete(CompleteAnimation);
	}

	/// <summary>
	/// Move OutQuad
	/// </summary>
	private void Animation_MOVE_OUTQUAD()
	{
		ResetAnimation();

		rectTransform.DOAnchorPos(moveGoalPosition, TimeMoveOutQuad).SetEase(Ease.OutQuad).OnComplete(CompleteAnimation);
	}

	/// <summary>
	/// Move InQuad Short
	/// </summary>
	private void Animation_MOVE_INQUAD_SHORT()
	{
		ResetAnimation();

		rectTransform.DOAnchorPos(moveGoalPosition, TimeMoveInQuadShort).SetEase(Ease.InQuad).OnComplete(CompleteAnimation);
	}
}
