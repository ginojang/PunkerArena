using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FindMissingScripts : EditorWindow
{
    [MenuItem("WindowHelper/FindMissingScripts")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FindMissingScripts));
    }

    public void OnGUI()
    {
        if(GUILayout.Button("Find Missing Resources"))
        {
            FindInCurrentScene();
        }
    }

    private static void FindInCurrentScene()
    {
        List<GameObject> objectsInScene = new List<GameObject>();
        foreach(GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.hideFlags != HideFlags.None)
                continue;

            objectsInScene.Add(go);
        }

        foreach( GameObject g in objectsInScene )
        {
            var result = IsNullComponents(g);

            if( result.Item2 > 0 )
                Debug.Log($"Searched {g} Gameobjects, {result.Item1} Mission:{result.Item2}");
        }
    }

    private static (int, int) IsNullComponents(GameObject _root)
    {
        int go_count = 0;
        int missing_count = 0;

        Component[] components = _root.GetComponents<Component>();
        go_count = components.Length;

        for( int i = 0; i < components.Length; i++)
        {
            if( components[i] == null )
            {
                missing_count++;
                string s = _root.name;
                Transform t = _root.transform;
                while( t.parent != null )
                {
                    s = t.parent.name + "/" + s;
                    t = t.parent;
                }

                Debug.Log($"{s} has an empty script attached in position: {i} : {_root}");
            }
        }

        return (go_count, missing_count);
    }
}
