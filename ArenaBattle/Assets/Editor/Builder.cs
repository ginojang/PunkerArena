using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;

public static class Builder
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
    private static string _buildVersion = "1.00";
    private static int _buildNumber = 0;
    private static int _resourceNumber = 0;
    private static bool _buildApp = true;
    private static bool _buildBundle = true;
    private static bool _buildBundleUpdate = false;

    private static bool ReadCommandLineParameters()
    {
        var phase = CommandLineReader.GetCustomArgument("phase");
        var buildNumber = CommandLineReader.GetCustomArgument("buildNumber");
        var resourceNumber = CommandLineReader.GetCustomArgument("resourceNumber");

        UnityEngine.Debug.Log($"ReadCommandLineParameters = Phase:{phase} | BuildNumber:{buildNumber} | ResourceNumber:{resourceNumber}");

        UnityEngine.Debug.Log($"ReadCommandLineParameters = Phase:{phase}");
        UnityEngine.Debug.Log($"ReadCommandLineParameters = Phase:{buildNumber}");

        if (System.Enum.TryParse(phase, false, out _phase) == false)
        {
            Debug.LogError($"invaild BUILD_PHASE {phase}");
            return false;
        }

        if (int.TryParse(buildNumber, out _buildNumber) == false)
        {
            Debug.LogError($"invaild BUILD_PHASE {buildNumber}");
            return false;
        }

        if (int.TryParse(resourceNumber, out _resourceNumber) == false)
        {
            Debug.LogError($"invaild BUILD_PHASE {resourceNumber}");
            return false;
        }

        Debug.Log("Parsed CustomArgs : -----------------------");
        Debug.Log($"phase = {phase}");
        Debug.Log($"buildNumber = {buildNumber}");
        Debug.Log($"resourceNumber = {resourceNumber}");
        Debug.Log("Parsed CustomArgs Finished : -----------------------");

        return true;
    }

    public static bool BuildAOS_All()
    {
        UnityEngine.Debug.LogWarning("Build Start");

        if (ReadCommandLineParameters() == false)
        {
            Debug.LogError("msjin build Rsturn False");
            return false;
        }
            

        if(_buildBundle == true)
        {
            using (var bundleBuilder = new AndroidBundleBuild())
            {
                if(_buildBundleUpdate == false)
                {
                    UnityEngine.Debug.LogWarning("Bundle Start");
                    bundleBuilder.Build(_phase, _resourceNumber);
                }
                else
                {
                    UnityEngine.Debug.LogWarning("Bundle Update");
                    bundleBuilder.UpdateBundle(_phase, _resourceNumber);
                }
            }
        }

        if(_buildApp == true)
        {
            using(var appBuilder = new AndroidBuild())
            {
                appBuilder.Build(_outputPath, _phase, _buildVersion, _buildNumber, _resourceNumber);
            }
        }

        return true;
    }
}
