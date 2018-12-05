using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextLevelButtonUI : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Load();
        }
    }

    public void Load()
    {
        LevelManager.Instance.LoadNextLevel();
    }
}
