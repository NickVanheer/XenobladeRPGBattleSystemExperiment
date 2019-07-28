using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupManager : MonoBehaviour
{

    // Use this for initialization
    private IEnumerator Start()
    {
        Screen.SetResolution(800, 600, false);
        while (!LocalizationManager.Instance.GetIsReady())
        {
            yield return null;
        }

        SceneManager.LoadScene("GameScene");
    }

}