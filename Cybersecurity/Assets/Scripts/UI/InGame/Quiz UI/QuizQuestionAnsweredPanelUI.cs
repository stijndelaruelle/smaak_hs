using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizQuestionAnsweredPanelUI : MonoBehaviour
{
    [SerializeField]
    private QuizUI m_QuizUI;

    [SerializeField]
    private CanvasGroup m_CanvasGroup;

    [SerializeField]
    private Animator m_FirstCorrectAnimator;

    [SerializeField]
    private Animator m_SecondCorrectAnimator;

    [SerializeField]
    private Animator m_IncorrectAnimator;

    [SerializeField]
    private QuizExplenationButtonUI m_ExplanationPanel;


    private bool m_ActiveThisQuiz = false;

    private void Start()
    {
        if (m_QuizUI != null)
        {
            m_QuizUI.QuizStartEvent += OnQuizStart;
            m_QuizUI.QuizNewQuestionEvent += OnQuizNewQuestion;
            m_QuizUI.QuizQuestionAnsweredEvent += OnQuizQuestionAnswered;
        }

        if (m_ExplanationPanel != null)
        {
            m_ExplanationPanel.CloseEvent += OnExplanationClose;
        }

        m_CanvasGroup.Show(false);
    }

    private void OnDestroy()
    {
        if (m_QuizUI != null)
        {
            m_QuizUI.QuizStartEvent -= OnQuizStart;
            m_QuizUI.QuizNewQuestionEvent -= OnQuizNewQuestion;
            m_QuizUI.QuizQuestionAnsweredEvent -= OnQuizQuestionAnswered;
        }

        if (m_ExplanationPanel != null)
        {
            m_ExplanationPanel.CloseEvent -= OnExplanationClose;
        }
    }

    private void OnQuizStart(int numberOfQuestions)
    {
        m_FirstCorrectAnimator.gameObject.SetActive(false);
        m_SecondCorrectAnimator.gameObject.SetActive(false);
        m_IncorrectAnimator.gameObject.SetActive(false);

        m_ActiveThisQuiz = (numberOfQuestions <= 3);
    }

    private void OnQuizNewQuestion(int questionID, QuestionDefinition question)
    {
        if (m_ActiveThisQuiz == false)
            return;

        m_FirstCorrectAnimator.gameObject.SetActive(false);
        m_SecondCorrectAnimator.gameObject.SetActive(false);
        m_IncorrectAnimator.gameObject.SetActive(false);
    }

    private void OnQuizQuestionAnswered(int numOfQuestionsAsked, int numOfQuestionsCorrect, bool success)
    {
        if (m_ActiveThisQuiz == false)
            return;

        m_CanvasGroup.Show(true);

        m_FirstCorrectAnimator.gameObject.SetActive(false);
        m_SecondCorrectAnimator.gameObject.SetActive(false);
        m_IncorrectAnimator.gameObject.SetActive(false);

        if (success)
        {
            if (numOfQuestionsCorrect == 1)
            {
                m_FirstCorrectAnimator.gameObject.SetActive(true);
                m_FirstCorrectAnimator.SetTrigger("Play");
            }
            else
            {
                m_SecondCorrectAnimator.gameObject.SetActive(true);
                m_SecondCorrectAnimator.SetTrigger("Play");
            }
        }
        else
        {
            m_IncorrectAnimator.gameObject.SetActive(true);
            m_IncorrectAnimator.SetTrigger("Play");
        }
    }

    //Callback
    public void OnAnswerAnimationEnded()
    {
        if (m_ActiveThisQuiz == false)
            return;

        m_FirstCorrectAnimator.gameObject.SetActive(false);
        m_SecondCorrectAnimator.gameObject.SetActive(false);
        m_IncorrectAnimator.gameObject.SetActive(false);

        //m_QuizUI.CompleteQuestion();

        //Show the explanation
        m_ExplanationPanel.Show();
    }

    private void OnExplanationClose()
    {
        if (m_ActiveThisQuiz == false)
            return;

        m_CanvasGroup.Show(false);
    }
}
