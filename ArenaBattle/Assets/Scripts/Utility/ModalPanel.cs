using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Devil.Gui;

namespace Devil.Gui
{
	[RequireComponent(typeof(LiteButton))]
	public class ModalPanel : MonoBehaviour
	{
		/// <summary>
		/// Modal의 Open history 정보 (Stack 으로 관리)
		/// </summary>
		private class ModalInfo
		{
			public Transform TargetWindow { get; private set; } = null;
			public UnityAction CancelEvent { get; private set; } = null;
			public bool CanClickCancel { get; private set; }

			public ModalInfo (Transform _targetWindow, UnityAction _cancelEvent, bool _canClickCancel)
			{
				TargetWindow = _targetWindow;
				CancelEvent = _cancelEvent;
				CanClickCancel = _canClickCancel;
			}
		}

		#region Managing modal (static)

		public static bool IsOpen { get; private set; } = false;

		private static Stack<ModalInfo> stackInfos = new Stack<ModalInfo>();
		private static ModalPanel panel;
		private static UnityAction cancelEvent;
		private static bool canClickCancel;

		/// <summary>
		/// panel의 상태 리셋
		/// </summary>
		private static void Reset ()
		{
			cancelEvent = null;
			canClickCancel = false;
			IsOpen = false;

			if (panel == null) return;

			panel.transform.SetParent(null);
			panel.transform.SetSiblingIndex(0);
			panel.gameObject.SetActive(false);
		}

		/// <summary>
		/// Modal Panel 을 오픈
		/// </summary>
		/// <param name="_targetWindow">panel을 붙히고자 하는 window gameObject</param>
		/// <param name="_cancelEvent">modal panel 영역 클릭 시 OR Escape(뒤로가기) 입력 시 발생할 cancel Event</param>
		/// <param name="_canClickCancel">modal panel 영역 클릭 가능 여부</param>
		public static void Open(Transform _targetWindow, UnityAction _cancelEvent, bool _canClickCancel = true, bool targetIsParent = false)
		{
			if (_targetWindow == null || (stackInfos.Count > 0 && _targetWindow == stackInfos.Peek().TargetWindow)) return;

			// panel instance 가 생성되어있지 않을 때, panel instance 생성
			GameObject instance = panel?.gameObject;
			if (instance == null)
			{
				var resource = Resources.Load<GameObject>("GUI/Framework/ModalPanel");
				instance = Instantiate(resource);
				instance.name = "ModalPanel";
			}

			Reset();

            var targetWindow = _targetWindow;            
			
			// 붙혀야할 정보 알아옴
			var parent = targetWindow.parent;
            if (targetIsParent)
                parent = targetWindow;
			var sibling = targetWindow.GetSiblingIndex();

			instance.transform.SetParent(parent, false);
			instance.transform.SetSiblingIndex(sibling);
			instance.SetActive(true);

			// Canvas's render mode 대응
			var rectTransform = instance.GetComponent<RectTransform>();
			rectTransform.anchoredPosition3D = Vector3.zero;			
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			rectTransform.localScale = Vector3.one;
			rectTransform.sizeDelta = new Vector2((float)Screen.width * 2, (float)Screen.height * 2);

			cancelEvent = _cancelEvent;
			canClickCancel = _canClickCancel;
			IsOpen = true;

			// CancelArea LiteButton Sound Set
			panel.CancelArea.CanPlaySound = canClickCancel == true && cancelEvent != null;

			// Stack에 Push
			stackInfos.Push(new ModalInfo(targetWindow, _cancelEvent, _canClickCancel));
		}

		/// <summary>
		/// ModalPanel Close
		/// </summary>
		/// <param name="targetWindow">targetWindow 지정으로, 마지막 Open Panel에 대한 유효성 검사</param>
		public static void Close(Transform targetWindow = null)
		{
			// 이미 모두 닫힌 상태면 더이상 작동 안 함
			if (stackInfos.Count < 1) return;
			if (targetWindow != null && targetWindow != stackInfos.Peek().TargetWindow) return;

			// 현재 modal을 stack에서 pop함 (현재 Modal을 닫았다)
			stackInfos.Pop();

			if (stackInfos.Count < 1)
			{
				Reset();
				return;
			}

			// 중첩 modal 이 있는 경우, Open
			var info = stackInfos.Pop();
			Open(info.TargetWindow, info.CancelEvent, info.CanClickCancel);
		}

		/// <summary>
		/// Escape(뒤로가기) 입력 시 ModalPanel.IsOpen 검사 후 오픈되어있으면, ModalPanel.Escape() 호출
		/// </summary>
		public static void Escape ()
		{
			if (cancelEvent == null) return;

			cancelEvent.Invoke();
		}

		/// <summary>
		/// Modal Panel 관련 Data 정리, 보통은 Unity Scene 이동 시 Modal Panel 이 Destroy 되면서 자동 호출 되지만, 필요 시 어디서든 호출하여 Modal Panel Data 를 초기화 시킬 수 있다.
		/// </summary>
		public static void CleanUp ()
		{
			//Debug.Log("CleanUp");
			Reset();
			stackInfos.Clear();
		}

		#endregion


		#region Instance modal

		public LiteButton CancelArea { get; private set; }

		void Awake()
		{
			// static에 현재 오픈된 panel 인스턴스를 연결
			panel = this;

			CancelArea = GetComponent<LiteButton>();

			// 영역 클릭 이벤트 등록
			CancelArea.onClick.AddListener(OnClickCancelArea);
		}

		void OnDestroy()
		{
			panel = null;
			CleanUp();
		}

		private void OnClickCancelArea()
		{
			// cancel event 도 존재하고, click 가능일 때 실행
			if (!canClickCancel || cancelEvent == null) return;

			cancelEvent.Invoke();
		}

		#endregion
	}
}
