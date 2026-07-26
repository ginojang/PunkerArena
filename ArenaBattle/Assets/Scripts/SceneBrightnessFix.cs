using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// URP 제거 후 Built-in에서 라이팅/앰비언트가 과해 화면이 하얗게 뜨는(워시아웃) 문제를
/// PPv2 없이 잡는다. 씬 로드 시 디렉셔널 라이트와 앰비언트를 일정 비율로 낮춘다.
/// (PostProcessLayer를 코드로 붙이면 PostProcessResources 없음 → 매 프레임 NRE 폭주하므로 이 방식 사용)
///
/// ▼ 튜닝: 값이 낮을수록 어두움. 아직 밝으면 더 낮추고(0.45 등), 어두우면 1.0 쪽으로.
/// </summary>
public class SceneBrightnessFix : MonoBehaviour
{
    // ===== 튜닝 포인트 =====
    const float DirectionalLightScale = 0.6f; // 디렉셔널 라이트 세기 배율
    const float AmbientScale           = 0.6f; // 환경광(앰비언트) 배율
    // =====================

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        var go = new GameObject("[SceneBrightnessFix]");
        DontDestroyOnLoad(go);
        go.AddComponent<SceneBrightnessFix>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 단일 씬 로드마다 1회 적용(가산 로드는 제외 → 중복 감쇄 방지)
    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Single)
            Apply();
    }

    static void Apply()
    {
        RenderSettings.ambientIntensity   *= AmbientScale;
        RenderSettings.ambientLight        = RenderSettings.ambientLight * AmbientScale;
        RenderSettings.ambientSkyColor     = RenderSettings.ambientSkyColor * AmbientScale;
        RenderSettings.ambientEquatorColor = RenderSettings.ambientEquatorColor * AmbientScale;
        RenderSettings.ambientGroundColor  = RenderSettings.ambientGroundColor * AmbientScale;

        var lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        for (int i = 0; i < lights.Length; i++)
            if (lights[i].type == LightType.Directional)
                lights[i].intensity *= DirectionalLightScale;
    }
}
