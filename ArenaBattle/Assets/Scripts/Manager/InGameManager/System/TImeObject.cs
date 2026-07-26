using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TImeObject : MonoBehaviour
{
    private MeshRenderer mesh = null;
    
    private void Awake()
    {
        if (mesh == null)
            mesh = GetComponent<MeshRenderer>();
        
        Messenger.AddListener<Color>(Definition.ChangeTimeColor, ChangeTimeColor);
    }

    private void ChangeTimeColor(Color color)
    {
        Material[] materials = mesh.materials;

        for (int i = 0; i < materials.Length; i++)
        {
            Tweener tween = materials[i].DOColor(color, "_MainColor", 1f);
            tween.onKill = () =>
            {

            };
        }
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<Color>(Definition.ChangeTimeColor, ChangeTimeColor);
    }
}
