using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizAnswerButtonUI : MonoBehaviour
{
    [SerializeField]
    private QuizUI m_QuizUI;

    [SerializeField]
    private Text m_Label;

    private int m_AnswerID = 0;

    public void InitializeAnswer(int answerID, QuestionDefinition question)
    {
        m_AnswerID = answerID;
        m_Label.text = LocalizationManager.GetText(question.GetAnswerText(m_AnswerID));
    }

    public void Click()
    {
        m_QuizUI.AnswerQuestion(m_AnswerID);
    }
}
