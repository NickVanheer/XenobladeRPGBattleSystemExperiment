using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable] 
public abstract class Event
{
    public bool IsFinished = false;
    public bool IsStarted = false;

    public Event NextEvent;

    public virtual void EventStart()
    {
        IsFinished = false;
        IsStarted = true;

        NextEvent = null;
    }

    public virtual void Update()
    {
    }

    public virtual void EventEnd()
    {
        IsFinished = true;
    }
}

[Serializable]
public class MessageBoxEvent : Event
{
    public string MessageBoxText;

    public MessageBoxEvent()
    {
        MessageBoxText = "";
    }

    public MessageBoxEvent(string text)
    {
        MessageBoxText = text;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void EventStart()
    {
        base.EventStart();
        CoreUIManager.Instance.ShowMessageBox(MessageBoxText, OnClose);
    }

    public void OnClose()
    {
        EventEnd();
    }
}


[Serializable]
public class QuickInfoBoxEvent : Event
{
    public string QuickInfoBoxText;
    public float Delay;

    public QuickInfoBoxEvent()
    {
        QuickInfoBoxText = "";
        Delay = 0f;
    }
    public QuickInfoBoxEvent(string text, float delay)
    {
        QuickInfoBoxText = text;
        Delay = delay;
    }

    public override void EventStart()
    {
        base.EventStart();
        CoreUIManager.Instance.ShowQuickInfoPanel(QuickInfoBoxText, Delay, OnClose);
    }

    public void OnClose()
    {
        EventEnd();
    }
}

[Serializable]
public class ActionEvent : Event
{
    public Action MethodToFire;

    public ActionEvent()
    {

    }
    public ActionEvent(Action action)
    {
        this.MethodToFire = action;
    }

    public override void EventStart()
    {
        base.EventStart();
        MethodToFire.Invoke();
        EventEnd();
    }
}

[Serializable]
public class ChangeLightEvent : Event
{
    public bool IsFadeIn = false;
    public float Duration = 3f;

    public ChangeLightEvent(bool isFadeIn, float duration)
    {
        this.IsFadeIn = isFadeIn;
        this.Duration = duration;
    }

    public override void EventStart()
    {
        base.EventStart();
        GameManager.Instance.GetComponent<SmoothLightChanger>().ChangeLight(IsFadeIn, Duration, 0.1f, 3f, OnClose);
    }

    public void OnClose()
    {
        EventEnd();
    }
}

[Serializable]
public class FocusEvent : Event
{
    public GameObject gFocusObject;
    public float Duration = 3f;
    public FocusMode Mode;

    public FocusEvent(GameObject target, float duration, FocusMode mode)
    {
        this.gFocusObject = target;
        this.Duration = duration;
        this.Mode = mode;
    }

    public override void EventStart()
    {
        base.EventStart();

        if(Mode == FocusMode.Slide)
            Camera.main.GetComponent<FocusCamera>().SlideCameraFocus(gFocusObject, Duration, OnClose);

        if (Mode == FocusMode.Static)
            Camera.main.GetComponent<FocusCamera>().StaticCameraFocus(gFocusObject, Duration, OnClose);
    }

    public void OnClose()
    {
        EventEnd();
    }
}

[Serializable]
public class ChangeSceneEvent : Event
{
    public string SceneToLoad;
    public ChangeSceneEvent(string sceneName)
    {
        this.SceneToLoad = sceneName;
    }

    public override void EventStart()
    {
        base.EventStart();
        SceneManager.LoadScene(7);
        EventEnd();
    }
}


[Serializable]
public class WaitEvent : Event
{
    public float WaitTime;

    public WaitEvent()
    {
        WaitTime = 0f;
    }

    public WaitEvent(float waitDuration)
    {
        WaitTime = waitDuration;
    }

    public override void Update()
    {
        WaitTime -= Time.deltaTime;
        Debug.Log("Wait event running");

        if (WaitTime <= 0)
            EventEnd();
    }
}


public class EventQueue : MonoBehaviour {

    public Queue<Event> gameEvents;
    public Event activeEvent;

    public bool IsDebug = false;
    public List<string> DebugEventVisualizer;

    private static EventQueue instance;
    public static EventQueue Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EventQueue();
            }
            return instance;
        }
    }

    public bool IsBusy
    {
        get
        {
            if (activeEvent != null)
                return true;
            else
                return false;
        }
    }

    void Awake()
    {
        // First we check if there are any other instances conflicting
        if (instance != null && instance != this)
        {
            Debug.Log("There's already a Event Queue in the scene, destroying this one. Existing instance: " + instance.gameObject.name + ", new instance: " + this.gameObject.name);
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            Debug.Log("Event Queue woke");
        }
    }

    // Use this for initialization
    void Start () {
        gameEvents = new Queue<Event>();
        DebugEventVisualizer = new List<string>();
    }
	
	void Update () {

        //check if we need to do something

        if (activeEvent != null)
        {
            if (activeEvent.IsFinished)
            {
                activeEvent.EventEnd();
                activeEvent = null;
                gameEvents.Dequeue(); //remove last event
            }
            else
            {
                if (!activeEvent.IsStarted)
                    activeEvent.EventStart();

                activeEvent.Update();
            }
        }

        if (gameEvents.Count > 0)
        {
            activeEvent = gameEvents.Peek();
        }

        //debug
        if(IsDebug)
        {
            DebugEventVisualizer.Clear();
            foreach (var ev in gameEvents)
            {
                DebugEventVisualizer.Add(ev.ToString());
            }
        }
    }


	public void AddMoney(int money)
	{
	}

    public void AddQuest(string questTag)
    {
    }

    public void ChangeScene(string levelName)
    {
        gameEvents.Enqueue(new ChangeSceneEvent(levelName));
    }

    public void AddAction(Action method)
    {
        ActionEvent ev = new ActionEvent(method);
        gameEvents.Enqueue(ev);
    }

    public void AddFocusEvent(GameObject target, float duration, FocusMode mode)
    {
        FocusEvent fe = new FocusEvent(target, duration, mode);
        gameEvents.Enqueue(fe);
    }

    public void AddMessageBox(string text, float messageSpeed)
    {
        var e = new MessageBoxEvent(text);

        if (gameEvents.Count > 0)
            gameEvents.Peek().NextEvent = e;

        gameEvents.Enqueue(e);
    }

    public void AddQuickInfoPanel(string text, float delayTime)
    {
        gameEvents.Enqueue(new QuickInfoBoxEvent(text, delayTime));
    }

    public void ChangeLightIntensity(bool isFadeIn, float duration)
    {
        gameEvents.Enqueue(new ChangeLightEvent(isFadeIn, duration));
    }

    private void SerializeElement(string filename)
    {
        XmlSerializer ser = new XmlSerializer(typeof(Queue<Event>));
        
        TextWriter writer = new StreamWriter(filename);
        ser.Serialize(writer, gameEvents);
        writer.Close();
    }

    private void Save<T>(string filename, object data)
    {
        Type[] extraTypes = { typeof(MessageBoxEvent), typeof(WaitEvent), typeof(QuickInfoBoxEvent) };
        var serializer = new XmlSerializer(typeof(T), extraTypes);

        FileStream file = File.Create(Application.persistentDataPath + "/" + filename + ".dat");

        //TODO: make secure with try catch/using?
        serializer.Serialize(file, data);
        file.Close();

        Debug.Log(filename + " Save File saved at " + Application.persistentDataPath);

    }

    public void WaitABit(float duration)
    {
        gameEvents.Enqueue(new WaitEvent(duration));
    }
}
