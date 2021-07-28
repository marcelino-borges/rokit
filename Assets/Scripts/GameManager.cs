using NotificationSamples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

public class GameManager : MonoBehaviour {
    public static float timeToWaitLoadingScreen = 3f;
    [HideInInspector]
    public static GameManager instance;
    [HideInInspector]
    public static bool isGamePaused = false;
    [HideInInspector]
    public static bool canMakeMoves;

    public static double scoreMultiplier = 1f; //Set in the QuizManager, used by the Match3Manager

    public static int playerMaxHearts = 5;
    public static int currentHearts = -1;
    
    [Tooltip("Panel with a debug text in the main menu scene, to debug the android notification callback")]
    public GameObject panelInScene; //DEBUG ONLY

    public static GameObject panel;

    public GameObject loadingBarCanvas;
    public static GameObject loadingBar;

    /*public Slider loadingBarSliderInScene;
    [SerializeField]
    private static Slider loadingBarSlider;*/

    /*[SerializeField]
    private static Text loadingBarText;
    public Text loadingBarTextInScene;*/

    [HideInInspector]
    public static Levels gameLevels;

    //public GameObject notificationManager;
    private static GameNotificationsManager notificationManager;

    void Awake() {
        
        //If PLAYING FOR THE > FIRST TIME <
        if (!PlayerPersistence.HasFileCreated()) {
            //Creates-saves a default PlayerData file to him
            PlayerPersistence.SavePlayerData(1f, 5);
            PlayerPersistence.SetMusicGameConfigs(true);
            PlayerPersistence.SetSfxGameConfig(true);
            //Debug.Log("GameManager > PlayerData not found. Creating a new one.");
            PlayerPersistence.SetScoreMultiplier(1);

        //If NOT the first time
        } else {
            if(PlayerPersistence.GetScoreMultiplier() < 1) {
                PlayerPersistence.SetScoreMultiplier(1);
            }

            GameManager.scoreMultiplier = PlayerPersistence.GetScoreMultiplier();
        }
    }

    // Start is called before the first frame update
    void Start() {
        panel = panelInScene;
        loadingBar = loadingBarCanvas;
        //loadingBarSlider = loadingBarSliderInScene;
        //loadingBarText = loadingBarTextInScene;
        //loadingBarSlider = loadingBar.GetComponentInChildren<Slider>();
        //loadingBarText = loadingBarSlider.GetComponentInChildren<Text>();

        //Debug.Log("Last level: " + SceneManager.GetSceneByBuildIndex(PlayerPersistence.GetLastLevelStored()));
        /*
#if UNITY_ANDROID
            try {
                notificationManager = GameObject.FindWithTag("NotificationManager").GetComponent<GameNotificationsManager>();
            } catch (Exception e) {
                Debug.LogWarning("Exception in GameManager.cs > Start(), couldn't get the notificationManager.");
            }
#endif*/

        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

    }

    public static void PauseGame() {
        if (!isGamePaused) {
            canMakeMoves = false;
            isGamePaused = true;
        }
    }

    public static void UnPauseGame() {
        if (isGamePaused) {
            canMakeMoves = true;
            isGamePaused = false;
        }
    }
       
    public static void SendHeartNotification() {
        //var channel = new GameNotificationChannel("RokitChannel", "Default Game Channel", "Heart notifications");
        //if (!notificationManager.Initialized) {
        //    notificationManager.Initialize(channel);
        //    IGameNotification notification = notificationManager.CreateNotification();
        //    if (notification != null) {
        //        notification.Title = "Rokit";
        //        notification.Body = "Você acabou de ganhar 1 coração!";
        //        notification.DeliveryTime = DateTime.Now.ToLocalTime() + TimeSpan.FromMinutes(1);
        //        notification.Subtitle = "Hora da diversão!";
        //        notification.LargeIcon = "icon_0";
        //        notification.SmallIcon = "icon_0";
        //        notificationManager.ScheduleNotification(notification);
        //    }
        //}
        /*
        Firebase.Messaging.FirebaseMessage message = new Firebase.Messaging.FirebaseMessage();
        message.To = SENDER_ID + "@gcm.googleapis.com";
        message.MessageId = get_unique_message_id();
        message.Data["my_message", "Hello World");
        message.Data["my_action", "SAY HELLO");
        message.TimeToLive = kTimetoLive;
        Firebase.Messaging.FirebaseMessaging.Send(message);
        */
    }

