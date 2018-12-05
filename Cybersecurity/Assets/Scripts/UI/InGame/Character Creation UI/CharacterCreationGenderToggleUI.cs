using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreationGenderToggleUI : MonoBehaviour
{
    [SerializeField]
    private Gender m_Gender;

    public void OnChange(bool state)
    {
        if (state == false)
            return;

        SaveGameManager.SetInt(SaveGameManager.SAVE_PLAYER_GENDER, (int)m_Gender);
    }
}
