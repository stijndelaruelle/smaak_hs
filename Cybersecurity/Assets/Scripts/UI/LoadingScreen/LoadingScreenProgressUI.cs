using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenProgressUI : MonoBehaviour
{
    [SerializeField]
    private Text m_Text;

    private void Start()
    {
        LevelManager levelManager = LevelManager.Instance;
        if (levelManager == null)
        {
            Debug.LogError("No LevelManager found to load!");
            return;
        }
    }

    private void Update()
    {
        if (LevelManager.Instance == null)
            return;

        float percent = Mathf.Ceil(LevelManager.Instance.SceneLoader.GetProgress() * 100.0f);
        m_Text.text = percent + "%";
    }
}
