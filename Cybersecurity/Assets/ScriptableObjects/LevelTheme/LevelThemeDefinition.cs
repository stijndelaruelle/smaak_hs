using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cyber Security/Level Theme")]
public class LevelThemeDefinition : ScriptableObject
{
    //Background
    //Soundtrack
    [Space(5)]
    [Header("Audio")]
    [SerializeField]
    private AudioClip m_Music;
    public AudioClip Music
    {
        get { return m_Music; }
    }

    //Tile Colors
    [Space(5)]
    [Header("Tile Materials")]

    [SerializeField]
    private Material m_TileMaterial;
    public Material TileMaterial
    {
        get { return m_TileMaterial; }
    }

    [SerializeField]
    private Material m_EdgeMaterial;
    public Material EdgeMaterial
    {
        get { return m_EdgeMaterial; }
    }

    [SerializeField]
    private Material m_NodeMaterial;
    public Material NodeMaterial
    {
        get { return m_NodeMaterial; }
    }

    //LevelObject Colors
    [Space(5)]
    [Header("LevelObject Materials")]

    [SerializeField]
    private List<Material> m_LevelObjectMaterials;
    public List<Material> LevelObjectMaterials
    {
        get { return m_LevelObjectMaterials; }
    }

    //Interactabe Colors (constrastin colors)
    [Space(5)]
    [Header("Interactable Materials")]

    [SerializeField]
    private List<Material> m_InteractableMaterials;
    public List<Material> InteractableMaterials
    {
        get { return m_InteractableMaterials; }
    }
}