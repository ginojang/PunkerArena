using System.Collections.Generic;
using Assets.Editor.CustomMaterialEditors;
using UnityEditor;
using UnityEngine;

public class CreatureFlexibleMaterialInspector : CustomMaterialInspector
{
    private const string Shininess = "_Shininess";
    private const string SpecularIntensity = "_SpecularIntensity";
    private const string SpecularMaterialColor = "_SpecularMaterialColor";

    private const string CutOffRange = "_CutOffRange";

    private const string FlowTex = "_FlowTex";
    private const string FlowSpeed = "_FlowSpeed";
    private const string FlowIntensity = "_FlowIntensity";

    private const string EmissiveColor = "_EmissiveColor";
    private const string EmissiveShininess = "_EmissiveShininess";
    private const string EmissiveIntensity = "_EmissiveIntensity";
    private const string EmissiveParameter = "_EmissiveParameter";

    private const string NormalTex = "_NormalTex";
    private const string MaskTex = "_MaskTex";

    protected override bool HideThisProperty(Material targetMat, string propertyName)
    {
        if (SpecularOnOffKeywordProcess.IsOn(targetMat) == false)
        {
            if (propertyName == Shininess ||
                propertyName == SpecularIntensity ||
                propertyName == SpecularMaterialColor) return true;
        }

        if (FlowKeywordProcess.IsOn(targetMat) == false)
        {
            if (propertyName == FlowTex || propertyName == FlowSpeed || propertyName == FlowIntensity) return true;
        }

        if (LightQualityKeywordProcess.IsPixelLighting(targetMat) == false)
        {
            if (propertyName == NormalTex) return true;
        }

        if (CutOffKeywordProcess.IsOn(targetMat) == false && SpecularOnOffKeywordProcess.IsOn(targetMat) == false && FlowKeywordProcess.IsOn(targetMat) == false && EmissiveKeywordProcess.IsOn(targetMat) == false)
        {
            if (propertyName == MaskTex) return true;
        }

        if (CutOffKeywordProcess.IsOn(targetMat) == false)
        {
            if (propertyName == CutOffRange) return true;
        }

        if (EmissiveKeywordProcess.IsOn(targetMat) == false)
        {
            if (propertyName == EmissiveColor ||
                propertyName == EmissiveShininess ||
                propertyName == EmissiveIntensity ||
                propertyName == EmissiveParameter) return true;
        }

        return false;
    }

    protected override void ProcessDefineKeywords(Material targetMat)
    {
        CutOffKeywordProcess.Process(targetMat);
        SpecularOnOffKeywordProcess.Process(targetMat);
        EmissiveKeywordProcess.Process(targetMat);
        FlowKeywordProcess.Process(targetMat);
        if (FlowKeywordProcess.IsOn(targetMat)) FlowModeKeywordProcess.Process(targetMat);

        var keyWords = new List<string>(targetMat.shaderKeywords);
        if (FlowKeywordProcess.IsOn(targetMat)) { keyWords.Remove(EmissiveKeywordProcess.KEYWORD_OFF); keyWords.Remove(EmissiveKeywordProcess.KEYWORD_ON); }
        if (EmissiveKeywordProcess.IsOn(targetMat)) { keyWords.Remove(FlowKeywordProcess.KEYWORD_OFF); keyWords.Remove(FlowKeywordProcess.KEYWORD_ON); }
        targetMat.shaderKeywords = keyWords.ToArray();

        LightQualityKeywordProcess.Process(targetMat);
    }
}