using UnityEngine;
using System.Collections;

// Cartoon FX  - (c) 2013, Jean Moreno

// Automatically destructs an object when it has stopped emitting particles and when they have all disappeared from the screen.
// Check is performed every 0.5 seconds to not query the particle system's state every frame.
// (only deactivates the object if the OnlyDeactivate flag is set, automatically used with CFX Spawn System)

[RequireComponent(typeof(ParticleSystem))]
public class CFX_AutoDestructShuriken : MonoBehaviour
{
    public float time;

    private ParticleSystem m_ParticleSystem = null;

    void OnEnable()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();

        StartCoroutine("CheckIfAlive");
    }

    IEnumerator CheckIfAlive()
    {
        if (m_ParticleSystem == null)
            yield break;

        while (true)
        {
            yield return new WaitForSeconds(time);
            if (!m_ParticleSystem.IsAlive(true))
            {
                GameObject.Destroy(this.gameObject);
                break;
            }
        }
    }
}
