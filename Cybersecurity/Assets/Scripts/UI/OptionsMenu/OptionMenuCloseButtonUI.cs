using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenuCloseButtonUI : MonoBehaviour
{
    public void Click()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.SceneLoader.UnloadActiveScene();

        //This scene just closed, if we are in game make sure to block the input!
        if (LevelDirector.Instance != null)
        {
            LevelDirector.Instance.RemoveInputBlocker("OptionMenuCloseButtonUI: Click");
        }
    }
}
