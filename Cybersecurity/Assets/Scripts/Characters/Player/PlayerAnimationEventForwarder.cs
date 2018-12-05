using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEventForwarder : MonoBehaviour
{
    [SerializeField]
    private Player m_Player;

    [SerializeField]
    private PlayerDissolveEffect m_DissolveEffect;

    public void OnStep()
    {
        //Play sound effect
    }

    public void OnUseAnimationStartUsing()
    {
        //The use animation starts by the character getting ready.
        //This event is fired when he actually start's "hacking".
        m_Player.OnUseAnimationStartUsing();
    }

    public void OnMoveAnimationEnd()
    {
        m_Player.OnMoveAnimationEnd();
    }


    public void OnTeleportUpAnimationEnd()
    {
        //The "go up" teleport
        m_Player.OnTeleportUpAnimationEnd();
    }

    public void OnTeleportDownAnimationEnd()
    {
        //The "go down" teleport
        m_Player.OnTeleportDownAnimationEnd();
    }

    public void OnIntroAnimationEnd()
    {
        m_Player.OnIntroAnimationEnd();
    }

    //Dissolve
    public void OnDissolveIn(float time)
    {
        if (m_DissolveEffect != null)
            m_DissolveEffect.DissolveIn(time, false);
    }

    public void OnDissolveOut(float time)
    {
        if (m_DissolveEffect != null)
            m_DissolveEffect.DissolveOut(time, false);
    }
}
