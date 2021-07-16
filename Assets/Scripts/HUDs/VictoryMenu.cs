using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryMenu : MonoBehaviour {

    private Match3Manager match3Manager;
    public GameObject star0;
    public GameObject star0_active;
    public GameObject star0_active_shine;
    public GameObject star1;
    public GameObject star1_active;
    public GameObject star1_active_shine;
    public GameObject star2;
    public GameObject star2_active;
    public GameObject star2_active_shine;
    private bool startScoreCount = false;
    public Text scoreText;
    private int scoreTemp = 0;

    private void Start() {
        //TODO: Ver forma de diminuir esses FindTagWith(). Aqui, provavelmente mandando o match3Manager salvar uma referencia de si próprio na variavel match3Manager daqui, no momento em que chamar o painal
        match3Manager = GameObject.FindWithTag("Match3Manager").GetComponent<Match3Manager>();
    }

    private void Update() {
        if (startScoreCount) {
            //Debug.Log("ScoreTEMP = " + scoreTemp);
            if (scoreText != null) {
                if (scoreTemp < match3Manager.currentScore) {

                    if (match3Manager.currentScore <= 5000)
                        scoreTemp += 50;
                    else if (match3Manager.currentScore > 5000 && match3Manager.currentScore <= 15000)
                        scoreTemp += 150;
                    else if (match3Manager.currentScore > 15000 && match3Manager.currentScore <= 30000)
                        scoreTemp += 250;
                    else if (match3Manager.currentScore > 30000)
                        scoreTemp += 500;

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
        yield return new WaitForSeconds(1f);
        startScoreCount = true;
        if (stars == 1) {
            InitializeStar(star0_active, star0_active_shine, 0);
        } else if (stars == 2) {
            InitializeStar(star0_active, star0_active_shine, 0);
            yield return new WaitForSeconds(0.5f);
            InitializeStar(star1_active, star1_active_shine, 1);
        } else if (stars == 3) {
            InitializeStar(star0_active, star1_active_shine, 0);
            yield return new WaitForSeconds(0.5f);
            InitializeStar(star1_active, star1_active_shine, 1);
            yield return new WaitForSeconds(0.5f);
            InitializeStar(star2_active, star2_active_shine, 2);
        }

        //yield return new WaitForSeconds(0.5f);
        //startScoreCount = true;
    }

    private void InitializeStar(GameObject star, GameObject shine, int starIndex) {
        shine.SetActive(true);
        star.GetComponent<Animator>().Play("star" + starIndex + "_Earned");
        SoundManagerScript.PlaySound("star");
    }

    public void loadQuiz() {
        GameManager.LoadLevel(SceneManager.GetActiveScene().name + "Q");
        SoundManagerScript.PlayClickButton();
    }
}
