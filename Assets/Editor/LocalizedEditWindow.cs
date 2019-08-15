using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class LocalizedEditWindow : EditorWindow {

    public UnityEvent OnEdit;
    public string KeyToEdit = "Tost";
    List<string> EditKeys = new List<string>();
    Vector2 scrollPosition;

    private void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        foreach (var key in EditKeys)
        {
            if (GUILayout.Button(key))
            {
                EditKey(key);
            }
        }

        GUILayout.EndScrollView();
    }

    private void EditKey(string keyName)
    {
        KeyToEdit = keyName;
        OnEdit.Invoke();
    }

    public void AddKeys(List<string> keys)
    {
        EditKeys = keys;
        Repaint();
    }
}
