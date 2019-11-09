using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/// <summary>
/// GameManager which holds global data and which won't be destroyed between scenes (by default), 
/// GameManager has a number of public variables and functions which can be called from other classes and as callbacks set in the inspector, as well as the game states.
/// 
/// Nick Vanheer
/// nickvanheer.com
/// </summary>
/// 

#region Game States

public class GameStartState : GameState
{
    public GameStartState(GameManager m) : base(m)
    { }

    public override void StateEntry()
    {
        CoreUIManager.Instance.HideTargetDisplay();
        CoreUIManager.Instance.HideSkillDisplay();
    }

    public override void StateExit()
    {
    }

    public override void StateUpdate()
    {
        gameManager.EnterIdleState();
    }
}

public class IdleState : GameState
{
    public IdleState(GameManager m) : base(m)
    { }

    public override void StateEntry()
    {
        CoreUIManager.Instance.HideTargetDisplay();
        CoreUIManager.Instance.HideSkillDisplay();

        if(CoreUIManager.Instance.SkillBarControl != null)
            CoreUIManager.Instance.SkillBarControl.InitializeSkills();

        if (ChainBarDisplayController.Instance != null)
            ChainBarDisplayController.Instance.gameObject.SetActive(false);
    }

    public override void StateExit()
    {
    }

    public override void StateUpdate()
    {
        //We don't have a soft target, show the pause/exit game message prompt.
        if(Input.GetKeyDown(KeyCode.Escape) && !gameManager.GetPartyLeader().GetComponent<PlayerTargetNearest>().HasSoftTarget)
        {
            gameManager.ExitGamePrompt();
        }
    }
}

public class BattleState : GameState
{
    public BattleState(GameManager m) : base(m)
    { }

    public float QTETimer = 30;

    public override void StateEntry()
    {
        ChainBarDisplayController.Instance.ResetValues();
        ChainBarDisplayController.Instance.gameObject.SetActive(true);

        //Reset QTE
        QTERingController.Instance.NumberOfOccurences = 0; 
        QTERingController.Instance.ResetAndShowQTE();

        SkillbarController.Instance.EnableAllSkills();

        //Delete to allow the player to engage with multiple enemies
        SkillbarController.Instance.ToggleAutoAttackSkill(false);
        CoreUIManager.Instance.ShowSkillDisplay();

        //Reset all skills and their cooldowns
        foreach (var player in gameManager.CurrentPartyMembers)
        {
            player.GetComponent<RPGActor>().ResetCommands();
        }

        //Disable initiate battle command icon
        Command startBattleCommand = GameManager.Instance.GetPartyLeader().GetComponent<RPGActor>().GetCommandAtSlotIndex(2);
        startBattleCommand.IsEnabled = false; 
    }

    public override void StateExit()
    {
        //Revive any dead party members
        foreach (var unit in gameManager.CurrentPartyMembers)
        {
            RPGActor actor = unit.GetComponent<RPGActor>();
            if (actor.Properties.CurrentHealth == 0)
                actor.RestoreHP(20, true);
            //Potentially check against the state of the unit (i.e: Dead) and clear status ailments,...
        }
    }

    public override void StateUpdate()
    {
        int partyMemberCount = GameManager.Instance.CurrentPartyMembers.Count;
        if (partyMemberCount > 0)
        {
            int deadCount = 0;

            foreach (var member in GameManager.Instance.CurrentPartyMembers)
            {
                if (member.GetComponent<RPGActor>().State == ActorState.Dead)
                    deadCount++;
            }

            if (deadCount == partyMemberCount)
                GameManager.Instance.EnterGameOverState();

            if(GameManager.Instance.CurrentPartyMembers[0].GetComponent<RPGActor>().State == ActorState.Idle)
            {
                //done with battle, time to move on.
                GameManager.Instance.EnterIdleState();

                //revive dead party members
                foreach (var member in GameManager.Instance.CurrentPartyMembers)
                {
                    if (member.GetComponent<RPGActor>().State == ActorState.Dead)
                        member.GetComponent<RPGActor>().Revive();
                }
            }

            QTETimer -= Time.deltaTime;
            if(QTETimer <= 0)
            {
                QTERingController.Instance.ResetAndShowQTE();
                QTETimer = UnityEngine.Random.Range(30, 50);
            }
        }
    }
}

public class GameOverState : GameState
{
    public GameOverState(GameManager m) : base(m)
    { }

