using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cyber Security/Localization Data")]
public class LocalizationData : ScriptableObject
{
    [SerializeField]
    private StringAndStringListDictionary m_Data; //Per key multiple language values

    public string GetText(string key, LocalizationManager.Language language)
    {
        //Initialization check
        if (m_Data == null)
        {
            return "DATA NOT READ: Please parse the database first.";
        }

        //Key check
        if (key == "")
            return "Enter a key";

        if (m_Data.ContainsKey(key) == false)
            return "INVALID KEY: " + key + " does not exist.";

        //Language check
        int languageID = (int)language;
        int numOfLanguages = Enum.GetNames(typeof(LocalizationManager.Language)).Length;

        if (languageID >= numOfLanguages)
            return "INVALID LANGUAGE: There are currently only " + numOfLanguages + " available.";

        string result = m_Data[key][languageID];

        if (result == "")
            result = "No " + language.ToString() + " translation for " + key + " yet!";

        //We got text, now add tokens
        return result;
    }

    public bool Deserialize(string fileText)
    {
        if (m_Data != null)
            m_Data.Clear();

        //Debug.Log("Parsing localization file...");

        m_Data = new StringAndStringListDictionary();

        string[,] parsedFile = UtilityMethods.ParseCSVRaw(fileText);

        if (parsedFile == null)
            return false;

        if (parsedFile.GetLength(1) < 2)
        {
            Debug.LogError("The localization file does not contain any data!");
            return false;
        }

        //For every row
        for (int y = 0; y < parsedFile.GetLength(1); ++y)
        {
            //Get the key
            string key = parsedFile[0, y];
            List<string> translations = new List<string>();

            //Get all the translations
            for (int x = 1; x < parsedFile.GetLength(0); ++x)
            {
                translations.Add(parsedFile[x, y]);
            }

            //Add it to our data
            if (m_Data.ContainsKey(key))
                Debug.LogError("Localization parsing: " + key + " is already in use!");
            else
                m_Data.Add(key, translations);
        }

        Debug.Log("Localization update successful!");
        return true;
    }
}
