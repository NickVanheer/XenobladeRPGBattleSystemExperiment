using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

/*************************** UIManager *********************************

    CoreUIManager class contains core UI functions, shared between all scenes

************************************************************************/

public enum CanvasMode
{
    SelectCanvas, CurrentGameObject
}
public class CoreUIManager : MonoBehaviour {

    private static CoreUIManager instance;
    public static CoreUIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CoreUIManager();
            }
            return instance;
        }
    }

    public GameObject CurrentCanvas;

    //core
    [Header("[GENERAL] Core Parameters")]
    public GameObject QuickInfoPanel;
    public GameObject ModalPanel;
    public GameObject BigTextPrefab;

    [Header("[GENERAL] Battle UI")]
    public GameObject TargetDisplay;
    public GameObject TargetMarker;
    public GameObject SkillbarDisplay;
    public GameObject PlayerHealthPanel;
    
    public GameObject DamageTextPrefab;
    public GameObject TextLabelPrefab;
    public GameObject RedTextLabelPrefab;
    public GameObject SpecialDamageTextPrefab;
    public GameObject PlayerBoxPrefab;
    public GameObject PlayerBoxMiniPrefab;
    public GameObject OverheadHealthBar;

    [Header("[GENERAL] Color Scheme")]
    public Color EnemyTakeDamageColor;
    public Color SpecialDamageColor;
    public Color PlayerTakeDamageColor;
    public Color RestoreHealthColor;

    public GameObject ParticleMuzzle;
    public GameObject ParticleBounce;
    public GameObject ParticleFire;
    public GameObject ParticleDeath;
    public GameObject ParticleNewPlayer;

    public SkillbarController SkillBarControl;

    public GameObject RevivePromptGO;    
    public const int BaseFontSize = 30;

    public const int MessageBoxMaxCharacter = 40;
    public const int SkillDescriptionMaxCharacter = 20;

    public Font GameFont;

    public CanvasMode CurrentCanvasMode;

    private GameObject GetCurrentCanvas()
    {
        if (CurrentCanvasMode == CanvasMode.CurrentGameObject)
            return this.gameObject;

        if (CurrentCanvasMode == CanvasMode.SelectCanvas)
            return CurrentCanvas;

        return null;
    }

    public void ShowRevivePrompt()
    {
        if (this.RevivePromptGO != null)
            this.RevivePromptGO.SetActive(true);
    }

    public void HideRevivePrompt()
    {
        if(this.RevivePromptGO != null)
            this.RevivePromptGO.SetActive(false);
    }

    void Awake()
    {
        // First we check if there are any other instances conflicting
        if (instance != null && instance != this)
        {
            Debug.Log("There's already a Core UI Manager in the scene, destroying this one.");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            Debug.Log("CoreUIManager woke");

            //DontDestroyOnLoad(gameObject);

            if (ModalPanel == null)
                Debug.Log("No modal panel on UI Manager");
        }
    }

    public void ShowYesNoMessageBox(string text, UnityAction yesAction, UnityAction noAction, UnityAction callBack = null)
    {
        if (ModalPanel == null)
            return;

        //disableControl();
        GameObject gO = Instantiate<GameObject>(ModalPanel);
        gO.GetComponent<RectTransform>().SetParent(GetCurrentCanvas().transform, false);
        ModalMessageBox msgBox = gO.GetComponent<ModalMessageBox>();

        msgBox.SetCloseEvent(callBack);
        msgBox.Initialize();

        msgBox.AddDialogOption(LocalizationManager.Instance.GetLocalizedValue("Yes"), yesAction);
        msgBox.AddDialogOption(LocalizationManager.Instance.GetLocalizedValue("No"), noAction);
        msgBox.Show(text, true);
    }

    public void ShowMessageBoxWithOptions(string text, params LocalizedMessageOption[] actions)
    {
        if (ModalPanel == null)
            return;

        //disableControl();
        GameObject gO = Instantiate<GameObject>(ModalPanel);
        gO.GetComponent<RectTransform>().SetParent(GetCurrentCanvas().transform, false);
        ModalMessageBox msgBox = gO.GetComponent<ModalMessageBox>();

        msgBox.SetCloseEvent(null);
        msgBox.Initialize();

        foreach (var action in actions)
        {
            msgBox.AddDialogOption(LocalizationManager.Instance.GetLocalizedValue(action.LocalizedValueTag), action.actionToPerform);
        }

        msgBox.Show(text, true);
    }

    public void ShowMessageBox(string text)
    {
        ShowMessageBox(text, null);
    }

    public void ShowMessageBox(string text, UnityAction callBack = null)
    {
        if (ModalPanel == null)
            return;

        //disableControl();
        GameObject gO = Instantiate<GameObject>(ModalPanel);
        gO.GetComponent<RectTransform>().SetParent(GetCurrentCanvas().transform, false);
        ModalMessageBox msgBox = gO.GetComponent<ModalMessageBox>();

        msgBox.SetCloseEvent(callBack);
        msgBox.Initialize();
        msgBox.Show(text, true);
    }

    /*
    public void TogglePartyMenu()
    {
        PartyMenuPanel.GetComponent<PartyUIManager>().InitializePartyView();
        PartyMenuPanel.SetActive(!PartyMenuPanel.activeInHierarchy);
    }
    */

    public void ShowQuickInfoPanel(string text, float delayTime = 1.5f, UnityAction callBack = null)
    {
        if (QuickInfoPanel == null)
            return;

        GameObject gO = Instantiate<GameObject>(QuickInfoPanel);
        gO.GetComponent<RectTransform>().SetParent(GetCurrentCanvas().transform, false);
        gO.SetActive(true);
        gO.GetComponent<NovaUIElement>().SetCloseEvent(callBack);
        gO.GetComponent<NovaUIElement>().Close(true, delayTime);

        foreach (Transform child in gO.transform)
        {
            if (child.name == "Text")
            {
                Text t = child.GetComponent<Text>();
                t.text = text;
            }
        }
    }

    public GameObject CreateOverheadHealthBar(GameObject target)
    {
        if (OverheadHealthBar == null)
            return null;

        GameObject gO = Instantiate<GameObject>(OverheadHealthBar);
        gO.GetComponent<RectTransform>().SetParent(GetCurrentCanvas().transform, false);
        gO.GetComponent<World3DUIObject>().Target = target.transform;
        gO.GetComponent<SetUIText>().ActorData = target.GetComponent<RPGActor>();
        gO.SetActive(true);

        return gO;
    }

    public void ShowBigText(string text)
    {
        if (BigTextPrefab == null)
            return;

        GameObject gO = Instantiate<GameObject>(BigTextPrefab);
        gO.GetComponent<RectTransform>().SetParent(GetCurrentCanvas().transform, false);
        gO.GetComponent<Text>().text = text;
    }

    public void ShowTargetDisplay(GameObject targetToShow)
    {
        //Set health slider
        foreach (Transform child in TargetDisplay.transform)
        {
            if(child.name == "HealthSlider")
            {
                child.GetComponent<HealthSlider>().ActorToShow = targetToShow.GetComponent<RPGActor>();
            }
        }

        if (TargetMarker != null)
        {
            Transform target = targetToShow.transform.FindDeepChild("Floor");

            TargetMarker.GetComponent<AlignWithTarget>().Target = target != null ? target.gameObject : targetToShow;
            TargetMarker.SetActive(true);
        }

        TargetDisplay.SetActive(true);
    }

    public void HideTargetDisplay()
    {
        if(TargetDisplay != null)
            TargetDisplay.SetActive(false);

        if (TargetMarker != null)
        {
            TargetMarker.SetActive(false);
        }
    }

    public void ShowSkillDisplay()
    {
        SkillbarDisplay.SetActive(true);
    }

    public void HideSkillDisplay()
    {
        if(SkillbarDisplay != null)
            SkillbarDisplay.SetActive(false);
    }

    public Vector2 GetWorldObjectUIPosition(Vector3 worldPosition)
    {
        Vector2 viewport = Camera.main.WorldToViewportPoint(worldPosition);
        float sizeDeltaX = CurrentCanvas.GetComponent<RectTransform>().sizeDelta.x;
        float sizeDeltaY = CurrentCanvas.GetComponent<RectTransform>().sizeDelta.y;
        float screenHeight = Screen.height;

        Vector2 screenPosition = new Vector2
        (
            viewport.x * sizeDeltaX,
            viewport.y * sizeDeltaY
        );

        screenPosition.y = sizeDeltaY - screenPosition.y;
        screenPosition.y = -screenPosition.y;

        return screenPosition;
    }

    public void SpawnText(string text, GameObject target)
    {
        if (DamageTextPrefab != null)
        {
            GameObject gO = Instantiate<GameObject>(DamageTextPrefab);
            gO.GetComponent<RectTransform>().SetParent(GetCurrentCanvas().transform);
            gO.GetComponent<DamageNumber>().SetText(text, target);
        }
    }

    public void AddPlayerBox(RPGActor player)
    {
        if (PlayerBoxPrefab != null)
        {
            GameObject gO = Instantiate<GameObject>(PlayerBoxPrefab);
            gO.GetComponent<RectTransform>().SetParent(PlayerHealthPanel.transform);
            gO.GetComponent<ChangeActorForAllUI>().ActorToDisplay = player;
        }
    }

    public void AddPlayerBoxMini(RPGActor player)
    {
        if (PlayerBoxMiniPrefab != null)
        {
            GameObject gO = Instantiate<GameObject>(PlayerBoxMiniPrefab);
            gO.GetComponent<RectTransform>().SetParent(PlayerHealthPanel.transform);
            gO.GetComponent<ChangeActorForAllUI>().ActorToDisplay = player;
        }
    }

    public void SpawnLabel(string text, GameObject target, bool isRedLabel = false)
    {
        GameObject toInstantiate = TextLabelPrefab;

        if (isRedLabel)
            toInstantiate = RedTextLabelPrefab;

        if (toInstantiate != null)
        {
            GameObject gO = Instantiate<GameObject>(toInstantiate);
            gO.GetComponent<RectTransform>().SetParent(GetCurrentCanvas().transform);
            gO.transform.FindDeepChild("TextLabel").GetComponent<Text>().text = text;

            Vector2 targetPos = Camera.main.WorldToScreenPoint(target.transform.position);
            targetPos.x += Random.Range(-5, 5);
            targetPos.y += Random.Range(-5, 5);
            gO.transform.position = targetPos;
        }
    }

    public void SpawnNewPlayerParticles(Vector3 position)
    {
        if (ParticleNewPlayer == null)
            return;

        Instantiate(ParticleNewPlayer, position, Quaternion.identity);
    }

    public void SpawnLabel(string text, GameObject target, string IconPath, bool isRedLabel = false)
    {
        GameObject toInstantiate = TextLabelPrefab;

        if (isRedLabel)
            toInstantiate = RedTextLabelPrefab;

        if (toInstantiate != null)
        {
            GameObject gO = Instantiate<GameObject>(toInstantiate);
            gO.GetComponent<RectTransform>().SetParent(GetCurrentCanvas().transform);
            gO.transform.FindDeepChild("TextLabel").GetComponent<Text>().text = text;
            Image img = gO.transform.FindDeepChild("Icon").GetComponent<Image>();
            img.sprite = Resources.Load<Sprite>(IconPath);
            img.gameObject.SetActive(true);

            Vector2 targetPos = Camera.main.WorldToScreenPoint(target.transform.position);
            targetPos.x += Random.Range(-5, 5);
            targetPos.y += Random.Range(-5, 5);
            gO.transform.position = targetPos;
        }
    }

    public void SpawnText(string text, GameObject target, Color colorToUse, bool isSpecialText = false)
    {
        if (DamageTextPrefab != null)
        {
            GameObject gO = Instantiate<GameObject>(DamageTextPrefab);
            gO.GetComponent<RectTransform>().SetParent(GetCurrentCanvas().transform);
            gO.GetComponent<DamageNumber>().SetText(text, target, colorToUse);
        }
    }

    public void SpawnText(string text, GameObject target, Color colorToUse, float fontIncrement, bool isSpecialText = false)
    {
        if (DamageTextPrefab != null)
        {
            GameObject gO = Instantiate<GameObject>(DamageTextPrefab);
            gO.GetComponent<RectTransform>().SetParent(GetCurrentCanvas().transform);
            gO.GetComponent<DamageNumber>().SetText(text, target, fontIncrement, colorToUse);
        }
    }

}
