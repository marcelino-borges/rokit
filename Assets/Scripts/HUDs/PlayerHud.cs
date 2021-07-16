using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour {
    [Header("ASSIGN:")]
    public GameObject pauseMenuUI;
    public GameObject keepPlaying_confirmationPanel;

    [Tooltip("The main UI image shown when there is a special item in the level")]
    public GameObject specialItemUI;
    [Tooltip("The check icon of the special item, shown when this mission is completed")]
    public GameObject specialItemCheckUI;
    [Tooltip("The main UI image shown when there are frozen rocks in the level")]
    public GameObject frozenRocksUI;
    [Tooltip("The check icon of the frozen rocks, shown when this mission is completed")]
    public GameObject frozenRocksCheckUI;
    [Tooltip("The main UI image shown when there are breakable tiles in the level")]
    public GameObject breakableTilesUI;
    [Tooltip("The check icon of the breakable tiles, shown when this mission is completed")]
    public GameObject breakableTilesCheckUI;
    [Tooltip("The UI text gameobject inside the \"Chamma\" powerup button")]
    public GameObject movementsToIncrease_UIText;

    public static Text heartsAvailable;

    [HideInInspector]
    public GameGrid grid;
    [HideInInspector]
    public int movementsToIncrease = 5;

    private Match3Manager match3Manager;

    private void Start() {
        grid = GameObject.FindWithTag("Grid").GetComponent<GameGrid>();
        match3Manager = grid.match3Manager;
        movementsToIncrease_UIText.GetComponent<Text>().text = "+" + movementsToIncrease;
    }

    public void PauseGame() {
        if (!GameManager.isGamePaused) {
            if(pauseMenuUI != null)
                pauseMenuUI.SetActive(true);
            GameManager.PauseGame();
        }
        SoundManagerScript.PlayClickButton();
    }

    //
    // ----------------- POWER UPS --------------------------------------------//
    //

    //Sets the button's image color to dark grey
    private static void DeactivatePowerUpButton(Button button) {
        //button.GetComponent<Image>().color = new Color32(80, 80, 80, 255);
        button.interactable = false;
    }

    //Sets the button's image color to white
    private static void ActivatePowerUpButton(Button button) {
        button.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }

    /// <summary>
    /// Shuffles the grid, changing randomly all gems in it
    /// </summary>
    public void ShuffleGrid(Button button) { 
        if (match3Manager.activePowerUp == PowerUp.None) {
            //If the Shuffle power up hasn't been used
            if (!match3Manager.powerUpsUsed.Contains(PowerUp.Shuffle)) {
                match3Manager.activePowerUp = PowerUp.Shuffle;

                //Shufle the grid
                grid.ShuffleGrid();

                DeactivatePowerUpButton(button);

                //Adds this powerup to the list of used powerups
                match3Manager.powerUpsUsed.Add(PowerUp.Shuffle);

                SoundManagerScript.PlaySound("shuffle_wind");

                Invoke("ClearPowerUpSelection", 0.2f);
            } else {
                // TODO: tell the user that he has already used the Shuffle power up
            }
            SoundManagerScript.PlayClickButton();
        }
    }

    public void Pick(Button button) {
        if (match3Manager.activePowerUp == PowerUp.None) {
            //If the Pick power up hasn't been used
            if (!match3Manager.powerUpsUsed.Contains(PowerUp.Pick)) {
                match3Manager.activePowerUp = PowerUp.Pick;

                DeactivatePowerUpButton(button);

                //ClearPowerUpSelectionCoRoutine();
            } else {
                // TODO: tell the user that he has already used the Pick power up
            }
            SoundManagerScript.PlayClickButton();
        } else if (match3Manager.activePowerUp == PowerUp.Pick) {
            ActivatePowerUpButton(button);
            match3Manager.activePowerUp = PowerUp.None;
            SoundManagerScript.PlaySound("cancel_powerup");
        }
    }

    public void Chamma(Button button) {
        if (match3Manager.activePowerUp == PowerUp.None) {
            //If the Chamma power up hasn't been used
            if (!match3Manager.powerUpsUsed.Contains(PowerUp.Chamma)) {
                match3Manager.activePowerUp = PowerUp.Chamma;

                //Sets the button's color to be dark
                DeactivatePowerUpButton(button);

                //Adds this powerup to the list of used powerups
                match3Manager.powerUpsUsed.Add(PowerUp.Chamma);

                //Plays the animation of the number to be increased "flying" to the MovesLeft UI counter
                gameObject.GetComponent<Animator>().Play("IncreaseMovementsLeft");

                Invoke("IncreaseMovementsLeft", 1f);

                Invoke("ClearPowerUpSelection", 0.2f);
            } else {
                // TODO: tell the user that he has already used the Chamma power up
            }
            SoundManagerScript.PlayClickButton();
        }
    }

    private void IncreaseMovementsLeft() {
        //Actually increasing the movements counter
        match3Manager.IncreaseMovesLeft(movementsToIncrease);
        //Make the proper sound
        SoundManagerScript.PlaySound("increaseMoves");
    }

    public void Dynamite(Button button) {
        if (match3Manager.activePowerUp == PowerUp.None) {
            //If the Dynamite power up hasn't been used
            if (!match3Manager.powerUpsUsed.Contains(PowerUp.Dynamite)) {
                match3Manager.activePowerUp = PowerUp.Dynamite;

                DeactivatePowerUpButton(button);

                //ClearPowerUpSelectionCoRoutine();
            } else {
                // TODO: tell the user that he has already used the Dynamite power up OR leave it with no feedback?
            }
            SoundManagerScript.PlayClickButton();
        } else if (match3Manager.activePowerUp == PowerUp.Dynamite) {
            ActivatePowerUpButton(button);
            match3Manager.activePowerUp = PowerUp.None;
            SoundManagerScript.PlaySound("cancel_powerup");
        }
    }

    private void ClearPowerUpSelection() {
        match3Manager.activePowerUp = PowerUp.None;
    }

    //Treats the format of the number set to the UI Text element (< 10)
    private string FormatNumber(int number) {
        if (number < 10) {
            return "0" + number.ToString();
        } else {
            return number.ToString();
        }
    }

    //Keep playing - Confirmation Panel

    public void ShowKeepPlayingConfirmation() {
        //SoundManagerScript.PlayClickButton();
        if (keepPlaying_confirmationPanel != null) {
            StartCoroutine(ShowKeepPlayingConfirmationCo());
        }
    }

    public IEnumerator ShowKeepPlayingConfirmationCo() {
        yield return new WaitForSeconds(1f);
        if (!keepPlaying_confirmationPanel.activeInHierarchy) {
            keepPlaying_confirmationPanel.SetActive(true);
            gameObject.GetComponent<Animator>().Play("KeepPlayingConfirmShowUp");
        }
    }

    public void KeepPlayingLevel() {
        if (keepPlaying_confirmationPanel != null) {
            //Hide panel
            keepPlaying_confirmationPanel.SetActive(false);
            grid.match3Manager.keepPlayingLevel = true;
        }
    }

    public void SkipPlayingLevel() {
        gameObject.GetComponent<Animator>().Play("KeepPlayingConfirmHide");
        keepPlaying_confirmationPanel.SetActive(false);
        grid.match3Manager.CallVictoryRoutines(3);
    }
}
