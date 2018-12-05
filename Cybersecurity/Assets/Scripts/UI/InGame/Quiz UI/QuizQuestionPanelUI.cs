using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizQuestionPanelUI : MonoBehaviour
{
    [SerializeField]
    private QuizUI m_QuizUI;

    [SerializeField]
    private CanvasGroup m_CanvasGroup;

    [SerializeField]
    private List<QuizAnswerButtonUI> m_AnswerButtons;
    private List<QuizAnswerButtonUI> m_DefaultAnswerButtonOrder;

    private void Start()
    {
        //Copy (lame, quick fix)
        m_DefaultAnswerButtonOrder = new List<QuizAnswerButtonUI>();
        foreach(QuizAnswerButtonUI answerButton in m_AnswerButtons)
        {
            m_DefaultAnswerButtonOrder.Add(answerButton);
        }

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

    private void OnQuizNewQuestion(int questionID, QuestionDefinition question)
    {
        //Randomize the order of answers (only when all buttons are present
        if (question.GetNumerOfAnswers() >= m_AnswerButtons.Count)
            m_AnswerButtons.Shuffle();
        else
            UnShuffle();

        for (int i = 0; i < m_AnswerButtons.Count; ++i)
        {
            bool answerExists = (i < question.GetNumerOfAnswers());
            m_AnswerButtons[i].gameObject.SetActive(answerExists);

            if (answerExists)
                m_AnswerButtons[i].InitializeAnswer(i, question);
        }

        m_CanvasGroup.Show(true);
    }

    private void OnQuizQuestionAnswered(int numOfQuestionsAsked, int numOfQuestionsCorrect, bool success)
    {
        //m_CanvasGroup.Show(false);
    }

    private void UnShuffle()
    {
        m_AnswerButtons.Clear();

        foreach(QuizAnswerButtonUI answerButton in m_DefaultAnswerButtonOrder)
        {
            m_AnswerButtons.Add(answerButton);
        }
    }
}
