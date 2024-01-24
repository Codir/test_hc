using System;
using System.Linq;
using UnityEngine;

public enum Sfx
{
    Click,
    Hit,
    Splash,
    Win,
    Fail,
    Shoot
}

[Serializable]
public class SoundEntity
{
    public Sfx Type;
    public AudioSource[] AudioSources;
}

public class SoundsController : MonoBehaviour
{
    [SerializeField] private SoundEntity[] Sounds;

    private static SoundsController _instance;

    private void Awake()
    {
        //TODO: remove this
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public static void PlaySound(Sfx type)
    {
        var sounds = GetSoundByType(type);

        if (sounds == null) return;

        var sound = sounds.AudioSources.GetRandomElement();

        sound.Play();
    }

    private static SoundEntity GetSoundByType(Sfx type)
    {
        return _instance.Sounds.FirstOrDefault(sfxEntity => sfxEntity.Type == type);
    }
}