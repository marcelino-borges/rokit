using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;

[Serializable()]
public struct Match3UIElements {
    public Text score_UIText, movesLeft_UIText, dialogBalloon_UIText;
    [Tooltip("Assign the sprite of the score bar.")]
    public Image scoreBar;
    [Tooltip("Assign the sprite of an active star.")]
    public Sprite activeStar;
    [Tooltip("Assign the sprite of an inactive star.")]
    public Sprite inactiveStar;
    [Tooltip("Stars sprites placed in scene (UI). Set 3 in the Size field. At slot 0, assign the left star, at slot 1 assign the center star and at slot 2 assign the right star sprites.")]
    public Image[] starsGoals_UIImage;
    [Tooltip("Assign the victory panel placed in the scene hierarchy.")]
    public GameObject victoryPanel;
    [Tooltip("Assign the dialog panel placed in the scene hierarchy (the prefab child).")]
    public GameObject dialogPanel;
    [Tooltip("Assign the game over panel placed in the scene hierarchy.")]
    public GameObject gameOverPanel;
    [Tooltip("Assign the player hud game object placed in the scene hierarchy.")]
    public GameObject playerHud;
}

[Serializable]
public class LevelInfo {
    public string name;
    public long stars;
    public long score;
    public long world;
    public long number;

    public LevelInfo(string name, long stars, long score, long world, long number) {
        this.name = name;
        this.stars = stars;
        this.score = score;
        this.world = world;
        this.number = number;
    }

    public override bool Equals(object obj) {
        if (obj == null) return false;
        LevelInfo objAsPart = obj as LevelInfo;
        if (objAsPart == null) return false;
        else return Equals(objAsPart);
    }

    public bool Equals(LevelInfo other) {
        if (other == null) return false;
        return (this.name.Equals(other.name));
    }

    public override string ToString() {
        return "LevelInfo = [Name: " + name + " | Stars won: " + stars + " | Score won: " + score + "]";
    }
}

public class Match3Manager : MonoBehaviour {
    [Tooltip("Assign all the UI elements into this.")]
    public Match3UIElements uIElements;  

    [Header("MATCH 3 RULES:")]
    //[Tooltip("Score that will make the player win the level.")]
    //public int victoryScore = 300;
    [Tooltip("Max amount of movementos the player can do in the level.")]
    public int maxMoves = 15;
    [Tooltip("Time the player must wait before playing again (cooldown).")]
    public float movementCooldownTime = .5f;
    [Tooltip("Base value used for scoring each gem in a match.")]
    public int gemBaseValue = 20;
    [Tooltip("Base value to the special item, when it reaches the bottom of the grid.")]
    public int specialItemBaseValue = 150;
    [Tooltip("Will the level show the character's dialog panel at the beginning of the level?")]
    public bool showCharacterDialogAtStart;
    [Tooltip("Type the text to be shown at the dialog panel (after the fixed text with 'Precisamos localizar no mínimo scoreGoals[0]'.")]
    [TextArea(3,3)]
    public string dialogTextComplement;
    [Tooltip("Goals the player must reach to achieve each one of the 3 stars.")]
    public int[] scoreGoals;
    [Tooltip("Should we spawn a special item in the level?")]
    public bool spawnSpecialItem;
    //[Tooltip("Ease that gems will use to move around.")]
    //public Ease gemMovement_Ease;
    [Tooltip("Time a gem takes to make a complete movement with Dotween.")]
    [Range(0.05f,1f)]
    public float gemMovement_time;
    public AnimationCurve gemMovement_CustomEase;

    [Header("ASSIGN:")]
    [Tooltip("The game object GameGrid, placed in the scene, found in the Hierarchy panel.")]
    public GameGrid grid;

    [Tooltip("How many power ups will be used in the level? Must be informed in order to check gameover when all power ups are used and player has no matchs left in the grid (deadlock).")]
    [HideInInspector]
    public int howManyPowerUps = 4;
    [Tooltip("Base score multiplier. Changes according to the player score at the Quiz.")]
    [HideInInspector]
    public float streakValue;
    [HideInInspector]
    public List<PowerUp> powerUpsUsed;
    [HideInInspector]
    public PowerUp activePowerUp = PowerUp.None;
    [HideInInspector]
    public bool isGameOver = false;
    [HideInInspector]
    public bool isGameWon = false;
    [HideInInspector]
    public static Match3Manager match3Manager;
    [HideInInspector]
    public bool hasCollectedSpecialItem = false;

