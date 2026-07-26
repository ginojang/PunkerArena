using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.CustomMaterialEditors
{
    public static class LightMapOnlyKeywordProcess
    {
        private const string KEYWORD_ON = "LIGHTMAP_ONLY_ON";
        private const string KEYWORD_OFF = "LIGHTMAP_ONLY_OFF";

        public static void Process(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);

            var lightmapOnly = keyWords.Contains(KEYWORD_ON);
            keyWords.Remove(KEYWORD_ON);
            keyWords.Remove(KEYWORD_OFF);

            EditorGUI.BeginChangeCheck();
            lightmapOnly = EditorGUILayout.Toggle("Lightmap Only", lightmapOnly);
            if (EditorGUI.EndChangeCheck())
            {
                if (lightmapOnly) keyWords.Add(KEYWORD_ON);
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