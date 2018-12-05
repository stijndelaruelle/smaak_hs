using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchProgressUI : MonoBehaviour
{
    [SerializeField]
    private Switch m_Switch;

    //Super simple for now, I know switches can have multiple req's and it would be nice if we'd display them all with the item icon next to it.
    //So make this more elaborate once we actually need it (kind of in a rush now because of an unforseen playtest)
    [SerializeField]
    private Text m_Label;

    private void Start()
    {
        if (m_Switch == null)
        {
            Debug.LogWarning("SwitchProgress UI (" + gameObject.name + ") doesn't have a Switch object assigned to it!");
            return;
        }

        m_Switch.SwitchProgressEvent += OnSwitchProgress;

        //Manually update at the start
        OnSwitchProgress(m_Switch.GetProgress());
    }

    private void OnDestroy()
    {
        if (m_Switch != null)
            m_Switch.SwitchProgressEvent -= OnSwitchProgress;
    }

    private void OnSwitchProgress(string progress)
    {
        m_Label.text = progress;
    }
}