    private LevelInfo currentLevelInfo;

    private int currentScoreGoal = 0;
    [Header("--> DEBUG ONLY: ")]
    public int currentScore = 0;
    public int Score { get { return currentScore; } set { currentScore = value; } }
    //[HideInInspector]
    public int movesLeft;

    private bool hasShownVictoryPanel;

    [HideInInspector]
    public bool keepPlayingLevel;

    [HideInInspector]
    public int quantityFrozenRocks = 0;
    [HideInInspector]
    public int quantityBreakable = 0;
    [HideInInspector]
    public List<Mission> missionsInLevel;

    // ---------- METHODS -----------------//

    private void Awake() {
        missionsInLevel = new List<Mission>();
    }

    // Start is called before the first frame update
    void Start() {
        keepPlayingLevel = false;
        //Debug.Log("LevelInfo:\nName: " + SceneManager.GetActiveScene().name);
        //Debug.Log("Score: " + currentScore);
        //Debug.Log("Stars: " + CheckHowManyStars());
        //Debug.Log("World: " + int.Parse(SceneManager.GetActiveScene().name.Substring(1, 1)));
        //Debug.Log("Number: " + int.Parse(SceneManager.GetActiveScene().name.Substring(3, 1)));
        streakValue = 1;
        isGameWon = false;
        hasShownVictoryPanel = false;
        
        powerUpsUsed = new List<PowerUp>();

        string characterDialogText = "Precisamos localizar no mínimo " + scoreGoals[0] + dialogTextComplement;
        SetDialogPanelText(characterDialogText);

        if(match3Manager == null) {            
            match3Manager = this;
        } else if(match3Manager != this) {
            Destroy(gameObject);
        }    

        movesLeft = maxMoves;
        if (uIElements.movesLeft_UIText != null) {
            uIElements.movesLeft_UIText.text = movesLeft.ToString();
        }

        if(showCharacterDialogAtStart) {
            ShowDialogPanel();
        }
        
        DeactivateStar(0);
        DeactivateStar(1);
        DeactivateStar(2);

        GameManager.scoreMultiplier = PlayerPersistence.GetScoreMultiplier();        

        //Showing on the UI that the special item exists
        if(spawnSpecialItem) {
            uIElements.playerHud.GetComponent<PlayerHud>().specialItemUI.SetActive(true);
            uIElements.playerHud.GetComponent<PlayerHud>().specialItemCheckUI.SetActive(false);
        }        
    }