    public override void StateEntry()
    {
        CoreUIManager.Instance.HideTargetDisplay();
        CoreUIManager.Instance.HideSkillDisplay();
        CoreUIManager.Instance.HideRevivePrompt();

        ChainBarDisplayController.Instance.gameObject.SetActive(false);
        
        gameManager.IsGameOver = true;
        gameManager.GameOverPrompt();
        Camera.main.GetComponent<PlayerCameraFollow>().enabled = false;
        Camera.main.GetComponent<RotateAroundTarget>().enabled = true;
    }

    public override void StateExit()
    {
        gameManager.IsGameOver = false;
    }

    public override void StateUpdate()
    {
    }

    public override void StateInitialize()
    {
        base.StateInitialize();
    }
}
#endregion

public class GameManager : MonoBehaviour {

    static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameManager();
            }
            return instance;
        }
    }

    public bool IsDebug = true;

    [Header("Core")]
    public bool IsGameOver = false;
    public bool IsMouseCursorHidden = false;
    public bool IsDontDestroyBetweenScenes = false;
    public bool IsPausedForUI = false;

    //States
    [HideInInspector]
    public GameStartState StateGameStart;
    public IdleState StateIdle;
    public BattleState StateBattle;
    public GameOverState StateGameOver;
    public GameState CurrentState;

    public string CurrentStateString;

    public List<GameObject> CurrentPartyMembers = new List<GameObject>();

    public GameObject PotionPickupPrefab;
    public float PotionThrust;
    public int Gold = 0;

    public GameObject AreaOfEffectPrefab;
    public GameObject TwoRectangleAreaOfEffectPrefab;
    public GameObject GuestMemberPrefab;

    [Header("Upgrade Prices")]
    public int HealthUpgradePrice = 200;
    public int PartyUpgradePrice = 300;

    public DisplayLanguage GameLanguage;
    public string LocalizationFileName;
    public KeyboardLayout KeyLayout;
    public bool IsDebugUseAzerty = false;

    public UnityEvent OnLocalizationChanged;

    public int ActiveAOEs = 0;
    public bool IsMainMenu = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Cursor.visible = !IsMouseCursorHidden;
            Cursor.lockState = CursorLockMode.Confined;
            
            if (IsDontDestroyBetweenScenes)
                DontDestroyOnLoad(gameObject);

            LocalizationManager.Instance.LoadLocalizedText(LocalizationFileName);
            Debug.Log("GameManager and localization info initialized");

            KeyLayout = KeyboardLayout.Azerty;

            if (IsDebugUseAzerty)
                return;

            if (Application.systemLanguage == SystemLanguage.Japanese || Application.systemLanguage == SystemLanguage.English)
            {
                KeyLayout = KeyboardLayout.Qwerty;
            }
        }
    }

    public Command GetCommandAtIndex(int index)
    {
        var leader = GetPartyLeader();
        if (leader != null)
        {
            return leader.GetComponent<RPGActor>().GetCommandAtSlotIndex(index);
        }

        return null;
    }

    public void Start()
    {
        StateGameStart = new GameStartState(this);
        StateIdle = new IdleState(this);
        StateBattle = new BattleState(this);
        StateGameOver = new GameOverState(this);

        ChangeState(StateGameStart);

        if (IsMainMenu)
            return;

        foreach (var member in CurrentPartyMembers)
        {
            CoreUIManager.Instance.AddPlayerBox(member.GetComponent<RPGActor>());
        }
    }

    public void Update()
    {
        CurrentStateString = CurrentState.ToString(); //For debugging

        //update game state (either idle, or battle)
        if (CurrentState != null)
            CurrentState.StateUpdate();
    }

    public BattleResults CalculateBattleResults(RPGActor defeatedEnemy)
    {
        //allocate exp
        BattleResults results = default(BattleResults);
        int expToAdd = defeatedEnemy.Properties.EarnedExperience;
        int goldToAdd = defeatedEnemy.Properties.EarnedGold;

        //add additional exp from passive bonusses, rest time, etc.

        results.Experience = expToAdd;
        results.Gold = goldToAdd;
        return results;
    }

    public List<RPGActor> GetPartyMembers()
    {
        List<RPGActor> units = new List<RPGActor>();

        foreach (var pMember in CurrentPartyMembers)
        {
            units.Add(pMember.GetComponent<RPGActor>());
        }

        return units;
    }

    public GameObject GetPartyLeader()
    {
        if (CurrentPartyMembers[0] != null)
            return CurrentPartyMembers[0];

        return null;
    }

    public GameObject GetPartyLeaderFloorObject()
    {
        if (CurrentPartyMembers[0] != null)
        {
            Transform floor = CurrentPartyMembers[0].transform.Find("Floor");

            if (floor != null)
                return floor.gameObject;
            return CurrentPartyMembers[0];
        }

        return null;
    }

    public GameObject GetFloorInUnit(GameObject unit)
    {
        if (unit != null)
        {
            Transform floor = unit.transform.Find("Floor");

            if (floor != null)
                return floor.gameObject;
            return unit;
        }

        return null;
    }

    public GameObject GetPartyLeaderTarget()
    {
        return CurrentPartyMembers[0].GetComponent<RPGActor>().TargetObject;
    }

    public void EnterBattleState()
    {
        if (CurrentState != StateBattle)
            ChangeState(StateBattle);
        else
            Debug.Log("Going from battle state into battle state again somehow.");
    }

    public void EnterIdleState()
    {
        if (CurrentState != StateIdle)
            ChangeState(StateIdle);
        else
            Debug.LogError("Going from idle state into idle state again somehow.");
    }

    public void EnterGameOverState()
    {
        if (CurrentState != StateGameOver)
            ChangeState(StateGameOver);
        else
            Debug.LogError("Going from idle state into idle state again somehow.");
    }

    public void RestoreAllHP(int percentage, bool showDamageNumber)
    {
        if(showDamageNumber)
        {
            foreach (var member in CurrentPartyMembers)
            {
                //Don't restore party members that are already dead.
                if(member.GetComponent<RPGActor>().Properties.CurrentHealth > 0)
                    member.GetComponent<RPGActor>().RestoreHP(percentage, showDamageNumber);
            }
        }
    }

    public void SpawnHealthPotion()
    {
        if (PotionPickupPrefab == null)
            return;
        Vector3 position = GetAvailableRandomPosition(60);
        position.y = GetPartyLeader().transform.position.y + 20;
        GameObject potion = Instantiate(PotionPickupPrefab, position, Quaternion.identity);
        //potion.GetComponent<Rigidbody>().AddForce(new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)) * PotionThrust, ForceMode.Impulse);
    }

    public void SpawnHealthPotion(Vector3 pos)
    {
        if (PotionPickupPrefab == null)
            return;
        
        GameObject potion = Instantiate(PotionPickupPrefab, pos, Quaternion.identity);
        //potion.GetComponent<Rigidbody>().AddForce(new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)) * PotionThrust, ForceMode.Impulse);
    }

    public Vector3 GetAvailableRandomPosition(float range)
    {
        Vector3 tryPos = GetPartyLeader().transform.position + UnityEngine.Random.insideUnitSphere * range;
        tryPos.y = GetPartyLeader().transform.position.y;

        //raycast check
        bool isHit = Physics.Raycast(tryPos, new Vector3(0, -1, 0), 5);

        if (isHit)
            return tryPos;
        else
            return GetAvailableRandomPosition(range - 4);
    }

    public void SpawnAoE(GameObject instigator, Vector3 position, int damage, float duration = 3f)
    {
        if (AreaOfEffectPrefab == null)
            return;

        GameObject AoE = Instantiate(AreaOfEffectPrefab, new Vector3(position.x, position.y, position.z), Quaternion.identity);
        AoE.GetComponent<AoEAttack>().Instigator = instigator;
        AoE.GetComponent<AoEAttack>().PreviewDuration = duration;
        AoE.GetComponent<AoEAttack>().Damage = damage;

        ActiveAOEs++;
    }

    public void SpawnTwoRectangleAoE(GameObject instigator, Vector3 position, int damage, float duration = 3f)
    {
        if (TwoRectangleAreaOfEffectPrefab == null)
            return;

        int offset = 30;
        GameObject AoE = Instantiate(TwoRectangleAreaOfEffectPrefab, new Vector3(position.x + UnityEngine.Random.Range(-offset, offset), position.y, position.z), Quaternion.identity /* Quaternion.Euler(0, UnityEngine.Random.Range(0,360) , 0) */);
        AoE.GetComponent<AoEAttack>().Instigator = instigator;
        AoE.GetComponent<AoEAttack>().PreviewDuration = duration;
        AoE.GetComponent<AoEAttack>().Damage = damage;

        ActiveAOEs++;
    }

    public void SpawnAoE(GameObject instigator, Vector3 position, int damage, float scale, float duration = 3f)
    {
        if (AreaOfEffectPrefab == null)
            return;

        GameObject AoE = Instantiate(AreaOfEffectPrefab, new Vector3(position.x, position.y, position.z), Quaternion.identity);
        AoE.GetComponent<AoEAttack>().Instigator = instigator;
        AoE.GetComponent<AoEAttack>().PreviewDuration = duration;
        AoE.GetComponent<AoEAttack>().Scale(scale);
        AoE.GetComponent<AoEAttack>().Damage = damage;

        ActiveAOEs++;
    }

    public void ChangeState(GameState newState)
    {
        string debugString = "Changing Game State to " + newState.ToString() + ".";

        if (CurrentState == newState)
            Debug.LogWarning("Trying to change to the same state like before. State: " + newState.ToString());

        if (CurrentState != null)
        {
            CurrentState.StateExit();
            string oldState = CurrentState.ToString();
            debugString += "(earlier state was:  " + oldState + ")";
        }

        GameManager.Instance.Log(debugString);
        CurrentState = newState;

        CurrentState.StateEntry();
    }

    public void SpawnGuestMember()
    {
        if (GuestMemberPrefab == null)
            return;

        Vector3 position = GetPartyLeader().transform.position + new Vector3(UnityEngine.Random.Range(-20, 20), 0, UnityEngine.Random.Range(-20, 20));
        GameObject member = Instantiate(GuestMemberPrefab, position, Quaternion.identity);

        CurrentPartyMembers.Add(member);
        CoreUIManager.Instance.AddPlayerBox(member.GetComponent<RPGActor>());
    }

    public void BuyPartyUpgradePrompt()
    {
        UnityAction yesAction = () =>
        {
            if (Gold >= PartyUpgradePrice)
            {
                Gold -= PartyUpgradePrice;
                SpawnGuestMember();
                EventQueue.Instance.AddMessageBox(LocalizationManager.Instance.GetLocalizedValue("PartyUpgraded"), 1f);
            }
            else
            {
                EventQueue.Instance.AddMessageBox(LocalizationManager.Instance.GetLocalizedValue("NotEnoughMoney"), 1f);
            }

        };
        UnityAction noAction = () => { EventQueue.Instance.AddMessageBox(LocalizationManager.Instance.GetLocalizedValue("Kaomoji"), 1f); };

        CoreUIManager.Instance.ShowYesNoMessageBox(LocalizationManager.Instance.GetLocalizedValue("UpgradePartyPrompt") + " " + "(" + PartyUpgradePrice + " " + LocalizationManager.Instance.GetLocalizedValue("Gold") + ")", yesAction, noAction);
    }

    public void GameOverPrompt()
    {
        UnityAction yesAction = () =>
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);

        };
        UnityAction noAction = () => { EndGame(); };

        CoreUIManager.Instance.ShowYesNoMessageBox(LocalizationManager.Instance.GetLocalizedValue("GameOverMessage"), yesAction, noAction);
    }

    public void ExitGamePrompt()
    {
        LocalizedMessageOption continueAction;
        LocalizedMessageOption exitGame;

        continueAction.LocalizedValueTag = "ContinueGame";
        continueAction.actionToPerform = () => {  };

        exitGame.LocalizedValueTag = "ExitGame";
        exitGame.actionToPerform = () => { EndGame(); };

        CoreUIManager.Instance.ShowMessageBoxWithOptions(LocalizationManager.Instance.GetLocalizedValue("IsExit"), continueAction, exitGame);
    }

    public void ChangeLanguage(DisplayLanguage language)
    {
        this.GameLanguage = language;
        if (OnLocalizationChanged != null)
            OnLocalizationChanged.Invoke();
        EventQueue.Instance.AddQuickInfoPanel(LocalizationManager.Instance.GetLocalizedValue("LanguageChanged"), 1);

    }

    public void LoadLevel(string name)
    {
        SceneManager.LoadScene(name.Trim());
        SoundManager.Instance.PlayConfirmSound();

        //reset global values
        IsGameOver = false;
    }

    public void EndGame()
    {
        #if UNITY_EDITOR
                Debug.Break();
        #else
                 Application.Quit();
        #endif  
    }

    public void PauseGame()
    {
        IsPausedForUI = true;
    }

    public void UnpauseGame()
    {
        IsPausedForUI = false;
    }

    public void TogglePause()
    {
        IsPausedForUI = !IsPausedForUI;
    }

    public void Log(string text)
    {
        Debug.Log(System.DateTime.Now.ToLongTimeString() + ": " + text);
    }

    public void LogError(string text)
    {
        Debug.LogError(Time.time + " :" + text);
    }

}
