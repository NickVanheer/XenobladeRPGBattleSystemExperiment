using System.Collections.Generic;

[System.Serializable]
public class LocalizationData
{
    public List<LocalizationItem> Items;
}

[System.Serializable]
public class LocalizationItem
{
    public string Key;
    public string ValueEnglish;
    public string ValueJapanese;
}