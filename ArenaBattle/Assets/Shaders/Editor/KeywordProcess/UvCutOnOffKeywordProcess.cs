using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.CustomMaterialEditors
{
    public static class UvCutOnOffKeywordProcess
    {
        public static string KEYWORD_ON = "UVCUT_ON";
        public static string KEYWORD_OFF = "UVCUT_OFF";

        public static void Process(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);

            var uvCutOn = keyWords.Contains(KEYWORD_ON);
            keyWords.Remove(KEYWORD_OFF);
            keyWords.Remove(KEYWORD_ON);

            EditorGUI.BeginChangeCheck();
            uvCutOn = EditorGUILayout.Toggle("UV Cut", uvCutOn);
            if (EditorGUI.EndChangeCheck())
            {
                if (uvCutOn) keyWords.Add(KEYWORD_ON);
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