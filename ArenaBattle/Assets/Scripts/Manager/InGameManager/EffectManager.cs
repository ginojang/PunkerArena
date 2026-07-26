using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EffectManager : MonoBehaviour
{
    private class EffectInfo
    {
        public int Count { set; get; }
        public GameObject EffectObject { set; get; }
    }

    public static EffectManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        InitData();
    }

    private Dictionary<GameObject, Dictionary<string, EffectInfo>> effectDic = null;
    private List<GameObject> doOnceList = new List<GameObject>();

    private EffectTable table = null;

    private void InitData()
    {
        effectDic = new Dictionary<GameObject, Dictionary<string, EffectInfo>>();
        table = CSVDataManager.GetTable<EffectTable>();
    }

    #region Get Data
    public Generated.CsvData.effectData GetData(string name)
    {
        Generated.CsvData.effectData data = null;
        data = table.GetData(name);

        return data;
    }
    #endregion

    #region Get Effect

    public void GetEffect(string name, GameObject effect, GameObject parent = null ,bool active = false)
    {
        ResourcePoolManager.Instance.AsyncGetData(name, objectType.effect, null, null, (obj) =>
        {
            effect = (GameObject)obj;
            effect.SetActive(active);

            if (parent != null)
                effect.transform.SetParent(parent.transform);
        }, "");
    }
    #endregion


    public void AddDoOnceEffect(GameObject parent, string name)
    {
        var data = table.GetData(name);
        GameObject effect = null;
        ResourcePoolManager.Instance.AsyncGetGameData(name, objectType.gameobject, default, default, (obj) =>
        {
            effect = (GameObject)obj;
            effect.transform.SetParent(parent.transform);
            effect.SetActive(true);

            doOnceList.Add(effect);
        }, "");
    }

    public void AddEffectList(GameObject target, string name, objectType type, bool effectOn = false)
    {
        if (name == "none")
            return;

        Dictionary<string, EffectInfo> effectList = null;
        var data = table.GetData(name);

        if (effectDic.TryGetValue(target, out effectList) == false)
            effectDic.Add(target, effectList = new Dictionary<string, EffectInfo>());

        if(effectList.ContainsKey(name) == true)
        {
            EffectInfo outInfo = null;
            effectList.TryGetValue(name, out outInfo);
            outInfo.Count++;

            if (effectOn == true)
                EffectOn(target, data.idx);
            else
                outInfo.EffectObject.SetActive(false);
        }
        else
        {
            ResourcePoolManager.Instance.AsyncGetData(data.idx, objectType.gameobject, null, null, (obj) =>
            {
                GameObject effect = (GameObject)obj;
                GameObject parent = null;
                
                switch(type)
                {
                    case objectType.character:
                        var charbase = target.GetComponent<CharacterBase>();
                        parent = charbase.GetEffectParent(data.active_pos_type, data.link_bone_move);
                        break;
                    case objectType.ui:
                        break;
                }

                effect.transform.SetParent(parent.transform);
                effect.transform.localPosition = Vector3.zero;
                effect.transform.localRotation = Quaternion.identity;

                EffectInfo newInfo = new EffectInfo();
                newInfo.Count = 1;
                newInfo.EffectObject = effect;

                effectList.Add(data.idx, newInfo);
                if (effectOn == true)
                    EffectOn(target, data.idx);
                else
                    effect.SetActive(false);
            }, "");
        }
    }

    public void EffectOn(GameObject charbase, string name)
    {
        if (name == "none")
            return;

        Dictionary<string, EffectInfo> dic = null;
        if (effectDic.TryGetValue(charbase, out dic) == false)
            return;

        EffectInfo info = null;
        if (dic.TryGetValue(name, out info) == false)
            return;

        GameObject effect = info.EffectObject;

        bool loop = effect.GetComponentInChildren<ParticleSystem>().main.loop;
        if (loop)
            effect.SetActive(true);
        else
            StartCoroutine(DoOnceEffect(effect));
    }


    public void DestroyEffect(GameObject charbase, string name)
    {
        if (name == "none")
            return;

        Dictionary<string, EffectInfo> dic = null;
        if (effectDic.TryGetValue(charbase, out dic) == false)
            return;

        EffectInfo outInfo = null;
        if (dic.TryGetValue(name, out outInfo) == false)
            return;

        outInfo.Count--;
        if(outInfo.Count <= 0)
        {
            ResourcePoolManager.Instance.DestroyGameObject(outInfo.EffectObject, objectType.effect);
            dic.Remove(name);
        }
    }


    #region OncePlay
    public void PlayDoOnceEffect(string name, Transform parent)
    {
        ResourcePoolManager.Instance.AsyncGetData(name, objectType.effect, null, null, (obj) =>
        {
            GameObject effect = (GameObject)obj;
            effect.transform.SetParent(parent);

            effect.transform.localPosition = Vector3.zero;
            effect.transform.localRotation = Quaternion.identity;

            effect.SetActive(true);
            StartCoroutine(DoOnceEffect(effect));
        }, "");
    }

    private IEnumerator DoOnceEffect(GameObject effect)
    {
        effect.SetActive(true);
        ParticleSystem particle = effect.GetComponentInChildren<ParticleSystem>();

        Debug.LogError($"Start Do Once Effect {effect.name}");

        yield return new WaitUntil(() => particle.isPlaying == false);
        effect.SetActive(false);

        ResourcePoolManager.Instance.DestroyGameObject(effect, objectType.effect);
    }
    #endregion

    #region LoopPlay

    #endregion

    #region Projectile
    public void GetProjectileObject(string name, Action<UnityEngine.Object> action)
    {
        ResourcePoolManager.Instance.AsyncGetData(name, objectType.gameobject ,null, null, action, "");
    }
    public void DestroyEffect(GameObject gameObject)
    {
        ResourcePoolManager.Instance.DestroyGameObject(gameObject, objectType.effect);
    }
    #endregion
}
