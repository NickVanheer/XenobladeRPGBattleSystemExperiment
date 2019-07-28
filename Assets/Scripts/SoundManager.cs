using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    static SoundManager instance;

    public AudioClip ShootSound;
    public AudioClip ConfirmSound;
    public AudioClip ObjectDestroyedSound;

    private AudioSource source;

    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SoundManager();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            source = GetComponent<AudioSource>();
            Debug.Log("SoundManager initialized");
        }
    }

    //TODO::Add sound effects
    public void PlayShootSound()
    {
        //source.PlayOneShot(ShootSound);
    }

    public void PlayConfirmSound()
    {
        source.PlayOneShot(ConfirmSound);
    }

    public void PlayObjectDestroyedSound()
    {
        //source.PlayOneShot(ObjectDestroyedSound);
    }
}