    //Adds a custom amount of points to the score
    public int AddScore(double amount) {
        currentScore += (int)Mathf.Round((float)(amount * GameManager.scoreMultiplier));
        SetFormattedNumberToUi(uIElements.score_UIText, currentScore);
        //Debug.Log("currentScore = " + currentScore);
        //Debug.Log("amount = " + amount);
        //Debug.Log("GameManager.scoreMultiplier = " + GameManager.scoreMultiplier);
        //Debug.Log("streakValue = " + streakValue);
        //Debug.Log("gemBaseValue = " + gemBaseValue);

        UpdateScoreBar();

        //Checks if the player's got a star
        if(CheckForStar()) {
            if (grid.levelCharacter != null) {
                grid.levelCharacter.GetComponent<ZelulaLevel>().MakeStarOrVictoryAnim();
            } else {
                //Debug.LogError("Variable grid.levelCharacter not assigned!");
            }
        }
        //Makes the level character show animation of celebration 
        else {
            if (grid.levelCharacter != null) {
                grid.levelCharacter.GetComponent<ZelulaLevel>().MakeMatchAnimation();
            } else {
                //Debug.LogError("Variable grid.levelCharacter not assigned!");
            }
        }

        //Check if player got 3 stars within the max moves allowed
        if (CheckHowManyStars() == 3) {
            //Debug.Log("1");
            //Have all missions been completed?
            if (WereMissionsCompleted()) {
                //if the player has moves left
                if (isOutOfMoves()) {
                    //Give the player victory
                    CallVictoryRoutines(3);
                } else {
                    if (!keepPlayingLevel) {
                        //Confirm if the player wants to keep playing 
                        uIElements.playerHud.GetComponent<PlayerHud>().ShowKeepPlayingConfirmation();
                    }
                }
            }
            ////if we have a special item in the level
            //if (spawnSpecialItem) {
            //    //Debug.Log("2");
            //    //Has already collected the special item?
            //    if (hasCollectedSpecialItem) {
            //        //Debug.Log("3");
            //        //if the player has moves left
            //        if (!isOutOfMoves()) {
            //            //Debug.Log("movesLeft > 0");
            //            if (!keepPlayingLevel) {
            //                //Confirm if the player wants to keep playing 
            //                uIElements.playerHud.GetComponent<PlayerHud>().ShowKeepPlayingConfirmation();
            //            }
            //        } else {
            //            //Debug.Log("movesLeft <= 0");
                        
            //            //If all missions were accomplished
            //            if(WereMissionsCompleted()) {
            //                //Give the player victory
            //                CallVictoryRoutines(3);
            //            }
            //        }
            //    }
            ////If there is no special item
            //} else {
            //    //if the player is NOT OUT OF MOVES
            //    if (!isOutOfMoves()) {
            //        //If all missions were accomplished
            //        if (WereMissionsCompleted()) {
            //            if (!keepPlayingLevel) {
            //                //Confirm with the player if he wants to keep playing 
            //                uIElements.playerHud.GetComponent<PlayerHud>().ShowKeepPlayingConfirmation();
            //            }
            //        }
            //    } else {
            //        //Debug.Log("movesLeft <= 0");

            //        //If all missions were accomplished
            //        if (WereMissionsCompleted()) {
            //            //Give the player victory
            //            CallVictoryRoutines(3);
            //        } else {
            //            CallGameOverRoutines();
            //        }
            //    }

            //    //TESTADO-VERIFICADO
            //}
        }

        return currentScore;
    }

    public bool WereMissionsCompleted() {
        int completedMissionsCount = 0;
        foreach (Mission mission in missionsInLevel) {
            //Debug.Log("Is misson " + mission.missionType + " completed? " + mission.isCompleted);
            if (!mission.isCompleted) {

                return false;
            }
        }
        //If all were accomplished
        //return (completedMissionsCount >= missionsInLevel.Count);
        return true;
    }

    public void CallVictoryRoutines(int stars) {        
        if (!hasShownVictoryPanel) {
            StartCoroutine(showVictoryPanel(stars, currentScore));
            //If the player wins the current level, saves the current level as the last one played
            SoundManagerScript.PlaySound("matchVictory");

            currentLevelInfo = new LevelInfo(SceneManager.GetActiveScene().name, CheckHowManyStars(), currentScore,
                                                int.Parse(SceneManager.GetActiveScene().name.Substring(1, 1)), int.Parse(SceneManager.GetActiveScene().name.Substring(3, 1)));

            PlayerPersistence.AddLevelToFile(currentLevelInfo);

            GameManager.canMakeMoves = false;
        }       
    }

    private IEnumerator showVictoryPanel(int stars, int score) {
        //Waits 2s before bringing the victory panel
        yield return new WaitForSeconds(2f);

        if (uIElements.victoryPanel != null) {
            //Debug.Log("Victory panel not null");
            //Calls the panel's slide in animation
            uIElements.victoryPanel.GetComponent<Animator>().Play("SlideIn");

            //Calls the method to set up the victory informations (stars and score)
            StartCoroutine(uIElements.victoryPanel.GetComponent<VictoryMenu>().SetUpPanel(stars, score));
        } else {
            //Debug.LogError("Variable uIElements.victoryPanel not assigned!");
        }
        hasShownVictoryPanel = true;
    }

    private void PlayStarAnimation(int spriteIndex) {
        uIElements.starsGoals_UIImage[spriteIndex].gameObject.GetComponent<Animator>().Play("StarEarned");
    }

    private void ActivateStar(int spriteIndex) {
        uIElements.starsGoals_UIImage[spriteIndex].sprite = uIElements.activeStar;
    }

    private void DeactivateStar(int spriteIndex) {
        uIElements.starsGoals_UIImage[spriteIndex].sprite = uIElements.inactiveStar;
    }

