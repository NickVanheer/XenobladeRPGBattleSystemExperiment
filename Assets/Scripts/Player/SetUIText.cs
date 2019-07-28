using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetUIText : MonoBehaviour {

    // Use this for initialization
    public RPGActor ActorData;
    public RPGPropertyType PropertyToDisplay;

    public enum SourceType { Current, CurrentTarget, CurrentSoftTarget };
    public enum ControlType { Text, Slider, ImageVisibility };

    public SourceType TypeOfSource;
    public ControlType TypeOfControl;

    private RPGActor data;

	// Update is called once per frame
	void Update () {

        if (TypeOfSource == SourceType.CurrentSoftTarget)
            data = ActorData.SoftTarget;
        else
            data = ActorData;

        if (data == null)
            return;

        switch (TypeOfControl)
        {
            case ControlType.Text:
                SetTextControl();
                break;
            case ControlType.Slider:
                SetSliderControl();
                break;
            case ControlType.ImageVisibility:
                SetImageVisibility();
                break;
            default:
                break;
        }
    }

    void SetTextControl()
    {
        GetComponent<Text>().text = data.Properties.GetPropertyString(PropertyToDisplay);

        if(PropertyToDisplay == RPGPropertyType.CurrentGold)
            GetComponent<Text>().text = GameManager.Instance.Gold.ToString();

        if (PropertyToDisplay == RPGPropertyType.CurrentSkillName)
        {
            GameObject gO = SkillbarController.Instance.ActiveSkills[SkillbarController.Instance.SelectedIndex];
            if (gO != null)
            {
                RPGActor leaderActor = GameManager.Instance.GetPartyLeader().GetComponent<RPGActor>();
                Command c = leaderActor.GetCommandAtSlotIndex(SkillbarController.Instance.SelectedIndex);
                if(c != null)
                    GetComponent<Text>().text = c.GetName();


            }
        }

        if (PropertyToDisplay == RPGPropertyType.CurrentSkillDescription)
        {
            GameObject gO = SkillbarController.Instance.ActiveSkills[SkillbarController.Instance.SelectedIndex];

            if (gO != null)
            {
                RPGActor leaderActor = GameManager.Instance.GetPartyLeader().GetComponent<RPGActor>();
                Command c = leaderActor.GetCommandAtSlotIndex(SkillbarController.Instance.SelectedIndex);
                if (c != null)
                    GetComponent<Text>().text = c.GetDescription();
            }
        }
    }

     void SetImageVisibility()
    {
        this.GetComponent<Image>().enabled = false;

        if (PropertyToDisplay == RPGPropertyType.IsBreak)
            this.GetComponent<Image>().enabled = data.Properties.IsBreak;
        if (PropertyToDisplay == RPGPropertyType.IsTopple)
            this.GetComponent<Image>().enabled = data.Properties.IsTopple;
    }

    void SetSliderControl()
    {
        float max = 0;
        float min = 0;
        float value = 0;

        if(PropertyToDisplay == RPGPropertyType.IsBreak || PropertyToDisplay == RPGPropertyType.IsTopple)
        {
            if(data.Properties.IsBreak)
            {
                this.transform.FindDeepChild("Fill Area").gameObject.SetActive(true);
                this.transform.FindDeepChild("Background").gameObject.SetActive(true);
                max = data.Properties.BreakDuration;
                value = data.Properties.CurrentBreakDuration;
            }
            else if (data.Properties.IsTopple)
            {
                this.transform.FindDeepChild("Fill Area").gameObject.SetActive(true);
                this.transform.FindDeepChild("Background").gameObject.SetActive(true);
                max = data.Properties.ToppleDuration;
                value = data.Properties.CurrentToppleDuration;
            }
            else
            {
                this.transform.FindDeepChild("Fill Area").gameObject.SetActive(false);
                this.transform.FindDeepChild("Background").gameObject.SetActive(false);
            }
        }

        if (PropertyToDisplay == RPGPropertyType.CurrentHealth)
        {
            max = data.Properties.MaxHealth;
            value = data.Properties.CurrentHealth;
        }

        if (PropertyToDisplay == RPGPropertyType.CurrentExperience)
        {
            max = data.Properties.NextLevelExperience;
            value = data.Properties.CurrentExperience;
        }

        GetComponent<Slider>().maxValue = max;
        GetComponent<Slider>().minValue = min;
        GetComponent<Slider>().value = value;
    }
}
