using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.CustomMaterialEditors
{
    public static class SubTexOpKeywordProcess
    {
        private static readonly string[] subTexOpDropDownContents = { "Add", "Multiply" };
        private static readonly string[] subTexOpDefineContents = { "SUB_TEX_ADD", "SUB_TEX_MUL" };

        public static void Process(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);

            var subTexOpIndex = GetSubTexOpIndex(keyWords);

            ClearSubTexOpKeywords(keyWords);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Sub Tex Operation");
            subTexOpIndex = EditorGUILayout.Popup(subTexOpIndex, subTexOpDropDownContents);

            EditorGUILayout.EndHorizontal();

            for (int i = 1; i < subTexOpDropDownContents.Length; ++i)
            {
                if (subTexOpIndex == i)
                {
                    keyWords.Add(subTexOpDefineContents[i]);
                    break;
                }
            }

            material.shaderKeywords = keyWords.ToArray();
            EditorUtility.SetDirty(material);
        }

        private static int GetSubTexOpIndex(List<string> keyWords)
        {
            for (int i = 0; i < subTexOpDefineContents.Length; ++i)
            {
                if (keyWords.Contains(subTexOpDefineContents[i]))
                {
                    return i;
                }
            }
            return 0;
        }

        private static void ClearSubTexOpKeywords(List<string> keyWords)
        {
            for (int i = 0; i < subTexOpDefineContents.Length; ++i)
            {
                keyWords.Remove(subTexOpDefineContents[i]);
            }
        }
    }
}