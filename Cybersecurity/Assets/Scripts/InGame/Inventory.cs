using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemAmountPair //Would love for it to be a struct, but i need a reference type.
{
    [SerializeField]
    private ItemTypeDefinition m_Item;
    public ItemTypeDefinition Item
    {
        get { return m_Item; }
        set { m_Item = value; }
    }

    [SerializeField]
    private int m_Amount;
    public int Amount
    {
        get { return m_Amount; }
        set { m_Amount = value; }
    }

    public ItemAmountPair(ItemTypeDefinition item, int amount = 1)
    {
        m_Item = item;
        m_Amount = amount;
        CheckAmount();
    }

    public ItemAmountPair(ItemAmountPair other)
    {
        //Copy constructor
        m_Item = other.Item;
        m_Amount = other.Amount;
        CheckAmount();
    }

    //You can't just use m_Items[itemSlot].amount += x; 
    public void Add(int amount)
    {
        m_Amount += amount;
    }

    public void Remove(int amount)
    {
        m_Amount -= amount;
        CheckAmount();
    }

    public void SetAmount(int amount)
    {
        m_Amount = amount;
        CheckAmount();
    }

    private void CheckAmount()
    {
        if (this.m_Amount < 0)
            this.m_Amount = 0;
    }
}

//For now items can stack indefinitly to keep the inventory UI small.
public class Inventory : ResetableObject
{
    public delegate void ItemChangedDelegate(int itemSlot, ItemAmountPair itemAmountPair);

    [SerializeField]
    private List<ItemAmountPair> m_Items; //Could also simply be a List<Item>, both have pro's & cons. I think this one has a "heavier" cost up front, but scales better in case the inventory becomes huge (also supports max stacks easier)
    private List<ItemAmountPair> m_OriginalItems; //Used OnReset

    //Events
    public event ItemChangedDelegate ItemChangedEvent;

    private void Awake()
    {
        m_OriginalItems = new List<ItemAmountPair>();

        if (m_Items == null)
            return;
        
        foreach (ItemAmountPair itemAmountPair in m_Items)
        {
            m_OriginalItems.Add(new ItemAmountPair(itemAmountPair));
        }
    }

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < m_Items.Count; ++i)
        {
            FireItemChangedEvent(i, m_Items[i]);
        }
    }

    public void AddItem(ItemTypeDefinition item, int amount = 1)
    {
        //Check if we already have an index for this item
        int itemSlot = GetItemSlot(item);

        //Yes, just add
        if (itemSlot >= 0)
        {
            m_Items[itemSlot].Add(amount);
            FireItemChangedEvent(itemSlot, m_Items[itemSlot]);
        }

        //No, add new record
        else
        {
            m_Items.Add(new ItemAmountPair(item, amount));

            int newID = m_Items.Count - 1;
            FireItemChangedEvent(newID, m_Items[newID]);
        }
    }

    public void AddItem(ItemAmountPair itemAmountPair)
    {
        AddItem(itemAmountPair.Item, itemAmountPair.Amount);
    }


    public void RemoveItem(ItemTypeDefinition item, int amount = 1)
    {
        int itemSlot = GetItemSlot(item);

        //We don't own this item
        if (itemSlot < 0)
            return;

        m_Items[itemSlot].Remove(amount);
        FireItemChangedEvent(itemSlot, m_Items[itemSlot]);

        //If we have none left, remove from inventory
        if (m_Items[itemSlot].Amount <= 0)
        {
            m_Items.RemoveAt(itemSlot);
        }
    }

    public void RemoveItem(ItemAmountPair itemAmountPair)
    {
        RemoveItem(itemAmountPair.Item, itemAmountPair.Amount);
    }


    public int GetItemSlot(ItemTypeDefinition item)
    {
        for (int i = 0; i < m_Items.Count; ++i)
        {
            //Check if this is the item
            if (m_Items[i].Item == item)
            {
                return i;
            }
        }

        return -1;
    }

    public int GetItemAmount(ItemTypeDefinition item)
    {
        int itemSlot = GetItemSlot(item);
        if (itemSlot < 0)
            return 0;

        return m_Items[itemSlot].Amount;
    }


    public bool HasItem(ItemTypeDefinition item, int amount = 1)
    {
        return (GetItemAmount(item) > 0);
    }

    public bool HasItem(ItemAmountPair itemAmountPair)
    {
        return HasItem(itemAmountPair.Item, itemAmountPair.Amount);
    }


    private void FireItemChangedEvent(int itemSlot, ItemAmountPair itemAmountPair)
    {
        if (ItemChangedEvent != null)
            ItemChangedEvent(itemSlot, m_Items[itemSlot]);
    }

    //ResetableObject
    protected override void OnReset()
    {
        //We could reuse slots and fill them with original data, that's more performant than starting over. (this is just typed quicker)
        if (m_Items == null)
            m_Items = new List<ItemAmountPair>();

        //Clear all items (Items.Clear doesn't fire all the events)
        for (int i = m_Items.Count - 1; i >= 0; --i)
        {
            RemoveItem(m_Items[i]);
        }

        //Refill with the original items
        foreach (ItemAmountPair itemAmountPair in m_OriginalItems)
        {
            AddItem(new ItemAmountPair(itemAmountPair));
        }
    }
}
