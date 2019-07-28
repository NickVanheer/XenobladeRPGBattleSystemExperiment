using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DamageNumber : MonoBehaviour {

    public float TimeToLive = 0.5f;
    public float Speed = 90f;
    public float OffsetRange = 4;
    void Start()
    {
        //Debug.Assert(damageText != null);
    }

    void Update()
    {
        TimeToLive -= Time.deltaTime;
        updateFont();

        if (TimeToLive <= 0)
            GameObject.Destroy(this.gameObject);
    }

    void updateFont()
    {
        transform.Translate(0, Time.deltaTime * Speed, 0);
    }

    public void SetText(string text, GameObject target, UnityEngine.Color? col = null)
    {
        Vector2 targetPos = Camera.main.WorldToScreenPoint(target.transform.position);

        targetPos.x += Random.Range(-OffsetRange, OffsetRange);
        targetPos.y += Random.Range(-OffsetRange, OffsetRange);
        transform.position = targetPos;

        GetComponent<Text>().text = text;
        GetComponent<Text>().color = col ?? Color.green;
    }

    public void SetText(string text, GameObject target, float fontIncrement, UnityEngine.Color? col = null)
    {
        Vector2 targetPos = Camera.main.WorldToScreenPoint(target.transform.position);
 

        targetPos.x += Random.Range(-OffsetRange, OffsetRange);
        targetPos.y += Random.Range(-OffsetRange, OffsetRange);

        transform.position = targetPos;

        int newFontSize = (int)(GetComponent<Text>().fontSize * fontIncrement);
        GetComponent<Text>().fontSize = newFontSize;
        GetComponent<Text>().text = text;
        GetComponent<Text>().color = col ?? Color.white;
    }
}
