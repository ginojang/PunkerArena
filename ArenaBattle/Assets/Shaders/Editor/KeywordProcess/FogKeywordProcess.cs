using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.CustomMaterialEditors
{
    public static class FogKeywordProcess
    {
        private const string KEYWORD_ON = "FOG_ON";
        private const string KEYWORD_OFF = "FOG_OFF";

        public static void Process(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);

            var fogOff = keyWords.Contains(KEYWORD_OFF);
            keyWords.Remove(KEYWORD_ON);
            keyWords.Remove(KEYWORD_OFF);

            EditorGUI.BeginChangeCheck();
            fogOff = EditorGUILayout.Toggle("Fog Off", fogOff);
            if (EditorGUI.EndChangeCheck())
            {
                if (fogOff) keyWords.Add(KEYWORD_OFF);
                material.shaderKeywords = keyWords.ToArray();
                EditorUtility.SetDirty(material);
            }
        }

        public static bool IsOn(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);
            return keyWords.Contains(KEYWORD_OFF);
        }
    }
}
