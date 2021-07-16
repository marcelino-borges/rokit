using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    //public GameObject pauseMenuUI;
    public GameObject musicInactiveSprite;
    public GameObject inactiveSfx_sprite;
    public GameObject exitConfirmationPanel;
    public GameObject restartConfirmationPanel;
    public GameObject homeConfirmationPanel;
    public GameObject playerHUD;

    private void Start() {
        if (!GameManager.isGamePaused) {
            gameObject.SetActive(false);
        }

        if (PlayerPersistence.GetGameConfigs().wishMusicOn) {
            musicInactiveSprite.SetActive(false);
        } else {
            musicInactiveSprite.SetActive(true);
        }

        if (PlayerPersistence.GetGameConfigs().wishSfxOn) {
            inactiveSfx_sprite.SetActive(false);
        } else {
            inactiveSfx_sprite.SetActive(true);
        }

    }

    private void OnEnable() {
        gameObject.GetComponent<Animator>().Play("ShowUp");
        
    }

    // ----------------- BUTTONS -----------------------------------------------//

    public void MuteMusic() {
        if (MusicManager.isOn) {
            MusicManager.Mute();
            musicInactiveSprite.SetActive(true);
            Debug.Log("Button to mute music clicked. Music was on.");
        } else {
            MusicManager.UnMute();
            musicInactiveSprite.SetActive(false);
            Debug.Log("Button to mute music clicked. Music was off.");
        }
    }

    public void MuteSfx() {
        if (SoundManagerScript.isOn) {
            SoundManagerScript.Mute();
            inactiveSfx_sprite.SetActive(true);
        } else {
            SoundManagerScript.UnMute();
            inactiveSfx_sprite.SetActive(false);
        }
    }

    public void ResumeGame() {
        // Debug.Log("Clicou resume button");
        StartCoroutine(ResumeGameDelayed());
    }

    private IEnumerator ResumeGameDelayed() {
        SoundManagerScript.PlayClickButton();
        gameObject.GetComponent<Animator>().Play("Hide");
        yield return new WaitForSeconds(.25f);
        gameObject.SetActive(false);
        GameManager.UnPauseGame();
    }

    public void LoadMainMenu() {
        SoundManagerScript.PlayClickButton();
        GameManager.UnPauseGame();
        SceneManager.LoadScene("MainMenu");
    }

    //Exit game confirmation

    public void ConfirmExit() {
        SoundManagerScript.PlayClickButton();
        exitConfirmationPanel.SetActive(true);
        //gameObject.GetComponent<Animator>().Play("ExitConfirmationShowUp");
    }

    public void QuitGame() {
        SoundManagerScript.PlayClickButton();
        Application.Quit();
    }

    public void ResumeExitConfirmation() {
        //gameObject.GetComponent<Animator>().Play("ExitConfirmationHide");
        SoundManagerScript.PlayClickButton();
        exitConfirmationPanel.SetActive(false);
    }

    //Restart the game confirmation

    public void ConfirmRestart() {
        SoundManagerScript.PlayClickButton();
        restartConfirmationPanel.SetActive(true);
    }

    public void RestartLevel() {

        GameManager.LoadLevel(SceneManager.GetActiveScene().name);

        //if (playerHUD != null) {
        //    playerHUD.GetComponent<PlayerHud>().grid.match3Manager.RestartLevel();
        //    ResumeRestartConfirmation();
        //    ResumeGame();
        //}
    }

    public void RestartQuiz() {
        GameManager.LoadLevel(SceneManager.GetActiveScene().name);
        ResumeRestartConfirmation();
    }

    public void ResumeRestartConfirmation() {
        SoundManagerScript.PlayClickButton();
        restartConfirmationPanel.SetActive(false);
    }

    //Go to home confirmation

    public void ConfirmHome() {
        SoundManagerScript.PlayClickButton();
        homeConfirmationPanel.SetActive(true);
    }

    public void ResumeHomeConfirmation() {
        SoundManagerScript.PlayClickButton();
        homeConfirmationPanel.SetActive(false);
        
    }
}
