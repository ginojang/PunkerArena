using UnityEditor;
using UnityEngine;

public abstract class CustomMaterialInspector : MaterialEditor
{
    static class Uniforms
    {
        internal static readonly int RenderingType = Shader.PropertyToID("_RenderingType");
        internal static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        internal static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
    }
    public enum EnumRenderingType
    {
        Opaque,
        Transparent
    }
    EnumRenderingType renderingType = EnumRenderingType.Opaque;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var theShader = serializedObject.FindProperty("m_Shader");
        if (!isVisible || theShader.hasMultipleDifferentValues || theShader.objectReferenceValue == null) return;

        Material targetMat = target as Material;

        EditorGUI.BeginChangeCheck();

        if (targetMat.HasProperty(Uniforms.RenderingType))
        {
            renderingType = (EnumRenderingType)targetMat.GetFloat(Uniforms.RenderingType);
            GUILayout.Label("SURFACE", EditorStyles.boldLabel);
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(2.0f) });
            EditorGUI.indentLevel++;
            renderingType = (EnumRenderingType)EditorGUILayout.EnumPopup("Rendering Type", renderingType);
            targetMat.SetFloat(Uniforms.RenderingType, (int)renderingType);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            switch (renderingType)
            {
                case EnumRenderingType.Opaque:
                    targetMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    targetMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    targetMat.SetOverrideTag("RenderType", "Opaque");
                    targetMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
                    break;

                case EnumRenderingType.Transparent:
                    targetMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    targetMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    targetMat.SetOverrideTag("RenderType", "Transparent");
                    targetMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    break;
            }

        }

        GUILayout.Label("DEFINE", EditorStyles.boldLabel);
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(2.0f) });
        EditorGUI.indentLevel++;
        //EditorGUILayout.LabelField("===========DEFINE==========================================================================");
        ProcessDefineKeywords(targetMat);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();

        GUILayout.Label("PROPERTY", EditorStyles.boldLabel);
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(2.0f) });
        //EditorGUILayout.LabelField("===========PROPERTY==========================================================================");
        EditorGUI.indentLevel++;
        ProcessProperties(targetMat);
        EditorGUI.indentLevel--;

        if (EditorGUI.EndChangeCheck()) 
            PropertiesChanged();

        EditorUtility.SetDirty(targetMat);
    }

    private void ProcessProperties(Material targetMat)
    {
        Shader shader = targetMat.shader;

        for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
        {
            ShaderProperty(targetMat, shader, i);
        }
    }
    
    private void ShaderProperty(Material material, Shader shader, int propertyIndex)
    {
        string propertyName = ShaderUtil.GetPropertyName(shader, propertyIndex);

        if (propertyName == "_RenderingType" || propertyName == "_SrcBlend" || propertyName == "_DstBlend")
            return;
         
        if (HideThisProperty(material, propertyName)) return;

        if (ShaderUtil.IsShaderPropertyHidden(shader, propertyIndex)) return;

        const float controlSize = 84;
        
        //EditorGUIUtility.LookLikeControls(Screen.width - controlSize, 0);
        EditorGUIUtility.labelWidth = Screen.width - controlSize;
        EditorGUIUtility.fieldWidth = 0;

        var property = GetMaterialProperty(new Object[] {material}, propertyIndex);
        string label = ShaderUtil.GetPropertyDescription(shader, propertyIndex);

        switch (ShaderUtil.GetPropertyType(shader, propertyIndex))
        {
            case ShaderUtil.ShaderPropertyType.Range: // float ranges
            {
                RangeProperty(property, label);
                break;
            }
            case ShaderUtil.ShaderPropertyType.Float: // floats
            {
                FloatProperty(property, label);
                break;
            }
            case ShaderUtil.ShaderPropertyType.Color: // colors
            {
                ColorProperty(property, label);
                break;
            }
            case ShaderUtil.ShaderPropertyType.TexEnv: // textures
            {
                //EditorGUIUtility.LookLikeControls(0, 70);
                EditorGUIUtility.labelWidth = 0;
                EditorGUIUtility.fieldWidth = 70;

                TextureProperty(property, label);
                GUILayout.Space(6);
                break;
            }
            case ShaderUtil.ShaderPropertyType.Vector: // vectors
            {
                VectorProperty(property, label);
                break;
            }
            default:
            {
                GUILayout.Label("ARGH" + label + " : " + ShaderUtil.GetPropertyType(shader, propertyIndex));
                break;
            }
        }
    }

    protected abstract bool HideThisProperty(Material targetMat, string propertyName);
    protected abstract void ProcessDefineKeywords(Material targetMat);
}