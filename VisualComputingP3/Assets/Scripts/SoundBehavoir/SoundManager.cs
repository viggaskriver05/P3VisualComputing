using UnityEngine;
using System;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public Audio[] AmbienceSFX, MusicSFX, OneShotSFX, SpeechSFX;
    public AudioSource AmbienceSource, MusicSource, OneShotSource, SpeechSource;




    private void Awake()
    {
        // --- Singleton setup ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);

        }

        // --- Ensure AudioSources exist ---
        if (AmbienceSource == null)
            AmbienceSource = gameObject.AddComponent<AudioSource>();
        if (MusicSource == null)
            MusicSource = gameObject.AddComponent<AudioSource>();

        if (OneShotSource == null)
            OneShotSource = gameObject.AddComponent<AudioSource>();

        AmbienceSource.loop = true;
    }
    /* void Start()
     {
         PlayMusicSFX("samba_mus");
         Debug.Log("SÅ DER SGU SAMBA");
     }*/

    public void PlayAmbienceSFX(string name)
    {
        Audio s = Array.Find(AmbienceSFX, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Music Not Found");

        }
        else
        {
            AmbienceSource.clip = s.clip;
            AmbienceSource.Play();
        }
    }

    public void PlayMusicSFX(string name)
    {
        Audio s = Array.Find(MusicSFX, x => x.name == name);
        if (s == null)
        {
            Debug.Log("SFX Not Found");

        }
        else
        {
            MusicSource.clip = s.clip;
            MusicSource.Play();
        }
    }
    public void PlayOneShotSFX(string name)
    {
        Audio s = Array.Find(OneShotSFX, x => x.name == name);
        if (s == null)
        {
            Debug.Log("One Shot Not Found");

        }
        else
        {
            //OneShotSource.clip = s.clip;
            OneShotSource.PlayOneShot(s.clip);
        }

    }
    public void PlaySpeechSFX(string name)
    {
        Audio s = Array.Find(SpeechSFX, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Speech Not Found");

        }
        else
        {
            SpeechSource.clip = s.clip;
            SpeechSource.PlayOneShot(s.clip);
        }
    }


    public void Stop(string name)
    {
        Audio s = Array.Find(SpeechSFX, x => x.name == name);
        if (s != null)
        {
            SpeechSource.Stop();
            return;
        }

        Audio o = Array.Find(OneShotSFX, x => x.name == name);
        if (o != null)
        {
            OneShotSource.Stop();
            return;
        }

        Audio m = Array.Find(MusicSFX, x => x.name == name);
        if (m != null)
        {
            MusicSource.Stop();
            return;
        }

        Audio a = Array.Find(AmbienceSFX, x => x.name == name);
        if (a != null)
        {
            AmbienceSource.Stop();
            return;
        }

    }

    public void StopAll()
    {
        SpeechSource.Stop();
        OneShotSource.Stop();
        MusicSource.Stop();
        AmbienceSource.Stop();
    }
}