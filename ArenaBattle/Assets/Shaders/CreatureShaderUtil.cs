using System.Collections.Generic;
using UnityEngine;

public static class CreatureShaderUtil
{
    public enum LightQuality
    {
        OnlyVertexLight,
        OnePixelLight,
        ManyPixelLight,
    }

    public static readonly string[] LightQualityDefine =
    {
        "ONLY_VERTEXLIGHT", "ONE_PIXEL_LIGHT", "MANY_PIXELLIGHT"
    };

    public static readonly string[] LightQualityContents =
    {
        "Only Vertex Light", "One Pixel Light", "Many Pixel Light"
    };

    private static readonly Shader CreatureFlexibleShader = Shader.Find("RPG/Creature Flexible");

    private static readonly string ManyPixelLightDefine = LightQualityDefine[(int) LightQuality.ManyPixelLight];
    private static readonly string OnePixelLightDefine = LightQualityDefine[(int)LightQuality.OnePixelLight];
    private static readonly string OnlyVertexLightDefine = LightQualityDefine[(int)LightQuality.OnlyVertexLight];

    private static readonly string ManyPixelLightBackupDefine = ManyPixelLightDefine + "_b";
    private static readonly string OnePixelLightBackupDefine = OnePixelLightDefine + "_b";
    private static readonly string OnlyVertexLightBackupDefine = OnlyVertexLightDefine + "_b";

    private static bool IsCreatureFlexibleShader(Shader shader)
    {
        return ReferenceEquals(shader, CreatureFlexibleShader);
    }

    private static void BackupLightQuality(Material material)
    {
        if (IsCreatureFlexibleShader(material.shader) == false) return;

        var keywords = new List<string>(material.shaderKeywords);
        if (keywords.Contains(ManyPixelLightBackupDefine) ||
            keywords.Contains(OnePixelLightBackupDefine) ||
            keywords.Contains(OnlyVertexLightBackupDefine)) return;

        if (keywords.Contains(ManyPixelLightDefine))
        {
            material.EnableKeyword(ManyPixelLightBackupDefine);
        }
        else if (keywords.Contains(OnePixelLightDefine))
        {
            material.EnableKeyword(OnePixelLightBackupDefine);
        }
        else
        {
            material.EnableKeyword(OnlyVertexLightBackupDefine);
        }
    }

    public static void ChangeLimitLightQuality(GameObject gameObject, LightQuality limitQuality)
    {
        var renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                if (IsCreatureFlexibleShader(material.shader) == false) continue;
                BackupLightQuality(material);

                var keywords = new List<string>(material.shaderKeywords);

                switch (limitQuality)
                {
                    case LightQuality.ManyPixelLight:
                        if (keywords.Contains(ManyPixelLightBackupDefine))
                        {
                            EnableManyPixelLightKeyword(material);
                        }
                        else if (keywords.Contains(OnePixelLightBackupDefine))
                        {
                            EnableOnePixelLightKeyword(material);
                        }
                        else
                        {
                            EnableOnlyVertexLightKeyword(material);
                        }
                        break;
                    case LightQuality.OnePixelLight:
                        if (keywords.Contains(ManyPixelLightBackupDefine)||
                            keywords.Contains(OnePixelLightBackupDefine))
                        {
                            EnableOnePixelLightKeyword(material);
                        }
                        else
                        {
                            EnableOnlyVertexLightKeyword(material);
                        }
                        break;
                    case LightQuality.OnlyVertexLight:
                        EnableOnlyVertexLightKeyword(material);
                        break;
                }
            }
        }
    }
    
    public static void ForceChangeLightQuality(GameObject gameObject, LightQuality quality)
    {
        var renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                if (IsCreatureFlexibleShader(material.shader) == false) continue;
                BackupLightQuality(material);

                switch (quality)
                {
                    case LightQuality.ManyPixelLight:
                        EnableManyPixelLightKeyword(material);
                        break;
                    case LightQuality.OnePixelLight:
                        EnableOnePixelLightKeyword(material);
                        break;
                    case LightQuality.OnlyVertexLight:
                        EnableOnlyVertexLightKeyword(material);
                        break;
                }
            }
        }
    }

    private static void EnableManyPixelLightKeyword(Material material)
    {
        material.EnableKeyword(ManyPixelLightDefine);
        material.DisableKeyword(OnePixelLightDefine);
        material.DisableKeyword(OnlyVertexLightDefine);
    }

    private static void EnableOnePixelLightKeyword(Material material)
    {
        material.DisableKeyword(ManyPixelLightDefine);
        material.EnableKeyword(OnePixelLightDefine);
        material.DisableKeyword(OnlyVertexLightDefine);
    }

    private static void EnableOnlyVertexLightKeyword(Material material)
    {
        material.DisableKeyword(ManyPixelLightDefine);
        material.DisableKeyword(OnePixelLightDefine);
        material.EnableKeyword(OnlyVertexLightDefine);
    }
}