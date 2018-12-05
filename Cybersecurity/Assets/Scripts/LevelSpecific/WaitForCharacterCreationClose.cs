using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForCharacterCreationClose : MonoBehaviour
{
    [SerializeField]
    private CharacterCreationUI m_CharacterCreationUI;

    [SerializeField]
    private GameObject m_GameObjectToShow;

    private void Start()
    {
        //Only bother when there is a character creation UI available
        if (m_CharacterCreationUI != null)
        {
            m_GameObjectToShow.SetActive(false);
            m_CharacterCreationUI.CharacterCreationCloseEvent += OnCharacterCreationClose;
        }
    }

    private void OnDestroy()
    {
        if (m_CharacterCreationUI != null)
            m_CharacterCreationUI.CharacterCreationCloseEvent -= OnCharacterCreationClose;
    }

    private void OnCharacterCreationClose()
    {
        m_GameObjectToShow.SetActive(true);
    }
}
