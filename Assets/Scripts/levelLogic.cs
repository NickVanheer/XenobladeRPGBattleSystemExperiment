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

    public int BossLevel = 3;
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

    public void Start()
    {
        CoreUIManager.Instance.ShowQuickInfoPanel("Welcome");
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
            EventQueue.Instance.AddQuickInfoPanel("Something happened...", 2);
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
        CoreUIManager.Instance.ShowMessageBox("Welcome! Use skills, evade attacks and watch out for your health as you beat enemies");
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
        if(BossPrefab != null)
        {
           GameObject gO = GameObject.Instantiate(BossPrefab, BossSpawnPoint.transform.position, Quaternion.identity);
            gO.GetComponent<RPGActor>().Properties.ChangeLevel(BossLevel);

            //Todo zoom
        }
    }
}
