using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Multiplayer.Center.Common;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public struct Question
    {
        public string Text;
        public bool isTrueOrfalse;
        public string AnswerA;
        public string AnswerB;
        public string A;
        public bool IsCorrect;
    }
    public string Name = "Player";
    public string FileLocation = $"Matty/questions.csv";

    public GameObject GameView;

    public GameObject TextObject;
    public TMP_Text M_TextObjectText;
    public GameObject RightObject;
    public TMP_Text RightText;
    public GameObject LeftObject;
    public TMP_Text LeftText;
    public GameObject ScoreObject;
    public TMP_Text M_ScoreText;

    public Question[] QuestionList;
    public int CurrentQuestionIndex = 0;
    public int Score = 0;
    public int totalQuestions = 0;

    public List<int> questionLeft;


    public GameObject WinScren;
    public GameObject WinScoreObject;
    public TMP_Text WinScoreText;

    void Start()
    {
        M_TextObjectText = TextObject.GetComponent<TMP_Text>();
        RightText = RightObject.GetComponent<TMP_Text>();
        LeftText = LeftObject.GetComponent<TMP_Text>();
        M_ScoreText = ScoreObject.GetComponent<TMP_Text>();
        WinScoreText = WinScoreObject.GetComponent<TMP_Text>();
        M_TextObjectText.text = "Hello " + Name + "!";


        string filePath = Application.dataPath + FileLocation;
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            QuestionList = new Question[lines.Length];
            for (int i = 0; i < lines.Length && i < QuestionList.Length; i++)
            {
                string[] parts = lines[i].Split(',');
                if (parts.Length >= 3)
                {
                    QuestionList[i].Text = parts[0];
                    QuestionList[i].isTrueOrfalse = false;
                    QuestionList[i].AnswerA = parts[1];
                    QuestionList[i].AnswerB = parts[2];
                    QuestionList[i].A = parts[3];
                }
                else
                {
                    QuestionList[i].isTrueOrfalse = true;
                    QuestionList[i].Text = parts[0];
                    QuestionList[i].IsCorrect = (parts[1] == "true" ? true : false);
                }
            }
            totalQuestions = QuestionList.Length;
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }

        questionLeft = new List<int>();
        for (int i = 0; i < QuestionList.Length; i++)
        {
            questionLeft.Add(i);
        }
    }

    void Update()
    {
        M_TextObjectText.text = QuestionList[CurrentQuestionIndex].Text;
        if (!QuestionList[CurrentQuestionIndex].isTrueOrfalse)
        {
            LeftText.text = QuestionList[CurrentQuestionIndex].AnswerA;
            RightText.text = QuestionList[CurrentQuestionIndex].AnswerB;
        }
        else
        {
            LeftText.text = "False";
            RightText.text = "True";
        }
        M_ScoreText.text = Score + " / " + totalQuestions;
    }


    [ContextMenu("right?")]
    public void SwipeRight()
    {
        if (QuestionList[CurrentQuestionIndex].isTrueOrfalse)
        {
            if (QuestionList[CurrentQuestionIndex].IsCorrect)
            {
                AnswerRight();
            }
            else
            {
                AnswerWrong();
            }
        }
        else
        {
            if (QuestionList[CurrentQuestionIndex].A == QuestionList[CurrentQuestionIndex].AnswerB)
            {
                AnswerRight();
            }
            else
            {
                AnswerWrong();
            }
        }
        CurrentQuestionIndex = (CurrentQuestionIndex + 1) % QuestionList.Length;
    }


    [ContextMenu("left?")]
    public void SwipeLeft()
    {
        if (QuestionList[CurrentQuestionIndex].isTrueOrfalse)
        {
            if (!QuestionList[CurrentQuestionIndex].IsCorrect)
            {
                AnswerRight();
            }
            else
            {
                AnswerWrong();
            }
        }
        else
        {
            if (QuestionList[CurrentQuestionIndex].A == QuestionList[CurrentQuestionIndex].AnswerA)
            {
                AnswerRight();
            }
            else
            {
                AnswerWrong();
            }
        }
        CurrentQuestionIndex = (CurrentQuestionIndex + 1) % QuestionList.Length;
    }

    void AnswerRight()
    {
        Score++;
        M_TextObjectText.color = Color.green;
        removeQuestion();
    }

    void AnswerWrong()
    {
        M_TextObjectText.color = Color.red;
        removeQuestion();
    }

    void removeQuestion()
    {
        questionLeft.Remove(CurrentQuestionIndex);
        if (questionLeft.Count > 0)
        {
            int randomIndex = Random.Range(0, questionLeft.Count);
            CurrentQuestionIndex = questionLeft[randomIndex];
        }
        if (questionLeft.Count == 0)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        WinScren.SetActive(true);
        WinScoreText = WinScoreObject.GetComponent<TMP_Text>();
        WinScoreText.text = "You Scored " + Score + " out of " + totalQuestions;
    }
}
