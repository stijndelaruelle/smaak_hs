using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireParticleOnVictory : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem m_ParticleEffect;

    private void Start()
    {
        if (LevelDirector.Instance == null)
            return;

        LevelDirector.Instance.LevelEndVictoryEvent += OnVictory;
    }

    private void OnDestroy()
    {
        if (LevelDirector.Instance == null)
            return;

        LevelDirector.Instance.LevelEndVictoryEvent -= OnVictory;
    }

    private void OnVictory()
    {
        m_ParticleEffect.Play();
    }
}
