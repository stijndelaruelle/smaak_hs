using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Gender
{
    None = 0,
    Male = 1,
    Female = 2
}

public class PlayerAppearance : MonoBehaviour
{
    [Header("Enable/Disable")]
    [SerializeField]
    private List<GameObject> m_MaleParts;

    [SerializeField]
    private List<GameObject> m_FemaleParts;

    [Header("Recolor")]
    [SerializeField]
    private List<Renderer> m_SkinColorRenderers;

    [SerializeField]
    private List<Renderer> m_ExtraColorRenderers;

    [Header("Retexture")]
    [SerializeField]
    private Renderer m_FaceRenderer;

    [SerializeField]
    private List<Texture> m_MaleFaceTextures;

    [SerializeField]
    private List<Texture> m_FemaleFaceTextures;

    private void Start()
    {
        SaveGameManager.VariableChangedEvent += OnSaveGameVariableChanged;
        SaveGameManager.SaveGameDeletedEvent += OnSaveGameDeletedEvent;

        UpdateCharacterGender((Gender)SaveGameManager.GetInt(SaveGameManager.SAVE_PLAYER_GENDER, 0));
        UpdateCharacterSkinColor(SaveGameManager.GetColor(SaveGameManager.SAVE_PLAYER_SKINCOLOR));
        UpdateCharacterExtraColor(SaveGameManager.GetColor(SaveGameManager.SAVE_PLAYER_EXTRACOLOR));
    }

    private void OnDestroy()
    {
        SaveGameManager.VariableChangedEvent -= OnSaveGameVariableChanged;
        SaveGameManager.SaveGameDeletedEvent -= OnSaveGameDeletedEvent;
    }

    private void UpdateCharacterGender(Gender gender)
    {
        if (gender == Gender.None)
            return;

        bool isMale = (gender == Gender.Male);
        bool isFemale = (gender == Gender.Female);

        foreach (GameObject obj in m_MaleParts) { obj.SetActive(isMale); }
        foreach (GameObject obj in m_FemaleParts) { obj.SetActive(isFemale); }

        UpdateFace();
    }

    private void UpdateCharacterSkinColor(Color color)
    {
        if (color.a < 1.0f)
            color.a = 1.0f;

        foreach(Renderer renderer in m_SkinColorRenderers)
        {
            renderer.material.SetColor("m_DiffuseColor", color);
        }

        UpdateFace();
    }

    private void UpdateCharacterExtraColor(Color color)
    {
        if (color.a < 1.0f)
            color.a = 1.0f;

        foreach (Renderer renderer in m_ExtraColorRenderers)
        {
            renderer.material.SetColor("m_DiffuseColor", color);
        }
    }

    private void UpdateFace()
    {
        Gender gender = (Gender)SaveGameManager.GetInt(SaveGameManager.SAVE_PLAYER_GENDER);
        Color skinColor = SaveGameManager.GetColor(SaveGameManager.SAVE_PLAYER_SKINCOLOR);

        List<Texture> faceTextures = null;

        if (gender == Gender.Male)   { faceTextures = m_MaleFaceTextures; }
        if (gender == Gender.Female) { faceTextures = m_FemaleFaceTextures; }

        if (faceTextures == null)
            return;

        string htmlColor = ColorUtility.ToHtmlStringRGB(skinColor).ToLower();
        Texture foundTexture = faceTextures.Find(x => x.name.Contains(htmlColor));

        if (foundTexture == null)
            return;

        m_FaceRenderer.material.SetTexture("m_DiffuseTexture", foundTexture);
    }

    private void OnSaveGameVariableChanged(string key, object value)
    {
        if (key == SaveGameManager.SAVE_PLAYER_GENDER)
            UpdateCharacterGender((Gender)value);

        if (key == SaveGameManager.SAVE_PLAYER_SKINCOLOR)
            UpdateCharacterSkinColor((Color)value);

        if (key == SaveGameManager.SAVE_PLAYER_EXTRACOLOR)
            UpdateCharacterExtraColor((Color)value);
    }

    private void OnSaveGameDeletedEvent()
    {
        UpdateCharacterGender((Gender)SaveGameManager.GetInt(SaveGameManager.SAVE_PLAYER_GENDER, 0));
        UpdateCharacterSkinColor(SaveGameManager.GetColor(SaveGameManager.SAVE_PLAYER_SKINCOLOR));
        UpdateCharacterExtraColor(SaveGameManager.GetColor(SaveGameManager.SAVE_PLAYER_EXTRACOLOR));
    }
}
