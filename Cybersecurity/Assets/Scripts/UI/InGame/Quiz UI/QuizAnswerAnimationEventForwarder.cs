using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizAnswerAnimationEventForwarder : MonoBehaviour
{
    [SerializeField]
    private QuizQuestionAnsweredPanelUI m_Panel;

    public void AnswerAnimationEnded()
    {
        m_Panel.OnAnswerAnimationEnded();
    }
}
