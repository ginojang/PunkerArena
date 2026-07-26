using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.CustomMaterialEditors
{
    public static class SpecularKeywordProcess
    {
        private static readonly string[] specularDropDownContents = {"Off", "On", "On WithMask"};
        private static readonly string[] specularDefineContents = {"SPECULAR_OFF", "SPECULAR_ON", "SPECULAR_MASK_ON"};

        public static void Process(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);

            var specularTypeIndex = GetSpecularTypeIndex(keyWords);

            ClearSpecularKeywords(keyWords);

            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField("Specular");
            specularTypeIndex = EditorGUILayout.Popup(specularTypeIndex, specularDropDownContents);

            EditorGUILayout.EndHorizontal();

            //off는 추가안함
            for (int i = 1; i < specularDropDownContents.Length; ++i)
            {
                if (specularTypeIndex == i)
                {
                    keyWords.Add(specularDefineContents[i]);
                    break;
                }
            }

            material.shaderKeywords = keyWords.ToArray();
            EditorUtility.SetDirty(material);
        }

        private static int GetSpecularTypeIndex(List<string> keyWords)
        {
            for (int i = 0; i < specularDefineContents.Length; ++i)
            {
                if (keyWords.Contains(specularDefineContents[i]))
                {
                    return i;
                }
            }
            return 0;
        }

        private static void ClearSpecularKeywords(List<string> keyWords)
        {
            for (int i = 0; i < specularDefineContents.Length; ++i)
            {
                keyWords.Remove(specularDefineContents[i]);
            }
        }

        public static bool IsEnable(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);
            return keyWords.Contains(specularDefineContents[1]) || keyWords.Contains(specularDefineContents[2]);
        }

        public static bool IsMaskOn(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);
            return keyWords.Contains(specularDefineContents[2]);
        }
    }
}