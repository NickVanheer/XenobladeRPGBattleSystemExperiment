using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LevelProgress
{
    Started, GatekeeperDefeated, BossDefeated, MainTitle
}

public class levelLogic : MonoBehaviour {

    public int Phase1DestroyedCount = 0;
    public GameObject Phase1Door;

    public int BossLevel = 5;
    public TextMesh BossLevelTextMesh; 

    public LevelProgress CurrentProgress { get; private set; }

    [Header("Spawnpoints")]
    public GameObject StartSpawnPoint;
    public GameObject PhaseTwoSpawnPoint;
    public GameObject BossSpawnPoint;

    [Space]
    [Header("Prefabs")]
    public GameObject BossPrefab;

    [Header("Debug")]
    public bool IsStartAtSpawnPoint2 = false;

    public GameObject BossObject = null;

    public void Start()
    {
        CurrentProgress = LevelProgress.Started;
        GameManager.Instance.OnEnterIdleState += OnEnterIdle;

        if(IsStartAtSpawnPoint2)
            GameManager.Instance.GetPartyLeader().GetComponent<Respawn>().RespawnAllAtSpawnPoint(PhaseTwoSpawnPoint);
       
    }

    //The beginning of the game
    void OnEnterIdle()
    {
        EventQueue.Instance.AddFocusEvent(Phase1Door, 3);
        CurrentProgress = LevelProgress.Started;
    }

    public void Update()
    {
        BossLevelTextMesh.text = BossLevel.ToString();
        if (CurrentProgress != LevelProgress.GatekeeperDefeated && Phase1DestroyedCount >= 1)
        {
            EventQueue.Instance.WaitABit(2f);
            EventQueue.Instance.AddAction(() => { Phase1Door.GetComponent<SmoothMoveToPosition>().enabled = true; });
            EventQueue.Instance.AddFocusEvent(Phase1Door, 3);
            EventQueue.Instance.AddQuickInfoPanel(LocalizationManager.Instance.GetLocalizedValue("SomethingChanged"), 2);

            Phase1DestroyedCount = 0;
            CurrentProgress = LevelProgress.GatekeeperDefeated;

            //Play sound effect
        }

        /*
        if(Input.GetKeyDown(KeyCode.P))
        {
            GameManager.Instance.SpawnHealthPotion();
        }
        */

        if(CurrentProgress == LevelProgress.BossDefeated)
        {
            //You won!
            GameManager.Instance.Gold += 200 * BossLevel;
        }
    }

    public void Phase1EnemyDestroyed()
    {
        Phase1DestroyedCount++;
    }

    public void BossDestroyed()
    {
        CurrentProgress = LevelProgress.BossDefeated;
    }

    public void SpawnIntroMessage()
    {
        CoreUIManager.Instance.ShowMessageBox(LocalizationManager.Instance.GetLocalizedValue("welcomeNodeText"));
    }

    public void IncreaseBossLevel()
    {
        BossLevel++;
        BossLevel = Mathf.Clamp(BossLevel, 1, 99);
    }

    public void DecreaseBossLevel()
    {
        BossLevel--;
        BossLevel = Mathf.Clamp(BossLevel, 1, 99);
    }

    public void SpawnBoss()
    {
        if (BossObject != null)
            Destroy(BossObject);

        if(BossPrefab != null)
        {
            BossObject = GameObject.Instantiate(BossPrefab, BossSpawnPoint.transform.position, Quaternion.identity);
            BossObject.GetComponent<RPGActor>().Properties.ChangeLevel(BossLevel);
            BossObject.GetComponent<RPGActor>().Properties.Name = LocalizationManager.Instance.GetLocalizedValue("BossName");

            EventQueue.Instance.AddFocusEvent(BossObject, 2);
            EventQueue.Instance.AddQuickInfoPanel(LocalizationManager.Instance.GetLocalizedValue("letsbegin"),1.4f);
        }
    }

    public void SwitchToJapanese()
    {
        GameManager.Instance.ChangeLanguage(DisplayLanguage.Japanese);
    }

    public void SwitchToEnglish()
    {
        GameManager.Instance.ChangeLanguage(DisplayLanguage.English);
    }
}
