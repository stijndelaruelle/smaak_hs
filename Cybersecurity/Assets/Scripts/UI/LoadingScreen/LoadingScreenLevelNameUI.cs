using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenLevelNameUI : MonoBehaviour
{
    [SerializeField]
    private Text m_Text;

    private void Start()
    {
        LevelManager levelManager = LevelManager.Instance;
        if (levelManager == null)
        {
            return;
        }

        LevelDataDefinition levelData = levelManager.CurrentLevel;

        if (levelData == null)
            return;

        m_Text.text = LocalizationManager.GetText(levelData.LevelNameLocalizationID);
    }
}

