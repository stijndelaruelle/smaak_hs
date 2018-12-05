using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : LevelObject
{  
    [SerializeField]
    private FactionTypeDefinition m_AllowedFaction;

    [SerializeField]
    private Renderer m_ExitColorModel;

    [SerializeField]
    private Material m_ExitEnabledMaterial;

    [SerializeField]
    private Material m_ExitDisabledMaterial;
    private bool m_VictoryInQueue = false;

    //Debug
    private void Update()
    {
        //Debug
        if (SaveGameManager.GetBool(SaveGameManager.SAVE_CHEATS) == true)
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.V))
            {
                LevelDirector.Instance.PlayerVictory();
            }
        }

        //We no longer call victory directly (allow for dialogue before victory is triggered)
        if (LevelDirector.Instance == null)
            return;

        if (m_VictoryInQueue && LevelDirector.Instance.HasPlayerInput())
        {
            LevelDirector.Instance.PlayerVictory();
            m_VictoryInQueue = false;
        }
    }

    //LevelObject
    public override void OnCharacterEnter(Character character, Direction direction, bool snap)
    {
        if (m_IsEnabled && character.Faction == m_AllowedFaction)
        {
            //We no longer call victory directly (allow for dialogue before victory is triggered)
            m_VictoryInQueue = true;
        }
    }

    public override void SetEnabled(bool state)
    {
        base.SetEnabled(state);

        if (m_ExitColorModel == null)
            return;

        if (m_IsEnabled)
        {
            if (m_ExitEnabledMaterial != null)
                m_ExitColorModel.material = m_ExitEnabledMaterial;
        }  
        else
        {
            if (m_ExitDisabledMaterial != null)
                m_ExitColorModel.material = m_ExitDisabledMaterial;
        }
    }

    protected override void OnReset()
    {
        //Nothing yet, will probably reset colour & animations here.
        base.OnReset();

        m_VictoryInQueue = false;
    }
}