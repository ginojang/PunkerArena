using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;
using UnityEngine;
using UnityEngine.U2D;

public enum objectType
{
    character = 0,
    ui,
    gameobject,
    effect,
    sprite,
    texture2d,
    others,
    atlasSprite,
    aniClip,
}

public class poolDataInfo
{
    public poolDataInfo(UnityEngine.Object obj, objectType type = objectType.character)
	{
        bUsed = false;
        poolObj = obj;
        objectype = type;
    }

    public bool bUsed;
    public objectType objectype;
    public UnityEngine.Object poolObj;
}

public class ResourcePoolManager : MonoBehaviour
{
    public static ResourcePoolManager instance_value;

    public static ResourcePoolManager Instance
    {
        get
        {
            if (instance_value == null)
            {
                GameObject go = new GameObject("ResourcePoolManager");
                instance_value = go.AddComponent<ResourcePoolManager>();
                ImmortalGameObject.AttachObject(go);
            }

            return instance_value;
        }
    }

    // 메모리상의 로드 데이터들
    private Dictionary<string, UnityEngine.Object> poolResourcesDataList = new Dictionary<string, UnityEngine.Object>();

    // 앱 플레이중 로드 되어진 모든 데이터들
    // 삭제는 사용자가 알아서.
    // 현재는 Scene이 종료 되면 Clear 예정
    private Dictionary<string, List<poolDataInfo>> poolGameDataList = new Dictionary<string, List<poolDataInfo>>();

    public void Initialize()
    {

    }

    // 오브젝트리스트를 한번에 로드 요청할때 사용
    // 기본 사용법은 CostumeManager.cs의 ChangeDinoPart(...) 참조
    // 일단 param은 오브젝트의 타입리스트를 꼭!!!. 필요한 값들은 각자 추가 해서 사용 
    // bClone은 복사본을 리턴할지 메모리 상의 데이터를 리턴할지...
    public void AsyncLoadDataList(List<AssetLoader> preloadAssetList, object param, bool bClone = false, Action<object> call = null)
    {
        StartCoroutine(AsyncLoadGameDataList(preloadAssetList,  param, bClone, call));
    }

    public IEnumerator AsyncLoadGameDataList(List<AssetLoader> preloadAssetList, object param, bool bClone = false, Action<object> call = null)
    {
        yield return AssetManager.Instance.LoadAssetAsyncForPreloadAsset<UnityEngine.Object>(preloadAssetList);

        List<object> paramlist = (List<object>)param;

        // 오브젝트 타입은 꼭 필요한 내용이라 param에 첫인자로 꼭 넣어서...
        List<objectType> typelist = (List<objectType>)(paramlist[0]);
        List<object> loadedList = new List<object>();

#if UNITY_EDITOR
        for (int i = 0; i < typelist.Count; i++)
        {
            if (typelist[i] != objectType.atlasSprite)
                continue;

            Debug.LogError("atlasSprites는 AsyncLoadGameDataList 함수를 사용 못합니다.");
            break;
        }
#endif

        for (int i = 0; i < preloadAssetList.Count; i++)
        {
            UnityEngine.Object obj = null;
            
            var loadObj = preloadAssetList[i].MainAsset;

            string objName = preloadAssetList[i].MainAsset.name;
            
            if(!poolResourcesDataList.ContainsKey(objName))
                poolResourcesDataList.Add(objName, loadObj);

            if (bClone)
            {
                obj = CloneGameData(objName, typelist[i]);

                poolDataInfo data = new poolDataInfo(obj, typelist[i]);
                data.bUsed = true;

                if (poolGameDataList.ContainsKey(objName))
                    poolGameDataList[objName].Add(data);
                else
                {
                    List<poolDataInfo> infolist = new List<poolDataInfo>();
                    infolist.Add(data);
                    poolGameDataList.Add(objName, infolist);
                }
                
                loadedList.Add(obj);
            }
            else
                loadedList.Add(loadObj);
        }

        call?.Invoke(param);
    }
    