    private void OnApplicationQuit() {
        //It's already been called in the GameOverMenu.TryAgain()
        //#if UNITY_ANDROID
        //    //Calls the notification method
        //    if (PlayerPersistence.GetHeartsStored() < 5) {
        //        SendHeartNotification();
        //    }
        //#endif
    }

    public static void CreateHeart() {
        instance.Invoke("CreateHeart2", 3);
    }

    public static void CreateHeart2() {
        if (PlayerPersistence.GetHeartsStored() < 5) {
            if (panel != null) {
                long heartsBefore = PlayerPersistence.GetHeartsStored();
                Text text = panel.GetComponentInChildren<Text>();
                PlayerPersistence.IncreaseHeart(1);

                text.text = "Hearts before: " + heartsBefore + " and after: " + PlayerPersistence.GetHeartsStored();
            } 
        }        
    }

    public static void LoadLevel (int sceneIndex) {        
        instance.StartCoroutine(LoadLevelAsynchronously(sceneIndex));
        try {
        } catch (Exception e) {
            //Debug.LogError("Not possible loading the scene. Probably you tried to load a scene that doesn't exist.");
        }
    }

    public static void LoadLevel(string sceneName) {
        try {
            instance.StartCoroutine(LoadLevelAsynchronously(sceneName));
        } catch (Exception e) {
            //Debug.LogError("Not possible loading the scene. Probably you tried to load a scene that doesn't exist.");
        }
    }

    private static IEnumerator LoadLevelAsynchronously (int sceneIndex) {
        //Calls the level to be loaded asynchronously in the background - The level will show up when it completes the loading
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;
        
        if (loadingBar != null) {
            //Shows up the loading screen
            loadingBar.SetActive(true);

            if(loadingBar.GetComponent<LoadingScreen>() != null) {
                loadingBar.GetComponent<LoadingScreen>().SetTipData();
            }

            //Tracks the progress of the loading level
            while (!operation.isDone) {
                //Get the actual progress
                float progress = Mathf.Clamp01(operation.progress / .9f);                
                /*
                if (loadingBarSlider != null) {
                    //Sets the loading bar
                    loadingBarSlider.value = progress;
                } else {
                    Debug.LogError("GameManager > variable loadingBarSlider not assigned!");
                }

                if (loadingBarText != null) {
                    //Sets the text into the loading bar
                    loadingBarText.text = Mathf.Round(progress * 100f) + "%";
                } else {
                    Debug.LogError("GameManager > variable loadingBarText not assigned!");
                }*/
                //Waits a frame
                yield return null;

                if (progress == 1) {
                    yield return new WaitForSeconds(timeToWaitLoadingScreen);
                    operation.allowSceneActivation = true;
                    yield return new WaitForSeconds(0.3f);
                    //Hides the loading screen
                    loadingBar.SetActive(false);
                }
            }
        } else {
            //Debug.LogError("GameManager > variable loadingBar not assigned!");
        }
    }

    private static IEnumerator LoadLevelAsynchronously(string sceneName) {
        //Calls the level to be loaded asynchronously in the background - The level will show up when it completes the loading
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        
        if (loadingBar != null) {
            //Shows up the loading screen
            loadingBar.SetActive(true);

            if (loadingBar.GetComponent<LoadingScreen>() != null) {
                loadingBar.GetComponent<LoadingScreen>().SetTipData();
            }

            //Tracks the progress of the loading level
            while (!operation.isDone) {
                //Get the actual progress
                float progress = Mathf.Clamp01(operation.progress / .9f);
                /*
                if (loadingBarSlider != null) {
                    //Sets the loading bar
                    loadingBarSlider.value = progress;
                } else {
                    Debug.LogError("GameManager > variable loadingBarSlider not assigned!");
                }

                if (loadingBarText != null) {
                    //Sets the text into the loading bar
                    loadingBarText.text = Mathf.Round(progress * 100f) + "%";
                } else {
                    Debug.LogError("GameManager > variable loadingBarText not assigned!");
                }*/
                //Debug.Log("no while. isDone = " + operation.isDone);
                //Waits a frame
                yield return null;

                if (progress == 1) {
                    yield return new WaitForSeconds(timeToWaitLoadingScreen);
                    operation.allowSceneActivation = true;
                    yield return new WaitForSeconds(0.3f);
                    //Hides the loading screen
                    loadingBar.SetActive(false);
                }
            }
        } else {
            //Debug.LogError("GameManager > variable loadingBar not assigned!");
        }
    }


}
