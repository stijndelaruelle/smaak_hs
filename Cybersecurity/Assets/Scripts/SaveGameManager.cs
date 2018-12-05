using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveGameManager
{
    //Temp
    public static string SAVE_PLAYER_NAME = "AvatarName";
    public static string SAVE_PLAYER_GENDER = "AvatarGender";
    public static string SAVE_PLAYER_SKINCOLOR = "AvatarSkinColor";
    public static string SAVE_PLAYER_EXTRACOLOR = "AvatarExtraColor";
    public static string SAVE_LAST_CAMPAIGN = "LastCampaignID";
    public static string SAVE_LAST_CHAPTER = "LastChapterID";
    public static string SAVE_LANGUAGE = "Language";
    public static string SAVE_VOLUME_MUSIC = "MusicVolume";
    public static string SAVE_VOLUME_SFX = "SFXVolume";
    public static string SAVE_VOLUME_VIDEO = "VideoVolume";
    public static string SAVE_CHEATS = "CheatsEnabled";
    public static string SAVE_HARDMODE = "HardModeEnabled";

    public static event Action SaveGameDeletedEvent;

    public static event Action<string, object> VariableChangedEvent;
    public static event Action<string> VariableDeletedEvent;

    public static event Action<string, int> IntChangedEvent;
    public static event Action<string, float> FloatChangedEvent;
    public static event Action<string, string> StringChangedEvent;
    public static event Action<string, bool> BoolChangedEvent;
    public static event Action<string, Color> ColorChangedEvent;

    //-----------------------------------
    // Currently this just a wrapper around playerprefs, but in case this changes we don't have to go trough the entire codebase to change everything
    // https://docs.unity3d.com/ScriptReference/PlayerPrefs.html
    //-----------------------------------

    //Writes all modified preferences to disk.
    public static void Save()
    {
        PlayerPrefs.Save();
    }

    //Removes all keys and values from the preferences.Use with caution.
    public static void DeleteAll() 
    {
        PlayerPrefs.DeleteAll();

        if (SaveGameDeletedEvent != null)
            SaveGameDeletedEvent();
    }

    //Removes key and its corresponding value from the preferences.
    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);

        if (VariableDeletedEvent != null)
            VariableDeletedEvent(key);
    }

    //-------------------
    // Accessors
    //-------------------

    //Returns true if key exists in the preferences.
    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    //Returns the value corresponding to key in the preference file if it exists.
    public static int GetInt(string key, int defaultValue = -1)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    //Returns the value corresponding to key in the preference file if it exists.
    public static float GetFloat(string key, float defaultValue = -1.0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    //Returns the value corresponding to key in the preference file if it exists.
    public static string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    //Returns the value corresponding to key in the preference file if it exists.
    public static bool GetBool(string key, bool defaultValue = false)
    {
        int intValue = PlayerPrefs.GetInt(key, -1);
        return (intValue == 1); //True if 1, false otherwise
    }

    public static Color GetColor(string key)
    {
        float redValue   = PlayerPrefs.GetFloat(key + "_R", -1);
        float greenValue = PlayerPrefs.GetFloat(key + "_G", -1);
        float blueValue  = PlayerPrefs.GetFloat(key + "_B", -1);
        float alphaValue = PlayerPrefs.GetFloat(key + "_A", -1);

        if (redValue < 0 || greenValue < 0 || blueValue < 0 || alphaValue < 0)
            return new Color(1.0f, 1.0f, 1.0f, 1.0f);

        return new Color(redValue, greenValue, blueValue, alphaValue);
    }

    //-------------------
    // Mutators
    //-------------------

    //Sets the value of the preference identified by key.
    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);

        //Events
        if (VariableChangedEvent != null)
            VariableChangedEvent(key, value);

        if (IntChangedEvent != null)
            IntChangedEvent(key, value);
    }

    //Sets the value of the preference identified by key.
    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);

        //Events
        if (VariableChangedEvent != null)
            VariableChangedEvent(key, value);

        if (FloatChangedEvent != null)
            FloatChangedEvent(key, value);
    }

    //Sets the value of the preference identified by key.
    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);

        //Events
        if (VariableChangedEvent != null)
            VariableChangedEvent(key, value);

        if (StringChangedEvent != null)
            StringChangedEvent(key, value);
    }

    //Sets the value of the preference identified by key.
    public static void SetBool(string key, bool value)
    {
        int intValue = 0;
        if (value == true) { intValue = 1; }

        PlayerPrefs.SetInt(key, intValue);

        //Events
        if (VariableChangedEvent != null)
            VariableChangedEvent(key, value);

        if (BoolChangedEvent != null)
            BoolChangedEvent(key, value);
    }

    public static void SetColor(string key, Color value)
    {
        PlayerPrefs.SetFloat(key + "_R", value.r);
        PlayerPrefs.SetFloat(key + "_G", value.g);
        PlayerPrefs.SetFloat(key + "_B", value.b);
        PlayerPrefs.SetFloat(key + "_A", value.a);

        //Events
        if (VariableChangedEvent != null)
            VariableChangedEvent(key, value);

        if (ColorChangedEvent != null)
            ColorChangedEvent(key, value);
    }
}
