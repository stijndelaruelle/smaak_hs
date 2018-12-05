using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenTipUI : MonoBehaviour
{
    [SerializeField]
    private Text m_Text;

    [SerializeField]
    private List<string> m_Tips;
    private List<int> m_TipIDsRemaining;

    [SerializeField]
    private float m_TimeTillNextTip;
    private float m_Timer = 0.0f;

    private void Start()
    {
        FillTipsRemaining();
        UpdateTip();
    }

    private void Update()
    {
        m_Timer += Time.deltaTime;

        if (m_Timer >= m_TimeTillNextTip)
        {
            m_Timer -= m_TimeTillNextTip;
            UpdateTip();
        }
    }

    private void UpdateTip()
    {
        if (m_Tips == null)
            return;

        if (m_Tips.Count <= 0)
            return;

        //Refill if we've seen all the tips
        if (m_TipIDsRemaining.Count <= 0)
            FillTipsRemaining();

        //Set new random tip
        int rand = Random.Range(0, m_TipIDsRemaining.Count);
        string tipText = m_Tips[m_TipIDsRemaining[rand]];

        m_Text.text = "Tip: " + LocalizationManager.GetText(tipText);


        //Remove it from the tips list, so we can't see it again this loading screen
        m_TipIDsRemaining.RemoveAt(rand);
    }

    //Utility
    private void FillTipsRemaining()
    {
        if (m_TipIDsRemaining == null)
            m_TipIDsRemaining = new List<int>();

        m_TipIDsRemaining.Clear();

        for (int i = 0; i < m_Tips.Count; ++i)
        {
            m_TipIDsRemaining.Add(i);
        }
    }
}
