using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Landscape : MonoBehaviour {

    public int renderQueue = 1500;

	// Use this for initialization
	void Start ()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null) return;

#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            for (int i = 0; i < renderer.materials.Length; ++i)
                renderer.materials[i].renderQueue = renderQueue;
        }
        else
        {
            for (int i = 0; i < renderer.sharedMaterials.Length; ++i)
                renderer.sharedMaterials[i].renderQueue = renderQueue;
        }
#else
        for (int i = 0; i < renderer.materials.Length; ++i)
            renderer.materials[i].renderQueue = renderQueue;
#endif
    }
}
