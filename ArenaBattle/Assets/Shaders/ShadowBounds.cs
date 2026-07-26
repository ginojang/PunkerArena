using UnityEngine;
using System.Collections;

public class ShadowBounds : MonoBehaviour
{
    public Light mainDirectionalLight;
    [Range(0.0f, 20.0f)]
    public float customRatio = 1.0f;

    Bounds _origBounds;
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        if (mesh != null) _origBounds = mesh.bounds;
        SetBounds();
    }

    void SetBounds()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        if (mesh == null) return;

        float ratio = 2.0f;
        if (mainDirectionalLight != null)
        {
            ratio = 4.0f - Vector3.Dot(Vector3.down, mainDirectionalLight.transform.forward) * 3.0f;
        }
        ratio *= customRatio;
        mesh.bounds = new Bounds(_origBounds.center, new Vector3(_origBounds.size.x * ratio, _origBounds.size.y * ratio, _origBounds.size.z * ratio));
    }

#if UNITY_EDITOR
    float _prevCustomRatio = 0.0f;
    void Update()
    {
        if (customRatio != _prevCustomRatio)
        {
            SetBounds();
            _prevCustomRatio = customRatio;
        }
    }
#endif
}