using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectResetter : ResetableObject
{
    [SerializeField]
    private List<GameObject> m_GameObjects;
    private List<bool> m_DefaultStates;

    private void Awake()
    {
        m_DefaultStates = new List<bool>(m_GameObjects.Count);
        
        foreach(GameObject go in m_GameObjects)
        {
            m_DefaultStates.Add(go.activeSelf);
        }
    }

    protected override void OnReset()
    {
        for (int i = 0; i < m_GameObjects.Count; ++i)
        {
            m_GameObjects[i].gameObject.SetActive(m_DefaultStates[i]);
        }
    }
}
