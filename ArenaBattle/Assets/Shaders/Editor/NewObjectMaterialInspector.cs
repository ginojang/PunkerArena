using System.Collections.Generic;
using Assets.Editor.CustomMaterialEditors;
using UnityEditor;
using UnityEngine;

public class NewObjectMaterialInspector : CustomMaterialInspector
{
    private const string Shininess = "_Shininess";
    private const string SpecularIntensity = "_SpecularIntensity";
    private const string SpecularMaterialColor = "_SpecularMaterialColor";

    private const string FlowTex = "_FlowTex";
    private const string FlowSpeed = "_FlowSpeed";
    private const string FlowIntensity = "_FlowIntensity";

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

        if (SpecularOnOffKeywordProcess.IsOn(targetMat) == false && FlowKeywordProcess.IsOn(targetMat) == false)
        {
            if (propertyName == MaskTex) return true;
        }

        return false;
    }

    protected override void ProcessDefineKeywords(Material targetMat)
    {
        CutOffKeywordProcess.Process(targetMat);
        SpecularOnOffKeywordProcess.Process(targetMat);
        FlowKeywordProcess.Process(targetMat);
        if (FlowKeywordProcess.IsOn(targetMat)) FlowModeKeywordProcess.Process(targetMat);
        LightQualityKeywordProcess.Process(targetMat);
        FogKeywordProcess.Process(targetMat);
    }
}

//public class CreatureFlexibleMaterialInspector : MaterialEditor
//{
//    public override void OnInspectorGUI()
//    {
//        // render the default inspector
//        base.OnInspectorGUI();

//        // if we are not visible... return
//        if (!isVisible)
//            return;

//        EditorGUILayout.LabelField("                    ");

//        Material targetMat = target as Material;
//        SpecularOnOffKeywordProcess.Process(targetMat);
//        FlowKeywordProcess.Process(targetMat);
//        FlowModeKeywordProcess.Process(targetMat);
//        LightQualityKeywordProcess.Process(targetMat);
//    }
//}