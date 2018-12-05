using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cyber Security/Faction")]
public class FactionTypeDefinition : ScriptableObject
{
    [SerializeField]
    private string m_FactionName;

    [SerializeField]
    private Color m_Color;

    [SerializeField]
    private List<FactionTypeDefinition> m_Allies;

    [SerializeField]
    private List<FactionTypeDefinition> m_Enemies;

    public bool IsAlly(FactionTypeDefinition factionType)
    {
        if (this == factionType)
            return true;

        return (m_Allies.Contains(factionType));
    }

    public bool IsEnemy(FactionTypeDefinition factionType)
    {
        return (m_Enemies.Contains(factionType));
    }

    public bool IsNeutral(FactionTypeDefinition factionType)
    {
        if (IsAlly(factionType))
            return false;

        if (IsEnemy(factionType))
            return false;

        return true;
    }

    public override string ToString()
    {
        return "Faction: " + m_FactionName;
    }
}