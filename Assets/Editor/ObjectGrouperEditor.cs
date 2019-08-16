using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


[CustomEditor(typeof(ObjectGrouper))]
public class ObjectGrouperEditor : Editor
{
    string newGroupName = "";
    string textToFind = "";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Group enemies", GUILayout.Height(30)))
        {
            GameObject parent = new GameObject("Enemies");

            GameObject[] obj = FindObjectsOfType<GameObject>();
            foreach (var item in obj)
            {
                if (item.tag == "Enemy")
                    item.transform.SetParent(parent.transform);
            }
        }

        GUILayout.Space(5);
        if (GUILayout.Button("Group players", GUILayout.Height(30)))
        {
            GameObject parent = new GameObject("Players");

            GameObject[] obj = FindObjectsOfType<GameObject>();
            foreach (var item in obj)
            {
                if (item.tag == "Player")
                    item.transform.SetParent(parent.transform);
            }
        }

        GUILayout.Space(5);
        if (GUILayout.Button("Group environment", GUILayout.Height(30)))
        {
            GameObject parent = new GameObject("Environment");

            GameObject[] obj = FindObjectsOfType<GameObject>();
            foreach (var item in obj)
            {
                if (item.gameObject.name.Contains("FloorCube") || item.gameObject.name.Contains("Wall"))
                    item.transform.SetParent(parent.transform);
            }
        }

        GUILayout.Space(5);
        if (GUILayout.Button("Group Interactables", GUILayout.Height(30)))
        {
            GameObject parent = new GameObject("Interactable");

            GameObject[] obj = FindObjectsOfType<GameObject>();
            foreach (var item in obj)
            {
                if (item.tag == "Interactable")
                    item.transform.SetParent(parent.transform);
            }
        }

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Custom group", EditorStyles.boldLabel);
        newGroupName = EditorGUILayout.TextField("Group name: ", newGroupName);
        textToFind = EditorGUILayout.TextField("GameObject name contains: ", textToFind);

        if (GUILayout.Button("Group Custom", GUILayout.Height(30)))
        {
            if (newGroupName.Length == 0 || textToFind.Length == 0)
                return;

            GameObject parent = new GameObject(newGroupName);

            GameObject[] obj = FindObjectsOfType<GameObject>();
            foreach (var item in obj)
            {
                //Don't modify nested children.
                if (item.transform.parent != null)
                    continue;

                if (item.gameObject.name.Contains(textToFind))
                    item.transform.SetParent(parent.transform);
            }
        }
    }
}
