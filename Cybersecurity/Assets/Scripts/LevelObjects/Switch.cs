using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : LevelObject
{
    public delegate void SwitchProgressDelegate(string progress);

    [SerializeField]
    private List<LevelObject> m_LinkedLevelObjects;

    [SerializeField]
    protected bool m_TriggerOnEnter = false;

    [SerializeField]
    protected bool m_TriggerOnLeave = false;

    [SerializeField]
    private bool m_TriggerOnUse = true;

    [SerializeField]
    private Renderer m_ColorModel;

    [SerializeField]
    private Material m_EnabledMaterial;

    [SerializeField]
    private Material m_DisabledMaterial;

    protected bool m_IsOn = false; //State of the switch, IsEnabled means it doesn't work. IsOn is just wether it's on or not.
    public bool IsOn
    {
        get { return m_IsOn; }
    }

    [Space(5)]
    [Header("Required Items")]
    [SerializeField]
    private List<ItemAmountPair> m_RequiredItems;

    [SerializeField]
    private bool m_ConsumeItems = false;

    [SerializeField]
    [Tooltip("The inventory were we're remotly going to check & activate with")]
    private Inventory m_AutoInventory;
    private bool m_AutoActivated = false;

    public event SwitchProgressDelegate SwitchProgressEvent;

    protected override void Start()
    {
        base.Start();

        if (m_AutoInventory != null)
            m_AutoInventory.ItemChangedEvent += OnInventoryChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (m_AutoInventory != null)
            m_AutoInventory.ItemChangedEvent -= OnInventoryChanged;
    }

    private void OnInventoryChanged(int itemSlot, ItemAmountPair itemAmountPair)
    {
        //We can only activate once automatically. Otherwise we would trigger on 2,3,4 etc datablocks (if we only need 2)
        //On top of that we'd keep triggering when the items dissapeared.
        if (m_AutoActivated)
            return;

        if (m_AutoInventory != null)
        {
            m_AutoActivated = UseSwitch(m_AutoInventory);

            if (SwitchProgressEvent != null)
                SwitchProgressEvent(GetProgress());
        }
    }

    //LevelObject
    public override void OnCharacterDirectionEnter(Character character)
    {
        base.OnCharacterEnter(character, Direction.None, false);

        if (m_TriggerOnEnter)
        if (m_TriggerOnLeave)
        {
            bool success = UseSwitch(character.Inventory);

            if (success)
                FireHintUsedEvent(character);
        }
    }

    public override void OnCharacterDirectionLeave(Character character)
    {
        base.OnCharacterLeave(character, Direction.None);

        if (m_TriggerOnLeave)
        {
            bool success = UseSwitch(character.Inventory);

            if (success)
                FireHintUsedEvent(character);
        }
    }

    public override bool OnCharacterUse(Character character)
    {
        base.OnCharacterUse(character);

        if (m_TriggerOnUse)
        {
            bool success = UseSwitch(character.Inventory);

            if (success)
                FireHintUsedEvent(character);
        }

        return false;
    }

    public override bool CanUse(Character character)
    {
        return CanUse(character.Inventory);
    }

    private bool CanUse(Inventory inventory)
    {
        if (m_IsEnabled == false)
            return false;

        //Check if we have an inventory & need items
        if (m_RequiredItems != null && inventory == null)
            return false;

        //Check if we have enough items
        foreach (ItemAmountPair requiredItem in m_RequiredItems)
        {
            int amountDiff = requiredItem.Amount - inventory.GetItemAmount(requiredItem.Item);
            if (amountDiff > 0)
            {
                return false;
            }
        }

        return true;
    }

    public override bool CanUse(Character character, out string errorMessage)
    {
        return CanUse(character.Inventory, out errorMessage);
    }

    private bool CanUse(Inventory inventory, out string errorMessage)
    {
        errorMessage = "";

        if (m_IsEnabled == false)
        {
            errorMessage = "Can't use the switch: It has been disabled!";
            return false;
        }

        //Check if we have an inventory & need items
        if (m_RequiredItems != null && inventory == null)
        {
            errorMessage = "Can't use the switch: We need items but the character doesn't have an inventory linked!";
            return false;
        }

        bool canSwitch = true;
        foreach (ItemAmountPair requiredItem in m_RequiredItems)
        {
            int amountDiff = requiredItem.Amount - inventory.GetItemAmount(requiredItem.Item);
            if (amountDiff > 0)
            {
                if (canSwitch == true)
                {
                    errorMessage += "Can't use the switch: We need " + amountDiff + " more " + LocalizationManager.GetText(requiredItem.Item.LocalizationID);
                }
                else
                {
                    errorMessage += "," + amountDiff + " more " + LocalizationManager.GetText(requiredItem.Item.LocalizationID);
                }

                canSwitch = false;
            }
        }

        return canSwitch;
    }

    

    protected bool UseSwitch(Inventory inventory)
    {
        //If we don't need anything, we can leave
        if (m_RequiredItems == null)
        {
            ToggleSwitch();
            return true;
        }

        //Check if we can use the switch
        /*string errorMessage = "";
        if (CanUse(inventory, out errorMessage) == false)
        {
            Debug.Log(errorMessage);
            return false;
        }
        */

        if (CanUse(inventory) == false)
            return false;
            
        //If so, let's switch!
        if (m_ConsumeItems)
        {
            //Remove items if needed
            foreach (ItemAmountPair requiredItem in m_RequiredItems)
            {
                inventory.RemoveItem(requiredItem);
            }
        }

        ToggleSwitch();
        return true;
    }

    private void ToggleSwitch()
    {
        m_IsOn = !m_IsOn;

        foreach (LevelObject levelObject in m_LinkedLevelObjects)
        {
            if (levelObject != null)
                levelObject.ToggleEnabled();
        }
    }

    public string GetProgress()
    {
        if (m_AutoInventory == null)
            return "";

        //Return the progress, so we can show it on a label(?)

        //For now return an ugly label, as we only ever need this for datablocks.
        //Once we need this for more items, make this nice and generic (I'm in a rush now due to an unforseen playtest)
        string temp = "";

        foreach (ItemAmountPair requiredItem in m_RequiredItems)
        {
            temp += m_AutoInventory.GetItemAmount(requiredItem.Item);
            temp += "/";
            temp += requiredItem.Amount;
        }

        return temp;
    }

    public override void SetEnabled(bool state)
    {
        base.SetEnabled(state);

        if (m_ColorModel == null)
            return;

        if (m_IsEnabled)
        {
            if (m_EnabledMaterial != null)
                m_ColorModel.material = m_EnabledMaterial;
        }
        else
        {
            if (m_DisabledMaterial != null)
                m_ColorModel.material = m_DisabledMaterial;
        }
    }

    //ResetableObject
    protected override void OnReset()
    {
        //Nothing yet, will probably reset colour & animations here.
        base.OnReset();
        m_AutoActivated = false;
    }
}