    // 선로드만 필요한 경우 로드 해서 poolResourcesDataList에 적재
    public void AsyncLoadData(string objName, objectType type, Action<UnityEngine.Object> call = null, string subObjName = "")
    {
        //if (poolResourcesDataList.ContainsKey(objName))
        //    return;
        
        if (!poolResourcesDataList.ContainsKey(objName))
        {
            //string filename = GetFilenameByObjectType(objName, type);
            
            AssetManager.Instance.LoadAssetAsync<UnityEngine.Object>(objName, (ld, p) =>
            {
                var loadObj = ld.MainAsset;

                if (loadObj.Equals(null))
                {
                    Debug.LogError($"File Load Fail!!!!   {objName} is Not Exist");
                }
                else
                {
                    poolResourcesDataList.Add(objName, loadObj);
                }

                switch (type)
                {
                    case objectType.atlasSprite:
                        var spriteAtlas = loadObj as SpriteAtlas;
                        var sprite = spriteAtlas.GetSprite(subObjName);
                        call?.Invoke(sprite);
                        break;
                    default:
                        call?.Invoke(loadObj);
                        break;
                }
            });
        }
        else
        {
            call?.Invoke(poolResourcesDataList[objName]);
        }
    }
    
    public void AsyncGetData(string objname, objectType type, UnityEngine.Object param = null, Transform parent = null,
        Action<UnityEngine.Object> call = null, string subObjName = "")
    {
        StartCoroutine(AsyncGetGameData(objname, type, param, parent, call, subObjName));
    }

