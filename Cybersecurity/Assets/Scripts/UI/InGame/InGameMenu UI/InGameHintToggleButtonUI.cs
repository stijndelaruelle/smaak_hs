using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameHintToggleButtonUI : MonoBehaviour
{
    [SerializeField]
    private GameObject m_HintsOnIcon;

    [SerializeField]
    private GameObject m_HintsOffIcon;

    [SerializeField]
    private GameObject m_HintRootObject;

    [SerializeField]
    private HintSystem m_HintSystem;

    private bool m_FirstTime = true;
    private bool m_HintsOn = false;

    private void OnEnable()
    {
        Initialize();
    }

    private void OnDisable()
    {
        //Hide everything
        gameObject.SetActive(false);

        if (m_HintRootObject != null)
            m_HintRootObject.SetActive(false);

        if (m_HintSystem != null)
            m_HintSystem.ActivateHints(false);
    }

    private void Initialize()
    {
        gameObject.SetActive(m_HintRootObject != null || m_HintSystem != null);

        if (m_HintRootObject)
            m_HintRootObject.SetActive(m_HintsOn);

        if (m_HintSystem != null)
            m_HintSystem.ActivateHints(m_HintsOn);

        m_HintsOnIcon.SetActive(m_HintsOn);
        m_HintsOffIcon.SetActive(!m_HintsOn);
    }

    public void Toggle()
    {
        m_HintsOn = !m_HintsOn;

        //Start the level from the beginning, only this way we can ensure that the hints are accurate
        if (m_HintsOn)
            LevelDirector.Instance.ResetLevel();

        //Show the right icon
        m_HintsOnIcon.SetActive(m_HintsOn);
        m_HintsOffIcon.SetActive(!m_HintsOn);

        //Show the hints!
        if (m_HintRootObject != null)
            m_HintRootObject.SetActive(m_HintsOn);

        if (m_HintSystem != null)
            m_HintSystem.ActivateHints(m_HintsOn);

        if (m_FirstTime == true)
        {
            if (LevelDirector.Instance != null)
                LevelDirector.Instance.CallHintsAnalyticsEvent();

            m_FirstTime = false;
        }
    }
}
