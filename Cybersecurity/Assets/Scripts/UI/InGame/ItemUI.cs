using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField]
    private Image m_Icon;

    [SerializeField]
    private Text m_AmountText;

    public void Initialize(ItemTypeDefinition item, int amount = 1)
    {
        m_Icon.sprite = item.InventorySprite;

        m_AmountText.text = amount + "x";
        m_AmountText.enabled = (amount > 1);  
    }

    public void Initialize(ItemAmountPair itemAmountPair)
    {
        Initialize(itemAmountPair.Item, itemAmountPair.Amount);
    }
}
