using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CheckUIClick : MonoBehaviour {

    // Normal raycasts do not work on UI elements, they require a special kind
    GraphicRaycaster raycaster;

    void Awake()
    {
        // Get both of the components we need to do this
        this.raycaster = GetComponent<GraphicRaycaster>();
    }

    void Update()
    {
        //Set up the new Pointer Event
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        pointerData.position = Input.mousePosition;
        this.raycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<Command>() != null)
            {
                SkillbarController.Instance.SelectedIndex = result.gameObject.GetComponent<Command>().SkillIndex;
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Command com = GameManager.Instance.GetCommandAtIndex(result.gameObject.GetComponent<Command>().SkillIndex);

                    if (com != null)
                        com.UseCommand();
                }
            }

            if (result.gameObject.GetComponent<QTERingController>() != null)
            {
                QTERingController r = result.gameObject.GetComponent<QTERingController>();
                if (Input.GetKeyDown(KeyCode.Mouse0))
                    r.IsUITouch = true;
            }

            if (result.gameObject.GetComponentInParent<QTERingController>() != null)
            {
                QTERingController r = result.gameObject.GetComponentInParent<QTERingController>();
                if (Input.GetKeyDown(KeyCode.Mouse0))
                    r.IsUITouch = true;
            }
        }
    }
}
