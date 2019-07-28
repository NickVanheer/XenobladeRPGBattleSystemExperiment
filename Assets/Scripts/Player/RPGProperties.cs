using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RPGPropertyType { CurrentHealth, MaxHealth, Name, Level, CurrentExperience, CurrentGold, CurrentSkillName, CurrentSkillDescription, IsBreak, IsTopple, IsDead };

public enum UnitWeakness
{
    None, Fire, Wind, Water, Electricity
}

[Serializable]
public class RPGProperties
{
    public string Name;
    [Header("Unit Level Properties")]
    public int Level = 3;
    public int StartExperience = 90;
    public int CurrentExperience = 90;
    public int NextLevelExperience = 160;

    [Header("Unit Basic Properties")]
    public int CurrentHealth = 100;
    public int MaxHealth = 100;
    private int baseHealth;

    public int WeaponAttack = 12;
    public int Strength; //physical offense power
    public int Magic; //magic offense power

    [Range(7, 18)]
    public int Skill; //hit-chance and critical damage

    [Range(1, 25)]
    public int Speed; //
    public int Luck; //hit-crit rate

    public int Defense; //defense against physical
    public int Resistance; //defense against magic

    //when defeated, this is the experience and gold we earn
    public int EarnedExperience = 30;
    public int EarnedGold = 50;

    //
    public bool IsBreak { get; private set; }
    public bool IsTopple { get; private set; }

    public float BreakDuration = 5;
    public float ToppleDuration = 5;

    public float CurrentBreakDuration;
    public float CurrentToppleDuration;

    public float AutoAttackDamage = 1;

    public UnitWeakness Weakness;

    public RPGProperties()
    {
        this.baseHealth = MaxHealth;
    }

    //increases or decreases health
    public void ChangeHealth(int value)
    {
        CurrentHealth += value;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
    }

    public float GetHealthPercentage()
    {
        float v = CurrentHealth / (float)MaxHealth;
        return v * 100;

    }

    public bool InflictBreak()
    {
        IsBreak = true;
        CurrentBreakDuration = BreakDuration;

        return IsBreak;
    }

    public bool InflictTopple()
    {
        if (IsBreak)
        {
            IsTopple = true;
            IsBreak = false;
            CurrentToppleDuration = ToppleDuration;
        }

        return IsTopple;
    }

    public void RemoveBreak()
    {
        IsBreak = false;
    }

    public void RemoveTopple()
    {
        IsTopple = false;
    }

    private int CalculateStartExperience(int lv)
    {
        return (int)Mathf.Pow(lv, 2) * 10;
    }

    private void CalculateNextExperience()
    {
        NextLevelExperience = CalculateStartExperience(Level + 1); //lv2: 40
    }

    public bool IncreaseExperience(int exp)
    {
        bool levelUp = false;
        CurrentExperience += exp;

        if (CurrentExperience > NextLevelExperience)
        {
            levelUp = true;
            //We can increase a level, increase exp too with remaining exp (we could potentially increase more than one level)
            IncreaseOneLevel();

            int remainder = CurrentExperience - NextLevelExperience;
            IncreaseExperience(remainder);
        }

        return levelUp;
    }

    public void IncreaseOneLevel()
    {
        ChangeLevel(Level + 1);
    }

    public void ChangeLevel(int levelToBecome)
    {
        Level = levelToBecome;
        //StartExperience = CalculateStartExperience(Level); //lv1: 10
        CurrentExperience = StartExperience;
        NextLevelExperience = CalculateStartExperience(Level + 1); //lv2: 40

        //increase max HP
        int hpIncr = 8;
        int generalScaler = 1;
        float res = MaxHealth + Level * hpIncr + Mathf.Pow(Level, 2) * 0.6f;

        MaxHealth = (int)res * generalScaler;

        //restore HP
        CurrentHealth = MaxHealth;
    }

    public int GetLevel()
    {
        return Level;
    }

    public string GetPropertyString(RPGPropertyType type)
    {
        string result = "";

        if (type == RPGPropertyType.CurrentHealth)
            result = this.CurrentHealth.ToString();

        if (type == RPGPropertyType.MaxHealth)
            result = this.MaxHealth.ToString();

        if (type == RPGPropertyType.Name)
            result = this.Name.ToString();

        if (type == RPGPropertyType.Level)
            result = this.Level.ToString();     

        return result;
    }

    public int GetStartExperience()
    {
        return StartExperience;
    }

    public int GetExperience()
    {
        return CurrentExperience;
    }

    public int GetNextLevelExperience()
    {
        return NextLevelExperience;
    }

    public int GetExperienceRequiredForNextLevel()
    {
        return NextLevelExperience - CurrentExperience;
    }
}