using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Super lame, but this makes sure the general UI doesn't show up when a "cutscene is playing
public class DisableObjectOnEnable : MonoBehaviour
{
    [SerializeField]
    private GameObject m_OtherGameObject;

    private void OnEnable()
    {
        if (m_OtherGameObject != null)
            m_OtherGameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (m_OtherGameObject != null)
            m_OtherGameObject.SetActive(true);
    }
}
