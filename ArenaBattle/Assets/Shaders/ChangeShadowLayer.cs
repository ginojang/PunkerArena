using UnityEngine;
using System.Collections;

public class ChangeShadowLayer : MonoBehaviour
{
    public enum eShadowLayer
    {
        Landscape,
        Shadow,
    }

    public eShadowLayer Layer;

	void Start()
    {
        int nLayer = 1;
        switch(Layer)
        {
            case eShadowLayer.Landscape: nLayer = LayerMask.NameToLayer("Landscape"); break;
            case eShadowLayer.Shadow: nLayer = LayerMask.NameToLayer("EnvShadow"); break;
        }
        gameObject.layer = nLayer;
        for (int i = 0; i < transform.childCount; ++i)
			transform.GetChild(i).gameObject.layer = nLayer;

        // change shader
        if (Layer == eShadowLayer.Shadow)
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                for (int i = 0; i < renderer.materials.Length; ++i)
                {
                    if (renderer.materials[i].shader.name.Contains("Nature"))
                        renderer.materials[i].shader = Shader.Find("Shadow/ProjectionGrass - ZTestAlways");
                    else
                        renderer.materials[i].shader = Shader.Find("Shadow/Projection - ZTestAlways");
                }
            }
        }
	}
}
