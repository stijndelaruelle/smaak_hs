using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Cyber Security/Quiz/Question")]
public class QuestionDefinition : ScriptableObject
{
    [Serializable]
    public class Answer
    {
        [SerializeField]
        [LocalizationID]
        private string m_Text;
        public string Text
        {
            get { return m_Text; }
        }

        [SerializeField]
        private bool m_IsCorrect;
        public bool IsCorrect
        {
            get { return m_IsCorrect; }
        }
    }

    [SerializeField]
    [LocalizationID]
    private string m_Question;
    public string Question
    {
        get { return m_Question; }
    }

    [SerializeField]
    [LocalizationID]
    private string m_CorrectExplanation;
    public string CorrectExplanation
    {
        get { return m_CorrectExplanation; }
    }

    [SerializeField]
    [LocalizationID]
    private string m_IncorrectExplanation;
    public string InorrectExplanation
    {
        get { return m_IncorrectExplanation; }
    }

    [SerializeField]
    private List<Answer> m_Answers;

    public int GetNumerOfAnswers()
    {
        if (m_Answers == null)
            return 0;

        return m_Answers.Count;
    }

    public Answer GetAnswer(int id)
    {
        if (id < 0 || id >= m_Answers.Count)
            return null;

        return m_Answers[id];
    }

    public string GetAnswerText(int id)
    {
        Answer answer = GetAnswer(id);

        if (answer == null)
            return "Unknown answer";

        return answer.Text;
    }

    //Save game
    public int NumberOfTimesAnswered()
    {
        return SaveGameManager.GetInt(m_Question, 0);
    }

    public void AnswerQuestion()
    {
        int numberOfTimesAnswered = NumberOfTimesAnswered();
        SaveGameManager.SetInt(m_Question, numberOfTimesAnswered + 1);
    }
}
