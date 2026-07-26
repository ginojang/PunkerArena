using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

/// <summary>
/// ArenaBattle 마이그레이션용 에디터 빌드 유틸. batchmode -executeMethod 로 호출.
/// </summary>
public static class ArenaBuildTools
{
    // Unity.exe -batchmode -quit -projectPath ... -executeMethod ArenaBuildTools.BuildAddressables
    public static void BuildAddressables()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("[ArenaBuildTools] AddressableAssetSettings not found.");
            EditorApplication.Exit(2);
            return;
        }

        AddressableAssetSettings.BuildPlayerContent(out var result);
        if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.LogError($"[ArenaBuildTools] Addressables build FAILED: {result.Error}");
            EditorApplication.Exit(3);
            return;
        }

        Debug.Log($"[ArenaBuildTools] Addressables build OK in {result.Duration:F1}s -> {result.OutputPath}");
    }
}
