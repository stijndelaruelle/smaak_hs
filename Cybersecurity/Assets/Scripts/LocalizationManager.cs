using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

//I think this can become a complete static class at some point. It really doesn't need run time data...

public class LocalizationManager : Singleton<LocalizationManager>
{
    public delegate void LocalizationLoadedDelegate(bool success);
    public delegate void LanguageDelegate(Language language);
    public delegate void TokenDelegate(string token);

    private static string s_PlayerNameToken = "PLAYER_NAME"; //Dirty, but I can only think of 1 token right now. If more pop up we'll expand the "token" system properly.

    public enum Language
    {
        Dutch = 0,
        English = 1,
        French = 2,
        German = 3
    }

    [SerializeField]
    private Language m_CurrentLanguage;
    public Language CurrentLanguage
    {
        get { return m_CurrentLanguage; }
    }
    
    //Reason for static: We want access to the localization data from within the editor
    private static LocalizationData m_Data;

    [SerializeField]
    [Tooltip("Google script url used to get the data from Google sheets")]
    [TextArea(3, 10)]
    private static string m_GoogleScriptURL = "https://script.google.com/macros/s/AKfycbzJdumFizactNgFMwwImILNQIq8-OseTva7uQ63g6-JYZQXN6A/exec"; //Static variables can't be seen in the inspector. This is a quick fix.
    private static Coroutine m_DeserializeFromGoogleSheetsRoutine;

    public event LocalizationLoadedDelegate LocalizationLoadedEvent;
    public event LanguageDelegate LanguageChangedEvent;
    public event TokenDelegate TokenChangedEvent;

    protected override void Awake()
    {
        base.Awake();
        LoadLocalizationData();

        //"Load settings"
        //Normally I would never access other objects during awake.
        //But as this scene is not active most of the time (especially when working in the editor) it still has to load before other objects want to access it.
        int languageID = SaveGameManager.GetInt(SaveGameManager.SAVE_LANGUAGE);

        if (languageID >= 0)
            m_CurrentLanguage = (Language)(languageID);
    }

    private void Start()
    {
        SaveGameManager.VariableChangedEvent += OnSaveGameVariableChanged;    
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SaveGameManager.VariableChangedEvent -= OnSaveGameVariableChanged;
    }

    public void SetLanguage(Language language)
    {
        if (m_CurrentLanguage == language)
            return;

        m_CurrentLanguage = language;

        //"Save setting"
        SaveGameManager.SetInt(SaveGameManager.SAVE_LANGUAGE, (int)m_CurrentLanguage);

        if (LanguageChangedEvent != null)
            LanguageChangedEvent(m_CurrentLanguage);
    }


    public static string GetText(string key, params object[] customTokens)
    {
        //Fallback
        if (LocalizationManager.Instance == null)
        {
            return GetText(key, Language.Dutch, customTokens);
        }
        else
        {
            return GetText(key, LocalizationManager.Instance.CurrentLanguage, customTokens);
        }
    }

    public static string GetText(string key, Language language, params object[] customTokens)
    {
        if (key == null)
            return "NO KEY FOUND";

        key = key.Trim();

        LoadLocalizationData();

        //Check if this text contains tokens, if so find the data for it.
        string text = m_Data.GetText(key, language);

        //Get rid of the annoying extra \'s that unity adds (messes with unicode characters such as font awesome)
        text = Regex.Unescape(text);

        bool tracedTokens = false;
        int currentLetterID = 0;
        int currentCustomTokenID = 0;
        List<string> substitutionData = new List<string>();

        while (tracedTokens == false)
        {
            //Find the starting bracket
            int startBracketID = text.IndexOf("{", currentLetterID);

            if (startBracketID < 0)
            {
                tracedTokens = true;
                continue;
            }

            //Find the end bracket
            int endBracketID = text.IndexOf("}", startBracketID + 1);

            if (endBracketID < 0)
            {
                tracedTokens = true;
                continue;
            }

            //Get the token text between the 2 brackets
            string tokenText = text.Substring(startBracketID + 1, endBracketID - startBracketID - 1);

            //Remove the token text & add a number instead
            string tokenID = substitutionData.Count.ToString();
            text = text.Remove(startBracketID + 1, endBracketID - startBracketID - 1);
            text = text.Insert(startBracketID + 1, tokenID);

            currentLetterID = startBracketID + tokenID.Length + 1; //Startbracket + length of the tokenID (0-9 = 1 / 10 - 99 = 2 / etc...) + last bracket

            //Decide what substitude data we're going to use
            if (tokenText == s_PlayerNameToken)
            {
                if (SaveGameManager.HasKey(SaveGameManager.SAVE_PLAYER_NAME))
                    substitutionData.Add(SaveGameManager.GetString(SaveGameManager.SAVE_PLAYER_NAME));
                else
                    substitutionData.Add(s_PlayerNameToken);
            }
            else
            {
                //We use the next custom one
                if (currentCustomTokenID < customTokens.Length)
                {
                    substitutionData.Add(customTokens[currentCustomTokenID].ToString());
                    currentCustomTokenID += 1;
                }
            }
        }

        if (substitutionData.Count > 0)
        {
            return string.Format(text, substitutionData.ToArray());
        }

        //No tokens, just return the regular text
        return text;
    }

