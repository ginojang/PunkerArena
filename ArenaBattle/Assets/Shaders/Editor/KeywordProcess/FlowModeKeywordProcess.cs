using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.CustomMaterialEditors
{
    public static class FlowModeKeywordProcess
    {
        private static readonly string[] flowModeDropDownContents = {"Add", "Overlay", "Multiply"};
        private static readonly string[] flowModeDefineContents = { "FLOW_MODE_ADD", "FLOW_MODE_OVERLAY", "FLOW_MODE_MULTIPLY" };

        public static void Process(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);

            var flowModeIndex = GetFlowModeIndex(keyWords);

            ClearFlowModeKeywords(keyWords);

            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField("Flow Mode");
            flowModeIndex = EditorGUILayout.Popup(flowModeIndex, flowModeDropDownContents);

            EditorGUILayout.EndHorizontal();

            for (int i = 1; i < flowModeDropDownContents.Length; ++i)
            {
                if (flowModeIndex == i)
                {
                    keyWords.Add(flowModeDefineContents[i]);
                    break;
                }
            }

            material.shaderKeywords = keyWords.ToArray();
            EditorUtility.SetDirty(material);
        }

        private static int GetFlowModeIndex(List<string> keyWords)
        {
            for (int i = 0; i < flowModeDefineContents.Length; ++i)
            {
                if (keyWords.Contains(flowModeDefineContents[i]))
                {
                    return i;
                }
            }
            return 0;
        }

        private static void ClearFlowModeKeywords(List<string> keyWords)
        {
            for (int i = 0; i < flowModeDefineContents.Length; ++i)
            {
                keyWords.Remove(flowModeDefineContents[i]);
            }
        }
    }
}