    /// <summary>
    /// Checks how many stars the player has won so far
    /// </summary>
    private bool CheckForStar() {
        // TODO: REFATORAR ESSE METODO, NA PARTE DAS CHAMADAS PARA O ACTIVATESTAR() E OS DEMAIS CÓDIGOS REPETIDOS
        //Check if reached the first score goal to set 1 star to the player
        if (CheckHowManyStars() == 1) {
            //If the current score goal is 0 (first score goal)
            if (currentScoreGoal == 0) {
                //Setting the star 0 sprite to the active one
                ActivateStar(0);
                //Playing star 0 animation
                PlayStarAnimation(0);

                SoundManagerScript.PlaySound("star");
                currentScoreGoal++;
                grid.levelCharacter.GetComponent<ZelulaLevel>().MakeStarOrVictoryAnim();

                return true;
            }
        }
        //Check if reached the second score goal to set 2 stars to the player
        else if (CheckHowManyStars() == 2) {
            //If the current score goal is 0 (first score goal)
            if (currentScoreGoal == 1) {
                //Setting the star 0 sprite to the active one
                ActivateStar(0);
                //Setting the star 1 sprite to the active one
                ActivateStar(1);
                //Playing star 1 animation
                PlayStarAnimation(1);

                SoundManagerScript.PlaySound("star");
                currentScoreGoal++;
                grid.levelCharacter.GetComponent<ZelulaLevel>().MakeStarOrVictoryAnim();

                return true;
            }
        }
        //Check if reached the third score goal to set 3 stars to the player
        else if (CheckHowManyStars() == 3) {
            //If the current score goal is 0 (first score goal)
            if (currentScoreGoal == 2) {
                //Setting the star 0 sprite to the active one
                ActivateStar(0);
                //Setting the star 1 sprite to the active one
                ActivateStar(1);
                //Setting the star 2 sprite to the active one
                ActivateStar(2);
                //Playing star 2 animation
                PlayStarAnimation(2);

                SoundManagerScript.PlaySound("star");
                currentScoreGoal++;
                grid.levelCharacter.GetComponent<ZelulaLevel>().MakeStarOrVictoryAnim();

                return true;
            }
        }

        return false;
    }

    private void UpdateScoreBar() {
        if (uIElements.scoreBar != null) {
            uIElements.scoreBar.fillAmount = ((float)currentScore / (float)scoreGoals[2]);
        }
    }

    private void SetDialogPanelText(string text) {
        uIElements.dialogBalloon_UIText.text = text;
    }

    private void ShowDialogPanel() {
        if (uIElements.dialogPanel != null) {
            uIElements.dialogPanel.GetComponent<Animator>().Play("SlideIn");
            GameManager.canMakeMoves = false;
        } else {
            //Debug.LogError("Variable dialogPanel not assigned!");
        }
    }

    public void HideDialogPanel() {
        if (uIElements.dialogPanel != null) {
            uIElements.dialogPanel.GetComponent<Animator>().Play("SlideOut");
            GameManager.canMakeMoves = true;
        } else {
            //Debug.LogError("Variable uIElements.dialogPanel not assigned!");
        }
    }

    /// <summary>
    /// Decrease an specific amount of moves
    /// </summary>
    public int IncreaseMovesLeft(int amount) {
        movesLeft += amount;
        SetFormattedNumberToUi(uIElements.movesLeft_UIText, movesLeft);

        return movesLeft;
    }

    /// <summary>
    /// Decreases one from score (by default)
    /// </summary>
    public int DecreaseMovesLeft() {
        return DecreaseMovesLeft(1);
    }

