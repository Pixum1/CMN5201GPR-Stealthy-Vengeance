using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Singleton 

    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }

    private void Initialize()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    private void Terminate()
    {
        if (this == instance)
            Destroy(this.gameObject);
    }

    #endregion

    [Header("Sound Effects")]
    public AudioSource JumpSound;
    public AudioSource HitHumanoidSound;
    public AudioSource HitObjectSound;
    public AudioSource LifeGainSound;

    private void Awake()
    {
        Initialize();
    }

    public void PlaySound(AudioSource _sound)
    {
        _sound.pitch = Random.Range(0.85f, 1.15f) ;
        _sound.Play();
    }


    private void OnDestroy()
    {
        Terminate();
    }
}
