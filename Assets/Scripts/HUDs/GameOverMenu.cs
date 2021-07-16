using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour {

    private Match3Manager match3Manager;
    public Animator heart;
    public GameObject gameOverMenu;
    public Text heartsText;
    public Text scoreText;
    private bool startScoreCount = false;
    private int scoreTemp = 0;

    private void Start() {
        match3Manager = GameObject.FindWithTag("Match3Manager").GetComponent<Match3Manager>();
    }

    public void Update() {
        if (startScoreCount) {
            //Debug.Log("ScoreTEMP = " + scoreTemp);
            if (scoreText != null) {
                if (scoreTemp < match3Manager.currentScore) {
                    scoreTemp += 50;
                    scoreText.text = scoreTemp.ToString();
                    SoundManagerScript.PlaySound("coin");
                } else {
                    scoreText.text = match3Manager.currentScore.ToString();
                    startScoreCount = false;
                    SoundManagerScript.PlaySound("coins");
                }
            } else {
                Debug.LogError("Variable scoreText not assigned!");
            }            
        }        
    }

    public IEnumerator SetUpPanel(int stars, int score) {        
        yield return new WaitForSeconds(0.5f);
        startScoreCount = true;
    }

    public void ClearPanel() {
        scoreText.text = "";
    }

    public void ActivateMenu() {
        if (!gameOverMenu.activeInHierarchy) {
            gameOverMenu.SetActive(true);
            GameManager.PauseGame();
            UpdateUIData();
        }
    }

    public void DeactivateMenu() {
        gameOverMenu.SetActive(false);
        GameManager.UnPauseGame();
    }

    public void UpdateUIData() {
        //Debug.Log(PlayerPersistence.GetHeartsStored().ToString());
        heartsText.text = PlayerPersistence.GetHeartsStored().ToString();
    }

    private void RestartLevel() {
        //Restarts the level
        Debug.Log("Restarting level...");        
        DeactivateMenu();
        GameManager.LoadLevel(SceneManager.GetActiveScene().name);
    }


    //---------------
    //BUTTONS:
    //---------------
    
    public void TryAgain() {        
        PlayClickSound();
        match3Manager.grid.currentState = GameState.move;
        //If the player has hearts in his stored data
        //if(PlayerPersistence.HasHearts()) {
        
        //Decreases player's hearts and stores it in a var
        //PlayerPersistence.DecreaseHeart(1);

        //heart.Play("HeartDecreasing");

        //UpdateUIData();

        //TODO: Updates the UI with the hearts left
        Invoke("RestartLevel", 1f);
        DeactivateMenu();

        //} else {
        //    //TODO: Show the user he hasn't enough hearts to play again
        //}

    }

    public void LoadMainMenu() {
        //Loads the next scene of the build settings
        SceneManager.LoadScene("MainMenu");
        GameManager.UnPauseGame();
        match3Manager.isGameOver = false;
        PlayClickSound();
    }

    public void QuitGame() {
        PlayClickSound();
        Application.Quit();
    }

    private void PlayClickSound() {
        //Plays click sound
        SoundManagerScript.PlaySound("btnClick");
    }

}
