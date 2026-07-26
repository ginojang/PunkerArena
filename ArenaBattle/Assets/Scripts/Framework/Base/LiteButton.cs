using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devil.Gui
{
	/// <summary>
	/// uGui의 Button 기능 확장
	/// 
	/// 1. Button Sound Play
	/// 2. onClick 에 대한 RemoveEventAll 기능
	/// 3. Child에 Text 컴포넌트 존재 시 접근 관리
	/// </summary>
	public class LiteButton : Button
	{
        /// <summary>
        /// Child 에 Text 컴포넌트 존재 시 링크
        /// </summary>
        [field: SerializeField]
        public Text Label { get; private set; } = null;
        private Color labelColorNormal;
		private Color labelColorDisable;
		private Outline labelOutline;

		/// <summary>
		/// 기본 링크 사운드, 필요한 경우 특정사운드로 변경 할 수 있다
		/// </summary>
        [field: SerializeField]
		public string SoundPath { get; set; } = "UIButtonClick";

        /// <summary>
        /// 사운드를 Play 할 수 있는지 여부 (버튼에 따라 사운드 플레이를 안하게 막을 수 있다)
        /// </summary>
        [field: SerializeField]
        public bool CanPlaySound { get; set; } = true;

        /// <summary>
        /// Button의 interactable 속성을 가려 추가 기능 설정
        /// 
        /// 1. Label의 색상값 변경
        /// </summary>
        public new bool interactable
		{
			get { return base.interactable; }
			set
			{
				if (Label != null)
				{
					//ColorUtility.TryParseHtmlString("#EEEEEE", out Color disableColor);
					//Label.color = value ? Color.white : disableColor;

					Label.color = value ? labelColorNormal : labelColorDisable;
					if (labelOutline) labelOutline.enabled = value;
				}

				base.interactable = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();

			onClick.AddListener(ClickHandler);
			Label = GetComponentInChildren<Text>();

			if (Label != null)
			{
				labelColorNormal = labelColorDisable = Label.color;
				labelColorDisable.a = 0.4f;
				labelOutline = Label.GetComponent<Outline>();
			}

			// 인스펙터의 interactable 설정값을, new 키워드로 가려진 LiteButton.interactable 에 적용
			interactable = base.interactable;
		}

		private void ClickHandler()
		{
			// UI Close 후에 Raycast 로 picking 먹는 현상을 막기 위해, 모든 Input Event를 삭제한다.
			// Input.ResetInputAxes();

			PlaySound();
		}

		private void PlaySound()
		{
			// 버튼 사운드 플레이
			if (!CanPlaySound) return;

			SoundManager.Instance.PlayInstanceSound($"UI_Menu_ButtonClick");
			//SoundManager.Instance.PlayInstanceSound($"UI_Menu_ButtonClick");
		}

		/// <summary>
		/// Button onClick에 등록된 모든 Listener 등록 해제 후 기본 사운드 플레이 이벤트 재등록
		/// </summary>
		public void RemoveAllListeners ()
		{
			onClick.RemoveAllListeners();
			onClick.AddListener(ClickHandler);
		}
	}
}
