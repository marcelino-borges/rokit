using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public GameObject homeConfigsHud;
    

    private void Start() {
        GameObject msgPanel = GameObject.FindWithTag("msgPanel");

        if (PlayerPersistence.HasFileCreated()) {
            //Debug.Log("MainMenu > Has a PlayerData file created.");
            //msgPanel.GetComponentInChildren<Text>().text = "Hearts Stored: " + PlayerPersistence.GetHeartsStored().ToString();            
        } else {
            //Debug.Log("MainMenu > Hasn't a PlayerData file created.");
        }
    }

    //Load the 1st level of the game
    //Change later to load the next level acording to the last one played
    public void StartGame() {
        SoundManagerScript.PlaySound("play_main_menu");
        //Loads the next scene of the build settings
        GameManager.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame() {
        SoundManagerScript.PlayClickButton();
        Application.Quit();
    }

    public void ShowConfigsMenu() {
        SoundManagerScript.PlayClickButton();
        if (homeConfigsHud != null) {
            homeConfigsHud.SetActive(true);
        }
    }

}