    // 연타등 여러번 나올수도 있는 오브젝트가 있기 때문에
    // 현재 사용이 끝나서 pool 에 있는 오브젝트가 있는지 체크해보고
    // 있으면 재활용, 없으면 새로 복사를 해서 넘겨준다.
    public IEnumerator AsyncGetGameData(string objName, objectType type, UnityEngine.Object param = null,
        Transform parent = null,  Action<UnityEngine.Object> call = null, string subObjName = "")
    {
        UnityEngine.Object obj = null;
        bool failed = false;

        if (poolGameDataList.ContainsKey(objName).Equals(true))
        {
            foreach (var data in poolGameDataList[objName])
            {
                if (!data.bUsed && type == data.objectype)
                {
                    obj = data.poolObj;
                    data.objectype = type;
                    data.bUsed = true;
                    SetParent(type, obj, parent);

#if DINO_SKILL_EDITOR
                    if (parent)
                    {
                        if(!parent.GetComponent<CharacterBase>().` ListTool.ContainsKey(index))
                            parent.GetComponent<CharacterBase>().effectListTool.Add(index, obj);
                    }
#endif
                    break;
                }
            }

            // 리스트에 있는 오브젝트들이 모두 사용중일때
            // 하나를 새로 로드 해야하는 상황.
            if (obj == null)
            {
                obj = CloneGameData(objName, type, parent);

                poolDataInfo data = new poolDataInfo(obj, type);
                data.bUsed = true;
                poolGameDataList[objName].Add(data);
                SetParent(type, obj, parent);
            }
        }
        else
        {
            if (!poolResourcesDataList.ContainsKey(objName))
            {
                //string filename = GetFilenameByObjectType(objName, type);
                
                AssetManager.Instance.LoadAssetAsync<UnityEngine.Object>(objName, (ld, p) =>
                {
                    var loadObj = ld.MainAsset;

                    if (loadObj == null)
                    {
                        Debug.LogError($"File Load Fail!!!!   {objName} is Not Exist");
                        failed = true;
                    }
                    else
                    {
                        if(!poolResourcesDataList.ContainsKey(objName))
                            poolResourcesDataList.Add(objName, loadObj);

                        obj = CloneGameData(objName, type, parent);
                        
                        poolDataInfo data = new poolDataInfo(obj, type);
                        data.bUsed = true;
                        
                        if(poolGameDataList.ContainsKey(objName))
                            poolGameDataList[objName].Add(data);
                        else
                        {
                            List<poolDataInfo> infolist = new List<poolDataInfo>();
                            infolist.Add(data);
                            poolGameDataList.Add(objName, infolist);
                        }
                    }
                });
            }
            else
            {
                obj = CloneGameData(objName, type, parent);

                poolDataInfo data = new poolDataInfo(obj, type);
                data.bUsed = true;
                
                if(poolGameDataList.ContainsKey(objName))
                    poolGameDataList[objName].Add(data);
                else
                {
                    List<poolDataInfo> infolist = new List<poolDataInfo>();
                    infolist.Add(data);
                    poolGameDataList.Add(objName, infolist);
                }
                SetParent(type, obj, parent);
            }
        }

        yield return new WaitUntil(() => obj != null || failed);

        if (failed)
        {
            call?.Invoke(null);
            yield break;
        }

        switch(type)
        {
            case objectType.atlasSprite:
                var spriteAtlas = obj as SpriteAtlas;
                var sprite = spriteAtlas.GetSprite(subObjName);
                call?.Invoke(sprite);
                break;
            default:
                call?.Invoke(obj);
                break;
        }
    }

    public UnityEngine.Object GetResourceData(string objName)
    {
        UnityEngine.Object obj = null;
        
        if (poolResourcesDataList.ContainsKey(objName))
            obj = poolResourcesDataList[objName];

        return obj;
    }
    
    private void SetParent(objectType type, UnityEngine.Object obj, Transform parent)
    {
        if (parent == null)
            return;
        
        switch (type)
        {
            case objectType.character:
            case objectType.ui:
            case objectType.effect:
            case objectType.others:
                {
                    ((GameObject)obj).transform.SetParent(parent);
                    ((GameObject)obj).transform.localPosition = Vector3.zero;
                    ((GameObject)obj).transform.localRotation = Quaternion.identity;
                    
                    ((GameObject)obj)?.SetActive(true);
                }
                break;
        }
    }
    
    // 이미 메모리상에는 로드 되어 있는 데이터를
    // 복사해서 리턴
    private UnityEngine.Object CloneGameData(string objName, objectType type, Transform parent = null)
    {
        UnityEngine.Object obj = null;

        if (poolResourcesDataList.ContainsKey(objName))
        {
            switch (type)
            {
                case objectType.character:
                case objectType.ui:
                case objectType.gameobject:
                case objectType.effect:
                case objectType.others:
                    {
                        GameObject gameObject = (GameObject)poolResourcesDataList[objName];
                        obj = Instantiate(gameObject);
                    }
                    break;
                case objectType.sprite:
                    {
                        obj = (Sprite)poolResourcesDataList[objName];
                    }
                    break;
                case objectType.texture2d:
                    {
                        Texture2D tex = (Texture2D)poolResourcesDataList[objName];
                        obj = Instantiate(tex);
                    }
                    break;
                case objectType.atlasSprite:
                    {
                        obj = (SpriteAtlas)poolResourcesDataList[objName];
                    }
                    break;
                case objectType.aniClip:
                    {
                        obj = (AnimationClip)poolResourcesDataList[objName];
                    }
                    break;
            }
            SetParent(type, obj, parent);
        }

#if DINO_SKILL_EDITOR
        if(parent)
		{
            if(!parent.GetComponent<CharacterBase>().effectListTool.ContainsKey(index))
                parent.GetComponent<CharacterBase>().effectListTool.Add(index, obj);
		}
#endif
        return obj;
    }

    private string GetFilenameByObjectType(string objname, objectType type)
    {
        string filename = "";
        
        switch (type)
        {
            case objectType.character:
                {
                    
                }
                break;
            case objectType.ui:
                {
                        
                }
                break;
            case objectType.effect:
                {
                    
                }
                break;
            case objectType.sprite:
                {
                        
                }
                break;
            case objectType.texture2d:
                {
                        
                }
                break;
            case objectType.atlasSprite:
                {

                }
                break;
            case objectType.aniClip:
                {

                }
                break;
        }

        return filename;
    }

    public void DestroyGameObject(GameObject obj, objectType type)
    {
        bool bRes = false;
        
        foreach (var list in poolGameDataList)
        {
            foreach (var data in list.Value)
            {
                if (data.poolObj.GetHashCode() == obj.GetHashCode())
                {
                    Destroy(data.poolObj);
                    data.poolObj = null;
                    list.Value.Remove(data);

                    bRes = true;
                    break;
                }
            }

            if (bRes)
                break;
        }
    }
    
    public void DestroyGameObject(UnityEngine.Object obj, bool bDestroy)
    {
        bool bRes = false;
        
        foreach (var list in poolGameDataList)
        {
            foreach (var data in list.Value)
            {
                if (data.poolObj.GetHashCode() == obj.GetHashCode())
                {
                    if (bDestroy)
                    {
                        Destroy(data.poolObj);
                        data.poolObj = null;
                        list.Value.Remove(data);
                    }
                    else
                    {
                        switch (data.objectype)
                        {
                            case objectType.character:
                            case objectType.ui:
                            case objectType.gameobject:
                            case objectType.effect:
                            case objectType.others:
                                {
                                    ((GameObject)data.poolObj).SetActive(false);
                                }
                                break;
                            default:
                                Destroy(data.poolObj);
                                data.poolObj = null;
                                list.Value.Remove(data);
                                break;
                        }
                        data.bUsed = false;
                    }

                    bRes = true;
                    break;
                }
            }

            if (bRes)
                break;
        }
    }
    
    public void DestroyGameDatapool()
    {
        poolGameDataList.Clear();
    }

    public bool IsContainsData(string objname)
    {
        return poolResourcesDataList.ContainsKey(objname);
    }
}
