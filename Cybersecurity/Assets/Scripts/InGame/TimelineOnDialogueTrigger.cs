using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

//Quick & dirty solution: Should create a nice queue system.
public class TimelineOnDialogueTrigger : MonoBehaviour
{
    [SerializeField]
    private Dialogue m_Dialogue;

    [SerializeField]
    private PlayableDirector m_Director;

    [SerializeField]
    private bool m_TriggerOnStart = false;

    [SerializeField]
    private bool m_TriggerOnEnd = false;

    [SerializeField]
    private bool m_TriggerOnLineID = false;

    [SerializeField]
    [Tooltip("Only used when we trigger on a line ID")]
    private int m_DialogueLineID = 0;

    private void Start()
    {
        if (m_Dialogue != null)
        {
            m_Dialogue.DialogueStartEvent += OnDialogueStart;
            m_Dialogue.DialogueStopEvent += OnDialogueStop;
            m_Dialogue.DialogueNewLineEvent += OnDialogueNewLine;
        }
    }

    private void OnDestroy()
    {
        if (m_Dialogue != null)
        {
            m_Dialogue.DialogueStartEvent -= OnDialogueStart;
            m_Dialogue.DialogueStopEvent -= OnDialogueStop;
            m_Dialogue.DialogueNewLineEvent -= OnDialogueNewLine;
        }
    }

    private void OnDialogueStart(int numOfLines)
    {
        if (m_TriggerOnStart)
            PlayTimeLine();
    }

    private void OnDialogueStop()
    {
        if (m_TriggerOnEnd)
            PlayTimeLine();
    }

    private void OnDialogueNewLine(int dialogueLineID, DialogueTypeDefinition.DialogueLine dialogueLine)
    {
        if (m_TriggerOnLineID)
        {
            if (m_DialogueLineID == dialogueLineID)
            {
                PlayTimeLine();
            }
        }
    }

    private void PlayTimeLine()
    {
        if (gameObject.activeSelf == false)
            return;

        if (m_Director == null)
            return;

        //Start Timeline
        m_Director.Stop();
        m_Director.Play();
    }
}
