using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[Serializable()]
public struct QuizUIElements {
    [Header("Question HUD")]
    public GameObject HolderPanel_Question;
    public Text info_UIText;
    public Button answer1_UIButton;
    public Button answer2_UIButton;
    public Button answer3_UIButton;
    public Button answer4_UIButton;
    public Sprite normal_button;
    public Sprite correct_button;
    public Sprite incorrect_button;
    [Space]
    [Header("Victory HUD")]
    public Text victoryHUDScoreMultiplier;
    public GameObject victoryPanel;
    public Button VictoryMainMenuButton;
}

public class QuizManager : MonoBehaviour {
    [HideInInspector]
    public static QuizManager quizManager;

    [Header("ASSIGN ON NEW LEVEL:")]
    public Question question;
    public QuizUIElements uiElements;
    public GameObject pauseMenuUI;

    private int wrongAttempts;
    private string button1Text;
    private string button2Text;
    private string button3Text;
    private string button4Text;

    private Button[] buttons_list;
    
    // Start is called before the first frame update
    void Start() {
        //Initializing array with the buttons references
        buttons_list = new Button[] {   uiElements.answer1_UIButton,
                                        uiElements.answer2_UIButton,
                                        uiElements.answer3_UIButton,
                                        uiElements.answer4_UIButton     };
        
        //Setting and displaying the question data on UI
        Display();

        //Initializing counter of wrong attempts
        wrongAttempts = 0;

        //Singleton checking
        if (quizManager == null) {
            quizManager = this;
        } else if (quizManager != this) {
            Destroy(gameObject);
        }
    }

    public void PauseGame() {
        if (pauseMenuUI != null) {
            pauseMenuUI.SetActive(true);
            SoundManagerScript.PlayClickButton();
        } else {
            Debug.LogError("Variable pauseMenuUI is not assigned!");
        }        
    }

    void Display() {
        //Saving reference of question's answers in each button
        button1Text = question.GetAnswers[0].GetInfo;
        button2Text = question.GetAnswers[1].GetInfo;
        button3Text = question.GetAnswers[2].GetInfo;
        button4Text = question.GetAnswers[3].GetInfo;

        //Setting each button's text with the question's answers
        uiElements.answer1_UIButton.GetComponentInChildren<Text>().text = button1Text;
        uiElements.answer2_UIButton.GetComponentInChildren<Text>().text = button2Text;
        uiElements.answer3_UIButton.GetComponentInChildren<Text>().text = button3Text;
        uiElements.answer4_UIButton.GetComponentInChildren<Text>().text = button4Text;
        uiElements.info_UIText.text = question.GetQuestion;

        //Setting the buttons onClick events 
        uiElements.answer1_UIButton.onClick.AddListener(() => CheckAnswerButtonClicked(button1Text, uiElements.answer1_UIButton));
        uiElements.answer2_UIButton.onClick.AddListener(() => CheckAnswerButtonClicked(button2Text, uiElements.answer2_UIButton));
        uiElements.answer3_UIButton.onClick.AddListener(() => CheckAnswerButtonClicked(button3Text, uiElements.answer3_UIButton));
        uiElements.answer4_UIButton.onClick.AddListener(() => CheckAnswerButtonClicked(button4Text, uiElements.answer4_UIButton));
    }

    void CheckAnswerButtonClicked(string answerInClickedButton, Button buttonClicked) {
        bool correctAnswer;
        //Debug.Log("Resposta escolhida: " + answerInClickedButton);
        //CORRECT ANSWER BUTTON
        if (answerInClickedButton.Equals(question.GetCorrectAnswer())) {
            correctAnswer = true;
            //Mostrar ao player que ele acertou

            switch(wrongAttempts) {
                case 0:
                    GameManager.scoreMultiplier = 2f;
                    break;
                case 1:
                    GameManager.scoreMultiplier = 1.6f;
                    break;
                case 2:
                    GameManager.scoreMultiplier = 1.2f;
                    break;
                case 3:
                    GameManager.scoreMultiplier = 1f;
                    break;
                default:
                    GameManager.scoreMultiplier = 1f;
                    break;
            }

            PlayerPersistence.SetScoreMultiplier(GameManager.scoreMultiplier);

            //buttonClicked.GetComponent<Image>().sprite = uiElements.correct_button;

            foreach(Button button in buttons_list) {
                if(button == buttonClicked) {
                    button.GetComponent<Image>().sprite = uiElements.correct_button;
                } else {
                    button.GetComponent<Image>().sprite = uiElements.incorrect_button;
                }
            }
        } else {
            //WRONG ANSWER BUTTON
            correctAnswer = false;
            wrongAttempts++;            
        }
        updateUI(correctAnswer);
    }

    void updateUI(bool correctAnswer) {
        if (correctAnswer) {
            //CORRECT ANSWER UI
            //Debug.Log("Resposta CORRETA!");
            try {
                //Sets the score multiplier text on UI
                if(GameManager.scoreMultiplier == 1f || GameManager.scoreMultiplier == 2f)
                    uiElements.victoryHUDScoreMultiplier.text = GameManager.scoreMultiplier.ToString("F0") + "x";
                else 
                    uiElements.victoryHUDScoreMultiplier.text = GameManager.scoreMultiplier.ToString("F1") + "x";
                //Calls the victory panel animation
                uiElements.victoryPanel.GetComponent<Animator>().Play("slidein_quiz");
                //Plays the victory sound
                SoundManagerScript.PlaySound("quizCorrect");
            } catch (NullReferenceException nre) {
                Debug.LogError("QuizManagerException -> Check QuizManager.updateUI()");
            }
        } else {
            //WRONG ANSWER UI            
            //Debug.Log("Resposta ERRADA!");
            SoundManagerScript.PlaySound("quizWrong");
            shakeHUD();
        }                
    }

    private void shakeHUD() {
        uiElements.HolderPanel_Question.GetComponent<Animator>().Play("Shake",-1,0f);
    }

    //public void loadMainMenu() {
    //    SceneManager.LoadScene("MainMenu");
    //}
}
