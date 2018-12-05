using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class VirtualCameraTrigger : LevelObject
{
    [Space(5)]
    [Header("Specific - Virtual Camera Trigger")]

    [SerializeField]
    private CinemachineVirtualCamera m_VirtualCamera;
    private CinemachineVirtualCamera[] m_AllVirtualCameras;

    [SerializeField]
    private FactionTypeDefinition m_AllowedFaction;

    protected override void Start()
    {
        base.Start();

        //Super ugly, but referencing these manually every single time will cause more problems than solve them
        m_AllVirtualCameras = (CinemachineVirtualCamera[])FindObjectsOfTypeAll(typeof(CinemachineVirtualCamera));
    }

    //LevelObject
    public override void OnCharacterDirectionEnter(Character character)
    {
        if (character.Faction != m_AllowedFaction)
            return;

        //Disable all camera's
        for (int i = 0; i < m_AllVirtualCameras.Length; ++i)
            m_AllVirtualCameras[i].gameObject.SetActive(false);

        //Show the required one
        m_VirtualCamera.gameObject.SetActive(true);
    }

    //ResetableObject
    protected override void OnReset()
    {
        base.OnReset();
    }
}