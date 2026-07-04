using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source")]
    public AudioSource sfxSource;

    [Header("General SFX")]
    public AudioClip correctDisposeSound;
    public AudioClip wrongDisposeSound;
    public AudioClip buttonClickSound;

    [Header("Footstep SFX")]
    public AudioClip[] concreteFootsteps;
    public AudioClip[] grassFootsteps;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Plays any AudioClip.
    /// </summary>
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip, volume);
    }

    //==================================================
    // General Sounds
    //==================================================

    public void PlayCorrectDispose()
    {
        PlaySound(correctDisposeSound, 0.5f);
    }

    public void PlayWrongDispose()
    {
        PlaySound(wrongDisposeSound, 0.5f);
    }

    public void PlayButtonClick()
    {
        PlaySound(buttonClickSound);
    }

    //==================================================
    // Footsteps
    //==================================================

    public void PlayConcreteFootstep()
    {
        if (concreteFootsteps == null || concreteFootsteps.Length == 0)
            return;

        AudioClip clip = concreteFootsteps[Random.Range(0, concreteFootsteps.Length)];
        PlaySound(clip, 0.6f);
    }

    public void PlayGrassFootstep()
    {
        if (grassFootsteps == null || grassFootsteps.Length == 0)
            return;

        AudioClip clip = grassFootsteps[Random.Range(0, grassFootsteps.Length)];
        PlaySound(clip, 0.6f);
    }
}