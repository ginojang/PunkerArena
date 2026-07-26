using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableDownload : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick_CheckSize()
    {
        Addressables.GetDownloadSizeAsync("Key").Completed +=
             (AsyncOperationHandle<long> SizeHandle) =>
             {
                 string sizeText = string.Concat(SizeHandle.Result, " byte");
                 Addressables.Release(SizeHandle);
             };
    }
}
