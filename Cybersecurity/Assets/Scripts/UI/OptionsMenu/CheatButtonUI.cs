using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheatButtonUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private float m_TimeToActivate;
    private Stopwatch m_StopWatch;

    private void Start()
    {
        m_StopWatch = new Stopwatch();
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        m_StopWatch.Reset();
        m_StopWatch.Start();
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if (m_StopWatch.Elapsed.TotalSeconds >= m_TimeToActivate)
        {
            bool cheatsEnabled = SaveGameManager.GetBool(SaveGameManager.SAVE_CHEATS);
            SaveGameManager.SetBool(SaveGameManager.SAVE_CHEATS, !cheatsEnabled);

            UnityEngine.Debug.Log("Cheats enabled: " + !cheatsEnabled);
        }

        m_StopWatch.Stop();
        m_StopWatch.Reset();
    }
}
