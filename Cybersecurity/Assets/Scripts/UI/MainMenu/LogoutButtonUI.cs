using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoutButtonUI : MonoBehaviour
{
    public void Click()
    {
        if (LevelManager.Instance == null)
            return;

        LevelManager.Instance.LoadSplashScene();
    }
}
