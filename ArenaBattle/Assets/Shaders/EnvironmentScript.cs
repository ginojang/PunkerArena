using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class EnvironmentScript : MonoBehaviour
{
    public const string CreatureRimLightColorContext = "CreatureRimLightColor";
    //private const string CreatureRimLightParameterContext = "CreatureRimLightParameter";
    public const string CreatureRimLightRangeBase = "CreatureRimLightRangeBase";
    public const string CreatureRimLightRangeShift = "CreatureRimLightRangeShift";
    public const string CreatureRimLightIntensity = "CreatureRimLightIntensity";
    public const string CreatureRimLightPow = "CreatureRimLightPow";
    public const string CreatureRimLightDirectionContext = "CreatureRimLightDirection";
    public const string ActorAmbientColor = "_ActorAmbientColor";
    private float[] layerDistance = new float[32];

    [Serializable]
    public class EnvironmentInfo
    {
        public float ShadowLength = 5;
        public float ShadowRotation = 130;
        public Color CreatureRimLightColor = new Color(0.48235f, 0.68235f, 0.9647f);
        public float CreatureRimLightRange = 1;
        public float CreatureRimLightIntensity = 0.5f;
        public float CreatureRimLightPow = 1.0f;
        public Vector3 CreatureRimLightDirection = new Vector3(-1, 1, -1);
        [Range(-1, 2)]
        public float ActorAmbientColorMultiply = 0.0f;
        public float GrassCullDistance = 0.0f;
        public float BushCullDistance = 0.0f;
    }
    public EnvironmentInfo m_stageEnvironment = new EnvironmentInfo();
    public Transform m_stageRoot;
    private Dictionary<Material, Color> _stageMaterialInfos = new Dictionary<Material, Color>();
    private Dictionary<Material, Color> _stageMaterialInfosForChangeColor = new Dictionary<Material, Color>();

    private static EnvironmentScript _instance = null;
    public static EnvironmentScript Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
        SetCreatureRimLightVariableToShader();
        SetCullDistance();
    }

    void Start()
    {
        GetEnvironmentMaterials(_stageMaterialInfos);
    }

    void OnDestroy()
    {
        Dictionary<Material, Color> restoration = _stageMaterialInfosForChangeColor;
        if(_stageMaterialInfosForChangeColor.Count == 0)
        {
            restoration = _stageMaterialInfos;
        }
        var e = restoration.GetEnumerator();
        while (e.MoveNext() == true)
        {
            e.Current.Key.SetColor("_MainColor", e.Current.Value);
        }
    }

    private void SetCreatureRimLightVariableToShader()
    {
        Shader.SetGlobalColor(CreatureRimLightColorContext, m_stageEnvironment.CreatureRimLightColor);

        float rangebase = 1 / m_stageEnvironment.CreatureRimLightRange;
        float rangeshfit = 1 - rangebase;
        float intensity = m_stageEnvironment.CreatureRimLightIntensity;
        float pow = m_stageEnvironment.CreatureRimLightPow;
        //Vector4 rimLightParameter = new Vector4(rangebase, rangeshfit, intensity, 0);
        //Shader.SetGlobalVector(CreatureRimLightParameterContext, rimLightParameter);
        Shader.SetGlobalFloat(CreatureRimLightRangeBase, rangebase);
        Shader.SetGlobalFloat(CreatureRimLightRangeShift, rangeshfit);
        Shader.SetGlobalFloat(CreatureRimLightIntensity, intensity);
        Shader.SetGlobalFloat(CreatureRimLightPow, pow);

        Vector3 rimLightDirection = Vector3.Normalize(m_stageEnvironment.CreatureRimLightDirection);
        Shader.SetGlobalVector(CreatureRimLightDirectionContext, rimLightDirection);

        Color baseAmbientColor = RenderSettings.ambientLight * RenderSettings.ambientIntensity;
        Color actorAmbient = baseAmbientColor + (baseAmbientColor * m_stageEnvironment.ActorAmbientColorMultiply);
        Shader.SetGlobalColor(ActorAmbientColor, actorAmbient);
    }

    private void SetCullDistance()
    {
        if (Camera.main == null)
            return;

        int GrassLayerIndex = LayerMask.NameToLayer("Grass");
        int BushLayerIndex = LayerMask.NameToLayer("Bush");
        layerDistance[GrassLayerIndex] = m_stageEnvironment.GrassCullDistance;
        layerDistance[BushLayerIndex] = m_stageEnvironment.BushCullDistance;
        Camera.main.layerCullDistances = layerDistance;
    }

    private void GetEnvironmentMaterials(Dictionary<Material, Color> instageMaterialInfos, bool includeSkyBox = true)
    {
        if (!Application.isPlaying)
            return;

        if (instageMaterialInfos.Count > 0)
            return;

        if (m_stageRoot == null)
        {
            Debug.Log("EnvironmentScript StateRoot is null!!");
            return;
        }
        
        // skybox를 걸러낼 방법이 없어 transform을 먼저 얻어오는 것으로 수정.
        Transform[] stageRenderers = m_stageRoot.GetComponentsInChildren<Transform>();

        for (int i = 0; i < stageRenderers.Length; ++i)
        {
            if (stageRenderers[i].name.ToLower().Contains("sky") && !includeSkyBox)
                continue;

            Renderer pRenderer = stageRenderers[i].GetComponent<Renderer>();
            if (pRenderer == null)
                continue;

            for (int j = 0; j < pRenderer.sharedMaterials.Length; ++j)
            {
                Material material = pRenderer.sharedMaterials[j];
                if (material.HasProperty("_MainColor"))
                {
                    if (instageMaterialInfos.ContainsKey(material) == false)
                    {
                        instageMaterialInfos.Add(material, material.GetColor("_MainColor"));
                    }
                }
            }
        }
    }

    public void EnvironmentToDark(bool dark)
    {
        if (_stageMaterialInfos.Count == 0)
        {
            GetEnvironmentMaterials(_stageMaterialInfos);
        }

        var e = _stageMaterialInfos.GetEnumerator();
        while (e.MoveNext() == true)
        {
            Color color = e.Current.Value;
            e.Current.Key.SetColor("_MainColor", dark ? color * 0.33333333f : color);
        }
    }
    public void EnvironmentToColor(Color inColor)
    {
        if (_stageMaterialInfosForChangeColor.Count == 0)
        {
            GetEnvironmentMaterials(_stageMaterialInfosForChangeColor, false);
        }

        var e = _stageMaterialInfosForChangeColor.GetEnumerator();
        while (e.MoveNext() == true)
        {
            e.Current.Key.SetColor("_MainColor", inColor);
        }

        // 스테이지의 오브젝트들의 색상이 바뀌엇기 때문에 보스 등장 연출에서 사용될 정보들을 바뀐 색상을 기준으로 다시 얻어 와야한다.
        _stageMaterialInfos.Clear();
        GetEnvironmentMaterials(_stageMaterialInfos);
    }

#if UNITY_EDITOR
    void Update()
    {
        SetCreatureRimLightVariableToShader();
    }
#endif
}