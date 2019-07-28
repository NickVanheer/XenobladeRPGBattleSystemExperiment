using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string Key;

    void Start()
    {
        if (LocalizationManager.Instance.GetIsReady() == false)
            return;
        UpdateLocalization();
        GameManager.Instance.OnLocalizationChanged.AddListener(() => UpdateLocalization());
    }

    public void UpdateLocalization()
    {
        Text text = GetComponent<Text>();
        TextMesh textMesh = GetComponent<TextMesh>();

        if (text == null && textMesh == null)
        {
            Debug.LogWarning("Can't find a text property when assigning localization key ' " + Key + "' to GameObject " + this.name);
            return;
        }

        if(text != null)
            text.text = LocalizationManager.Instance.GetLocalizedValue(Key);

        if (textMesh != null)
            textMesh.text = LocalizationManager.Instance.GetLocalizedValue(Key);
    }

}