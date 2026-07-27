using UnityEngine;

/// <summary>
/// [Addressables 제거] 어드레스블 번들 빌드 + S3 업로드 파이프라인은 obsolete다
/// (번들/원격 CDN 없음, S3 서버도 dead). Builder.cs 호환을 위해 셸만 유지하고 실제 동작은 없다.
/// 매니페스트 기반의 새 빌드 전략(WebGL/모바일)은 추후 별도로 구축한다.
/// </summary>
public class AndroidBundleBuild : System.IDisposable
{
    public void Build(Definition.BUILD_PHASE phase, int resourceNumber)
    {
        Debug.LogWarning("[AndroidBundleBuild] Addressables 제거됨 — 어드레스블 번들 빌드 스킵.");
    }

    public void UpdateBundle(Definition.BUILD_PHASE phase, int resourceNumber)
    {
        Debug.LogWarning("[AndroidBundleBuild] Addressables 제거됨 — 어드레스블 번들 업데이트 스킵.");
    }

    public void Dispose() { }
}
