using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LocalizedTextEditor : EditorWindow
{
    public LocalizationData LocalizationData;
    string filename = "localization.json";
    string filePath;
    Vector2 scrollPosition;
    string key;
    string english;
    string japanese;

    bool isTextLimited = false;
    bool limitMessageBox = false;
    bool limitSkillDescription = false;
    int maxTextCount = 99;

    float sideMargins = 10;
    bool hasFile = false;
    bool isEditing = false;
    string keyToEdit;

    [MenuItem("Window/Localization Editor")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(LocalizedTextEditor)).Show();
    }

    private void OnGUI()
    {
        GUILayout.Space(sideMargins);

        GUILayout.BeginHorizontal(GUILayout.Height(30));
        GUILayout.Space(sideMargins);

        if (GUILayout.Button("Create new localization data"))
            CreateNewData();
        if (GUILayout.Button("Open data file"))
            LoadGameData();
        GUILayout.Space(sideMargins);
        GUILayout.EndHorizontal();

        if (!hasFile)
            return;

        //
        GUILayout.Space(50);
        EditorGUILayout.LabelField("Data input", EditorStyles.boldLabel);

        //Text limits and its input
        GUILayout.Label("Limit character count");
        GUILayout.BeginHorizontal(GUILayout.Height(20));
        GUILayout.Space(sideMargins);
        isTextLimited = EditorGUILayout.BeginToggleGroup("Limit character count", isTextLimited);
        limitMessageBox = EditorGUILayout.Toggle("MessageBox text (" + CoreUIManager.MessageBoxMaxCharacter + ")", limitMessageBox);
        limitSkillDescription = EditorGUILayout.Toggle("Skill Description text (" + CoreUIManager.SkillDescriptionMaxCharacter + ")", limitSkillDescription);
        EditorGUILayout.EndToggleGroup();
        GUILayout.Space(sideMargins);
        GUILayout.EndHorizontal();

        if (limitSkillDescription && limitMessageBox)
        {
            limitMessageBox = false;
            limitSkillDescription = false;
        }

        if(limitMessageBox)
            maxTextCount = CoreUIManager.MessageBoxMaxCharacter;

        if (limitSkillDescription)
            maxTextCount = CoreUIManager.SkillDescriptionMaxCharacter;

        if (LocalizationData != null)
        {
            EditorGUI.HelpBox(new Rect(20, 35, 350, 40), "Now editing: " + filename, MessageType.Info);
           
            if(isEditing)
                EditorGUILayout.LabelField("Editing key: " + key, EditorStyles.boldLabel);
            else
                key = EditorGUILayout.TextField("Key: ", key);

            GUILayout.Label("English");
            english = EditorGUILayout.TextArea(english, GUILayout.Height(40));
            GUILayout.Label("Japanese");
            japanese = EditorGUILayout.TextArea(japanese, GUILayout.Height(40));

            if (isTextLimited)
            {
                if (english.Length > maxTextCount)
                    english = english.Substring(0, maxTextCount);
                if (japanese.Length > maxTextCount)
                    japanese = japanese.Substring(0, maxTextCount);
            }

            string labelName = isEditing ? "Edit" : "Add";
            //
            if (GUILayout.Button(labelName, GUILayout.Height(30)))
            {
                if(isEditing)
                {
                    foreach (var item in LocalizationData.Items)
                    {
                        if(item.Key == keyToEdit)
                        {
                            item.ValueJapanese = japanese;
                            item.ValueEnglish = english;
                        }
                    }
                    isEditing = false;
                    return;
                }

                bool exists = false;
                foreach (var item in LocalizationData.Items)
                {
                    if (item.Key == key)
                        exists = true;
                }

                if(!exists)
                {
                    LocalizationItem newItem = new LocalizationItem();
                    newItem.Key = key;
                    newItem.ValueJapanese = japanese;
                    newItem.ValueEnglish = english;
                    LocalizationData.Items.Add(newItem);

                    //reset values.
                    key = "";
                    japanese = "";
                    english = "";
                }
                else
                {
                    EditorUtility.DisplayDialog("Duplicate", "Key already exists. Please edit it in the hierarchy when needed.", "Ok");
                }
            }

            EditorGUILayout.LabelField("Data view", EditorStyles.boldLabel);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("LocalizationData");
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();

            GUILayout.EndScrollView();
        }

        GUILayout.BeginHorizontal(GUILayout.Height(20));
        GUILayout.Space(sideMargins);

        if (GUILayout.Button("Save Current"))
        {
            SaveGameDataToSameFileName();
        }

        if (GUILayout.Button("Save as new"))
        {
            SaveGameData();
        }
        GUILayout.Space(sideMargins);
        GUILayout.EndHorizontal();

        //Edit
        GUILayout.BeginHorizontal(GUILayout.Height(30));
        GUILayout.Space(sideMargins);

        if (GUILayout.Button("Edit entry"))
        {
            LocalizedEditWindow window = (LocalizedEditWindow)EditorWindow.GetWindow(typeof(LocalizedEditWindow), true, "Localization Keys");

            List<string> keys = new List<string>();
            foreach (var item in LocalizationData.Items)
            {
                keys.Add(item.Key);
            }

            window.AddKeys(keys);
            window.OnEdit = new UnityEngine.Events.UnityEvent();
            window.OnEdit.AddListener(() => 
            {
                keyToEdit = window.KeyToEdit;
                EditorUtility.DisplayDialog("Confirm", "Now editing: " + keyToEdit, "Ok");
                isEditing = true;

                foreach (var item in LocalizationData.Items)
                {
                    if (item.Key == keyToEdit)
                    {
                        key = item.Key;
                        english = item.ValueEnglish;
                        japanese = item.ValueJapanese;
                    }
                }

                window.Close();
                Repaint();
            });
        }

        if (isEditing && GUILayout.Button("Stop editing"))
        {
            isEditing = false;
            Repaint();
        }
        GUILayout.Space(sideMargins);
        GUILayout.EndHorizontal();
    }

    private void LoadGameData()
    {
        filePath = EditorUtility.OpenFilePanel("Open localization data file", Application.streamingAssetsPath, "json");
        key = "";
        english = "";
        japanese = "";

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            filename = Path.GetFileName(filePath);
            LocalizationData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
            hasFile = true;
        }
    }

    private void SaveGameData()
    {
        filePath = EditorUtility.SaveFilePanel("Save As", Application.streamingAssetsPath, filename, "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = JsonUtility.ToJson(LocalizationData);
            File.WriteAllText(filePath, dataAsJson);
            hasFile = true;
        }
    }

    private void SaveGameDataToSameFileName()
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = JsonUtility.ToJson(LocalizationData);
            File.WriteAllText(filePath, dataAsJson);
            hasFile = true;
        }
    }

    private void CreateNewData()
    {
        LocalizationData = new LocalizationData();
        LocalizationData.Items = new List<LocalizationItem>();
        filename = "localization.json";

        SaveGameData();
    }

}