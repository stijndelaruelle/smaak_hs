using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Cyber Security/Quiz/Question Pool")]
public class QuestionPoolDefinition : ScriptableObject
{
    [SerializeField]
    [Tooltip("Do we ask the questions from this pool in order? Useful for more moderated quizzes")]
    private bool m_ShowInOrder = false;
    public bool ShowInOrder
    {
        get { return m_ShowInOrder; }
    }

    [SerializeField]
    [Tooltip("0 = all the questions in the pool")]
    private int m_MaxNumberOfQuestions = 3;
    public int MaxNumberOfQuestions
    {
        get { return m_MaxNumberOfQuestions; }
    }

    [SerializeField]
    private int m_MinNumberOfCorrectAnswers = 2;
    public int MinNumerOfCorrectAnswers
    {
        get { return m_MinNumberOfCorrectAnswers; }
    }

    [SerializeField]
    private int m_MaxNumberOfIncorrectAnswers = 1;
    public int MaxNumberOfIncorrectAnswers
    {
        get { return m_MaxNumberOfIncorrectAnswers; }
    }

    [SerializeField]
    private List<QuestionDefinition> m_Questions;

    public int GetNumberOfQuestions()
    {
        return m_Questions.Count;
    }

    public QuestionDefinition GetQuestion(int id)
    {
        if (id < 0 || id >= m_Questions.Count)
            return null;

        return m_Questions[id];
    }
}
