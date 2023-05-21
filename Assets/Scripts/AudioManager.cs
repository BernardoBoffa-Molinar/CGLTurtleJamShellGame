using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    public bool loop;

    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    void Awake()
    {
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;
        }
    }

    public void PlaySound(string name)
    {
        Sound sound = FindSoundByName(name);
        if (sound == null)
        {
            Debug.LogWarning("Sound with name " + name + " not found.");
            return;
        }

        if (sound.source.isPlaying)
        {
            sound.source.Stop();
        }
        
        sound.source.Play();
    }

    private Sound FindSoundByName(string name)
    {
        return System.Array.Find(sounds, sound => sound.name == name);
    }
}

