using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    [HideInInspector]
    public static MusicManager musicManager;
    public static AudioClip music;
    public static AudioSource audioSource;
    public static bool isOn = true;
    public static float maxVolume = .2f;

    private void Awake() {
        if (musicManager == null) {
            DontDestroyOnLoad(gameObject);
            musicManager = this;
        } else if (musicManager != this) {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start() {
        music = Resources.Load<AudioClip>("Music/music1");
        
        audioSource = GetComponent<AudioSource>();

        //Music prefs
        if (PlayerPersistence.GetGameConfigs().wishMusicOn) {
            UnMute();
        } else {
            Mute();
        }

        //GameManager.Start() is controlling if music must be muted or not when game is initialized
    }

    /// <summary>
    /// Mutes the music if it's playing.
    /// </summary>
    public static void Mute() {
        if (audioSource != null) {
            audioSource.volume = 0;
            isOn = false;

            //Storing to player prefs
            PlayerPersistence.SetMusicGameConfigs(false);
        } else {
            Debug.LogError("audioSorce NULL!");
        }
    }

    /// <summary>
    /// Unmutes the music. If it's not playing, plays it unmuted.
    /// </summary>
    public static void UnMute() {
        if (audioSource != null) {
            if (!audioSource.isPlaying) {
                audioSource.clip = music;
                audioSource.Play();
            }
            audioSource.volume = maxVolume;
            isOn = true;

            //Storing to player prefs
            PlayerPersistence.SetMusicGameConfigs(true);
        } else {
            Debug.LogError("audioSorce NULL!");
        }
    }
}
