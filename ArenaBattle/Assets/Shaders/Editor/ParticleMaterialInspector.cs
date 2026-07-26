using System;
using System.Collections.Generic;
using Assets.Editor.CustomMaterialEditors;
using UnityEditor;
using UnityEngine;

public class ParticleMaterialInspector : CustomMaterialInspector
{
    private const string DistortionTex = "_DistortionTex";
    private const string DistortionRate = "_DistortionRate";
    private const string CutOffRange = "_CutOffRange";
    private const string SubTex = "_SubTex";
    private const string MaskTex = "_MaskTex";
    private const string RotateUV = "_RotateUVParameter";

    protected override bool HideThisProperty(Material targetMat, string propertyName)
    {
        if (DistortionOnOffKeywordProcess.IsOn(targetMat) == false)
        {
            if (propertyName == DistortionTex || propertyName == DistortionRate) return true;
        }

        if (ParticleCutOffKeywordProcess.IsEnable(targetMat) == false)
        {
            if (propertyName == CutOffRange) return true;
        }

        if (RotateUVKeywordProcess.IsOn(targetMat) == false)
        {
            if (propertyName == RotateUV) return true;
        }

        return false;
    }

    protected override void ProcessDefineKeywords(Material targetMat)
    {
        var queryResults = ShaderPropertyUtil.ExistPropertyName(targetMat.shader,
                                                                MaskTex,
                                                                DistortionTex,
                                                                SubTex,
                                                                RotateUV);

        ParticleCutOffKeywordProcess.Process(targetMat);
        UvCutOnOffKeywordProcess.Process(targetMat);
        SecondUVOnOffKeywordProcess.Process(targetMat);
        if (queryResults[0]) SecondUVMaskOnOffKeywordProcess.Process(targetMat);
        if (queryResults[1]) DistortionOnOffKeywordProcess.Process(targetMat);
        if (queryResults[2]) SubTexOpKeywordProcess.Process(targetMat);
        if (queryResults[3]) RotateUVKeywordProcess.Process(targetMat);
    }
}


//public class ParticleMaterialInspector : MaterialEditor
//{
//    public override void OnInspectorGUI()
//    {
//        Material targetMat = target as Material;
//        EditorUtility.SetDirty(targetMat);
//        // render the default inspector
//        base.OnInspectorGUI();

//        // if we are not visible... return
//        if (!isVisible)
//            return;

//        EditorGUILayout.LabelField("                    ");

//        ParticleCutOffKeywordProcess.Process(targetMat);
//        UvCutOnOffKeywordProcess.Process(targetMat);
//    }
//}