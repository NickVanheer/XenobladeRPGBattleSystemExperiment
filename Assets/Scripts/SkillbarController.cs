using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillbarController : MonoBehaviour {

    //public bool IsActive = false;
    public int SelectedIndex = 2;

    public List<GameObject> ActiveSkills = new List<GameObject>();
    private RPGActor leaderActor;

    static SkillbarController instance;
    public static SkillbarController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SkillbarController();
            }
            return instance;
        }
    }

    float updateDelay = 0.1f;

    public void InitializeSkills()
    {
        //At the beginning of a battle, starting all cooldowns   
        leaderActor = GameManager.Instance.CurrentPartyMembers[0].GetComponent<RPGActor>();

        foreach (var displaySkill in ActiveSkills)
        {
            displaySkill.SetActive(false);
        }

        foreach (Command cmd in leaderActor.PartyMemberCommands)
        {
            ActiveSkills[cmd.Slot].SetActive(true);
            //cmd.ResetCommand(); //Done manually for each command in case some skills have to be active from the start etc.

            //icon
            Transform iconTransform = ActiveSkills[cmd.Slot].gameObject.transform.FindDeepChild("SkillIcon");

            if (iconTransform != null)
            {
                iconTransform.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(cmd.Illustration);
            }
        }

        SelectedIndex = 2;

        Command startBattleCommand = leaderActor.GetCommandAtSlotIndex(SelectedIndex);

        startBattleCommand.IsEnabled = true; //enable second skill
        startBattleCommand.IsCooledDown = true;
    }

    public void EnableAllSkills()
    {
        foreach (var skill in ActiveSkills)
        {
            skill.GetComponent<Command>().IsEnabled = true;
        }
    }

    public void ToggleAutoAttackSkill(bool isEnabled)
    {
        ActiveSkills[2].GetComponent<Command>().IsEnabled = isEnabled;
    }

    public void Awake()
    {
        if (instance == null)
            instance = this;
        Debug.Log("awake called");
    }

    private void OnEnable()
    {
        StartCoroutine(UpdateSkillDisplay());
        Debug.Log("enable called");
    }

    void Update () {

        if (this.gameObject.activeInHierarchy)
            HandleInput();
    }

    IEnumerator UpdateSkillDisplay()
    {
        while(true)
        {
            if (!this.gameObject.activeInHierarchy || leaderActor == null)
                yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < ActiveSkills.Count; i++)
            {
                Command toShow = leaderActor.GetCommandAtSlotIndex(i);
                if (toShow == null)
                    continue;

                ActiveSkills[i].GetComponent<Command>().ShowCommandDisplay(leaderActor.GetCommandAtSlotIndex(i), false);

                if (i == SelectedIndex)
                    ActiveSkills[i].GetComponent<Command>().ShowCommandDisplay(leaderActor.GetCommandAtSlotIndex(i), true);
            }

            yield return new WaitForSeconds(updateDelay);
        }
    }

    public void HandleInput()
    {
        //maybe not every frame -> See UpdateSkillDisplay coroutine
        //UpdateSkillDisplay();

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SelectedIndex++;
            SelectedIndex = Mathf.Clamp(SelectedIndex, 0, ActiveSkills.Count - 1);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SelectedIndex--;
            SelectedIndex = Mathf.Clamp(SelectedIndex, 0, ActiveSkills.Count - 1);
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            for (int i = 0; i < ActiveSkills.Count; i++)
            {
                if(i == SelectedIndex)
                {
                    Command com = leaderActor.GetCommandAtSlotIndex(SelectedIndex);
                    com.UseCommand();
                }
            }
        }

        NumericalKeysInput();
    }

    public void NumericalKeysInput()
    {
        Command com = null;
        if (Input.GetKeyDown(KeyCode.Alpha1))
            com = leaderActor.GetCommandAtSlotIndex(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            com = leaderActor.GetCommandAtSlotIndex(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            com = leaderActor.GetCommandAtSlotIndex(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            com = leaderActor.GetCommandAtSlotIndex(3);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            com = leaderActor.GetCommandAtSlotIndex(4);

        if (com != null)
            com.UseCommand();
    }
}
