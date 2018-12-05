using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DevelopmentMenu
{
    //PARSE LOCALIZATION
    [MenuItem("Cyber Security/Localization/From Google Sheets...")]
    public static void ParseLocalizationDatabaseFromGoogleSheets()
    {
        /*
        if (Application.isPlaying)
        {
            if (LocalizationManager.Instance == null)
            {
                Debug.LogWarning("Can't find a localization manager!");
                return;
            }

            LocalizationManager.Instance.DeserializeFromGoogleSheets();
        }
        else
        {
        */
            LocalizationManager.DeserializeFromGoogleSheetsNotRunning();
        /*}  */
    }

    [MenuItem("Cyber Security/Localization/From File...")]
    public static void ParseLocalizationDatabaseFromFile()
    {
        LocalizationManager.DeserializeFromFile();
    }

    //SET LOCALIZATION LANGUAGE
    [MenuItem("Cyber Security/Language/Dutch")]
    public static void SetLanguageDutch()
    {
        SetLanguage(LocalizationManager.Language.Dutch);
    }

    [MenuItem("Cyber Security/Language/English")]
    public static void SetLanguageEnglish()
    {
        SetLanguage(LocalizationManager.Language.English);
    }

    [MenuItem("Cyber Security/Language/French")]
    public static void SetLanguageFrench()
    {
        SetLanguage(LocalizationManager.Language.French);
    }

    [MenuItem("Cyber Security/Language/German")]
    public static void SetLanguageGerman()
    {
        SetLanguage(LocalizationManager.Language.German);
    }

    private static void SetLanguage(LocalizationManager.Language language)
    {
        //When playing go the "official way"
        if (Application.isPlaying)
        {
            if (LocalizationManager.Instance == null)
            {
                Debug.LogWarning("Can't find a localization manager!");
                return;
            }

            LocalizationManager.Instance.SetLanguage(language);
        }

        //Otherwise, bypass everything and write straight to the save game
        else
        {
            SaveGameManager.SetInt(SaveGameManager.SAVE_LANGUAGE, (int)language);
        }
    }

    //SAVE GAME

    //Avatar
    [MenuItem("Cyber Security/Save Game/Avatar/Male")]
    public static void SetMaleAvatar()
    {
        SaveGameManager.SetInt(SaveGameManager.SAVE_PLAYER_GENDER, (int)Gender.Male);
    }

    [MenuItem("Cyber Security/Save Game/Avatar/Female")]
    public static void SetFemaleAvatar()
    {
        SaveGameManager.SetInt(SaveGameManager.SAVE_PLAYER_GENDER, (int)Gender.Female);
    }

    [MenuItem("Cyber Security/Save Game/Avatar/Skin Color/White")]
    public static void SetSkinColorWhite()
    {
        SaveGameManager.SetColor(SaveGameManager.SAVE_PLAYER_SKINCOLOR, Color.white);
    }

    [MenuItem("Cyber Security/Save Game/Avatar/Skin Color/Light Brown")]
    public static void SetSkinColorLightBrown()
    {
        SaveGameManager.SetColor(SaveGameManager.SAVE_PLAYER_SKINCOLOR, new Color(0.75f, 0.75f, 0.75f, 1.0f));
    }

    [MenuItem("Cyber Security/Save Game/Avatar/Skin Color/Dark Brown")]
    public static void SetSkinColorDarkBrown()
    {
        SaveGameManager.SetColor(SaveGameManager.SAVE_PLAYER_SKINCOLOR, new Color(0.5f, 0.5f, 0.5f, 1.0f));
    }

    [MenuItem("Cyber Security/Save Game/Avatar/Extra Color/Red")]
    public static void SetExtraColorRed()
    {
        SaveGameManager.SetColor(SaveGameManager.SAVE_PLAYER_EXTRACOLOR, Color.red);
    }

    [MenuItem("Cyber Security/Save Game/Avatar/Extra Color/Green")]
    public static void SetExtraColorGreen()
    {
        SaveGameManager.SetColor(SaveGameManager.SAVE_PLAYER_EXTRACOLOR, Color.green);
    }

    [MenuItem("Cyber Security/Save Game/Avatar/Extra Color/Blue")]
    public static void SetExtraColorBlue()
    {
        SaveGameManager.SetColor(SaveGameManager.SAVE_PLAYER_EXTRACOLOR, Color.blue);
    }

    [MenuItem("Cyber Security/Save Game/Avatar/Extra Color/Yellow")]
    public static void SetExtraColorYellow()
    {
        SaveGameManager.SetColor(SaveGameManager.SAVE_PLAYER_EXTRACOLOR, Color.yellow);
    }

    [MenuItem("Cyber Security/Save Game/Avatar/Extra Color/White")]
    public static void SetExtraColorWhite()
    {
        SaveGameManager.SetColor(SaveGameManager.SAVE_PLAYER_EXTRACOLOR, new Color(0.9f, 0.9f, 0.9f, 1.0f));
    }

    [MenuItem("Cyber Security/Save Game/Avatar/Extra Color/Black")]
    public static void SetExtraColorBlack()
    {
        SaveGameManager.SetColor(SaveGameManager.SAVE_PLAYER_EXTRACOLOR, new Color(0.25f, 0.25f, 0.25f, 1.0f));
    }

    [MenuItem("Cyber Security/Save Game/Cheats/Enable")]
    public static void EnableCheats()
    {
        SaveGameManager.SetBool(SaveGameManager.SAVE_CHEATS, true);
    }

    [MenuItem("Cyber Security/Save Game/Cheats/Disable")]
    public static void DisableCheats()
    {
        SaveGameManager.SetBool(SaveGameManager.SAVE_CHEATS, false);
    }

    [MenuItem("Cyber Security/Save Game/Wipe")]
    public static void WipeSaveGame()
    {
        SaveGameManager.DeleteAll();
    }
}
