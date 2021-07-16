using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectionButton : MonoBehaviour {
    [SerializeField, Tooltip("Select the name of this level. DO NOT select the quiz level (ending with Q). If you can't find it, you might have forgotten to put this level's name in EnumCollection.Levels.")]
    private Levels sceneName;
    [Header("ASSIGN:")]
    public Text scoreText;
    public GameObject star0_active;
    public GameObject star1_active;
    public GameObject star2_active;
    public Sprite inactiveButton_Sprite;
    public Sprite activeButton_Sprite;
    public Sprite completedButton_Sprite;

    private void OnBecameVisible() {
        SetLevelButtons();
    }

    private void Start() {
        //If the LevelsPlayed List<> doesn't exist in the PlayerData stored
        //if (PlayerPersistence.LoadPlayerData().levelsPlayed != null) {
            //Debug.Log("LevelsPlayed in file: " + PlayerPersistence.LoadPlayerData().levelsPlayed);
        //} else {
            //If the player hasn't played yet, create a LevelsPlayed List<>, add the first level in it and save it to the local file
            //LevelInfo firstLevelInfo = new LevelInfo();
            //firstLevelInfo.name = SceneManager.GetActiveScene().name + "L1";
            //firstLevelInfo.score = 0;
            //firstLevelInfo.stars = 0;
            //firstLevelInfo.world = int.Parse(SceneManager.GetActiveScene().name.Substring(1,1));
            //firstLevelInfo.number = 1;

            //PlayerPersistence.AddLevelToFile(firstLevelInfo);
                        
            //Debug.Log("Saved LevelInfo to file: " + PlayerPersistence.LoadPlayerData().LevelsPlayed);
        //}

        SetLevelButtons();
    }

    

    public void SetLevelButtons() {
        if (PlayerPersistence.HasFileCreated()) {
            //Debug.Log("currentButton = " + sceneName);
            //Trying to retrieve the level info from the PlayerData, if available
            LevelInfo levelInfo = (LevelInfo)PlayerPersistence.HasPlayedLevel(sceneName.ToString());
            
            //
            //-----------LEVEL FOUND in PlayerData
            //
            //Debug.Log("------------------- Inicio LevelSelection.Start()");
            if (levelInfo != null) {
                //Debug.Log("level " + sceneName + " FOUND!");
                //Debug.Log("level (" + levelInfo.name + ") stars = " + levelInfo.stars);
                //Setting-Activating the stars
                switch (levelInfo.stars) {
                    case 1:
                    if (star0_active != null) {
                        star0_active.SetActive(true);
                    } else {
                        Debug.LogError("Variable star0_active nor assigned!");
                    }
                    break;
                    case 2:
                    if (star0_active != null) {
                        star0_active.SetActive(true);
                    } else {
                        Debug.LogError("Variable star0_active nor assigned!");
                    }
                    if (star1_active != null) {
                        star1_active.SetActive(true);
                    } else {
                        Debug.LogError("Variable star1_active nor assigned!");
                    }
                    break;
                    case 3:
                    if (star0_active != null) {
                        star0_active.SetActive(true);
                    } else {
                        Debug.LogError("Variable star0_active nor assigned!");
                    }
                    if (star1_active != null) {
                        star1_active.SetActive(true);
                    } else {
                        Debug.LogError("Variable star1_active nor assigned!");
                    }
                    if (star2_active != null) {
                        star2_active.SetActive(true);
                    } else {
                        Debug.LogError("Variable star2_active nor assigned!");
                    }
                    break;
                }

                scoreText.text = levelInfo.score.ToString();

                CompleteLevel();
            //
            //-----------LEVEL NOT FOUND in PlayerData
            //
            } else {
                //Debug.Log("current level " + sceneName + " NOT FOUND!");
                int currentLevelNumber = int.Parse(sceneName.ToString().Substring(3, 1));
                //Debug.Log("current level number in button = " + currentLevelNumber);

                //If it's the first level of the world, then make interactible
                if (currentLevelNumber <= 1) {
                    ActivateLevel();

                    //If it's not the first level of the world
                } else {
                    //Gets the number of the last level
                    int priorLevelNumber = int.Parse(sceneName.ToString().Substring(3, 1)) - 1;
                    //Debug.Log("previous level number = " + priorLevelNumber);
                    //Mounting last level's name
                    string priorLevelName = sceneName.ToString().Substring(0, 3) + priorLevelNumber.ToString();
                    //Debug.Log("previous level name = " + priorLevelName);

                    //Check if the last level is stored
                    LevelInfo lastLevelInfo = (LevelInfo)PlayerPersistence.HasPlayedLevel(priorLevelName);
                    //If we find the last level stored in PlayerData, Activate this button
                    if (lastLevelInfo != null) {
                        //Debug.Log("previous level FOUND");
                        ActivateLevel();
                    } else {
                        //Debug.Log("previous level NOT FOUND");
                        DeactivateLevel();
                    }
                }
            }
        } else {
            int currentLevelNumber = int.Parse(sceneName.ToString().Substring(3, 1));
            //Debug.Log("currentLevelNumber = " + currentLevelNumber);
            //If it's the first level of the world, then make interactible
            if (currentLevelNumber <= 1) {
                ActivateLevel();
                //If it's not the first level of the world
            }
        }
        
    }    

    public void loadLevel() {
        SoundManagerScript.PlayClickButton();
        GameManager.LoadLevel(sceneName.ToString());
    }

    public void loadWorldSelection() {
        GameManager.LoadLevel("WorldSelection");
    }

    public void loadMainMenu() {
        GameManager.LoadLevel("MainMenu");
    }

    public void ActivateLevel() {
        gameObject.GetComponent<Button>().interactable = true;
        gameObject.GetComponent<Image>().sprite = activeButton_Sprite;
    }

    public void DeactivateLevel() {
        gameObject.GetComponent<Button>().interactable = false;
        gameObject.GetComponent<Image>().sprite = inactiveButton_Sprite;
    }

    public void CompleteLevel() {
        gameObject.GetComponent<Button>().interactable = true;
        gameObject.GetComponent<Image>().sprite = completedButton_Sprite;
    }
}
