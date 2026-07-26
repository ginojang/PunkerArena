using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// URP 제거 후 Built-in에서 가산(additive)/발광 이펙트가 톤매핑 없이 흰색으로 뭉개지는(워시아웃)
/// 문제를 Post Processing v2 톤매핑으로 잡는다.
/// 카메라가 HDR이라 ACES 톤매핑이 1을 넘는 값을 압축 -> 흰 블롭이 정상 밝기로.
///
/// 핵심: 코드로 PostProcessLayer를 붙일 땐 반드시 layer.Init(PostProcessResources)로
/// 리소스를 넣어야 한다(안 넣으면 AmbientOcclusion 렌더에서 매프레임 NRE 폭주).
///
/// ▼ 튜닝: 여전히 밝으면 PostExposure를 더 낮추고(-1.0), 톤이 세면 Tonemapper.Neutral로.
/// </summary>
public class GlobalPostProcess : MonoBehaviour
{
    // ===== 튜닝 포인트 =====
    static readonly Tonemapper Tonemap = Tonemapper.ACES; // ACES(강한 압축) / Neutral(부드러움)
    const float PostExposure = -0.8f; // 전체 밝기(EV). 파스텔은 밝게. 더 어둡게: 낮춤
    const float Saturation   = 25f;   // 채도(+): ACES가 뺀 채도 보강 → 파스텔 팝
    const float Temperature  = 8f;    // 화이트밸런스: 양수=따뜻, 음수=차갑게
    const float Tint         = 0f;    // 그린-마젠타 밸런스(필요시)
    // =====================

    static GlobalPostProcess _instance;
    PostProcessProfile _profile;
    PostProcessResources _resources;
    int _volumeLayer;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (_instance != null) return;
        new GameObject("[GlobalPostProcess]").AddComponent<GlobalPostProcess>();
    }

    void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        _resources = LoadResources();
        if (_resources == null)
        {
            Debug.LogWarning("[GlobalPostProcess] PostProcessResources를 못 찾음 -> 톤매핑 생략");
            return;
        }

        _volumeLayer = LayerMask.NameToLayer("Default");

        _profile = ScriptableObject.CreateInstance<PostProcessProfile>();
        var grade = _profile.AddSettings<ColorGrading>();
        grade.gradingMode.overrideState = true;
        grade.gradingMode.value = GradingMode.HighDefinitionRange;
        grade.tonemapper.overrideState = true;
        grade.tonemapper.value = Tonemap;
        grade.postExposure.overrideState = true;
        grade.postExposure.value = PostExposure;
        grade.temperature.overrideState = true;
        grade.temperature.value = Temperature;
        grade.tint.overrideState = true;
        grade.tint.value = Tint;
        grade.saturation.overrideState = true;
        grade.saturation.value = Saturation;

        var volGo = new GameObject("[GlobalPPVolume]") { layer = _volumeLayer };
        DontDestroyOnLoad(volGo);
        var vol = volGo.AddComponent<PostProcessVolume>();
        vol.isGlobal = true;
        vol.priority = 1000f;
        vol.sharedProfile = _profile;

        StartCoroutine(AttachLayers());
    }

    static PostProcessResources LoadResources()
    {
        var r = Resources.Load<PostProcessResources>("PostProcessResources"); // 빌드용(있으면)
        if (r != null) return r;
#if UNITY_EDITOR
        var guids = AssetDatabase.FindAssets("t:PostProcessResources");
        if (guids != null && guids.Length > 0)
            return AssetDatabase.LoadAssetAtPath<PostProcessResources>(AssetDatabase.GUIDToAssetPath(guids[0]));
#endif
        return null;
    }

    IEnumerator AttachLayers()
    {
        var wait = new WaitForSeconds(0.5f);
        while (true)
        {
            var cams = Camera.allCameras;
            for (int i = 0; i < cams.Length; i++)
            {
                var cam = cams[i];
                if (cam == null || cam.targetTexture != null) continue; // 스냅샷/렌더텍스처 제외
                if (cam.GetComponent<PostProcessLayer>() != null) continue;

                var layer = cam.gameObject.AddComponent<PostProcessLayer>();
                layer.Init(_resources); // ★ 반드시 리소스 주입
                layer.volumeLayer = ~0;  // Everything: 씬 고유 Bloom/Vignette 등도 함께 적용(파스텔 룩)
                layer.volumeTrigger = cam.transform;
                layer.antialiasingMode = PostProcessLayer.Antialiasing.None;
            }
            yield return wait;
        }
    }
}
