using System.Collections;
using UnityEngine;

public class AssetBundleLoader
{
    // asset bundle loading state
    public enum PROCESS_STATE
    {
        NONE = 0,

        DOWNLOADING,
        LOADING,

        IDLE,
        LOADSUCCESSED,
        LOADFAILED
    };

    /************************************************************************/
    // Variables
    public virtual bool IsStarted => true;
    public bool IsLoadSucceed => PROCESS_STATE.LOADSUCCESSED == processState;
    public virtual bool IsIdleState
    {
        get
        {
            bool bResult = false;
            bResult |= (processState == PROCESS_STATE.LOADFAILED);
            bResult |= (processState == PROCESS_STATE.LOADSUCCESSED);

            return bResult;
        }
    }

    // holder
    public int RefCount { get; set; }
    public string Key { get; set; }

    public bool LoadAll { get; set; }

    protected PROCESS_STATE processState = PROCESS_STATE.NONE;

    protected string path;
	public string Path { get { return path; } } 

    /************************************************************************/
    // Structors

    public AssetBundleLoader()
    {
        LoadAll = false;
        RefCount = 0;
    }

    /************************************************************************/
    // Functions

    // asset bundle 로딩을 위한 경로 설정
    public virtual void Update() {}
    public virtual void SetDownloadFilePath(string fullPath, AssetLoader loader, bool isSceneAsset = false) {}
    public virtual void CallEventFuncs() {}

    public virtual void UnloadSafe(bool clearMemory) {}
    public virtual void Release() {}

}
