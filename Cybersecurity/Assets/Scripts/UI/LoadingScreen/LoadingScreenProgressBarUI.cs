using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenProgressBarUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_Bar;

    [SerializeField]
    private float m_MinRightValue;

    [SerializeField]
    private float m_MaxRightValue;

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

        float percent = Mathf.Ceil(LevelManager.Instance.SceneLoader.GetProgress());
        float rightValue = Mathf.Lerp(m_MinRightValue, m_MaxRightValue, percent);

        m_Bar.offsetMax = new Vector2(-rightValue, m_Bar.offsetMax.y);
    }

}
