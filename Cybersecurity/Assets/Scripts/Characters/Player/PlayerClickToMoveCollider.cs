using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClickToMoveCollider : MonoBehaviour
{
    [SerializeField]
    private Player m_Player;

    [SerializeField]
    private RelativeDirection m_RelativeDirection;

    private void OnMouseDown()
    {
        //Only allow tap to move on non-mobile builds!
        #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL

            if (m_Player != null)
                    m_Player.Move(m_RelativeDirection);

        #endif
    }
}
