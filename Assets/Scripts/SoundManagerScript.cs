using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour {

    public static AudioClip matchSound, noMatchSound, moveSound, btnClickSound, quizWrong, quizCorrect, star, 
                            collectedItem, pickGem, dynamite, coin, coins, matchVictory, lineBomb, normalBomb, 
                            projectile, play_main_menu, cancel_powerup, increaseMoves, shuffle_wind;
    public static AudioSource audioSource;
    public static bool isOn = true;
    public static float maxVolume = .5f;

    // Start is called before the first frame update
    void Start() {
        matchSound = Resources.Load<AudioClip>("Sounds/match");
        noMatchSound= Resources.Load<AudioClip>("Sounds/noMatch");
        moveSound = Resources.Load<AudioClip>("Sounds/movement");
        btnClickSound = Resources.Load<AudioClip>("Sounds/btn_click2");
        quizWrong = Resources.Load<AudioClip>("Sounds/quizWrong");
        quizCorrect = Resources.Load<AudioClip>("Sounds/quizCorrect");
        star = Resources.Load<AudioClip>("Sounds/star");
        collectedItem = Resources.Load<AudioClip>("Sounds/collectedItem");
        pickGem = Resources.Load<AudioClip>("Sounds/pickGem");
        dynamite = Resources.Load<AudioClip>("Sounds/dynamite2");
        coin = Resources.Load<AudioClip>("Sounds/coin");
        coins = Resources.Load<AudioClip>("Sounds/coins");
        matchVictory = Resources.Load<AudioClip>("Sounds/matchVictory");
        lineBomb = Resources.Load<AudioClip>("Sounds/lineBomb");
        normalBomb = Resources.Load<AudioClip>("Sounds/bomb");
        projectile = Resources.Load<AudioClip>("Sounds/projectile");
        play_main_menu = Resources.Load<AudioClip>("Sounds/play_main_menu");
        cancel_powerup = Resources.Load<AudioClip>("Sounds/cancel_powerup");
        increaseMoves = Resources.Load<AudioClip>("Sounds/collect2");
        shuffle_wind = Resources.Load<AudioClip>("Sounds/shuffle_wind");

        audioSource = GetComponent<AudioSource>();

        //SFX prefs
        if (PlayerPersistence.GetGameConfigs().wishSfxOn) {
            UnMute();
        } else {
            Mute();
        }

    }

    public static void PlaySound(string clip) {
        if (PlayerPersistence.GetGameConfigs().wishSfxOn) {
            switch (clip) {
                case "match":
                audioSource.PlayOneShot(matchSound);
                break;
                case "noMatch":
                audioSource.PlayOneShot(noMatchSound);
                break;
                case "movement":
                audioSource.PlayOneShot(moveSound);
                break;
                case "btnClick":
                audioSource.PlayOneShot(btnClickSound);
                break;
                case "quizWrong":
                audioSource.PlayOneShot(quizWrong);
                break;
                case "quizCorrect":
                audioSource.PlayOneShot(quizCorrect);
                break;
                case "star":
                audioSource.PlayOneShot(star);
                break;
                case "collectedItem":
                audioSource.PlayOneShot(collectedItem);
                break;
                case "pickGem":
                audioSource.PlayOneShot(pickGem);
                break;
                case "dynamite":
                audioSource.PlayOneShot(dynamite);
                break;
                case "coin":
                audioSource.volume = 0.7f;
                audioSource.PlayOneShot(coin);
                audioSource.volume = 1f;
                break;
                case "coins":
                audioSource.PlayOneShot(coins);
                break;
                case "matchVictory":
                audioSource.volume = 0.7f;
                audioSource.PlayOneShot(matchVictory);
                audioSource.volume = 1f;
                break;
                case "lineBomb":
                audioSource.PlayOneShot(lineBomb);
                break;
                case "normalBomb":
                audioSource.PlayOneShot(normalBomb);
                break;
                case "projectile":
                audioSource.PlayOneShot(projectile);
                break;
                case "play_main_menu":
                audioSource.PlayOneShot(play_main_menu);
                break;
                case "cancel_powerup":
                audioSource.PlayOneShot(cancel_powerup);
                break;
                case "increaseMoves":
                audioSource.PlayOneShot(increaseMoves);
                break;
                case "shuffle_wind":
                audioSource.PlayOneShot(shuffle_wind);
                break;
            }
        }
    }

    public static void PlayClickButton() {
        PlaySound("btnClick");
    }

    /// <summary>
    /// Mutes music
    /// </summary>
    public static void Mute() {
        if (audioSource != null) {
            audioSource.volume = 0;
            isOn = false;

            //Storing to player prefs
            PlayerPersistence.SetSfxGameConfig(false);
        } else {
            Debug.LogError("audioSorce NULL!");
        }
    }

    /// <summary>
    /// Unmutes SFX
    /// </summary>
    public static void UnMute() {
        if (audioSource != null) {
            audioSource.volume = maxVolume;
            isOn = true;

            //Storing to player prefs
            PlayerPersistence.SetSfxGameConfig(true);
        } else {
            Debug.LogError("audioSorce NULL!");
        }
    }
}
