using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cyber Security/Game Structure/Video")]
public class VideoDataDefinition : ScriptableObject
{
    [SerializeField]
    private string m_VideoPath;
    public string VideoPath
    {
        get { return m_VideoPath; }
    }

    //Save game (don't know it we need this right now)
    //public bool HasVideoBeenWatched()
    //{
    //    return SaveGameManager.GetBool(m_SaveGameVariableName, false);
    //}

    //public void SetVideoWatched()
    //{
    //    SaveGameManager.SetBool(m_SaveGameVariableName, true);
    //}
}