using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cyber Security/Item")]
public class ItemTypeDefinition : ScriptableObject
{
    [SerializeField]
    private string m_ReferenceID;
    public string ReferenceID
    {
        get { return m_ReferenceID; }
    }

    [SerializeField]
    private string m_LocalizationID;
    public string LocalizationID
    {
        get { return m_LocalizationID; }
    }

    [SerializeField]
    private Sprite m_InventorySprite;
    public Sprite InventorySprite
    {
        get { return m_InventorySprite; }
    }

    //Eventueel onzichtbare items (state van "je hebt met die of die gepraat") Kunnen switches dan ook mee checken!

    //Operator overloading
    public static bool operator == (ItemTypeDefinition a, ItemTypeDefinition b)
    {
        //https://stackoverflow.com/questions/4219261/overriding-operator-how-to-compare-to-null
        if (object.ReferenceEquals(a, null))
        {
            return object.ReferenceEquals(b, null);
        }

        return a.Equals(b);
    }

    public static bool operator != (ItemTypeDefinition a, ItemTypeDefinition b)
    {
        //https://stackoverflow.com/questions/4219261/overriding-operator-how-to-compare-to-null
        if (object.ReferenceEquals(a, null))
        {
            return object.ReferenceEquals(b, null);
        }

        return (a.Equals(b) == false);
    }

    public override bool Equals(object obj)
    {
        ItemTypeDefinition b = (ItemTypeDefinition)obj;

        if (b == null)
            return false;

        //If this were ever called every frame please move away from a string compare
        return (this.ReferenceID == b.ReferenceID);
    }

    public override int GetHashCode()
    {
        if (m_ReferenceID == null)
            return -1;

        return m_ReferenceID.GetHashCode();
    }
}
