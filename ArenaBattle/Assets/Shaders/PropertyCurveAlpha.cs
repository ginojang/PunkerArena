using UnityEngine;
using System.Collections;

public class PropertyCurveAlpha : MonoBehaviour {

    public string properyName = "_TintColor";
    public AnimationCurve curve;
    public bool LockRGB;

    public bool ResetOnStart;
    protected float StartTime;

    protected int propertyID;
    protected Renderer cachedRenderer;
    protected Color startColor = Color.white;
    void Start()
    {
        propertyID = Shader.PropertyToID(properyName);
        cachedRenderer = GetComponent<Renderer>();
        if (LockRGB && cachedRenderer.materials.Length > 0) startColor = cachedRenderer.materials[0].GetColor(propertyID);
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
            Color color;
            if (LockRGB) color = startColor;
            else color = cachedRenderer.materials[i].GetColor(propertyID);
            color.a = result;
            cachedRenderer.materials[i].SetColor(propertyID, color);
        }
    }
}
