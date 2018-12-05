using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizQuestionPipsUI : MonoBehaviour
{
    [SerializeField]
    private QuizUI m_QuizUI;

    [SerializeField]
    [Tooltip("Let's not overengineer this for now, once we can have more questions than 10 we'll make this generic (dynamically create, pooling, etc..)")]
    private List<QuizQuestionPipUI> m_Pips;

    [Header("Sprites")]
    [SerializeField]
    private Sprite m_DefaultSprite;

    [SerializeField]
    private Sprite m_ActiveSprite;

    [SerializeField]
    private Sprite m_CorrectSprite;

    [SerializeField]
    private Sprite m_IncorrectSprite;

    private bool m_ActiveThisQuiz = false;

    private void Start()
    {
        if (m_QuizUI != null)
        {
            m_QuizUI.QuizStartEvent += OnQuizStart;
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

    private void OnQuizStart(int numOfQuestions)
    {
        m_ActiveThisQuiz = (numOfQuestions > 3);

        //Show only the required pips
        for (int i = 0; i < m_Pips.Count; ++i)
        {
            m_Pips[i].SetActive((i < numOfQuestions) && (m_ActiveThisQuiz == true));
            m_Pips[i].SetSprite(m_DefaultSprite); //Reset color
        }
    }

    private void OnQuizNewQuestion(int questionID, QuestionDefinition question)
    {
        if (m_ActiveThisQuiz == false)
            return;

        if (questionID < 0 || questionID >= m_Pips.Count)
            return;

        m_Pips[questionID].SetSprite(m_ActiveSprite);
    }

    private void OnQuizQuestionAnswered(int numOfQuestionsAsked, int numOfQuestionsCorrect, bool success)
    {
        if (m_ActiveThisQuiz == false)
            return;

        if (numOfQuestionsAsked < 0 || numOfQuestionsAsked >= m_Pips.Count)
            return;

        Sprite sprite = m_CorrectSprite;
        if (success == false) sprite = m_IncorrectSprite;

        m_Pips[numOfQuestionsAsked].SetSprite(sprite);

        //Continue to the next question
        m_QuizUI.CompleteQuestion();
    }
}