using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

/************ ModalMessageBox ****************

    A messagebox class. 

    Main Functions
    - ResetDialogue()
    - AddDialogue()
    - Show()

    You can pass in options to present dialog choices and pass in a UnityAction function prototype to control behavior of what happens when the action is selected.
    You can add them with AddDialogOption() before calling Show()

****************************************************/

public class ModalMessageBox : NovaUIElement {

    public Text Message;

    //
    public GameObject DialogOptionsHolder; //the container to instantiate them in
    public GameObject DialogueWindow; //The overlapping window that should be animated
    public Color HighlightColor = Color.yellow;
    private List<GameObject> dialogOptionObjects; //the in-code instantiated text objects
    private List<UnityAction> dialogOptionActions; //the actions related to the in-code instantiated text objects
 
    //Selection option
    private bool isSelectingOption = false;
    private int selectedIndex = 0;

    private List<string> dialogueLines;

    //false: modal single message
    private int dialogueIndex = 0;

    //
    private string textToShow = "";
    private int textCharacterIndex = 0;
    public float characterScrollTimer = 0.02f;
    private float characterScrollCurrentTime = 0;

    bool isInitialized = false;

    // Use this for initialization


    public override void Initialize()
    {
        if (!isInitialized)
        {
            base.Initialize();
            dialogOptionObjects = new List<GameObject>();
            dialogOptionActions = new List<UnityAction>();
            dialogueLines = new List<string>();

            isInitialized = true;
        }
    }

    #region options

    public void AddDialogOption(string text, UnityAction action)
    {
        //GameObject gO = Instantiate(CommandLineToInstantiate) as GameObject;
        //Manually create this so we might have more control when needed.
        GameObject gO = new GameObject();
        gO.AddComponent<RectTransform>();
        gO.AddComponent<LayoutElement>();

        Text t = gO.AddComponent<Text>();
        t.text = text;
        t.font = CoreUIManager.Instance.GameFont;
        t.fontSize = 33;
        //t.resizeTextForBestFit = true;

        gO.transform.SetParent(DialogOptionsHolder.transform);
        gO.transform.localScale = new Vector3(1, 1, 1);

        //set disabled by default
        gO.SetActive(false);
        dialogOptionObjects.Add(gO);
        dialogOptionActions.Add(action);

    }

    private void disableModalOptions()
    {
        foreach (var item in dialogOptionObjects)
        {
            item.SetActive(false);

        }

        isSelectingOption = false;
    }

    public void DeleteModalOptionsAndActions()
    {
        if (dialogOptionObjects == null)
            dialogOptionObjects = new List<GameObject>();

        if (dialogOptionActions == null)
            dialogOptionActions = new List<UnityAction>();

        //removes all text objects
         for (int i = 0; i < dialogOptionObjects.Count; i++)
        {
            Destroy(dialogOptionObjects[i]);
        }

        dialogOptionObjects.Clear();

        //removes all action objects
        dialogOptionActions.Clear();

        isSelectingOption = false;
        selectedIndex = 0;
    }

    private void enableModalOptions()
    {
        foreach (var item in dialogOptionObjects)
        {
            item.SetActive(true);
        }

        isSelectingOption = true;
        selectedIndex = 0;

        //We make this slightly larger 
        if (DialogueWindow != null)
        {
            int increase = dialogOptionObjects.Count * 30;
            DialogueWindow.GetComponent<RectTransform>().sizeDelta = new Vector2(DialogueWindow.GetComponent<RectTransform>().sizeDelta.x, DialogueWindow.GetComponent<RectTransform>().sizeDelta.y + increase);
        }
    }
    #endregion

    // Update is called once per frame
    public override void Update () {

        if (Input.GetKeyDown(KeyCode.Return))
            MoveNext();

        if (Input.GetMouseButton(0))
            MoveNext();

        if (textToShow.Length > 0)
        {
            //scrolling text
            characterScrollCurrentTime += Time.deltaTime;
            float t = Time.deltaTime;
            if (characterScrollCurrentTime >= characterScrollTimer)
            {
                characterScrollCurrentTime = 0; //reset
                textCharacterIndex += 4;

                textCharacterIndex = Mathf.Clamp(textCharacterIndex, 0, textToShow.Length);
                //add one character to string 
            }

            //set message text 
            Message.text = textToShow.Substring(0, textCharacterIndex);
        }

        //for when there are dialogue options to chose from
        if (!isSelectingOption)
            return;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            MoveCursor(1);

        if (Input.GetKeyDown(KeyCode.UpArrow))
            MoveCursor(-1);

        for (int i = 0; i < dialogOptionObjects.Count; i++)
        {
            Color colorToUse = Color.white;

            if (i == selectedIndex)
                colorToUse = HighlightColor;

            dialogOptionObjects[i].GetComponent<Text>().color = colorToUse;
        }
    }

    //0 black. 100 dark transparent. 255 full black
    public void SetBackgroundFill(float value)
    {
        GetComponent<Image>().color = new Color(0,0,0,value);
    }

    public void ResetDialogue()
    {
        //if they haven't been initialized already
        if (dialogueLines == null)
            dialogueLines = new List<string>();

        dialogueLines.Clear();

        DeleteModalOptionsAndActions(); 
    }

    public void AddDialogue(string text)
    {
        dialogueLines.Add(text);
    }

    //Just show the dialog box, needs to have called AddDialogue() before, can be multiple lines too. 
    public override void Show(bool pauseGame = false)
    {
        base.Show(pauseGame);
        dialogueIndex = 0;

        Debug.Assert(dialogueLines.Count > 0);

        displayNewText(dialogueLines[dialogueIndex]);
    }

    //shows dialog, split lines with \n
    public void Show(string txt, bool pauseGame = false)
    {
        base.Show(pauseGame);

        if(DialogueWindow != null)
        {
            Animator anim = DialogueWindow.GetComponent<Animator>();
            anim.Play("ModalPanelAnimation");
        }

        dialogueIndex = 0;

        var lines = txt.Split('\n');

        foreach (var line in lines)
        {
            AddDialogue(line);
        }

        displayNewText(dialogueLines[dialogueIndex]);
        Debug.Log("Showing MessageBox (text: " + txt + ") and waiting for user input.");
    }

    private void displayNewText(string text)
    {
        base.Show(true);
        BringToFront();
        textToShow = text;
        textCharacterIndex = 0;

        //clear text
        Message.text = "";

        //there are dialog objects and we're at the last line
        if (dialogOptionObjects.Count > 0 && dialogueIndex == dialogueLines.Count - 1)
            enableModalOptions();

    }

    private void ExecuteOption()
    {
        if(isSelectingOption)
        {
            UnityAction action = dialogOptionActions[selectedIndex];
            action.Invoke();
            Close(true, 0f);
        }
    }

    public void MoveNext()
    {
        dialogueIndex++;

        if (dialogueIndex >= dialogueLines.Count)
        {
            if (dialogOptionObjects.Count > 0)
            {
                ExecuteOption();
            }
            else
            {
                Close(true, 0f);
                return;
            }
        }
        else
        {
            displayNewText(dialogueLines[dialogueIndex]);
        }
    }

    //moves cursor when selecting an option in a message box which has multiple options set with AddDialogOption
    public void MoveCursor(int value)
    {
        selectedIndex += value;
        selectedIndex = Mathf.Clamp(selectedIndex, 0, dialogOptionObjects.Count - 1);
    }
}
