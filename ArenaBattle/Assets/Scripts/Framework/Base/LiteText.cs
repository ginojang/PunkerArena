using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Devil.Gui
{
	/// <summary>
	/// uGui의 Text 기능 확장
	/// 
	/// 1. 동적으로 폰트 변경
	/// </summary>

	[DisallowMultipleComponent]
	[RequireComponent(typeof(Text))]
	public class LiteText : MonoBehaviour
	{
		private Text mainText;

		public Font font 
		{ 
			get { return mainText.font; }
			set { mainText.font = value; }
		}

		private void Awake()
		{
			mainText = GetComponent<Text>();

			//Debug.Log("LiteText >>>>> Awake >> " + this.gameObject);
//			GuiMain.Instance?.AddLocalizeText(this);
		}

		private void OnDestroy()
		{
			//Debug.Log("LiteText >>>>> OnDestroy >> " + this.gameObject);
//			GuiMain.Instance?.RemoveLocalizeText(this);
		}
	}
}
