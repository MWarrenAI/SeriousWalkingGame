using NUnit.Framework;
using System.Collections;
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
        public string left;
        public string right;
        public string Answer;
        public bool IsCorrect;
    }
    public string Name = "Player";
    public string FileLocation = $"";

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

    public GameObject QuestionSquare;
    public GameObject BGPanel;
    public GameObject WinScren;
    public GameObject WinScoreObject;
    public TMP_Text WinScoreText;

    public Image flashImage;
    public int fukcyou;

    void Start()
    {
        string sub = PlayerPrefs.GetString("subject");
        Debug.Log(sub);
        switch (sub) 
        {
            case "maths":
                FileLocation = $"maths";
                break;
            case "compsci":
                FileLocation = $"compsci";
                break;
            case "sci":
                FileLocation = $"sci";
                break;
            case "english":
                FileLocation = $"english";
                break;
            case "history":
                FileLocation = $"history";
                break;
        }
        Debug.Log(FileLocation);
        M_TextObjectText = TextObject.GetComponent<TMP_Text>();
        RightText = RightObject.GetComponent<TMP_Text>();
        LeftText = LeftObject.GetComponent<TMP_Text>();
        M_ScoreText = ScoreObject.GetComponent<TMP_Text>();
        WinScoreText = WinScoreObject.GetComponent<TMP_Text>();
        M_TextObjectText.text = "Hello " + Name + "!";

        TextAsset levelData = Resources.Load<TextAsset>(FileLocation);
        Debug.Log(levelData.text);
        string filePath = levelData.text;
        if (levelData != null)
        {
            string[] lines = levelData.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            QuestionList = new Question[lines.Length];
            for (int i = 0; i < lines.Length && i < QuestionList.Length; i++)
            {
                string line = lines[i].Replace("\r", "");
                string[] parts = line.Split(',');
                if (parts.Length >= 3)
                {
                    QuestionList[i].Text = parts[0];
                    QuestionList[i].isTrueOrfalse = false;
                    QuestionList[i].left = parts[1];
                    QuestionList[i].right = parts[2];
                    QuestionList[i].Answer = parts[3];
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
            LeftText.text = QuestionList[CurrentQuestionIndex].left;
            RightText.text = QuestionList[CurrentQuestionIndex].right;
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
            if (QuestionList[CurrentQuestionIndex].Answer == QuestionList[CurrentQuestionIndex].right)
            {
                AnswerRight();
            }
            else
            {
                AnswerWrong();
            }
        }
        //CurrentQuestionIndex = (CurrentQuestionIndex + 1) % QuestionList.Length;
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
            if (QuestionList[CurrentQuestionIndex].Answer == QuestionList[CurrentQuestionIndex].left)
            {
                AnswerRight();
            }
            else
            {
                AnswerWrong();
            }
        }
        //CurrentQuestionIndex = (CurrentQuestionIndex + 1) % QuestionList.Length;
    }

    void AnswerRight()
    {
        Score++;
        //M_TextObjectText.color = Color.green;
        removeQuestion();
        StartCoroutine(FlashGreenWithFade());
    }

    void AnswerWrong()
    {
        //M_TextObjectText.color = Color.red;
        removeQuestion();
        StartCoroutine(FlashRedWithFade());

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
        QuestionSquare.SetActive(false);
        BGPanel.SetActive(false);
        WinScren.SetActive(true);
        WinScoreText = WinScoreObject.GetComponent<TMP_Text>();
        WinScoreText.text = "You Scored " + Score + " out of " + totalQuestions;
    }

    public IEnumerator FlashGreenWithFade()
    {
        flashImage.gameObject.SetActive(true);
        flashImage.color = new Color(0f, 1f, 0f, 0.5f);

        yield return new WaitForSeconds(0.1f);

        // Fade out over 0.15 seconds
        float fadeTime = 0.15f;
        float elapsedTime = 0f;
        Color startColor = flashImage.color;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsedTime / fadeTime);
            flashImage.color = new Color(0f, 1f, 0f, alpha);
            yield return null;
        }

        flashImage.gameObject.SetActive(false);
    }
    public IEnumerator FlashRedWithFade()
    {
        flashImage.gameObject.SetActive(true);
        flashImage.color = new Color(1f, 0f, 0f, 0.5f);

        yield return new WaitForSeconds(0.1f);

        // Fade out over 0.15 seconds
        float fadeTime = 0.15f;
        float elapsedTime = 0f;
        Color startColor = flashImage.color;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsedTime / fadeTime);
            flashImage.color = new Color(0f, 1f, 0f, alpha);
            yield return null;
        }

        flashImage.gameObject.SetActive(false);
    }
}
