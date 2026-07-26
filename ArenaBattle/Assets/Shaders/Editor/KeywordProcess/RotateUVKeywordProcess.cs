using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.CustomMaterialEditors
{
    public static class RotateUVKeywordProcess
    {
        public static string KEYWORD_ON = "ROTATE_UV_ON";
        public static string KEYWORD_OFF = "ROTATE_UV_OFF";

        public static void Process(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);

            var rotateUVOn = keyWords.Contains(KEYWORD_ON);
            keyWords.Remove(KEYWORD_ON);
            keyWords.Remove(KEYWORD_OFF);

            EditorGUI.BeginChangeCheck();
            rotateUVOn = EditorGUILayout.Toggle("Rotate UV", rotateUVOn);
            if (EditorGUI.EndChangeCheck())
            {
                if (rotateUVOn) keyWords.Add(KEYWORD_ON);
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