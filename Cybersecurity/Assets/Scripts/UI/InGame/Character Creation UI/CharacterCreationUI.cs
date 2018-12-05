using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CharacterCreationUI : MonoBehaviour
{
    [Header("Required references")]
    [SerializeField]
    private CanvasGroup m_CanvasGroup;

    [SerializeField]
    private GameObject m_PlayerModel;

    [SerializeField]
    private GameObject m_NameErrorLabel;

    [SerializeField]
    [Tooltip("Randomize the selected toggle when we are visible.")]
    private List<CustomToggleGroup> m_ToggleGroups;

    [SerializeField]
    private bool m_RandomizeOnOpen;

    private Stopwatch m_Stopwatch;

    public event Action CharacterCreationOpenEvent;
    public event Action CharacterCreationCloseEvent;

    private void Start()
    {
        m_Stopwatch = new Stopwatch();

        if (LevelDirector.Instance == null)
            return;

        LevelDirector.Instance.LevelStartEvent += OnLevelStart;
    }

    private void OnDestroy()
    {
        if (LevelDirector.Instance == null)
            return;

        LevelDirector.Instance.LevelStartEvent -= OnLevelStart;
    }

    private void OnLevelStart()
    {
        //If we don't have a character, open the window
        bool hasCharacter = SaveGameManager.HasKey(SaveGameManager.SAVE_PLAYER_NAME);

        m_CanvasGroup.Show(!hasCharacter);
        m_PlayerModel.SetActive(!hasCharacter);
        m_NameErrorLabel.SetActive(false);

        if (hasCharacter == false)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    private void Open()
    {
        if (LevelDirector.Instance != null)
            LevelDirector.Instance.AddInputBlocker("CharacterCreationUI: Open");

        if (CharacterCreationOpenEvent != null)
            CharacterCreationOpenEvent();

        m_Stopwatch.Reset();
        m_Stopwatch.Start();

        //Randomize all the settings (except the name)
        if (m_RandomizeOnOpen)
        {
            foreach (CustomToggleGroup toggleGroup in m_ToggleGroups)
            {
                toggleGroup.SelectRandomToggle();
            }
        }
        else
        {
            //TODO: Set them to the current setting
        }
    }

    private void Close()
    {
        if (CharacterCreationCloseEvent != null)
            CharacterCreationCloseEvent();
    }

    //Button callback
    public void RequestCloseWindow()
    {
        //You HAVE to fill in a name
        if (SaveGameManager.HasKey("AvatarName") == false)
        {
            m_NameErrorLabel.SetActive(true);
            return;
        }

        m_CanvasGroup.Show(false);
        m_PlayerModel.SetActive(false);

        if (CharacterCreationCloseEvent != null)
            CharacterCreationCloseEvent();

        if (LevelDirector.Instance != null)
            LevelDirector.Instance.RemoveInputBlocker("CharacterCreationUI: RequestCloseWindow");

        //Analytics
        Gender gender = (Gender)SaveGameManager.GetInt(SaveGameManager.SAVE_PLAYER_GENDER);
        bool isMale = (gender == Gender.Male);
        bool isFemale = (gender == Gender.Female);

        Color skinColor = SaveGameManager.GetColor(SaveGameManager.SAVE_PLAYER_SKINCOLOR);
        string skinColorHex = ColorUtility.ToHtmlStringRGB(skinColor).ToLower();

        Color extraColor = SaveGameManager.GetColor(SaveGameManager.SAVE_PLAYER_EXTRACOLOR);
        string extraColorHex = ColorUtility.ToHtmlStringRGB(extraColor).ToLower();

        m_Stopwatch.Stop();

        AnalyticsManager.CharacterCreationEvent(isMale, isFemale, skinColorHex, extraColorHex, m_Stopwatch.Elapsed.TotalSeconds);
    }
}
