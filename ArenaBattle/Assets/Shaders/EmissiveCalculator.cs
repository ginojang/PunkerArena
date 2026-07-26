using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EmissiveCalculator : MonoBehaviour
{
    protected string PROPERTY_NAME = "_EmissiveParameter";
    protected float _currentTime;
    protected int _propertyID;
    protected List<Material> _cachedMaterials = new List<Material>();
    protected List<Vector4> _cachedDefaultParameters = new List<Vector4>();

    void Start()
    {
        _propertyID = Shader.PropertyToID(PROPERTY_NAME);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers == null) return;

        for (int i = 0; i < renderers.Length; ++i)
        {
            for (int j = 0; j < renderers[i].materials.Length; ++j)
            {
                if (renderers[i].materials[j].HasProperty(_propertyID))
                {
                    _cachedMaterials.Add(renderers[i].materials[j]);
                    _cachedDefaultParameters.Add(renderers[i].materials[j].GetVector(_propertyID));
                }
            }
        }
    }

    void Update()
    {
        if (_cachedMaterials.Count == 0)
            return;

        Vector4 resultValue = Vector4.zero;
        for (int i = 0; i < _cachedMaterials.Count; ++i)
        {
            _currentTime += Time.deltaTime;

#if UNITY_EDITOR
            _cachedDefaultParameters[i] = _cachedMaterials[i].GetVector(_propertyID);
#endif

            resultValue = _cachedDefaultParameters[i];
            resultValue.z = Mathf.Cos(_currentTime * _cachedDefaultParameters[i].x);
            resultValue.w = Mathf.Cos(_currentTime * _cachedDefaultParameters[i].y);
            _cachedMaterials[i].SetVector(_propertyID, resultValue);
        }
    }
}