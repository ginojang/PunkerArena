using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.CustomMaterialEditors
{
    public static class SpecularOnOffKeywordProcess
    {
        private const string KEYWORD_ON = "SPECULAR_ON";
        private const string KEYWORD_OFF = "SPECULAR_OFF";

        public static void Process(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);

            var specularOn = keyWords.Contains(KEYWORD_ON);
            keyWords.Remove(KEYWORD_ON);
            keyWords.Remove(KEYWORD_OFF);

            EditorGUI.BeginChangeCheck();
            specularOn = EditorGUILayout.Toggle("Specular", specularOn);
            if (EditorGUI.EndChangeCheck())
            {
                if (specularOn) keyWords.Add(KEYWORD_ON);
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