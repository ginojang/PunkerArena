using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// URP 제거로 생긴 워시아웃(하얗게 뜸)을 Built-in + Post Processing v2로 잡는다.
/// 씬별 PPv2 배선이 제각각이라, 부트스트랩으로 전역 볼륨을 강제 생성하고
/// 화면에 렌더하는 카메라(렌더텍스처 제외)에 PostProcessLayer를 자동 부착한다.
///
/// ▼ 밝기 튜닝: 아직 밝으면 PostExposure를 더 낮추고(-1.0 등), 색이 바래면 Tonemap을 ACES로.
/// </summary>
public class GlobalPostProcess : MonoBehaviour
{
    // ===== 튜닝 포인트 =====
    static readonly Tonemapper Tonemap = Tonemapper.Neutral; // Neutral(부드러움) / ACES(강함)
    const float PostExposure = -0.6f; // 전체 밝기(EV). 더 어둡게: 값 낮춤(-1.0). 밝게: 0에 가깝게.
    const float Saturation   = 0f;    // 채도 보정(-100~100). 워시아웃으로 색 빠지면 +10~20.
    const bool  AffectUI     = false; // UI 카메라까지 톤매핑할지(기본 제외)
    // =====================

    static GlobalPostProcess _instance;
    PostProcessProfile _profile;
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

        _volumeLayer = LayerMask.NameToLayer("Default");

        _profile = ScriptableObject.CreateInstance<PostProcessProfile>();

        // PPv2는 톤매핑이 ColorGrading(HDR 모드)의 tonemapper 필드로 들어간다.
        var grade = _profile.AddSettings<ColorGrading>();
        grade.gradingMode.overrideState = true;
        grade.gradingMode.value = GradingMode.HighDefinitionRange;
        grade.tonemapper.overrideState = true;
        grade.tonemapper.value = Tonemap;
        grade.postExposure.overrideState = true;
        grade.postExposure.value = PostExposure;
        grade.saturation.overrideState = true;
        grade.saturation.value = Saturation;

        var volGo = new GameObject("[GlobalPostProcessVolume]") { layer = _volumeLayer };
        DontDestroyOnLoad(volGo);
        var vol = volGo.AddComponent<PostProcessVolume>();
        vol.isGlobal = true;
        vol.priority = 1000f; // 씬 볼륨보다 우선(안 먹던 배선을 확실히 덮음)
        vol.sharedProfile = _profile;

        StartCoroutine(AttachLayers());
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
                if (cam == null) continue;
                if (cam.targetTexture != null) continue; // 스냅샷/렌더텍스처 카메라 제외
                if (!AffectUI && (cam.cullingMask & ~(1 << LayerMask.NameToLayer("UI"))) == 0) continue; // UI 전용 카메라 제외
                if (cam.GetComponent<PostProcessLayer>() != null) continue;

                var layer = cam.gameObject.AddComponent<PostProcessLayer>();
                layer.volumeLayer = 1 << _volumeLayer;
                layer.volumeTrigger = cam.transform;
                layer.antialiasingMode = PostProcessLayer.Antialiasing.None;
            }
            yield return wait;
        }
    }
}
