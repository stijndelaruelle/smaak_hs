using System;
using System.Collections.Generic;
using UnityEngine;

//Variation on SerializableDictionary

[Serializable]
public class StringAndStringListDictionary : Dictionary<string, List<string>>, ISerializationCallbackReceiver
{
    [Serializable]
    private class StringAndStringListPair
    {
        [SerializeField]
        private string m_Key;
        public string Key
        {
            get { return m_Key; }
        }

        [SerializeField]
        private List<string> m_ValueList;
        public List<string> ValueList
        {
            get { return m_ValueList; }
        }

        public StringAndStringListPair(string key, List<string> valueList)
        {
            m_Key = key;
            m_ValueList = valueList;
        }
    }

    [SerializeField]
    private List<StringAndStringListPair> m_Dictionary = new List<StringAndStringListPair>();

    //Save the dictionary to lists
    public void OnBeforeSerialize()
    {
        m_Dictionary.Clear();

        foreach (KeyValuePair<string, List<string>> pair in this)
        {
            StringAndStringListPair temp = new StringAndStringListPair(pair.Key, pair.Value);
            m_Dictionary.Add(temp);
        }
    }

    //Load dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();

        for (int i = 0; i < m_Dictionary.Count; i++)
        {
            this.Add(m_Dictionary[i].Key, m_Dictionary[i].ValueList);
        }
    }
}