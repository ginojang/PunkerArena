using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.CustomMaterialEditors
{
    public static class LightQualityKeywordProcess
    {
        public static void Process(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);

            var lightQualityIndex = GetLightQualityIndex(keyWords);

            ClearLightQualityKeywords(keyWords);

            // only vertex light
            lightQualityIndex = 0;

            //off는 추가안함
            for (int i = 1; i < CreatureShaderUtil.LightQualityDefine.Length; ++i)
            {
                if (lightQualityIndex == i)
                {
                    keyWords.Add(CreatureShaderUtil.LightQualityDefine[i]);
                    break;
                }
            }

            material.shaderKeywords = keyWords.ToArray();
            EditorUtility.SetDirty(material);
        }

        private static int GetLightQualityIndex(List<string> keyWords)
        {
            for (int i = 0; i < CreatureShaderUtil.LightQualityDefine.Length; ++i)
            {
                if (keyWords.Contains(CreatureShaderUtil.LightQualityDefine[i]))
                {
                    return i;
                }
            }
            return 0;
        }

        private static void ClearLightQualityKeywords(List<string> keyWords)
        {
            for (int i = 0; i < CreatureShaderUtil.LightQualityDefine.Length; ++i)
            {
                keyWords.Remove(CreatureShaderUtil.LightQualityDefine[i]);
            }
        }

        public static bool IsPixelLighting(Material material)
        {
            var keyWords = new List<string>(material.shaderKeywords);
            return keyWords.Contains(CreatureShaderUtil.LightQualityDefine[1]) ||
                   keyWords.Contains(CreatureShaderUtil.LightQualityDefine[2]);
        }
    }
}