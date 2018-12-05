using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaterialType
{
    Primary = 0,
    Secondary = 1,
    Tertiary = 2,
    //gap for more colors?
    Interactive1 = 5,
    Interactive2 = 6,
    Interactive3 = 7,
    Interactive4 = 8,
    Interactive5 = 9,
    //gap for more colors
    Tile = 13,
    Edge = 14,
    Node = 15
}

[System.Serializable]
public class RendererMaterialTypePair //Would love for it to be a struct, but i need a reference type.
{
    [SerializeField]
    private Renderer m_Renderer;
    public Renderer Renderer
    {
        get { return m_Renderer; }
    }

    [SerializeField]
    private MaterialType m_Material = MaterialType.Primary;
    public MaterialType Material
    {
        get { return m_Material; }
    }

    public RendererMaterialTypePair()
    {
        m_Renderer = null;
        m_Material = MaterialType.Primary;
    }

    public RendererMaterialTypePair(MeshRenderer renderer, MaterialType materialType)
    {
        m_Renderer = renderer;
        m_Material = materialType;
    }
}

public delegate void CharacterDelegate(Character character);

public abstract class LevelObject : ResetableObject, IHintable
{
    [Header("General")]
    [Space(5)]
    [HideInInspector] //Custom inspector
    [SerializeField]
    protected bool m_IsEnabled = true;
    private bool m_IsEnabledReset;

    [Header("Materials (Level Wizard)")]
    [Space(5)]
    [SerializeField]
    private List<RendererMaterialTypePair> m_Renderers;

    protected Transform m_Transform;
    private Transform m_OriginalParent;  //Moving

    public event CharacterDelegate CharacterEnterEvent;
    public event CharacterDelegate CharacterLeaveEvent;
    public event CharacterDelegate CharacterUseEvent;
    public event HintDelegate HintUsedEvent;

    protected virtual void Awake()
    {
        m_Transform = transform;

        m_IsEnabledReset = m_IsEnabled;
        SetEnabled(m_IsEnabled);
    }

    public Transform GetTransform()
    {
        return m_Transform;
    }

    public Vector3 GetPosition()
    {
        return m_Transform.position;
    }

    public void MoveWithTransform(Transform newParent)
    {
        //We're already moving
        if (m_OriginalParent != null)
            return;

        m_OriginalParent = m_Transform.parent;
        m_Transform.parent = newParent;
    }

    public void StopMoveWithTransform()
    {
        if (m_OriginalParent == null)
            return;

        m_Transform.parent = m_OriginalParent;
        m_OriginalParent = null;
    }

    public virtual void OnCharacterEnter(Character character, Direction direction, bool snap) { if (CharacterEnterEvent != null) { CharacterEnterEvent(character); } }
    public virtual void OnCharacterLeave(Character character, Direction direction) { if (CharacterLeaveEvent != null) { CharacterLeaveEvent(character); } }
    public virtual void OnCharacterDirectionEnter(Character character) {}
    public virtual void OnCharacterDirectionLeave(Character character) {}
    public virtual bool OnCharacterUse(Character character) { if (CharacterUseEvent != null) { CharacterUseEvent(character); } return false; } //The character activly wants to use this (button/switch/etc...)
    //public virtual void OnCharacterStopUsing(Character character) {}

    public virtual bool BlocksCharacters() { return false; }
    public virtual bool CanUse(Character character) { return false; }
    public virtual bool CanUse(Character character, out string errorMessage) { errorMessage = "";  return false; }
    public virtual void ToggleEnabled() { SetEnabled(!m_IsEnabled); }
    public virtual void SetEnabled(bool state) { m_IsEnabled = state; }

    public bool IsEnabled() { return m_IsEnabled; }

    public void SetMaterials(Material[] materials)
    {
        foreach (RendererMaterialTypePair meshRendererMaterialTypePair in m_Renderers)
        {
            meshRendererMaterialTypePair.Renderer.material = materials[(int)meshRendererMaterialTypePair.Material];
        }
    }

    //IHintable
    protected void FireHintUsedEvent(Character character)
    {
        if (HintUsedEvent != null)
            HintUsedEvent(this, character);
    }

    //ResetableObject
    protected override void OnReset()
    {
        SetEnabled(m_IsEnabledReset);
    }
}
