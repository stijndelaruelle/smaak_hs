using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizExplenationButtonUI : MonoBehaviour
{
    [SerializeField]
    private QuizUI m_QuizUI;

    [SerializeField]
    private Text m_Label;

    [SerializeField]
    private CanvasGroup m_CanvasGroup;

    private QuestionDefinition m_CurrentQuestion;

    public event Action CloseEvent;

    private void Start()
    {
        if (m_QuizUI != null)
        {
            m_QuizUI.QuizNewQuestionEvent += OnQuizNewQuestion;
            m_QuizUI.QuizQuestionAnsweredEvent += OnQuizQuestionAnswered;
        }
    }

    private void OnDestroy()
    {
        if (m_QuizUI != null)
        {
            m_QuizUI.QuizNewQuestionEvent -= OnQuizNewQuestion;
            m_QuizUI.QuizQuestionAnsweredEvent -= OnQuizQuestionAnswered;
        }
    }

    public void Show()
    {
        m_CanvasGroup.Show(true);
    }

    private void OnQuizNewQuestion(int questionID, QuestionDefinition question)
    {
        m_CurrentQuestion = question;
    }

    private void OnQuizQuestionAnswered(int numOfQuestionsAsked, int numOfQuestionsCorrect, bool success)
    {
        if (success == true)
            m_Label.text = LocalizationManager.GetText(m_CurrentQuestion.CorrectExplanation);
        else
            m_Label.text = LocalizationManager.GetText(m_CurrentQuestion.InorrectExplanation);
    }

    public void Click()
    {
        m_QuizUI.CompleteQuestion();
        m_CanvasGroup.Show(false);

        if (CloseEvent != null)
            CloseEvent();
    }
}
