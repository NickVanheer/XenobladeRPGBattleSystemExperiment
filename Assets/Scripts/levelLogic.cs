using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LevelProgress
{
    Started, GatekeeperDefeated, BossDefeated
}

public class levelLogic : MonoBehaviour {

    public int Phase1DestroyedCount = 0;
    public GameObject Phase1Door;

    public int BossLevel = 5;
    public TextMesh BossLevelTextMesh; 

    public LevelProgress CurrentProgress { get; private set; }

    //public GameObject currentSpawnPoint;

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

        if(IsStartAtSpawnPoint2)
            GameManager.Instance.GetPartyLeader().GetComponent<Respawn>().RespawnAllAtSpawnPoint(PhaseTwoSpawnPoint);
    }

    public void Update()
    {
        BossLevelTextMesh.text = BossLevel.ToString();
        if (Phase1DestroyedCount >= 1)
        {
            EventQueue.Instance.WaitABit(2f);
            EventQueue.Instance.AddAction(() => { Phase1Door.GetComponent<SmoothMoveToPosition>().enabled = true; });
            EventQueue.Instance.AddAction(() => { Camera.main.GetComponent<FocusCamera>().Focus(Phase1Door); });
            EventQueue.Instance.WaitABit(3f);
            EventQueue.Instance.AddQuickInfoPanel(LocalizationManager.Instance.GetLocalizedValue("SomethingChanged"), 2);
            //CoreUIManager.Instance.ShowQuickInfoPanel("Something happened...");

            Phase1DestroyedCount = 0;
            CurrentProgress = LevelProgress.GatekeeperDefeated;

            //Play sound effect
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            GameManager.Instance.SpawnHealthPotion();
        }

        if(CurrentProgress == LevelProgress.BossDefeated)
        {
            //You won!
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
    }

    public void DecreaseBossLevel()
    {
        BossLevel--;
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

            EventQueue.Instance.AddAction(() => { Camera.main.GetComponent<FocusCamera>().Focus(BossObject); });
            EventQueue.Instance.WaitABit(2);
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
