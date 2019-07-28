using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    private Dictionary<string, string> localizedTextEnglish;
    private Dictionary<string, string> localizedTextJapanese;
    private bool isReady = false;
    private string missingTextString = "Localized text not found";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        Debug.Log("LocalizationManager Awake!"); 
    }

    public void LoadLocalizedText(string fileName)
    {
        localizedTextEnglish = new Dictionary<string, string>();
        localizedTextJapanese = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.Items.Count; i++)
            {
                localizedTextEnglish.Add(loadedData.Items[i].Key, loadedData.Items[i].ValueEnglish);
                localizedTextJapanese.Add(loadedData.Items[i].Key, loadedData.Items[i].ValueJapanese);
            }

            Debug.Log("Data loaded, dictionary contains: " + localizedTextEnglish.Count + " entries");
        }
        else
        {
            Debug.LogError("Cannot find localization file: " + filePath);
        }

        isReady = true;
    }

    public void Reset()
    {
        isReady = false;
        Instance = null;
    }

    public string GetLocalizedValue(string key)
    {
        if (key == "")
            return "[EMPTY KEY]";

        Dictionary<string, string> dictionaryToUse;

        if (GameManager.Instance.GameLanguage == DisplayLanguage.English)
            dictionaryToUse = localizedTextEnglish;
        else
            dictionaryToUse = localizedTextJapanese;

        string result = missingTextString;
        if (dictionaryToUse.ContainsKey(key))
        {
            result = dictionaryToUse[key].Replace("\\n", "\n"); //Get the text and also add the linebreaks
        }

        return result;
    }

    public bool GetIsReady()
    {
        return isReady;
    }

}