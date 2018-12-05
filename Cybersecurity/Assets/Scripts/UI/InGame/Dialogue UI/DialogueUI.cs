using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField]
    private CanvasGroup m_CanvasGroup;

    //Super dirty, but I feel like this is the most "error prone" way of enabling & disabling the dialogue menu.
    //We can manually drag all the dialogues in here, but we all know how that's going to end.
    private Dialogue[] m_DialogueObjects;

    //Pass trough events, so all our sub components don't have to subscribe to the original source
    public event StartDialogueDelegate DialogueStartEvent;
    public event StopDialogueEvent DialogueStopEvent;
    public event NewDialogueLineDelegate DialogueNewLineEvent;

    private void Start()
    {
        m_CanvasGroup.Show(false);

        LevelDirector levelManager = LevelDirector.Instance;
        if (levelManager != null)
        {
            levelManager.LevelEndVictoryEvent += OnReset;
            levelManager.LevelEndDefeatEvent += OnReset;
            levelManager.LevelResetEvent += OnReset;
        }

        //Note: See variable
        m_DialogueObjects = FindObjectsOfType<Dialogue>();

        if (m_DialogueObjects != null)
        {
            foreach (Dialogue dialogueObject in m_DialogueObjects)
            {
                if (dialogueObject != null)
                {
                    dialogueObject.DialogueStartEvent += OnDialogueStart;
                    dialogueObject.DialogueStopEvent += OnDialogueStop;
                    dialogueObject.DialogueNewLineEvent += OnDialogueNewLineEvent;
                }
            }
        }
    }

    private void OnDestroy()
    {
        LevelDirector levelManager = LevelDirector.Instance;
        if (levelManager != null)
        {
            levelManager.LevelEndVictoryEvent -= OnReset;
            levelManager.LevelEndDefeatEvent -= OnReset;
            levelManager.LevelResetEvent -= OnReset;
        }
    }

    private void OnDialogueStart(int numOfLines)
    {
        LevelDirector.Instance.AddInputBlocker("DialogueUI: On Dialogue Start");
        m_CanvasGroup.Show(true);

        //Pass trough
        if (DialogueStartEvent != null)
            DialogueStartEvent(numOfLines);
    }

    private void OnDialogueStop()
    {
        LevelDirector.Instance.RemoveInputBlocker("DialogueUI: On Dialogue Stop");
        m_CanvasGroup.Show(false);

        //Pass trough
        if (DialogueStopEvent != null)
            DialogueStopEvent();
    }

    private void OnDialogueNewLineEvent(int dialogueLineID, DialogueTypeDefinition.DialogueLine dialogueLine)
    {
        //Pass trough
        if (DialogueNewLineEvent != null)
            DialogueNewLineEvent(dialogueLineID, dialogueLine);
    }

    private void OnReset()
    {
        //Enable player input
        if (m_CanvasGroup.IsVisible())
        {
            LevelDirector.Instance.RemoveInputBlocker("DialogueUI: On Dialogue Reset");
            m_CanvasGroup.Show(false);
        }
    }
}
