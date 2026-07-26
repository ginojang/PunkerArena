using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.CustomMaterialEditors
{
    public static class FlowKeywordProcess
    {
        public static string KEYWORD_ON = "FLOW_TEX_ON";
        public static string KEYWORD_OFF = "FLOW_TEX_OFF";

        public static void Process(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);

            var flowOn = keyWords.Contains(KEYWORD_ON);
            keyWords.Remove(KEYWORD_ON);
            keyWords.Remove(KEYWORD_OFF);

            EditorGUI.BeginChangeCheck();
            flowOn = EditorGUILayout.Toggle("Flow", flowOn);
            if (EditorGUI.EndChangeCheck())
            {
                if (flowOn) keyWords.Add(KEYWORD_ON);
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