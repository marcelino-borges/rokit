using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuizVictoryButton : MonoBehaviour {
    public GameObject victoryHolderPanel;

    public void loadLevel() {
        //Gets the first 2 letters of the current scene's name (according to our scene naming pattern - "W1L1, W1L1Q, W1L2, W2L1 etc.")
        string worldName = SceneManager.GetActiveScene().name.Substring(0, 2);
        //Debug.Log("world name: " + worldName);
        GameManager.LoadLevel(worldName);
        victoryHolderPanel.SetActive(false);
    }
}
