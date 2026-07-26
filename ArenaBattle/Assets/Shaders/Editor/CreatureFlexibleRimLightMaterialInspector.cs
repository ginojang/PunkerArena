using System.Collections.Generic;
using Assets.Editor.CustomMaterialEditors;
using UnityEditor;
using UnityEngine;

public class CreatureFlexibleRimLightMaterialInspector : CustomMaterialInspector
{
    private const string Shininess = "_Shininess";
    private const string SpecularIntensity = "_SpecularIntensity";
    private const string SpecularMaterialColor = "_SpecularMaterialColor";

    private const string FlowTex = "_FlowTex";
    private const string FlowSpeed = "_FlowSpeed";
    private const string FlowIntensity = "_FlowIntensity";

    private const string NormalTex = "_NormalTex";
    private const string MaskTex = "_MaskTex";

    private const string RimLightParameter = "_RimLightParameter";
    private const string RimLightDirection = "_RimLightDirection";
	private const string CutOffRange = "_CutOffRange";

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

		if (CutOffKeywordProcess.IsOn(targetMat) == false)
		{
			if (propertyName == CutOffRange) return true;
		}
		
		return false;
    }

    protected override void ProcessDefineKeywords(Material targetMat)
    {
        SpecularOnOffKeywordProcess.Process(targetMat);
        FlowKeywordProcess.Process(targetMat);
		CutOffKeywordProcess.Process(targetMat);
		if (FlowKeywordProcess.IsOn(targetMat)) FlowModeKeywordProcess.Process(targetMat);
        LightQualityKeywordProcess.Process(targetMat);
    }
}