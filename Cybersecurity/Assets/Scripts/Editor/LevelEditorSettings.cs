using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cyber Security/Level Editor Settings")]
public class LevelEditorSettings : ScriptableObject
{
    //Only used in the Level Wizard, we could also let the user drag a scriptable object in every single time but that would become tedious very quickly.
    public enum LevelTheme
    {
        Cafe = 0,
        Cloud = 1,
        Hardware = 2,
        Darkweb = 3
    }

    [SerializeField]
    private Texture2D m_Merlin;
    public Texture2D Merlin
    {
        get { return m_Merlin; }
    }

    [SerializeField]
    private float m_GridSize = 4.0f;
    public float GridSize
    {
        get { return m_GridSize; }
    }

    [SerializeField]
    private string m_GlobalScenePath = "";
    public string GlobalScenePath
    {
        get { return m_GlobalScenePath; }
    }

    [SerializeField]
    private GameObject m_TilePrefab;
    public GameObject TilePrefab
    {
        get { return m_TilePrefab; }
    }

    [SerializeField]
    private List<LevelThemeDefinition> m_LevelThemes;

    //Could make this into a list at some point to potentionally add more neighbours.
    [SerializeField]
    private Mesh[] m_DirectionMeshes = new Mesh[5]; //0 = One Direction, 1 = Two Directions (straight), 2 = Two directions (corner), 3 = Three Directions, 4 = Four Directions

    [SerializeField]
    private List<LevelObject> m_LevelObjects;


    public LevelThemeDefinition GetLevelTheme(LevelTheme levelTheme)
    {
        int levelThemeID = (int)levelTheme;

        if (levelThemeID < 0 || levelThemeID >= m_LevelThemes.Count)
            return null;

        return m_LevelThemes[levelThemeID];
    }

    public Mesh GetDirectionMesh(DirectionConnection connectionType)
    {
        int connectionTypeID = (int)connectionType;

        if (connectionTypeID < 0 || connectionTypeID >= m_DirectionMeshes.Length)
            return null;

        return m_DirectionMeshes[connectionTypeID];
    }

    public List<LevelObject> GetLevelObjects()
    {
        return m_LevelObjects;
    }
}