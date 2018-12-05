using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class CharacterCreationNameInputFieldUI : MonoBehaviour
{
    private InputField m_InputField;

    private void Start()
    {
        m_InputField = GetComponent<InputField>();
        m_InputField.text = SaveGameManager.GetString(SaveGameManager.SAVE_PLAYER_NAME, "");
    }

    public void SaveName(string value)
    {
        SaveGameManager.SetString(SaveGameManager.SAVE_PLAYER_NAME, value);
    }
}
