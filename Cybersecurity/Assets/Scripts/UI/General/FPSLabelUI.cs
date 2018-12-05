using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FPSLabelUI : MonoBehaviour
{
    private Text m_Text;
    private Coroutine m_UpdateCorutine;

    private void Start()
    {
        SaveGameManager.BoolChangedEvent += OnSaveGameBoolVariableChanged;
        SaveGameManager.SaveGameDeletedEvent += OnSaveGameDeletedEvent;

        //Thanks to require component
        m_Text = gameObject.GetComponent<Text>();

        if (SaveGameManager.GetBool(SaveGameManager.SAVE_CHEATS) == true)
            StartFPSCounter();
        else
            StopFPSCounter();
    }

    private void OnDestroy()
    {
        SaveGameManager.BoolChangedEvent -= OnSaveGameBoolVariableChanged;
        SaveGameManager.SaveGameDeletedEvent -= OnSaveGameDeletedEvent;

        StopFPSCounter();
    }


    private void StartFPSCounter()
    {
        //Show the label
        m_Text.enabled = true;

        //Start the coroutine
        if (m_UpdateCorutine != null)
            StopCoroutine(m_UpdateCorutine);

        m_UpdateCorutine = StartCoroutine(UpdateFPSRoutine());
    }

    private void StopFPSCounter()
    {
        //Hide the label
        m_Text.enabled = false;

        //Stop the coroutine
        if (m_UpdateCorutine == null)
            return;

        StopCoroutine(m_UpdateCorutine);
        m_UpdateCorutine = null; 
    }

    private IEnumerator UpdateFPSRoutine()
    {
        bool isRunning = true;

        while (isRunning == true)
        {
            float fps = (1 / Time.deltaTime);
            m_Text.text = "FPS: " + (Mathf.Round(fps));

            yield return new WaitForSeconds(0.1f);
        }

        m_UpdateCorutine = null;
        yield return null;
    }


    private void OnSaveGameBoolVariableChanged(string key, bool value)
    {
        if (key == SaveGameManager.SAVE_CHEATS)
        {
            if (value == true) { StartFPSCounter(); }
            else               { StopFPSCounter(); }
        }
    }

    private void OnSaveGameDeletedEvent()
    {
        StopFPSCounter();
    }
}
