using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 원본(Project_FruitDino)은 Built-in RP + RealToon + Post Processing v2. Unity 2020.3 원본은
/// 톤매핑 없이도 안 터졌지만, Unity 6은 같은 씬 데이터를 더 밝게 렌더 → 최종 합성값이 1을 초과해
/// 톤매핑이 없으면 흰색으로 터진다(화이트 워시). 그래서 톤매핑 자체는 "필요"하다.
///
/// ACES는 워시를 잡지만 하이라이트 채도까지 빼서 "색 빠짐"으로 보인다. → Neutral 톤매퍼 사용:
/// 하이라이트만 부드럽게 압축하고 색/채도는 최대한 보존(파스텔 룩).
///
/// 이 컴포넌트가 하는 일:
///  1) 화면 카메라에 PostProcessLayer 부착 + Init(PostProcessResources) + volumeLayer=Everything
///     → 씬 고유 Bloom이 적용됨(Init 안 하면 AmbientOcclusion 매프레임 NRE 폭주).
///  2) 전역 볼륨(priority 1000)으로 Neutral 톤매핑만 얹어 Unity6 과밝음을 1 이하로 롤오프.
///
/// ▼ 튜닝(한 줄씩): 아직 밝으면 PostExposure를 -0.3~-0.6으로, 채도 부족하면 Saturation +10~+20.
///   그래도 파스텔이 안 나오면 UseLDRClamp=true (카메라 HDR 끔 → 물리적으로 워시 불가, 무채도손실).
/// </summary>
public class GlobalPostProcess : MonoBehaviour
{
    // ===== 튜닝 포인트 =====
    const bool ApplyGlobalGrading = true;  // Unity6 과밝음 롤오프(워시 방지). 필요
    const bool UseLDRClamp        = false; // 최후수단: 카메라 HDR 끔(LDR 클램프). 워시 원천봉쇄, Bloom은 LDR

    static readonly Tonemapper Tonemap = Tonemapper.Neutral; // Neutral(색보존) / ACES(강압축·채도손실)
    const float PostExposure = 0f;    // 전체 밝기(EV). 아직 밝으면 -0.3~-0.6
    const float Saturation   = 0f;    // 채도(+). Neutral은 색보존 → 0에서 시작, 부족하면 +10~+20
    const float Temperature  = 0f;    // 화이트밸런스: 양수=따뜻, 음수=차갑게
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

        // 원본 재현 기본값(ApplyGlobalGrading=false)에서는 전역 그레이딩 볼륨을 만들지 않고,
        // 씬 고유 프로파일(Bloom 단독)이 그대로 룩을 결정하게 둔다.
        if (ApplyGlobalGrading)
        {
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
        }

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

                if (UseLDRClamp) cam.allowHDR = false; // LDR 클램프: 1 초과값 원천 차단 → 워시 불가

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
