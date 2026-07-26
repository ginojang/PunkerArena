using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UiScreenTransition : UiBase<UiScreenTransition>
{
	public enum EVENT_ID : byte
	{
		START_FADE_IN,
		FINISH_FADE_IN,

		START_FADE_OUT,
		FINISH_FADE_OUT,

		START_FADE_OUTIN,
		FINISH_FADE_OUTIN,
	}

	// IsPlaying 은 FadeOut -> In 등의 한 묶음 셋트가 아직 Play 중인지를 체크 (FadeOut 상태로 In 진행 대기 중도 IsPlaying = TRUE 임)
	public bool IsPlaying { get; private set; } = false;
	public bool IsPlayingFadeOut { get; private set; } = false;
	public bool IsPlayingFadeIn { get; private set; } = false;
	public bool IsPlayingFadeOutIn { get; private set; } = false;

	public float DefaultTime { get; private set; } = 0.6f;

	[SerializeField] private GuiAnimation rectPanel = null;

	private UnityAction onComplete = null;

	protected override void Awake()
	{
		base.Awake();

		rectPanel.gameObject.SetActive(false);
	}

	private void SetColor(Color color)
	{
		if (color == default) color = Color.black;

		// 다양한 Panel 종류가 있을 시 모든 Panel에 대해 색상 변경 적용
		rectPanel.GetComponent<Image>().color = color;
	}

	private void PrepareTransition(UnityAction onComplete, Color color)
	{
		this.onComplete = onComplete;
		SetColor(color);
		rectPanel.gameObject.SetActive(true);
	}

	private void CompleteTransition()
	{
		rectPanel.gameObject.SetActive(false);
		onComplete?.Invoke();
	}

	/// <summary>
	/// 작동중인 모든 Transition에 대해 강제 종료
	/// </summary>
	public void ForceStopAll()
	{
		// FadeIn, FadeOut, FadeInOut Animation STOP
		rectPanel.StopAnimation();
		rectPanel.gameObject.SetActive(false);
	}

	#region Kind of transition
	/// <summary>
	/// Fade Out (default color : BLACK)
	/// </summary>
	public void StartFadeOut(UnityAction onComplete, Color color = default)
	{
		StartFadeOut(onComplete, DefaultTime, color);
	}

	public void StartFadeOut(UnityAction onComplete, float duration, Color color = default)
	{
		PrepareTransition(onComplete, color);

		IsPlaying = true;
		IsPlayingFadeOut = true;
		Broadcast(EVENT_ID.START_FADE_OUT);

		rectPanel.TimeFadeIn = duration;
		rectPanel.PlayAnimation(GuiAnimation.ANIMATION_TYPE.FADE_IN, () => 
		{
			IsPlayingFadeOut = false;
			Broadcast(EVENT_ID.FINISH_FADE_OUT);

			onComplete?.Invoke();
		});
	}

	/// <summary>
	/// Fade In (default color : BLACK)
	/// </summary>
	public void StartFadeIn(UnityAction onComplete, Color color = default)
	{
		StartFadeIn(onComplete, DefaultTime, color);
	}

	public void StartFadeIn(UnityAction onComplete, float duration, Color color = default)
	{
		PrepareTransition(onComplete, color);

		IsPlaying = true;
		IsPlayingFadeIn = true;
		Broadcast(EVENT_ID.START_FADE_IN);

		rectPanel.TimeFadeOut = duration;
		rectPanel.PlayAnimation(GuiAnimation.ANIMATION_TYPE.FADE_OUT, () => 
		{
			IsPlaying = false;
			IsPlayingFadeIn = false;
			Broadcast(EVENT_ID.FINISH_FADE_IN);

			CompleteTransition();
		});
	}

	/// <summary>
	/// Fade OutIn (default color : BLACK)
	/// </summary>
	public void StartFadeOutIn(UnityAction onComplete, Color color = default)
	{
		StartFadeOutIn(onComplete, DefaultTime * 2, color);
	}

	public void StartFadeOutIn(UnityAction onComplete, float duration, Color color = default)
	{
		PrepareTransition(onComplete, color);

		IsPlaying = true;
		IsPlayingFadeOutIn = true;
		Broadcast(EVENT_ID.START_FADE_OUTIN);

		rectPanel.TimeFadeIn = rectPanel.TimeFadeOut = duration * 0.5f;
		rectPanel.PlayAnimation(GuiAnimation.ANIMATION_TYPE.FADE_IN, () =>
		{
			rectPanel.PlayAnimation(GuiAnimation.ANIMATION_TYPE.FADE_OUT, () =>
			{
				IsPlaying = false;
				IsPlayingFadeOutIn = false;
				Broadcast(EVENT_ID.FINISH_FADE_OUTIN);

				CompleteTransition();
			});
		});
	}
	#endregion
}
