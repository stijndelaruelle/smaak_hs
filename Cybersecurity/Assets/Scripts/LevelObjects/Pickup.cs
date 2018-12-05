using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : LevelObject
{
    [Space(5)]
    [Header("Specific")]
    [SerializeField]
    protected ItemTypeDefinition m_Item;

    [SerializeField]
    protected int m_Amount = 1;

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();    
    }

    //LevelObject
    public override void OnCharacterEnter(Character character, Direction direction, bool snap)
    {
        if (gameObject.activeSelf == false)
            return;

        //Doe dingen met inventory
        Inventory inventory = character.Inventory;

        if (inventory != null)
        {
            inventory.AddItem(m_Item, m_Amount);
            FireHintUsedEvent(character);
        }

        gameObject.SetActive(false);
    }

    //ResetableObject
    protected override void OnReset()
    {
        gameObject.SetActive(true);
        base.OnReset();
    }
}
