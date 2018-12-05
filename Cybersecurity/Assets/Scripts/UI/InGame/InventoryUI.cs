using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private Inventory m_Inventory;

    [SerializeField]
    private ItemUI m_ItemUIPrefab;
    private List<ItemUI> m_ItemUIList;

    private void Awake()
    {
        m_ItemUIList = new List<ItemUI>();
    }

    private void Start()
    {
        if (m_Inventory != null)
            m_Inventory.ItemChangedEvent += OnItemChanged;
        else
            Debug.LogError("InventoryUI: No inventory connected, UI will not update!");
    }

    private void OnDestroy()
    {
        if (m_Inventory != null)
            m_Inventory.ItemChangedEvent -= OnItemChanged;
    }

    private void OnItemChanged(int itemSlot, ItemAmountPair itemAmountPair)
    {
        //Create slots if needed
        while (itemSlot >= m_ItemUIList.Count)
        {
            ItemUI newItemUI = GameObject.Instantiate<ItemUI>(m_ItemUIPrefab);
            newItemUI.gameObject.transform.SetParent(transform);
            newItemUI.gameObject.transform.localScale = Vector3.one;

            m_ItemUIList.Add(newItemUI);
        }

        //Change the slot in question
        m_ItemUIList[itemSlot].Initialize(itemAmountPair);

        if (itemAmountPair.Amount <= 0)
        {
            GameObject.Destroy(m_ItemUIList[itemSlot].gameObject); //Will use pooling one day
            m_ItemUIList.RemoveAt(itemSlot);
        }
    }
}
