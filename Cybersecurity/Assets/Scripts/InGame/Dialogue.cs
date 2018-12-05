using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void StartDialogueDelegate(int numOfLines);
public delegate void NewDialogueLineDelegate(int dialogueLineID, DialogueTypeDefinition.DialogueLine dialogueLine);
public delegate void StopDialogueEvent();

public class Dialogue : ResetableObject
{
    [Header("Dialogue Data")]
    [SerializeField]
    private DialogueTypeDefinition m_DialogueData;
    private int m_CurrentDialogueLineID = -1;

    public event StartDialogueDelegate DialogueStartEvent;
    public event StopDialogueEvent DialogueStopEvent;
    public event NewDialogueLineDelegate DialogueNewLineEvent;

    private void Update()
    {
        if (m_CurrentDialogueLineID < 0)
            return;

        if (LevelDirector.Instance != null)
        {
            if (LevelDirector.Instance.HasGeneralInput())
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
                {
                    NextLine();
                }
            }
        }
    }

    public void StartDialogue()
    {
        m_CurrentDialogueLineID = -1;

        //Call event
        if (DialogueStartEvent != null)
            DialogueStartEvent(m_DialogueData.GetNumberOfLines());

        //Immediatly load the first line
        NextLine();
    }

    private void NextLine()
    {
        m_CurrentDialogueLineID += 1;

        if (m_CurrentDialogueLineID < 0 || m_CurrentDialogueLineID >= m_DialogueData.GetNumberOfLines())
        {
            StopDialogue();
            return;
        }

        //Call event
        if (DialogueNewLineEvent != null)
            DialogueNewLineEvent(m_CurrentDialogueLineID, m_DialogueData.GetDialogueLine(m_CurrentDialogueLineID));
    }

    private void StopDialogue()
    {
        m_CurrentDialogueLineID = -1;

        //Call event
        if (DialogueStopEvent != null)
            DialogueStopEvent();
    }

    protected override void OnReset()
    {
        m_CurrentDialogueLineID = -1;
    }
}
