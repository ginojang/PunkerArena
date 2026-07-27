using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;


public class AndroidBuild : IDisposable
{
    public static void ApplyDefines(params string[] defines)
    {
        var defs = string.Join(";", defines);
        foreach(BuildTargetGroup type in Enum.GetValues(typeof(BuildTargetGroup)))
        {
            switch(type)
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
    
    /// <summary>
    /// android apk
    /// </summary>
    /// <param name="outputPath"> apk 생성 폴더 </param>
    /// <param name="phase">BUild Phase</param>
    /// <param name="buildNumber">Build Number</param>
    
    public void Build(string outputPath, Definition.BUILD_PHASE phase, string buildVersion, int buildNumber, int resourceNumber)
    {
        UnityEngine.Debug.Log("Android Build Start");

        var buildOption = BuildOptions.None;

        PlayerSettings.SplashScreen.showUnityLogo = false;
        PlayerSettings.SplashScreen.show = false;
        PlayerSettings.companyName = "Monoverse";
#if DEVELOPMENT
        QualitySettings.skinWeights = SkinWeights.FourBones;
#else
        QualitySettings.skinWeights = SkinWeights.OneBone;
#endif
        PlayerSettings.Android.bundleVersionCode = buildNumber;
        PlayerSettings.accelerometerFrequency = 0;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

        //Using Existing Build

        string remoteLoadPath = string.Empty;

        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        //BuildPlayerOptions build = new BuildPlayerOptions();


        Debug.LogError($"(msjin) BUILD_PHASE : {phase}");
        switch(phase)
        {
            case Definition.BUILD_PHASE.TEST:
            {
                    //var scenes = EditorBuildSettings.scenes;
                    //List<string> sceneList = new List<string>();

                    //foreach (var scene in scenes)
                    //{
                    //    if (scene.enabled)
                    //        sceneList.Add(scene.path);
                    //}
                    //var sceneArray = sceneList.ToArray();

                    PlayerSettings.bundleVersion = buildVersion + "." + buildNumber.ToString("000") + ".1";
                    //PlayerSettings.Android.useCustomKeystore = true;
                    //PlayerSettings.applicationIdentifier = "";
                    PlayerSettings.productName = "Frutti Dino";
                    PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
                    //PlayerSettings.Android.keystoreName = "";
                    //PlayerSettings.Android.keyaliasName = "";
                    //PlayerSettings.Android.keyaliasPass = "";
                    ApplyDefines("NETSTANDARD", "ENABLE_UNSAFE_MSGPACK", "UNITY_POST_PROCESSING_STACK_V2", "DEVELOPMENT");


                    remoteLoadPath = $"{Builder.AssetBundleDevServer}Android";
                    Debug.LogError($"(msjin) RemoteLoadPath : {remoteLoadPath}");


                    buildOption = BuildOptions.Development;
                    EditorUserBuildSettings.buildAppBundle = false;

                    //build.scenes = sceneArray;
                    //build.locationPathName = string.Format($"Build(AOS)/Test_{0}.apk");
                    //build.target = BuildTarget.Android;
                    //build.options = BuildOptions.Development;

                    break;
            }
        }

        UnityEngine.Debug.Log("Build Setting Complete(msjin)");

        //임시 폴더
        int apkIndex = 0;
        string apkName = $"FuttiDino_{apkIndex}.apk";
        string folder = string.Format($"Android_Build/{apkName}");
        if (System.IO.Directory.Exists(folder) == false)
        {
            System.IO.Directory.CreateDirectory(folder);
        }
        else
        {
            string[] files = System.IO.Directory.GetFiles("Android_Build/");
            Debug.LogError($"File Count : {files.Length}");

            for (int i= 0; i  < files.Length; i++)
            {
                Debug.LogError($"Index : {i} / Name {files[i]}");

            }

            System.IO.Directory.CreateDirectory(folder);
        }

   

        var scenes = EditorBuildSettings.scenes;
        List<string> sceneList = new List<string>();

        foreach (var scene in scenes)
        {
            if (scene.enabled)
                sceneList.Add(scene.path);
        }
        var sceneArray = sceneList.ToArray();
        UnityEngine.Debug.Log("Scene Setting Complete(msjin)");

        UnityEngine.Debug.Log("Build Start(msjin)");
        BuildReport report = BuildPipeline.BuildPlayer(sceneArray, folder, BuildTarget.Android, buildOption);

        UnityEngine.Debug.Log("Build Complete = Wait Result(msjin)");
        //BuildReport report = BuildPipeline.BuildPlayer(build);
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build Success : ");
        }
        else if(report.summary.result == BuildResult.Failed)
        {
            
            Debug.Log($"Build Fail{report.summary.totalErrors}");
        }

        UnityEngine.Debug.LogWarning($"Android Build Completed ... Version:{PlayerSettings.bundleVersion} Resource Number:{resourceNumber}");
    }


    public void Dispose()
    {
    }
}
