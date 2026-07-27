using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandLineReader
{
    private const string CUSTOM_ARGS_PREFIX = "-CustomArgs:";
    private const char CUSTOM_ARGS_SEPARATOR = ';';

    public static string[] GetCommandLineArgs()
    {
        return Environment.GetCommandLineArgs();
    }

    public static string GetCommandLine()
    {
        string[] args = GetCommandLineArgs();
        if (args.Length > 0)
        {
            return string.Join(" ", args);
        }
        else
        {
            Debug.LogError("100 CommandLineReader.cs - GetCommandLine() - Can't find any command line arguments!");

            return "";
        }
    }
    public static Dictionary<string, string> GetCustomArguments()
    {
        Debug.LogWarning("107 GetCustomArguments Start");
        Dictionary<string, string> customArgsDict = new Dictionary<string, string>();
        string[] commandLineArgs = GetCommandLineArgs();
        string[] customArgs;
        string[] customArgsBuffer;
        string customArgsStr = "";

        try
        {
            customArgsStr = commandLineArgs.Where(row => row.Contains(CUSTOM_ARGS_PREFIX)).Single();
            //customArgsStr = commandLineArgs.Single(row => row.Contains(CUSTOM_ARGS_PREFIX));
        }
        catch(Exception e)
        {
            Debug.LogException(e);
            return customArgsDict;
        }

        customArgsStr = customArgsStr.Replace(CUSTOM_ARGS_PREFIX, "");
        customArgs = customArgsStr.Split(CUSTOM_ARGS_SEPARATOR);

        foreach(string customArg in customArgs)
        {
            customArgsBuffer = customArg.Split('=');
            if(customArgsBuffer.Length == 2)
            {
                Debug.LogError($"101 arguments : {customArgsBuffer[0]} / value : {customArgsBuffer[1]}");
                customArgsDict.Add(customArgsBuffer[0], customArgsBuffer[1]);
            }
            else
            {
                Debug.LogWarning("102 CommandLineReader.cs - GetCustomArguments() - The custom argument [" + customArg + "] seem to be malformed.");
            }
        }
        return customArgsDict;
    }

    public static string GetCustomArgument(string argumentName)
    {
        Dictionary<string, string> customArgsDict = GetCustomArguments();

        if (customArgsDict.ContainsKey(argumentName))
        {
            return customArgsDict[argumentName];
        }
        else
        {
            return "";
        }
    }
}
