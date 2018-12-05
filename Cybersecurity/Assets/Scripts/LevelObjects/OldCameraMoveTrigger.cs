using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class OldCameraMoveTrigger : LevelObject
{
    [Space(5)]
    [Header("Specific - Camera Move Trigger")]

    [SerializeField]
    private CinemachineVirtualCamera m_OriginalCamera;

    [SerializeField]
    private CinemachineVirtualCamera m_NewCamera;
    private CinemachineVirtualCamera[] m_Cameras = new CinemachineVirtualCamera[2]; //Works alot easier than if/elsing on original/new position

    private int m_CurrentPositionID = 0;

    [SerializeField]
    private FactionTypeDefinition m_AllowedFaction;

    private Direction m_LastDirection;

    protected override void Start()
    {
        base.Start();

        m_Cameras[0] = m_OriginalCamera;
        m_Cameras[1] = m_NewCamera;
    }

    public override void OnCharacterEnter(Character character, Direction direction, bool snap)
    {
        if (character.Faction != m_AllowedFaction)
            return;

        MoveCamera();
        m_LastDirection = direction;
    }

    public override void OnCharacterLeave(Character character, Direction direction)
    {
        if (character.Faction != m_AllowedFaction)
            return;

        //We are returning
        if (m_LastDirection == direction)
        {
            MoveCamera();
        }

        m_LastDirection = Direction.None;
    }

    private void MoveCamera()
    {
        if (m_IsEnabled == false)
            return;

        //Determine the new position
        int newPositionID = m_CurrentPositionID + 1;
        if (newPositionID >= m_Cameras.Length)
            newPositionID = 0;

        if (m_Cameras[newPositionID].gameObject.activeSelf)
            return;

        //Actually move there
        for (int i = 0; i < m_Cameras.Length; ++i)
        {
            bool active = (i == newPositionID);
            m_Cameras[i].gameObject.SetActive(active);
        }

        m_CurrentPositionID = newPositionID;
    }

    protected override void OnReset()
    {
        base.OnReset();
        m_CurrentPositionID = 0;

        //We are not resetting the virtual camera states, as there can be multiple of these (CameraMoveTriggers) in the level.
        //These virtual camera's will have to make sure to reset themselves
    }
}
