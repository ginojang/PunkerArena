using System.Collections.Generic;
using Assets.Editor.CustomMaterialEditors;
using UnityEditor;
using UnityEngine;

public class ObjectMaterialInspector : CustomMaterialInspector
{
    private const string Shininess = "_Shininess";
    private const string SpecularIntensity = "_SpecularIntensity";
    private const string SpecularMaterialColor = "_SpecularMaterialColor";

    private const string CutOffRange = "_CutOffRange";

    private const string MaskTex = "_MaskTex";

    protected override bool HideThisProperty(Material targetMat, string propertyName)
    {
        if (SpecularKeywordProcess.IsEnable(targetMat) == false)
        {
            if (propertyName == Shininess ||
                propertyName == SpecularIntensity ||
                propertyName == SpecularMaterialColor) return true;
        }

        if (SpecularKeywordProcess.IsMaskOn(targetMat) == false)
        {
            if (propertyName == MaskTex) return true;
        }

        if (CutOffKeywordProcess.IsOn(targetMat) == false)
        {
            if (propertyName == CutOffRange) return true;
        }

        return false;
    }

    protected override void ProcessDefineKeywords(Material targetMat)
    {
        CutOffKeywordProcess.Process(targetMat);
        LightMapOnlyKeywordProcess.Process(targetMat);
        SpecularKeywordProcess.Process(targetMat);
    }
}