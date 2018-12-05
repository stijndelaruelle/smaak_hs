using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenuOpen : MonoBehaviour
{
    [SerializeField]
    private float m_RegularHeight = 750;

    [SerializeField]
    private float m_InGameHeight = 600;

    [SerializeField]
    private RectTransform m_PanelBackground;

    [SerializeField]
    private GameObject m_DeleteSaveButton;

    private void Start()
    {
        bool inGame = (LevelDirector.Instance != null);

        //Set the size
        Vector2 newSize = new Vector2(m_PanelBackground.sizeDelta.x, m_RegularHeight);
        if (inGame) { newSize.y = m_InGameHeight; }

        m_PanelBackground.sizeDelta = newSize;

        //Hide the delete save game button
        m_DeleteSaveButton.SetActive(inGame == false);

        //This scene just opened, if we are in game make sure to block the input!
        if (LevelDirector.Instance != null)
        {
            LevelDirector.Instance.AddInputBlocker("OptionMenuOpen: Start");
        }
    }

    private void Update()
    {
        //Enable cheats
        if (Input.GetKey(KeyCode.H) &&
            Input.GetKey(KeyCode.A) &&
            Input.GetKey(KeyCode.C) &&
            Input.GetKey(KeyCode.K) &&
            Input.GetKeyDown(KeyCode.S))
        {
            bool cheatsEnabled = SaveGameManager.GetBool(SaveGameManager.SAVE_CHEATS);
            SaveGameManager.SetBool(SaveGameManager.SAVE_CHEATS, !cheatsEnabled);

            Debug.Log("Cheats enabled: " + !cheatsEnabled);
        }
    }
}
