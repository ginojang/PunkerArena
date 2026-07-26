using UnityEngine;
using System.Collections;

public class PropertyCurve : MonoBehaviour {

    public string properyName;
    public AnimationCurve curve;
    
    public bool ResetOnStart;
    protected float StartTime;

    protected int propertyID;
    protected Renderer cachedRenderer;
    void Start()
    {
        propertyID = Shader.PropertyToID(properyName);
        cachedRenderer = GetComponent<Renderer>();
        if (ResetOnStart) StartTime = Time.time;
    }

    void Update()
    {
        if (cachedRenderer == null) return;
        float currentTime = Time.time;
        if (ResetOnStart) currentTime -= StartTime;
        float result = curve.Evaluate(currentTime);
        for (int i = 0; i < cachedRenderer.materials.Length; ++i)
        {
            cachedRenderer.materials[i].SetFloat(propertyID, result);
        }
    }
}
