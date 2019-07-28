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

    [MenuItem("Window/Localization Editor")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(LocalizedTextEditor)).Show();
    }

    private void OnGUI()
    {
        GUILayout.Space(sideMargins);
        if (GUILayout.Button("Create new localization data"))
            CreateNewData();
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
            EditorGUI.HelpBox(new Rect(20, 35, 400, 40), "Now editing: " + filename, MessageType.Info);
           
            key = EditorGUILayout.TextField("Key: ", key);
            japanese = EditorGUILayout.TextField("Japanese: ", japanese);
            english = EditorGUILayout.TextField("English: ", english);

            if (isTextLimited)
            {
                if (english.Length > maxTextCount)
                    english = english.Substring(0, maxTextCount);
                if (japanese.Length > maxTextCount)
                    japanese = japanese.Substring(0, maxTextCount);
            }

            //
            if (GUILayout.Button("Add", GUILayout.Height(30)))
            {
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

                    /*
                    List<LocalizationItem> temp = new List<LocalizationItem>();
                    foreach (var item in LocalizationData.items)
                        temp.Add(item);
                    temp.Add(newItem);
                    LocalizationData.items = temp.ToArray();
                    */
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

        GUILayout.BeginHorizontal(GUILayout.Height(30));
        GUILayout.Space(sideMargins);
        if (GUILayout.Button("Save As"))
        {
            SaveGameData();
        }
        if (GUILayout.Button("Open data file"))
        {
            LoadGameData();
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
        }
    }

    private void SaveGameData()
    {
        filePath = EditorUtility.SaveFilePanel("Save As", Application.streamingAssetsPath, filename, "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = JsonUtility.ToJson(LocalizationData);
            File.WriteAllText(filePath, dataAsJson);
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