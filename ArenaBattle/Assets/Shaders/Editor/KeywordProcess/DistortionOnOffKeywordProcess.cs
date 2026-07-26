using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.CustomMaterialEditors
{
    public static class DistortionOnOffKeywordProcess
    {
        private const string KEYWORD_ON = "DISTORTION_ON";
        private const string KEYWORD_OFF = "DISTORTION_OFF";

        public static void Process(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);

            var distortionOn = keyWords.Contains(KEYWORD_ON);
            keyWords.Remove(KEYWORD_OFF);
            keyWords.Remove(KEYWORD_ON);

            EditorGUI.BeginChangeCheck();
            distortionOn = EditorGUILayout.Toggle("Distortion", distortionOn);
            if (EditorGUI.EndChangeCheck())
            {
                if (distortionOn) keyWords.Add(KEYWORD_ON);
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