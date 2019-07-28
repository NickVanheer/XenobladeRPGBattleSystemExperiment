using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public enum CommandTarget
{
    SelectedTarget,
    Self,
    PartyMemberSelect,
    AllEnemies,
    AllPartyMembers
}

public enum MagicElemancy
{
    None, Fire, Water, Lighting, Blizzard
}

public enum CommandType
{
    Attack,
    StatusEffectDamage,
    Heal,
    StealGil,
    OpenContextMenu,
    SpecialBattleCommand
}

/*
public class Skill
{
    public string Name;
    public string Description;
    public int Damage;
    public int Cost = 0;
    public CommandType Type;
    public UnityAction ActionVariable;
    public string Illustration;
    public int Slot;
    public int Cooldown;
    public bool IsInflictBreak;
    public bool IsInflictTopple;
    public MagicElemancy Element;

}

*/

public class Command : MonoBehaviour
{
    public string Name;
    public string Description;
    public int Damage;
    public int Cost = 0;
    public CommandType Type;
    public UnityAction ActionVariable;
    public string Illustration;
    public int Slot;
    public bool IsInflictBreak;
    public bool IsInflictTopple;
    public MagicElemancy Element;

    //public Skill SkillData = new Skill();

    public GameObject ParticleCommandEffect;

    //needed when we select the skill with mouse.
    public int SkillIndex;
    public float Cooldown = 10f;
    public float CurrentCooldown;
    public bool IsCooledDown = false;

    public bool IsEnabled = false;

    public void Update()
    {
        GameObject leader = GameManager.Instance.GetPartyLeader();
        if (leader.GetComponent<RPGActor>().State != ActorState.Engaged)
            return;

        if (!IsEnabled)
            return;

        if (!IsCooledDown)
        {
            CurrentCooldown -= Time.deltaTime;

            if (CurrentCooldown <= 0)
                IsCooledDown = true;
        }
    }

    public void SetCooldownRing(Command data)
    {
        GetComponent<Image>().fillAmount = 1 - (data.CurrentCooldown / data.Cooldown);
    }

    //UI representation of command
    public void ShowCommandDisplay(Command data, bool isSelected)
    {
        Transform arrow = transform.FindDeepChild("Arrow");
        Transform arrow2 = transform.FindDeepChild("Arrow 2");
        Transform chainSegment = transform.FindDeepChild("ChainSegmentDisplay");
        Transform breakIcon = transform.FindDeepChild("Break");
        Transform toppleIcon = transform.FindDeepChild("Topple");

        Transform selectionCircle = transform.FindDeepChild("SelectionCircle");

        if (data.IsEnabled && data.CanExecute())
            transform.FindDeepChild("SkillIcon").GetComponent<Image>().color = new Color(1, 1, 1, 1f);
        else
            transform.FindDeepChild("SkillIcon").GetComponent<Image>().color = new Color(0, 0, 0, 0.4f);

        if (arrow != null)
            arrow.gameObject.GetComponent<Image>().enabled = isSelected;

        if (arrow2 != null)
            arrow2.gameObject.GetComponent<Image>().enabled = isSelected;

        if(selectionCircle != null)
            selectionCircle.gameObject.GetComponent<Image>().enabled = isSelected;

        if (chainSegment != null)
            chainSegment.gameObject.SetActive(false);
       
        if(chainSegment != null && data.Cost > 0)
            chainSegment.gameObject.SetActive(true);

        if (breakIcon == null && toppleIcon == null)
            return;

        breakIcon.gameObject.SetActive(data.IsInflictBreak);
        toppleIcon.gameObject.SetActive(data.IsInflictTopple);

        SetCooldownRing(data);
    }

    public bool CanExecute()
    {
        bool canUse = true;
        if (Cost > 0)
        {
            canUse = ChainBarDisplayController.Instance.HasSegmentReady();
        }

        if (!IsCooledDown)
            return false;

        if (!canUse)
            return false;

        return true;
    }

    public void ResetCommand()
    {
        IsCooledDown = false;
        CurrentCooldown = Cooldown;
        IsEnabled = false;
    }

    public void UseCommand()
    {
        if (!CanExecute())
            return;

        if (!IsEnabled)
            return;

        IsCooledDown = false;
        CurrentCooldown = Cooldown;

        if (Cost > 0)
            ChainBarDisplayController.Instance.UseChainSegment();

        if (ActionVariable != null)
            ActionVariable.Invoke();
    }
}
