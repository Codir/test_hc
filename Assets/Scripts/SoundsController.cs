using System;
using System.Linq;
using UnityEngine;
using Utils;

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

    private static SoundEntity[] _sounds;

    private void Awake()
    {
        _sounds = Sounds;
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
        return _sounds.FirstOrDefault(sfxEntity => sfxEntity.Type == type);
    }
}