    private static void LoadLocalizationData()
    {
        //Load the localization Data
        if (m_Data == null)
            m_Data = (LocalizationData)Resources.Load("LocalizationData", typeof(LocalizationData));
    }

    //Save game changed
    private void OnSaveGameVariableChanged(string key, object value)
    {
        if (key != "AvatarName")
            return;

        if (TokenChangedEvent != null)
            TokenChangedEvent(s_PlayerNameToken);
    } 

    //Serialization (will become static at some point, so we can load without having to run the game)
    private void Serialize()
    {
        //TODO(?)
    }

    public void DeserializeFromGoogleSheets()
    {
        //Don't spam deserialize!
        if (m_DeserializeFromGoogleSheetsRoutine != null)
            StopCoroutine(m_DeserializeFromGoogleSheetsRoutine);

        m_DeserializeFromGoogleSheetsRoutine = StartCoroutine(DeserializeFromGoogleSheetsRoutine());
    }

    public static void DeserializeFromGoogleSheetsNotRunning()
    {
        LoadLocalizationData();

        IEnumerator enumerator = DeserializeFromGoogleSheetsRoutine();

        // Current points to null here, so move it forward
        enumerator.MoveNext();

        // This blocks, but you can always use a thread
        while (!((WWW)(enumerator.Current)).isDone) ;

        // This triggers your 'Debug.Log(www.text)'
        enumerator.MoveNext();
    }

    //Coroutine version (better, but can't be run in the editor)
    private static IEnumerator DeserializeFromGoogleSheetsRoutine()
    {
        bool success = true;

        //Debug.Log("Getting localization file from Google Sheets... (can take a couple seconds)");

        //Get the data
        WWW www = new WWW(m_GoogleScriptURL);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("SendToGoogleSheets encountered an error: " + www.error.ToString());
            success = false;
        }

        else if (!string.IsNullOrEmpty(www.text))
        {
            //http://answers.unity3d.com/questions/844423/wwwtext-not-reading-utf-8-text.html
            string fileText = www.text;

            //Deserialize the data
            success = m_Data.Deserialize(fileText);

            #if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(m_Data);
            #endif
        }

        //Cleanup
        www.Dispose();
        m_DeserializeFromGoogleSheetsRoutine = null;

        //if (LocalizationLoadedEvent != null)
        //    LocalizationLoadedEvent(success);
    }

    public static bool DeserializeFromFile()
    {
        bool success = false;

        #if UNITY_EDITOR
            //Select a file
            string filePath = UnityEditor.EditorUtility.OpenFilePanelWithFilters("Select a localization file to load", Application.dataPath, new string[] { "Localization Database", "ldb" });

            //Load the text from the file
            string fileText = "";
            try
            {
                fileText = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            }
            catch (Exception e)
            {
                //The file was not found, but that shouldn't crash the game!
                Debug.LogError(e.Message);

                //if (LocalizationLoadedEvent != null)
                //    LocalizationLoadedEvent(false);

                return false;
            }

            //Deserialize the data
            LoadLocalizationData();
            success = m_Data.Deserialize(fileText);

            #if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(m_Data);
            #endif

            //if (LocalizationLoadedEvent != null)
            //    LocalizationLoadedEvent(success);
        #endif

        return success;
    }
}