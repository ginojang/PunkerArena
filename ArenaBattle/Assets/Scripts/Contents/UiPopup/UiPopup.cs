using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Devil.Gui
{
	public class UiPopup : UiBase<UiPopup>
	{
		public enum EVENT_ID : byte
		{
			CLICK_YES,
			CLICK_NO,
			CLICK_CLOSE,
		}

		// OPEN Popup 시, Instantiate 할 원본 Object
		[SerializeField] private GameObject popupSystemNotice = null;
		[SerializeField] private GameObject popupSystemCaution = null;
		[SerializeField] private GameObject popupDinoNotice = null;
		[SerializeField] private GameObject popupDinoCaution = null;

		#region about PoolManager
		// Pool 관리 (이미 오픈 되어있고, 사용하지 않는 같은 타입의 팝업이 있으면 Pool에서 사용함)
		public enum POPUP_TYPE { SYSTEM_NOTICE, SYSTEM_CAUTION, DINO_NOTICE, DINO_CAUTION }
		private class PopupPoolData
		{
			public POPUP_TYPE type;
			public UiPopupBody instance;
		}

		// Pool 관련 코드
		private PopupPoolData AddPoolData (PopupPoolData _popupData)
		{
			_popupData.instance.Unuse();
			popupPools.Add(_popupData);

			return _popupData;
		}

		private PopupPoolData GetPooledData (POPUP_TYPE _type)
		{
			for (int i = 0; i < popupPools.Count; i++)
			{
				if (popupPools[i].type == _type && !popupPools[i].instance.IsUsing())
				{
					popupPools[i].instance.Use();

					return popupPools[i];
				}
			}

			return null;
		}

		private PopupPoolData GetUsingPoolData()
		{
			for (int i = 0; i < popupPools.Count; i++)
			{
				if (popupPools[i].instance.IsUsing())
				{
					return popupPools[i];
				}
			}

			return null;
		}

		private void ReturnPooledData (PopupPoolData _popupData)
		{
			_popupData.instance.Unuse();
		}
		#endregion

		private List<PopupPoolData> popupPools = new List<PopupPoolData>();
		private Dictionary<string, PopupPoolData> popupInstances = new Dictionary<string, PopupPoolData>();
		private RectTransform __pool__;
		private RectTransform __instance__;

		protected override void Awake ()
		{
			base.Awake();

			// Pool Object를 관리할 Hidden Object 동적 생성
			__pool__ = new GameObject("__pool__").AddComponent<RectTransform>();
			__pool__.SetParent(transform, false);
			__pool__.sizeDelta = Vector2.zero;
			__pool__.anchorMax = Vector2.one;
			__pool__.anchorMin = Vector2.zero;
			__pool__.gameObject.SetActive(false);

			// Usable Popup Managing Object
			__instance__ = new GameObject("__instance__").AddComponent<RectTransform>();
			__instance__.SetParent(transform, false);
			__instance__.sizeDelta = Vector2.zero;
			__instance__.anchorMax = Vector2.one;
			__instance__.anchorMin = Vector2.zero;
		}

		protected override void Start ()
		{
			base.Start();

			// Popup Open TEST
			//StartCoroutine(ProcPopupTest());
		}

		//private IEnumerator ProcPopupTest ()
		//{
		//	yield return new WaitForSeconds(3f);
		//	OpenPopup("TEST_1", POPUP_TYPE.DINO_CAUTION, "캐릭터를 만들 때 선택한 성별과 이름은 다시 바꿀 수 없어.\n\n이 성별과 이름으로 선택할까?", "네", () => { ClosePopup("TEST_1"); }, "아니요", () => { ClosePopup("TEST_1"); });
		//	yield return new WaitForSeconds(3f);
		//	OpenPopup("TEST_2", POPUP_TYPE.SYSTEM_CAUTION, "입력하신 ID가 틀리거나 존재하지 않아 호두 잉글리시에 접속 할 수 없습니다.\n\n학습자 ID를 잃어버리셨다면 호두 잉글리시 홈페이지 로그인 후 [학습자 정보]에서 찾으실 수 있습니다.", "확인", () => { ClosePopup("TEST_2"); }, "아이디 찾기", () => { ClosePopup("TEST_2"); });
		//	yield return new WaitForSeconds(3f);
		//	OpenPopup("TEST_3", POPUP_TYPE.DINO_NOTICE, "이 성별과 이름으로 선택할까?", "네", () => { ClosePopup("TEST_3"); });
		//	yield return new WaitForSeconds(3f);
		//	OpenPopup("TEST_4", POPUP_TYPE.SYSTEM_NOTICE, "입력하신 ID가 틀리거나 존재하지 않아 호두 잉글리시에 접속 할 수 없습니다.", null, null, "아니요", () => { ClosePopup("TEST_4"); });
		//	yield return new WaitForSeconds(3f);
		//	OpenPopup("TEST_5", POPUP_TYPE.SYSTEM_NOTICE, "캐릭터를 만들 때 선택한 성별과 이름은 다시 바꿀 수 없어.\n\n이 성별과 이름으로 선택할까?", "네", () => { ClosePopup("TEST_5"); }, "아니요", () => { ClosePopup("TEST_5"); });
		//	yield return new WaitForSeconds(3f);
		//	OpenPopup("TEST_6", POPUP_TYPE.DINO_NOTICE, "입력하신 ID가 틀리거나 존재하지 않아 호두 잉글리시에 접속 할 수 없습니다.\n\n학습자 ID를 잃어버리셨다면 호두 잉글리시 홈페이지 로그인 후 [학습자 정보]에서 찾으실 수 있습니다.", "확인", () => { ClosePopup("TEST_6"); }, "아이디 찾기", () => { ClosePopup("TEST_6"); });
		//	yield return new WaitForSeconds(3f);
		//	OpenPopup("TEST_7", POPUP_TYPE.SYSTEM_CAUTION, "이 성별과 이름으로 선택할까?", "네", () => { ClosePopup("TEST_7"); });
		//	yield return new WaitForSeconds(3f);
		//	OpenPopup("TEST_8", POPUP_TYPE.DINO_CAUTION, "입력하신 ID가 틀리거나 존재하지 않아 호두 잉글리시에 접속 할 수 없습니다.", null, null, "아니요", () => { ClosePopup("TEST_8"); });
		//}

		#region APIs
		public void OpenPopup (string _id, POPUP_TYPE _type, string _message, string _yesLabel, UnityAction _yesCallback, string _noLabel = null, UnityAction _noCallback = null, UnityAction closeCallback = null, bool useCloseButton = false)
		{
			// 1. 같은 아이디 이미 존재 하는지 확인
			if (popupInstances.ContainsKey(_id))
			{
				Debug.Log("Exist popup instance id");
				return;
			}

			// 2. pools에 같은 타입의, 사용하지 않고 있는 Popup이 있으면 갖고 옴
			var popupData = GetPooledData(_type);
			
			// 3. 2번에 Popup이 없으면 새로 생성
			if (popupData == null)
			{
				var instance = GetInstance(_type).GetComponent<UiPopupBody>();
				instance.transform.SetParent(__pool__, false);

				AddPoolData(new PopupPoolData() { type = _type, instance = instance });
				popupData = GetPooledData(_type);
			}

			// 4. popupInstances에 담기
			popupData.instance.transform.SetParent(__instance__, false);
			popupInstances.Add(_id, popupData);

			// Modal Panel Open
			ModalPanel.Open(
				popupData.instance.transform,
				() =>
				{
					ClosePopup(_id);

					if (closeCallback != null)
					{
						closeCallback.Invoke();
						return;
					}

					SendEventClickClose(_id);
				},
				useCloseButton
			);

			// Set PopupBody data
			popupData.instance.SetPopupBody(_id, _message, _yesLabel, _yesCallback, _noLabel, _noCallback, closeCallback, useCloseButton);
		}

		public void ClosePopup (string _id)
		{
			// 1. 생성되어 있는 _id 인지 확인
			if (!popupInstances.ContainsKey(_id))
			{
				Debug.Log("Does not exist, instance id");
				return;
			}

			// 2. Popup을 닫고 Pool에 등록
			var popupData = popupInstances[_id];
			popupData.instance.transform.SetParent(__pool__, false);
			ReturnPooledData(popupData);

			// 3. popupInstances에서 지우기
			popupInstances.Remove(_id);

			// Modal Panel Close
			ModalPanel.Close();
		}

		public void CloseAllPopup()
		{
			// 1. 생성되어 있는 _id 인지 확인
			foreach(var popupData in popupInstances)
			{
				popupData.Value.instance.transform.SetParent(__pool__, false);
				ReturnPooledData(popupData.Value);
				// Modal Panel Close
				ModalPanel.Close();
			}
			// 3. popupInstances에서 지우기
			popupInstances.Clear();
		}
		#endregion

		#region Events
		public void SendEventClickYes (string _id)
		{
			Broadcast(EVENT_ID.CLICK_YES, _id);
		}

		public void SendEventClickNo (string _id)
		{
			Broadcast(EVENT_ID.CLICK_NO, _id);
		}

		public void SendEventClickClose (string _id)
		{
			Broadcast(EVENT_ID.CLICK_CLOSE, _id);
		}
		#endregion

		/// <summary>
		/// POPUP_TYPE에 따라, 생성되어야 할 팝업을 Instantiate 후 넘겨줌
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private GameObject GetInstance (POPUP_TYPE _type)
		{
			GameObject popupObject = null;

			switch (_type)
			{
				case POPUP_TYPE.SYSTEM_NOTICE:
					popupObject = popupSystemNotice;
					break;

				case POPUP_TYPE.SYSTEM_CAUTION:
					popupObject = popupSystemCaution;
					break;

				case POPUP_TYPE.DINO_NOTICE:
					popupObject = popupDinoNotice;
					break;

				case POPUP_TYPE.DINO_CAUTION:
					popupObject = popupDinoCaution;
					break;

			}

			return Instantiate(popupObject);
		}
	}
}
