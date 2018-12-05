using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizQuestionLabelUI : MonoBehaviour
{
    [SerializeField]
    private QuizUI m_QuizUI;

    [SerializeField]
    private Text m_Label;

    private void Start()
    {
        if (m_QuizUI != null)
            m_QuizUI.QuizNewQuestionEvent += OnQuizNewQuestion;
    }

    private void OnDestroy()
    {
        if (m_QuizUI != null)
            m_QuizUI.QuizNewQuestionEvent -= OnQuizNewQuestion;
    }

    private void OnQuizNewQuestion(int questionID, QuestionDefinition question)
    {
        m_Label.text = LocalizationManager.GetText(question.Question);
    }
}
