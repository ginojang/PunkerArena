using UnityEngine;
using UnityEditor;
using Malee.Editor;

namespace Devil.Editor
{
	public class PoolControllerEditor : UnityEditor.Editor
	{
		public int count;
		private ReorderableList list;

		private void OnEnable()
		{
			list = new ReorderableList(serializedObject.FindProperty("list"));
		}

		private bool Contains(string addressableID)
		{
			int i, len = list.List.arraySize;
			for (i = 0; i < len; i++)
			{
				SerializedProperty element = list.List.GetArrayElementAtIndex(i);
				if (element.FindPropertyRelative("addressableKey").stringValue == addressableID)
				{
					return true;
				}
			}

			return false;
		}

		private void OnDragContents(UnityEngine.Object dragged_object)
		{
			string curID = UnityEditor.AssetDatabase.GetAssetPath(dragged_object); // [Addressables 제거] 매니페스트가 풀패스 키를 보유
			if (string.IsNullOrEmpty(curID))
			{
				Debug.LogWarning($"{curID} is not an addressable object. Register it as addressable first.");
				return;
			}

			if (Contains(curID) == false)
			{
				SerializedProperty theLast = list.AddItem();
				theLast.FindPropertyRelative("addressableKey").stringValue = curID;
				theLast.FindPropertyRelative("count").intValue = count;
			}
			else
			{
				Debug.LogWarning($"The addressable object({curID}) already exists.");
			}
		}

		public void DropAreaGUI()
		{
			Event evt = Event.current;
			Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));

			GUIStyle style = new GUIStyle(EditorStyles.helpBox);
			// Position the Text in the center of the Box
			style.alignment = TextAnchor.MiddleCenter;
			style.fontSize = 10;
			style.fontStyle = FontStyle.Bold;
			GUI.Box(drop_area, "Drag & Drop addressable files", style);

			switch (evt.type)
			{
				case EventType.DragUpdated:
				case EventType.DragPerform:
					if (!drop_area.Contains(evt.mousePosition))
						return;

					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

					if (evt.type == EventType.DragPerform)
					{
						DragAndDrop.AcceptDrag();

						foreach (UnityEngine.Object dragged_object in DragAndDrop.objectReferences)
						{
							// Do On Drag Stuff here
							OnDragContents(dragged_object);
						}
					}
					break;
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			{				
				// Default count.
				count = EditorGUILayout.IntSlider("Pool count (Default)", count, 1, 500, GUILayout.ExpandWidth(true));

				// Drag & Drop stuff.
				DropAreaGUI();

				//base.OnInspectorGUI();

				// Draw the list using GUILayout, you can of course specify your own position and label
				list.DoLayoutList();
			}
			serializedObject.ApplyModifiedProperties();
		}
	}
}