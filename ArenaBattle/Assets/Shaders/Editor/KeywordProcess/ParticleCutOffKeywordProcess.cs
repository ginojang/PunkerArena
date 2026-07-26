using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.CustomMaterialEditors
{
    public static class ParticleCutOffKeywordProcess
    {
        private static readonly string[] cutOffDropDownContents = { "Off", "CutOff With Alpha", "CutOff Only" };
        private static readonly string[] cutOffDefineContents = { "NO_CUTOFF", "CUTOFF_WITH_ALPHA", "CUTOFF_ONLY" };

        public static void Process(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);
            var cutOffTypeIndex = GetSCutOffTypeIndex(keyWords);

            ClearCutOffKeywords(keyWords);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("CutOff");
            cutOffTypeIndex = EditorGUILayout.Popup(cutOffTypeIndex, cutOffDropDownContents);

            EditorGUILayout.EndHorizontal();

            //off는 추가안함
            for (int i = 1; i < cutOffDefineContents.Length; ++i)
            {
                if (cutOffTypeIndex == i)
                {
                    keyWords.Add(cutOffDefineContents[i]);
                    break;
                }
            }

            material.shaderKeywords = keyWords.ToArray();
            EditorUtility.SetDirty(material);
        }

        private static int GetSCutOffTypeIndex(List<string> keyWords)
        {
            for (int i = 0; i < cutOffDefineContents.Length; ++i)
            {
                if (keyWords.Contains(cutOffDefineContents[i]))
                {
                    return i;
                }
            }
            return 0;
        }

        private static void ClearCutOffKeywords(List<string> keyWords)
        {
            for (int i = 0; i < cutOffDefineContents.Length; ++i)
            {
                keyWords.Remove(cutOffDefineContents[i]);
            }
        }

        public static bool IsEnable(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);
            return keyWords.Contains(cutOffDefineContents[1]) || keyWords.Contains(cutOffDefineContents[2]);
        }
    }
}