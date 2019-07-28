using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

/************ NovaUIElement ****************

   Base class that can be applied to UI elements. Contains a small state system, functions to open and close, 
   an initialize function which can be used to initialize UI elements manually that start unabled and which Awake/Start function won't get called until it's enabled.

****************************************************/

public class NovaUIElement : MonoBehaviour {

    // Use this for initialization
    UnityAction DialogClosedAction; //event that gets fired when the dialog closes

    void Start () {
        Initialize();
	}

    //When GameObject is inactive, Start won't get called automatically. Manual function
    public virtual void Initialize()
    {

    }

    // Update is called once per frame
    public virtual void Update () {
    }

    public void SetCloseEvent(UnityAction uAction)
    {
        DialogClosedAction = uAction;
    }


    public virtual void Close(bool unpause = true, float delay = 0f)
    {
        StartCoroutine(Destroy(unpause, delay));
    }

    public IEnumerator Destroy(bool unpause, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        string eventString = "";

        if (DialogClosedAction != null)
        {
            DialogClosedAction.Invoke();
            eventString += "(event: " + DialogClosedAction.ToString() + ")";
        }

        if (unpause)
            GameManager.Instance.UnpauseGame();

        Debug.Log("Closing and removing UI Element. Called event: " + eventString);

        //Hide();
        GameObject.Destroy(this.gameObject);
    }

    public virtual void Show(bool pauseGame = false)
    {
        this.gameObject.SetActive(true);

        if(pauseGame)
            GameManager.Instance.PauseGame();
    }

    public virtual void Hide(bool unpauseGame = false)
    {
        this.gameObject.SetActive(false);

        if (unpauseGame)
            GameManager.Instance.UnpauseGame();
    }


    public void Toggle()
    {
        if (this.gameObject.activeInHierarchy)
            Hide();
        else
            Show();
    }

    public virtual void BringToFront()
    {
        transform.SetAsLastSibling();
    }

    public virtual void FadeOut()
    {
        //ImageComponent.CrossFadeAlpha(0.0f, 0.4f, false);
    }

    public virtual void FadeIn()
    {
        //ImageComponent.CrossFadeAlpha(1.0f, 0.1f, false);
    }

}
