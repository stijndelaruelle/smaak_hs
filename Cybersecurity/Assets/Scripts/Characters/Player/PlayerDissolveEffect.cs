using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDissolveEffect : ResetableObject
{
    [SerializeField]
    private List<Renderer> m_DissolveRenderers;
    private float m_DissolveTime = 0.0f;

    private Coroutine m_DissolveRoutine;

    /*
    public void Update()
    {
        //Text effect
        if (m_DissolveRoutine == null)
        {
            if (m_DissolveTime < 0.5f)
                DissolveOut();
            else
                DissolveIn();
        }
    }
    */

    public void DissolveOut(float speed, bool force)
    {
        if (m_DissolveRoutine != null)
            StopCoroutine(m_DissolveRoutine);

        if (force)
        {
            m_DissolveTime = 0.0f;
            SetDissolvePercentage();
        }

        m_DissolveRoutine = StartCoroutine(DissolveRoutine(true, speed));
    }

    public void DissolveIn(float speed, bool force)
    {
        if (m_DissolveRoutine != null)
            StopCoroutine(m_DissolveRoutine);

        if (force)
        {
            m_DissolveTime = 1.0f;
            SetDissolvePercentage();
        }

        m_DissolveRoutine = StartCoroutine(DissolveRoutine(false, speed));
    }

    private IEnumerator DissolveRoutine(bool fadeIn, float speed)
    {
        //Limit
        bool isFading = true;

        while (isFading)
        {
            if (m_DissolveTime < 0.0f)
            {
                m_DissolveTime = 0.0f;
                isFading = false;
            }

            if (m_DissolveTime > 1.0f)
            {
                m_DissolveTime = 1.0f;
                isFading = false;
            }

            //Actually dissolve
            SetDissolvePercentage();

            if (isFading)
            {
                //Increase the time
                if (fadeIn) { m_DissolveTime += Time.deltaTime * speed; }
                else { m_DissolveTime -= Time.deltaTime * speed; }
            }

            yield return new WaitForEndOfFrame();
        }

        m_DissolveRoutine = null;
        yield return null;
    }

    private void SetDissolvePercentage()
    {
        foreach (Renderer renderer in m_DissolveRenderers)
        {
            renderer.material.SetFloat("m_DissolvePercentage", m_DissolveTime);
        }
    }

    protected override void OnReset()
    {
        //Stop dissolving if we are.
        if (m_DissolveRoutine != null)
        {
            StopCoroutine(m_DissolveRoutine);
            m_DissolveRoutine = null;
        }

        //Make ourselves completely invisible (Intro will make us visible again).
        m_DissolveTime = 1.0f;
        foreach (Renderer renderer in m_DissolveRenderers)
        {
            renderer.material.SetFloat("m_DissolvePercentage", m_DissolveTime);
        }
    }
}
