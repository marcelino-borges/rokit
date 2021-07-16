using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Answer {

    [SerializeField]
    private string info;
    public string GetInfo { get { return info; } }
    
    [SerializeField]
    private bool isCorrect;
    public bool GetIsCorrect { get { return isCorrect; } }
}
[CreateAssetMenu(fileName = "New Question", menuName = "Rokit/Quiz/New Question")]
public class Question : ScriptableObject {

    public enum AnswerType { Multi, Single}

    [SerializeField]
    [TextArea(5, 10)]
    private string question = string.Empty;
    public string GetQuestion { get { return question; } }

    
    [SerializeField]
    Answer[] answers = new Answer[4];
    public Answer[] GetAnswers { get { return answers; } }

    //Parameters
    /*
    [SerializeField]
    private bool useTimer = false;
    public bool GetUseTimer { get { return useTimer; } }
        
    [SerializeField]
    private int timer = 0;
    public int GetTimer { get { return timer; } }

    [SerializeField]
    private AnswerType answerType = AnswerType.Multi;
    public AnswerType GetAnswerType { get { return answerType; } }
    */

    [SerializeField]
    private int addScore = 10;
    public int GetAddScore { get { return addScore; } }
    /*
    public List<int> GetCorrectAnswers () {
        List<int> correctAnswers = new List<int>();

        for(int i = 0; i < GetAnswers.Length; i++) {
            if(GetAnswers[i].GetIsCorrect) {
                correctAnswers.Add(i);
            }
        }

        return correctAnswers;
    }
    */

    public string GetCorrectAnswer() { 
        for (int i = 0; i < GetAnswers.Length; i++) {
            if (GetAnswers[i].GetIsCorrect) {
                return GetAnswers[i].GetInfo;
            }
        }

        return null;
    }
}
