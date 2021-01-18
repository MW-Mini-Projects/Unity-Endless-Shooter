using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//We are overriding be base interface with our own
[CustomEditor (typeof (MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        MapGenerator map = target as MapGenerator;

        if (DrawDefaultInspector())
        {
            map.GenerateMap();
        }

        if (GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }

    }
}
