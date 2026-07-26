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
    const string SettingsPath = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";

    public static void BuildAddressables()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            // Default object not registered (common after migrating the project folder).
            // Load the settings asset directly and register it as the project default.
            settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(SettingsPath);
            if (settings == null)
            {
                Debug.LogError($"[ArenaBuildTools] settings asset not found at {SettingsPath}");
                EditorApplication.Exit(2);
                return;
            }
            AddressableAssetSettingsDefaultObject.Settings = settings;
            AssetDatabase.SaveAssets();
            Debug.Log("[ArenaBuildTools] Registered default AddressableAssetSettings.");
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
