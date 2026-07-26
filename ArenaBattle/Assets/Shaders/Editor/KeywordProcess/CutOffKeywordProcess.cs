using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.CustomMaterialEditors
{
    public static class CutOffKeywordProcess
    {
        private const string KEYWORD_ON = "CUTOFF_ON";
        private const string KEYWORD_OFF = "CUTOFF_OFF";

        public static void Process(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);

            var cutoffOn = keyWords.Contains(KEYWORD_ON);
            keyWords.Remove(KEYWORD_ON);
            keyWords.Remove(KEYWORD_OFF);

            EditorGUI.BeginChangeCheck();
            cutoffOn = EditorGUILayout.Toggle("CutOff", cutoffOn);
            if (EditorGUI.EndChangeCheck())
            {
                if (cutoffOn) keyWords.Add(KEYWORD_ON);
                material.shaderKeywords = keyWords.ToArray();
                EditorUtility.SetDirty(material);
            }
        }

        public static bool IsOn(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);
            return keyWords.Contains(KEYWORD_ON);
        }
    }
}
