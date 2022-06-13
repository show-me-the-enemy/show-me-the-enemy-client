using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 볼륨조절은 AudioListener.volume = 0...;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound {
        public string name;
        public AudioClip clip;
    }
    public Sound[] bgms;
    public Sound[] sfxs;
    public string[] playingSfxNames;

    AudioSource bgmAS;
    public int sfxBufferSize = 9;
    AudioSource[] sfxASs;

    #region Singelton
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
                if (FindObjectsOfType<AudioManager>().Length > 1)
                {
                    Debug.LogError("AudioManager more than 1 singleton!");
                    return _instance;
                }

                if (_instance == null)
                {
                    _instance = Instantiate(Resources.Load<AudioManager>("Prefabs/AudioManager"), Vector3.zero, Quaternion.identity);
                }
            }

            return _instance;
        }
    }
    #endregion
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        bgmAS = gameObject.AddComponent<AudioSource>();
        bgmAS.loop = true;
        bgmAS.playOnAwake = false;
        bgmAS.volume = 0.75f;

        sfxASs = new AudioSource[sfxBufferSize];
        for (int i = 0; i < sfxBufferSize; i++)
        {
            sfxASs[i] = gameObject.AddComponent<AudioSource>();
            sfxASs[i].loop = false;
            sfxASs[i].playOnAwake = false;
        }
        playingSfxNames = new string[sfxs.Length];
    }

    public void PlaySFX(string name, float v=1)
    {
        for (int i = 0; i < sfxs.Length; i++)
        {
            if (name == sfxs[i].name)
            {
                for (int j = 0; j < sfxASs.Length; j++)
                {
                    if (!sfxASs[j].isPlaying)
                    {
                        sfxASs[j].volume = v;
                        sfxASs[j].clip = sfxs[i].clip;
                        sfxASs[j].Play();
                        playingSfxNames[j] = sfxs[i].name;
                        return;
                    }
                }
                Debug.Log("sfx buffer overflow.");
                return;
            }
        }
        Debug.Log(name+ " is not registered.");
    }
    public void PlayBGM(string name, float v=1)
    {
        //bgmAS.volume = v;
        for (int i = 0; i < bgms.Length; i++)
        {
            if (name == bgms[i].name)
            {
                bgmAS.Stop();
                bgmAS.clip = bgms[i].clip;
                bgmAS.Play();
                return;
            }
        }
        Debug.Log(name + " is not registered.");
    }
    public void StopBGM()
    {
        bgmAS.Stop();
    }
    public void StopSFX(string name)
    {
        for(int i = 0; i < sfxs.Length; i++)
        {
            if(playingSfxNames[i] == name)
            {
                sfxASs[i].Stop();
            }
        }
    }
    public void LogPlayList()
    {
        for (int i = 0; i < bgms.Length; i++)
        {
            Debug.Log(bgms[i].name + " " + bgms[i].clip.name);
        }
        for (int i = 0; i < sfxs.Length; i++)
        {
            Debug.Log(sfxs[i].name + " " + sfxs[i].clip.name);
        }
    }
}
