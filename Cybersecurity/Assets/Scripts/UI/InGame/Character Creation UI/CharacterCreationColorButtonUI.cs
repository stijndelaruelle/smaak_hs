using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreationColorButtonUI : MonoBehaviour
{
    public enum CharacterColorType
    {
        SkinColor,
        ExtraColor
    }

    [SerializeField]
    private CharacterColorType m_ColorType;

    [SerializeField]
    private Image m_Image; //Take the color from image

    public void OnClick(bool state)
    {
        if (state == false)
            return;

        string variableName = "";
        if (m_ColorType == CharacterColorType.SkinColor) { variableName = SaveGameManager.SAVE_PLAYER_SKINCOLOR; }
        if (m_ColorType == CharacterColorType.ExtraColor) { variableName = SaveGameManager.SAVE_PLAYER_EXTRACOLOR; }

        SaveGameManager.SetColor(variableName, m_Image.color);
    }
}
