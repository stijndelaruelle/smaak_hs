using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetLevelButtonUI : MonoBehaviour
{
    public void Click()
    {
        if (LevelDirector.Instance != null)
        { 
            LevelDirector.Instance.CallLevelFailAnalyticsEvent(false, false, true, false);
            LevelDirector.Instance.ResetLevel();
        }
    }
}
