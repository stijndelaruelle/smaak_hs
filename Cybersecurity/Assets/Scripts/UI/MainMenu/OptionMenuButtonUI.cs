using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenuButtonUI : MonoBehaviour
{
    public void Click()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadOptionMenuScene();
        }
    }
}
