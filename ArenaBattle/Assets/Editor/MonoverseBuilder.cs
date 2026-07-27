using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEditor.Build.Reporting;

public class MonoverseBuilder
{
    public const string AssetBundleDevServer = "https://futtidino-data.s3.ap-southeast-1.amazonaws.com/asset-data/";
    public const string AssetBundleDevServeriOS = "";

    public const string AssetBundleStagingServer = "";

    public const string AssetBundleProductionServer = "";

    public const string JenkinsServerAddress = "";
    public const string JenkinsServerID = "admin";
    public const string JenkinsServerToken = "";

    public const string BUNDLE_DIR = "HostedData";

    private static Definition.BUILD_PHASE _phase = Definition.BUILD_PHASE.INVALID;
    private static string _outputPath = string.Empty;

    public static void ApplyDefines(params string[] defines)
    {
        var defs = string.Join(";", defines);
        foreach (BuildTargetGroup type in Enum.GetValues(typeof(BuildTargetGroup)))
        {
            switch (type)
            {
                case BuildTargetGroup.Android:
                case BuildTargetGroup.iOS:
                case BuildTargetGroup.Standalone:
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(type, defs);
                    Debug.Log($"(msjin) {type.ToString()}, {defs.ToString()}");
                    break;

                default:
                    break;
            }
        }
    }
    //[MenuItem("Build/Build AOS(Debug")]
    public static void Build_AOS()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.target = BuildTarget.Android;

        ReadyForBuildSetting();

        var phase = CommandLineReader.GetCustomArgument("phase");
        System.Enum.TryParse(phase, false, out _phase);

        switch(_phase)
        {
            case Definition.BUILD_PHASE.TEST:
                buildPlayerOptions.options = BuildOptions.Development;

                //ApplyDefines("NETSTANDARD", "ENABLE_UNSAFE_MSGPACK", "UNITY_POST_PROCESSING_STACK_V2", "DEVELOPMENT");
                break;
        }

        
        
        buildPlayerOptions.scenes = FindEnabledEditorScenes();
        buildPlayerOptions.locationPathName = string.Format("Build(AOS)/Test_{0}.apk", PlayerSettings.bundleVersion);
        
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if(summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Success");
        }
        else if(summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

    private static void ReadyForBuildSetting()
    {
        #region 로고
        PlayerSettings.SplashScreen.showUnityLogo = false;
        PlayerSettings.SplashScreen.show= false;
        #endregion

        #region Company
        PlayerSettings.companyName = "Monoverse";
        #endregion


#if DEVELOPMENT
        QualitySettings.skinWeights = SkinWeights.FourBones;
#else
        QualitySettings.skinWeights = SkinWeights.OneBone;
#endif

        PlayerSettings.accelerometerFrequency = 0;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;


        #region 최소 안드로이드 버전
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
        #endregion

    }


    private static string[] FindEnabledEditorScenes()
    {
        List<string> editorScenes = new List<string>();
        
        foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            editorScenes.Add(scene.path);
        }

        return editorScenes.ToArray();
    }

    

    public void Dispose()
    {

    }
}
