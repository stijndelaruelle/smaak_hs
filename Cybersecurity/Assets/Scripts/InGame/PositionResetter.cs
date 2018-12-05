using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionResetter : ResetableObject
{
    private Vector3 m_OriginalPosition;

    private void Awake()
    {
        //As we always have a transform, I can talk to it in awake (unlike other components)
        m_OriginalPosition = transform.position.Copy();
    }

    protected override void OnReset()
    {
        transform.position = m_OriginalPosition;
    }
}