    /// <summary>
    /// Decrease an specific amount of moves
    /// </summary>
    public int DecreaseMovesLeft(int amount) {
        movesLeft -= amount;
        SetFormattedNumberToUi(uIElements.movesLeft_UIText, movesLeft);

        //Debug.Log("How many missions in level: " + missionsInLevel.Count);
        //Debug.Log("WereMissionsCompleted() = " + WereMissionsCompleted());

        //Checks if the player is out of moves
        if (isOutOfMoves()) {
            //Checks how many stars were earned in the level
            switch(CheckHowManyStars()) {
                //if there is 0 star in the level
                case 0:
                    //Give the player a game over
                    CallGameOverRoutines();        
                    break;
                //if there is 1 star in the level
                case 1:
                    //if we have a special item in the level
                    if (spawnSpecialItem) {                     
                        
                        //If all missions were accomplished
                        if(WereMissionsCompleted()) {
                            //Give the player victory
                            CallVictoryRoutines(1);                        

                        //Hasn't completed the missions?
                        } else {
                            //Give the player a game over
                            CallGameOverRoutines();
                        }
                    } else {                        
                        //Give the player victory
                        CallVictoryRoutines(1);
                    }
                    break;
                //if there are 2 stars in the level
                case 2:
                    //if we have a special item in the level
                    if (spawnSpecialItem) {                        
                        //If all missions were accomplished
                        if(WereMissionsCompleted()) {
                            //Give the player victory
                            CallVictoryRoutines(2);
                        
                        //Hasn't completed the missions?
                        } else {
                            //Give the player a game over
                            CallGameOverRoutines();
                        }
                    } else {
                        //Give the player victory
                        CallVictoryRoutines(2);
                    }
                    break;
                //if there are 2 stars in the level
                case 3:
                    //if we have a special item in the level
                    if (spawnSpecialItem) {                        
                        //If all missions were accomplished
                        if(WereMissionsCompleted()) {
                            //Give the player victory
                            CallVictoryRoutines(3);
                        
                        //Hasn't completed the missions?
                        } else {
                            //Give the player a game over
                            CallGameOverRoutines();
                        }
                    } else {
                        //Give the player victory
                        CallVictoryRoutines(3);
                    }
                    break;
            }

            //The 3 stars case is covered in addScore()

            //#if UNITY_ANDROID
            ////Calls the notification method
            //if (PlayerPersistence.GetHeartsStored() < 5) {
            //        GameManager.SendHeartNotification();
            //    }
            //#endif

            //TODO: Avisar ao player que daqui a 20min chegará notificação com +1 coração
        }

        return movesLeft;
    }

    /// <summary>
    /// Checks how many stars, if any, the player has earned according to the current score
    /// </summary>
    /// <returns>Stars already earned</returns>
    public int CheckHowManyStars() {
        //If the player got no star yet (hasn't reached the first score goal
        if (currentScore < scoreGoals[0]) {
            return 0;
        }
        //If the player has exactly one star (reached only the first score goal)
        else if (currentScore >= scoreGoals[0] && currentScore < scoreGoals[1]) {
            return 1;
        }
        //If the player has exactly two stars (reached only the second score goal)
        else if (currentScore >= scoreGoals[1] && currentScore < scoreGoals[2]) {
            return 2;
        }
        //If the player has exactly three stars (reached only the second score goal)
        else if (currentScore >= scoreGoals[2]) {
            return 3;
        }
        return -1;
    }

    private bool isOutOfMoves() {
        return movesLeft <= 0;
    }

    public void CallGameOverRoutines() {
        //If game is over
        //Checks if the panel is assigned (not null) and shows it
        if (uIElements.gameOverPanel != null) {
            StartCoroutine(showGameOverPanel());
        } else {
            //Debug.LogError("Variable uIElements.gameOverPanel not assigned!");
        }
    }

    private IEnumerator showGameOverPanel() {
        yield return new WaitForSeconds(2f);
        isGameOver = true;
        uIElements.gameOverPanel.GetComponent<GameOverMenu>().ActivateMenu();
        StartCoroutine(uIElements.gameOverPanel.GetComponent<GameOverMenu>().SetUpPanel(CheckHowManyStars(), currentScore));
    }

    public void RestartLevel() {
        currentScore = 0;
        movesLeft = maxMoves;
        SetFormattedNumberToUi(uIElements.movesLeft_UIText, movesLeft);
        SetFormattedNumberToUi(uIElements.score_UIText, currentScore);
        GameManager.UnPauseGame();
        isGameOver = false;
        grid.ResetGrid();
        GameManager.canMakeMoves = true;
    }

    //Treats the format of the number set to the UI Text element (< 10)
    private void SetFormattedNumberToUi(Text textField, int number) {
        if (number < 10) {
            textField.text = "0" + number.ToString();
        } else {
            textField.text = number.ToString();
        }
    }
}


