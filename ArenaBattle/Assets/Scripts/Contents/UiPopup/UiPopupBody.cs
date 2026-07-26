using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Devil.Gui;

namespace Devil.Gui
{
	public class UiPopupBody : MonoBehaviour
	{
		private string id;

		[SerializeField] private Text message = null;
		[SerializeField] private LiteButton btnYes = null;
		[SerializeField] private LiteButton btnNo = null;
		[SerializeField] private LiteButton btnClose = null;
		[SerializeField] private Image[] itemImages = null;

		// Callback
		private UnityAction yesCallback;
		private UnityAction noCallback;
		private UnityAction closeCallback;

		private GuiAnimation guiAnimation;

		void Awake ()
		{
			// LiteButton에 Listener 등록
			btnYes.onClick.AddListener(OnClickYes);
			btnNo.onClick.AddListener(OnClickNo);
			if (btnClose != null) btnClose.onClick.AddListener(OnClickClose);

			// GuiAnimation 추가
			guiAnimation = GetComponent<GuiAnimation>();
		}

		private void OnClickYes ()
		{
			if (yesCallback != null)
			{
				yesCallback.Invoke();
				return;
			}

			// yesCallback이 Null이면, EVENT로 대체
			UiPopup.Instance.SendEventClickYes(id);
		}

		private void OnClickNo ()
		{
			if (noCallback != null)
			{
				noCallback.Invoke();
				return;
			}

			// noCallback이 Null이면, EVENT로 대체
			UiPopup.Instance.SendEventClickNo(id);
		}

		private void OnClickClose()
		{
			UiPopup.Instance?.ClosePopup(id);

			if (closeCallback != null)
			{
				closeCallback.Invoke();
				return;
			}

			// closeCallback이 Null이면, EVENT로 대체
			UiPopup.Instance?.SendEventClickClose(id);
		}

		private void EnableButtons(bool flag)
		{
			btnYes.enabled = flag;
			btnNo.enabled = flag;
			if (btnClose != null) btnClose.enabled = flag;
		}

		/// <summary>
		/// Popup Body Data 셋팅
		/// </summary>
		/// <param name="_id"></param>
		/// <param name="_message"></param>
		/// <param name="_yesLabel"></param>
		/// <param name="_yesCallback"></param>
		/// <param name="_noLabel"></param>
		/// <param name="_noCallback"></param>
		public void SetPopupBody (string _id, string _message, string _yesLabel, UnityAction _yesCallback, string _noLabel, UnityAction _noCallback, UnityAction closeCallback, bool useCloseButton)
		{
			id = _id;

			// about message
			message.text = _message;

			// about buttons
			btnYes.Label.text = _yesLabel;
			btnNo.Label.text = _noLabel;

			yesCallback = _yesCallback;
			noCallback = _noCallback;
			this.closeCallback = closeCallback;

			// label이 null인 경우 || label이 empty인 경우, 해당버튼은 Deactive 시킴
			btnYes.gameObject.SetActive(!string.IsNullOrEmpty(_yesLabel));
			btnNo.gameObject.SetActive(!string.IsNullOrEmpty(_noLabel));
			if (btnClose != null) btnClose.gameObject.SetActive(useCloseButton);

			if (guiAnimation == null)
			{
				EnableButtons(true);
			}
			else
			{
				EnableButtons(false);
				guiAnimation.PlayAnimation(() => { EnableButtons(true); });
			}
		}

		public void SetItemImage(string itemName)
		{
			foreach(Image image in itemImages)
			{
				if (image.name.Equals(itemName))
					image.gameObject.SetActive(true);
				else
					image.gameObject.SetActive(false);
			}
		}

		public bool IsUsing ()
		{
			return gameObject.activeSelf;
		}

		public void Use ()
		{
			gameObject.SetActive(true);
		}

		public void Unuse ()
		{
			gameObject.SetActive(false);
		}
	}
}
