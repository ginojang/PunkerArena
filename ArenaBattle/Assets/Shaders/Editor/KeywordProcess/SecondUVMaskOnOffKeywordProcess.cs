using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.CustomMaterialEditors
{
    public static class SecondUVMaskOnOffKeywordProcess
    {
        public static string KEYWORD_ON = "SECOND_UV_MASK_ON";
        public static string KEYWORD_OFF = "SECOND_UV_MASK_OFF";

        public static void Process(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);

            var useSecondUvMaskKeyword = keyWords.Contains(KEYWORD_ON);
            keyWords.Remove(KEYWORD_OFF);
            keyWords.Remove(KEYWORD_ON);

            EditorGUI.BeginChangeCheck();
            useSecondUvMaskKeyword = EditorGUILayout.Toggle("Use Second UV Mask", useSecondUvMaskKeyword);
            if (EditorGUI.EndChangeCheck())
            {
                if (useSecondUvMaskKeyword) keyWords.Add(KEYWORD_ON);
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