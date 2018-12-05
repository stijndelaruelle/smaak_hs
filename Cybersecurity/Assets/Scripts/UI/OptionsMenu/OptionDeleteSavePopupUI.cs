using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionDeleteSavePopupUI : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup m_CanvasGroup;

    private void Start()
    {
        Close();    
    }

    public void Show()
    {
        m_CanvasGroup.Show(true);
    }

    public void Close()
    {
        m_CanvasGroup.Show(false);
    }

    public void DeleteSaveGame()
    {
        //Analytics
        AnalyticsManager.DeleteSaveGameEvent();

        SaveGameManager.DeleteAll();
    }
}
