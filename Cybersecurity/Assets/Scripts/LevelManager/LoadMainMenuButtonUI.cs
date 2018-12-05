using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainMenuButtonUI : MonoBehaviour
{
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            Load();
        }
    }

    public void Load()
    {
        LevelManager.Instance.LoadMainMenuScene();
    }
}
