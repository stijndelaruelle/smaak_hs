using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomToggleGroup : MonoBehaviour
{
    private Toggle[] m_Toggles;

    private void Start()
    {
        //Super lame, but hey. We're not going to bother too much for scripts like these
        m_Toggles = GetComponentsInChildren<Toggle>();
    }

    public void SelectRandomToggle()
    {
        int randToggleID = Random.Range(0, m_Toggles.Length);

        //Because these "should be" in a toggle group, it deaticate the others.
        m_Toggles[randToggleID].isOn = true;
    }
}
