using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtonUI : MonoBehaviour
{
    public void Click()
    {
        if (LevelManager.Instance != null)
        {
            if (LevelDirector.Instance != null)
            {
                LevelDirector.Instance.CallLevelQuitAnalyticsEvent(false, true);
            }

            LevelManager.Instance.LoadMainMenuScene();
        }
    }
}
