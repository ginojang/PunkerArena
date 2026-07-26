using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PathData
{
    public int assetID;
    public string addressablePath;
}

[System.Serializable]
public class BundleDownload
{
    public string addressablePath;
    public bool bundleCompress;
}

[System.Serializable]
public class AddressableAssetPathDataBase
{
    public PathData[] assetPaths;
    public BundleDownload[] addressablePathUsingDownload;
}
